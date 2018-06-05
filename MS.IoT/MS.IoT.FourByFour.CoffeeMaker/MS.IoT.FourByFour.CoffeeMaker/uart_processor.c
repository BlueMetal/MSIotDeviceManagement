#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include <time.h>

#include <applibs/applibs_result.h>
#include <applibs/uart.h>
#include <applibs/sopris_uarts.h>
#include <applibs/gpio.h>
#include <applibs/sopris_pins.h>
#include <applibs/sopris_cdvb2_pins.h>
#include <applibs/log.h>
#include <applibs/lifecycle.h>
#include <applibs/config.h>
#include <applibs/networking.h>

#include "constants.h"
#include "log.h"
#include "azure_iot_hub.h"
#include "uart_processor.h"

//Static Variables
static UART_Handle handle_uart = 0;
static uint8_t header_tag[] = HEADER_TAG;

/// <summary>
///		Init UART
///		Create a valid handle for UART.
/// </summary>
bool UART_InitUART()
{
	Log_Debug("INFO: Initializing UART...\n");

	if (UART_CreateHandle(SOPRIS_CA7_UART1, SERIAL_PORT_BAUDS, &handle_uart) != Result_Success) {
		Log_Debug("ERROR: UART handle not opened\n");
		return false;
	}
	return true;
}

/// <summary>
///		Destroy UART Handle
/// </summary>
void UART_DestroyUART()
{
	Log_Debug("INFO: Destroying UART handle\n");

	if (UART_DestroyHandle(handle_uart) != Result_Success) {
		Log_Debug("ERROR: UART handle was invalid\n");
	}
}

/// <summary>
///		Init Data Processor.
///		This method sets the initial values of the datapacket processor
///		variables, and initialize the array of processed packets to 1.
/// </summary>
bool UART_InitDataProcessor(datapacket_processor *pproc)
{
	Log_Debug("INFO: Initializing Packet Processor...\n");

	//Initialize the number of packets ready and cursor to 0
	pproc->nbr_packets_ready = 0;

	//We initialize this array with one entry. However it is possible that
	//more than one packet becomes ready by the time we leave the handle_reception method.
	//this typically happens when debugging the app and sending continous amounts of packets
	//to the 4x4.
	pproc->packets = (datapacket*)malloc(sizeof(datapacket));
	if (pproc->packets == NULL) {
		Log_Debug("ERROR: Packets memory could not be allocated.\n");
		return false;
	}
	//Reminder of the current size
	pproc->packets_size = 1;
	return true;
}

/// <summary>
///		Destroy Data Processor
///		Make sure that the memory of the current packet being processed is freed if necessary.
///		Free the memory of the array of processed packets.
/// </summary>
void UART_DestroyDataProcessor(datapacket_processor *pproc)
{
	Log_Debug("INFO: Destroying Packet Processor...\n");
	UART_CleanPackets(pproc);
	if (pproc->packets_size > 0)
		free(pproc->packets);
	if (pproc->current_packet.data > 0)
		free(pproc->current_packet.data);
}

/// <summary>
///		Push the last processed packet to the datapacket_processor
/// </summary>
bool UART_GenerateNewPacket(datapacket_processor *pproc, datapacket *current_packet)
{
	datapacket* temp_array;
	datapacket* new_packet;
	
	if (current_packet->data_size > 0)
		Log_Debug("INFO: Data received: '%s'\n", (char*)current_packet->data);

	//Increment number of packets ready
	++pproc->nbr_packets_ready;

	//If the array that holds the processed packet is too small, we need to allocate more memory to it
	if (pproc->nbr_packets_ready > pproc->packets_size) {
		temp_array = (datapacket*)realloc(pproc->packets, pproc->nbr_packets_ready * sizeof(datapacket));
		if (temp_array == NULL) {
			Log_Debug("ERROR: Could not re-allocate required memory for processing packets\n");
			return false;
		}
		pproc->packets_size = pproc->nbr_packets_ready;
		pproc->packets = temp_array;
	}

	//Configuration of the new packet
	new_packet = &pproc->packets[pproc->nbr_packets_ready - 1];
	memcpy(new_packet->cmd, current_packet->cmd, HEADER_COMMAND_SIZE);
	new_packet->message_id = current_packet->message_id;
	new_packet->data_size = current_packet->data_size;
	new_packet->data = current_packet->data;

	//Reset current_packet
	current_packet->data_size = 0;
	current_packet->data = NULL;

	return true;
}

