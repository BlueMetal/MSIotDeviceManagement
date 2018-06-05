#pragma once
/// <summary>
///		Enumeration used to keep track of what is being analyzed.
/// </summary>
typedef enum processing_step { TAG, HEADER, DATA } processing_step;

/// <summary>
///		Structure datapacket
///		Used to store the command name, data size and data of a packet
///		received from UART.
/// </summary>
typedef struct datapacket {
	//Message
	uint16_t message_id;

	//Command name of the packet, in 4 bytes
	uint8_t cmd[4];

	//Data of the packet, up to 2^16 characters
	uint8_t *data;

	//Size of the packet, in 2 bytes
	size_t data_size;
} datapacket;

/// <summary>
///		Datapacket processor
///		The Data Processor contains all the necessary buffers and pointers
///		in order to analyse the incoming data from UART.
/// </summary>
typedef struct datapacket_processor {
	//Dynamic array representing a number of packets already processed.
	//Most of the time, the length of this array will be equal to one time
	//the structure of datapacket. However it is possible that more than one packet
	//gets per loop cycle. In this case, the size of the array will be increased.
	datapacket *packets;

	//Value representing the size of the processed_packets array, per unit of datapackets
	size_t packets_size;

	//Value representing the number of packets already processed and ready.
	//It can only be equal of inferior to processed_packets_size, otherwise
	//proccessed_packets will allocate more memory.
	uint16_t nbr_packets_ready;

	//Structure representing the packet being processed.
	//This structure is used a buffer as the data is being
	//retrieved and analyzed. The data_size and cmd will be
	//filled first, and then the data.
	//Once the packet is fully retrieved, its data will be moved
	//to the processed_packets array.
	datapacket current_packet;
} datapacket_processor;

/// <summary>
///		Init UART
///		Create a valid handle for UART.
/// </summary>
bool UART_InitUART();

/// <summary>
///		Destroy UART Handle
/// </summary>
void UART_DestroyUART();

/// <summary>
///		Init Data Processor.
///		This method sets the initial values of the datapacket processor
///		variables, and initialize the array of processed packets to 1.
/// </summary>
bool UART_InitDataProcessor(datapacket_processor *pproc);

/// <summary>
///		Destroy Data Processor
///		Make sure that the memory of the current packet being processed is freed if necessary.
///		Free the memory of the array of processed packets.
/// </summary>
void UART_DestroyDataProcessor(datapacket_processor *pproc);

/// <summary>
///		Handles the reception of packets through UART. Waits to receive the specific header_tag, 
///		then allocate memory for the data and waits for the whole package to be sent.
/// </summary>
bool UART_HandleReception(datapacket_processor *pproc);

/// <summary>
///		Free memory of the processed packets
/// </summary>
void UART_CleanPackets(datapacket_processor *pproc);

/// <summary>
///		Send a Datapacket back to the device through UART
/// </summary>
bool UART_SendPacket(datapacket *p);