#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>
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
#include "state.h"
#include "azure_iot_hub.h"
#include "network.h"
#include "azure_iot_hub.h"
#include "uart_commands.h"
#include "coffeemaker.h"
#include "uart_processor.h"
#include "utils.h"

/// <summary>
///		Get a new packet ID
/// </summary>
uint16_t UART_GetNewId() {
	static uint16_t index_id = 0;
	uint16_t new_id = index_id;

	if (index_id == INT16_MAX)
		index_id = 0;
	else
		++index_id;

	return new_id;
}

/// <summary>
///		Send a Ping Command to the UI
/// </summary>
bool UART_CommandSendPing() {
	datapacket packet;
	bool result;

	//Build packet
	packet.message_id = UART_GetNewId();
	memcpy(packet.cmd, CMD_PING, HEADER_COMMAND_SIZE);
	packet.data_size = 0;

	result = UART_SendPacket(&packet);

	return result;
}

/// <summary>
///		Send a Pong Command response to the UI
/// </summary>
bool UART_CommandSendPong(uint16_t sourceMessageId) {
	datapacket packet;
	bool result;

	//Build packet
	packet.message_id = sourceMessageId;
	memcpy(packet.cmd, CMD_PONG, HEADER_COMMAND_SIZE);
	packet.data_size = 0;

	result = UART_SendPacket(&packet);

	return result;
}

/// <summary>
///		Send a Send Property Command response to the UI.
/// </summary>
bool UART_CommandSendPropertyReply(const char *propertyName, const char *propertyValue, uint16_t sourceMessageId)
{
	datapacket packet;
	bool result_send;

	//Creating return packet
	memcpy(packet.cmd, CMD_SEND_PROPERTY, HEADER_COMMAND_SIZE);
	packet.message_id = sourceMessageId;

	if (propertyValue) {
		packet.data_size = strlen(propertyName) + strlen(propertyValue) + 2;
		packet.data = (uint8_t*)malloc(packet.data_size);
		memset(packet.data, 0, packet.data_size);
		memcpy(packet.data, propertyName, strlen(propertyName));
		memcpy(&packet.data[strlen(propertyName) + 1], propertyValue, strlen(propertyValue));
	}
	else {
		packet.data_size = strlen(propertyName) + 1;
		packet.data = (uint8_t*)malloc(packet.data_size);
		memset(packet.data, 0, packet.data_size);
		memcpy(packet.data, propertyName, strlen(propertyName));
	}

	result_send = UART_SendPacket(&packet);
	free(packet.data);

	return result_send;
}

/// <summary>
///		Send a Send Property Command to UI.
/// </summary>
bool UART_CommandSendPropertyString(const char *propertyName, const char *propertyValue)
{
	return UART_CommandSendPropertyReply(propertyName, propertyValue, UART_GetNewId());
}

/// <summary>
///		Send a Send Property Command to UI using a state_variable.
/// </summary>
bool UART_CommandSendProperty(const state_variable *property)
{
	const char* propertyValue = Utils_GetVariableStringValue(property);
	return UART_CommandSendPropertyReply(property->varname, propertyValue, UART_GetNewId());
}

/// <summary>
///		Send a Send Action Confirm Command to UI.
/// </summary>
bool UART_CommandSendActionConfirm(const char *actionName, uint16_t source_message_id) {
	datapacket packet;
	bool result_send;

	//Creating return packet
	memcpy(packet.cmd, CMD_CONFIRM_ACTION, HEADER_COMMAND_SIZE);
	packet.message_id = source_message_id;
	packet.data_size = strlen(actionName);
	packet.data = (uint8_t*)malloc(packet.data_size);
	memset(packet.data, 0, packet.data_size);
	memcpy(packet.data, actionName, strlen(actionName));

	result_send = UART_SendPacket(&packet);
	free(packet.data);

	return result_send;
}

/// <summary>
///		Process action received by the UI (forward to Device_DirectMethodCall) 
/// </summary>
bool UART_ProcessAction(const char *payload) {
	const char* propertyName = payload;
	const char* propertyValue = &payload[strlen(payload) + 1];

	//Process Change Brew Strength
	Device_DirectMethodCall(propertyName, propertyValue, NULL, NULL);

	return true;
}

/// <summary>
///		Main command processing method that analyzes the command name and redirect to the right command method.
/// </summary>
bool UART_ProcessCommand(datapacket *p)
{
	if (memcmp(p->cmd, CMD_PING, HEADER_COMMAND_SIZE) == 0)
		return UART_CommandSendPong(p->message_id);
	else if (memcmp(p->cmd, CMD_PONG, HEADER_COMMAND_SIZE) == 0)
		return true; //TODO: Handle timeout
	else if (memcmp(p->cmd, CMD_GET_PROPERTY, HEADER_COMMAND_SIZE) == 0) {
		const state_variable* stateVariable = State_GetVariable((char*)p->data);
		const char* propertyValue = Utils_GetVariableStringValue(stateVariable);
		return UART_CommandSendPropertyReply((char*)p->data, propertyValue, p->message_id);
	}
	else if (memcmp(p->cmd, CMD_SEND_ACTION, HEADER_COMMAND_SIZE) == 0) {
		if(UART_ProcessAction((char*)p->data))
			return UART_CommandSendActionConfirm((char*)p->data, p->message_id);
		return false;
	}
	else if (memcmp(p->cmd, CMD_CONFIRM_PROPERTY, HEADER_COMMAND_SIZE) == 0)
		return true; //TODO: Handle timeout
	Log_Debug("ERROR: Command not found. Make sure UART is connected properly");
	return true;
}