/// <summary>
///		Handles the reception of packets through UART. Waits to receive the specific header_tag, 
///		then allocate memory for the data and waits for the whole package to be sent.
/// </summary>
bool UART_HandleReception(datapacket_processor *pproc)
{
	//Nbr of bytes read in uart_buffer in the current loop cycle 
	size_t bytes_read;

	//Pointer to the current packet being processed
	datapacket *current_packet;

	//A simple cursor used to navigate within the UART buffer
	static uint16_t cursor_stream = 0;

	//Buffer of the incoming data from UART
	static uint8_t uart_buffer[UART_BUFFER_SIZE];

	//Enumeration used to keep track of what is being analyzed.
	//Values can be TAG, HEADER or DATA
	//There is a validation for the first two phases, ensuring that the right information was
	//retrieved before looking for the data.
	static processing_step current_step = TAG;

	//Array of 6 bytes used to analyze the header.
	//Byte 1 and 2: Size of the data in bytes
	//Byte 3 to 6 : Name of the command in 4 bytes.
	static uint8_t current_header[0x08];


	//UART_Receive will get all the bytes that could get processed during one loop cycle.
	if (UART_Receive(handle_uart, uart_buffer, UART_BUFFER_SIZE, 0, UART_BUFFER_SIZE, &bytes_read) != Result_Success) {
		Log_IoT_Error(ERROR_CODE_UART_CONNECTION, "ERROR: Cannot read from UART\n");
		return false;
	}

	if (bytes_read > 0) {
		current_packet = &pproc->current_packet;
		for (uint16_t i = 0; i < bytes_read; ++i) {
			switch (current_step) {
			case DATA:
				//Header processing, command correct, processing data
				current_packet->data[cursor_stream] = uart_buffer[i];
				++cursor_stream;

				if (cursor_stream == current_packet->data_size) {
					UART_GenerateNewPacket(pproc, current_packet);
					current_step = TAG;
					cursor_stream = 0;
				}
				break;
			case HEADER:
				//Tag found, processing header
				current_header[cursor_stream] = uart_buffer[i];
				++cursor_stream;

				if (cursor_stream == HEADER_MESSAGE_ID_SIZE + HEADER_DATASIZE_SIZE + HEADER_COMMAND_SIZE) {
					cursor_stream = 0;

					//Get size and command
					current_packet->message_id = (uint16_t) (current_header[0] | current_header[1] << 8);
					current_packet->data_size = (size_t) (current_header[2] | current_header[3] << 8);
					memcpy(current_packet->cmd, &current_header[4], HEADER_COMMAND_SIZE);

					if (memcmp(current_packet->cmd, CMD_PING, HEADER_COMMAND_SIZE) == 0 || memcmp(current_packet->cmd, CMD_GET_PROPERTY, HEADER_COMMAND_SIZE) == 0
						|| memcmp(current_packet->cmd, CMD_PONG, HEADER_COMMAND_SIZE) == 0 || memcmp(current_packet->cmd, CMD_SEND_PROPERTY, HEADER_COMMAND_SIZE) == 0
						|| memcmp(current_packet->cmd, CMD_SEND_ACTION, HEADER_COMMAND_SIZE) == 0 || memcmp(current_packet->cmd, CMD_CONFIRM_PROPERTY, HEADER_COMMAND_SIZE) == 0
						|| memcmp(current_packet->cmd, CMD_CONFIRM_ACTION, HEADER_COMMAND_SIZE) == 0)
					{
						//Command valid, proceed to the parsing data (if needed)
						Log_Debug("INFO: Command received: '%.4s', id: '%d',  data size: '%d' bytes\n", (char *)current_packet->cmd, current_packet->message_id, current_packet->data_size);
						if (current_packet->data_size > 0) {
							current_packet->data = (uint8_t*)malloc((size_t)(current_packet->data_size + 1)); //+1 for properly display the end of string in debug
							if (current_packet->data == NULL) {
								Log_Debug("ERROR: Packet Data memory could not be allocated.\n");
								return -1;
							}
							memset(current_packet->data, 0, current_packet->data_size + 1);
							current_step = DATA;
						}
						else {
							//No data, sending the packet immediately, skipping DATA phase
							current_packet->data_size = 0;
							UART_GenerateNewPacket(pproc, current_packet);
							current_step = TAG;
						}
					}
					else {
						//Command is not recognized, go back to tag seeking
						current_step = TAG;
					}
				}
				break;
			case TAG:
				//Looking for Header tag
				if (uart_buffer[i] == header_tag[cursor_stream]) {
					++cursor_stream;
					if (cursor_stream == HEADER_TAG_SIZE) {
						//Pattern found, saving the rest of the packet as it comes
						current_step = HEADER;
						cursor_stream = 0;
					}
				}
				else
					cursor_stream = 0;
				break;
			}
		}
	}
	return true;
}

