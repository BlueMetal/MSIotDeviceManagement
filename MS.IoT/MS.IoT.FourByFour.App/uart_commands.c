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
#include "simulator_structures.h"
#include "network.h"
#include "azure_iot_hub.h"
#include "uart_processor.h"

/// <summary>
/// Command POKE: Return a simple packet to signify that the connection is working.
/// </summary>
bool _process_poke_alive_command()
{
	datapacket poke_alive_packet;

	memcpy(poke_alive_packet.cmd, CMD_POKE_ALIVE, HEADER_COMMAND_SIZE);
	poke_alive_packet.data_size = 0;

	return send_packet(&poke_alive_packet);
}

/// <summary>
/// Command GET CONFIG: Retrieves the connection string from the appconfig store CONFIG_CONNECTION_KEY and sends it through UART.
/// </summary>
bool _process_get_config_command()
{
	datapacket get_config_packet;
	Applibs_Result result_config;
	bool result_send;
	char* connection_string = (char*)malloc(MAX_CONNECTION_STRING_SIZE * 3);;

	//Read CONFIG_CONNECTION_KEY fron the Appconfig store
	result_config = Config_ReadString(CONFIG_CONNECTION_KEY, connection_string, MAX_CONNECTION_STRING_SIZE);
	if (result_config == Result_Config_KeyNotFound) {
		Log_Debug("INFO: The key '%s' was not found in the config store.\n", (char*)CONFIG_CONNECTION_KEY);
	}
	else if (result_config != Result_Success) {
		Log_Debug("ERROR: failure while attempting to read the config store.\n");
		return false;
	}

	//Create return packet
	memcpy(get_config_packet.cmd, CMD_GET_CONFIG, HEADER_COMMAND_SIZE);
	if (result_config == Result_Success) {
		//A config was found, sending it
		get_config_packet.data_size = strlen(connection_string);
		get_config_packet.data = connection_string;
	}
	else {
		//No config was found, sending empty data
		get_config_packet.data_size = 0;
	}

	//Send the packet
	result_send = send_packet(&get_config_packet);

	//Free memory
	if (get_config_packet.data_size > 0)
		free(connection_string);

	return result_send;
}

/// <summary>
/// Command SET CONFIG: Sets the connection string CONFIG_CONNECTION_KEY in the appconfig store.
/// </summary>
bool _process_set_config_command(datapacket *p)
{
	datapacket config_set_packet;
	Applibs_Result result_config;
	bool result_send;
	bool network_connection_on = false;

	//Write in memory
	result_config = Config_WriteString(CONFIG_CONNECTION_KEY, p->data);
	if (result_config == Result_Config_KeyTooLong)
		Log_Debug("ERROR: failure while attempting to write in the config store. The key is too long\n");
	else if (result_config == Result_Config_InvalidArgumentNullValue)
		Log_Debug("ERROR: failure while attempting to write in the config store. The value passed was empty\n");
	else if (result_config != Result_Success) {
		//If unknown error, exit the app
		Log_Debug("ERROR: failure while attempting to write in the config store.\n");
		return false;
	}
	else
		Log_Debug("INFO: New connection string set '%s', attempting a connection to IoT Hub.\n", (char*)CONFIG_CONNECTION_KEY);

	//Reset connection status
	if (check_network_connection(&network_connection_on, true) != Result_Success) {
		Log_Debug("ERROR: failure during network status check\n");
		return false;
	}

	//Creating return packet
	memcpy(config_set_packet.cmd, CMD_SET_CONFIG, HEADER_COMMAND_SIZE);
	config_set_packet.data_size = 1;
	config_set_packet.data = (uint8_t*)malloc(config_set_packet.data_size);
	if (result_config == Result_Success) {
		//Indicate that the set succeeded
		config_set_packet.data[0] = '1';
	}
	else {
		//Indicate that the set failed
		config_set_packet.data[0] = '0';
	}

	result_send = send_packet(&config_set_packet);
	free(config_set_packet.data);

	return result_send;
}

/// <summary>
/// Command IOT POKE: Checks if the connection was established with IoT Hub.
/// </summary>
bool _process_poke_iot_command()
{
	datapacket poke_iot_packet;
	bool result_send;
	bool network_connection_on = false;

	//Creating return packet
	memcpy(poke_iot_packet.cmd, CMD_POKE_IOT_HUB, HEADER_COMMAND_SIZE);
	poke_iot_packet.data_size = 1;
	poke_iot_packet.data = (uint8_t*)malloc(poke_iot_packet.data_size);

	//Check connection status
	if (check_network_connection(&network_connection_on, false) != Result_Success) {
		Log_Debug("ERROR: failure during network status check\n");
		return false;
	}
	if (!network_connection_on)
		poke_iot_packet.data[0] = '0';
	else
		poke_iot_packet.data[0] = '1';

	result_send = send_packet(&poke_iot_packet);
	free(poke_iot_packet.data);

	return result_send;
}

/// <summary>
/// Command SEND TEMPLATE: Sends the content of a template to IoT Hub to be processed.
/// </summary>
bool _process_send_template_command(datapacket *p)
{
	//Calls to IoT Hub SDK method sendMessage
	AzureIoT_SendMessage((char*)p->data);
	return true;
}

/// <summary>
/// Main command processing method that analyzes the command name and redirect to the right command method.
/// </summary>
bool process_command(datapacket *p)
{
	if (memcmp(p->cmd, CMD_POKE_ALIVE, HEADER_COMMAND_SIZE) == 0)
		return _process_poke_alive_command();
	else if (memcmp(p->cmd, CMD_SET_CONFIG, HEADER_COMMAND_SIZE) == 0)
		return _process_set_config_command(p);
	else if (memcmp(p->cmd, CMD_GET_CONFIG, HEADER_COMMAND_SIZE) == 0)
		return _process_get_config_command();
	else if (memcmp(p->cmd, CMD_POKE_IOT_HUB, HEADER_COMMAND_SIZE) == 0)
		return _process_poke_iot_command();
	else if (memcmp(p->cmd, CMD_SEND_TEMPLATE, HEADER_COMMAND_SIZE) == 0)
		return _process_send_template_command(p);

	return false;
}