/// <summary>
///		Free memory of the processed packets
/// </summary>
void UART_CleanPackets(datapacket_processor *pproc) {
	for (uint16_t p = 0; p < pproc->nbr_packets_ready; p++) {
		datapacket *packet_to_delete = &pproc->packets[p];
		memset(packet_to_delete->cmd, 0, HEADER_COMMAND_SIZE);
		if (packet_to_delete->data_size > 0)
			free(packet_to_delete->data);
		packet_to_delete->data_size = 0;
	}
	pproc->nbr_packets_ready = 0;
}

/// <summary>
///		Send a Datapacket back to the device through UART
/// </summary>
bool UART_SendPacket(datapacket *p) {
	static uint8_t header_size = HEADER_TAG_SIZE + HEADER_MESSAGE_ID_SIZE + HEADER_DATASIZE_SIZE + HEADER_COMMAND_SIZE;

	//Not ready
	if (handle_uart == 0)
		return true;

	//Creating header
	uint8_t packet[header_size + p->data_size];
	memcpy(packet, header_tag, HEADER_TAG_SIZE);
	memcpy(&packet[HEADER_TAG_SIZE], &p->message_id, HEADER_MESSAGE_ID_SIZE);
	memcpy(&packet[HEADER_TAG_SIZE + HEADER_MESSAGE_ID_SIZE], &p->data_size, HEADER_DATASIZE_SIZE);
	memcpy(&packet[HEADER_TAG_SIZE + HEADER_MESSAGE_ID_SIZE + HEADER_DATASIZE_SIZE], p->cmd, HEADER_COMMAND_SIZE);
	memcpy(&packet[header_size], p->data, p->data_size);

	size_t total_bytes_sent = 0;
	size_t bytes_to_send = (size_t) (header_size + p->data_size);
	int send_iterations = 0;
	while (total_bytes_sent < bytes_to_send) {
		size_t bytes_sent = 0;
		send_iterations++;
		Applibs_Result result = UART_Send(handle_uart, packet, bytes_to_send, total_bytes_sent,
			bytes_to_send - total_bytes_sent, &bytes_sent);
		if (result != Result_Success) {
			Log_IoT_Error(ERROR_CODE_UART_CONNECTION, "ERROR: Could not send data on serial port\n");
			return false;
		}
		total_bytes_sent += bytes_sent;
	}
	Log_Debug("INFO: Command '%.4s' sent over UART, id: '%d', data size: '%d' (%d calls)\n", p->cmd, p->message_id, total_bytes_sent, send_iterations);

	return true;
}