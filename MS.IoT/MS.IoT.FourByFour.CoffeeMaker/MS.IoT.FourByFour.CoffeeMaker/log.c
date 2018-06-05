#include<stdarg.h>
#include<stdlib.h>
#include<time.h>
#include<stdio.h>
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
#include "uart_commands.h"               
#include "state.h"
#include "utils.h"
#include "azure_iot_hub.h"

/// <summary>
///		Log Method of type INFO that will also send a message to IoT Hub if debug is activated.
/// </summary>
void Log_IoT_Info(const char *fmt, ...) {
	static state_variable *debugState;
	char *message;
	//char *json;
	size_t bufsz;

	if(!debugState)
		debugState = State_GetVariable(FEATURE_VARNAME_DEBUG);

	va_list args;
	va_start(args, fmt);
	bufsz = (unsigned int)vsnprintf(NULL, 0, fmt, args);
	message = malloc(bufsz + 1);
	vsnprintf(message, bufsz + 1, fmt, args);
	va_end(args);

	Log_Debug(message);
	
	if (debugState->valBool) {
		/*bufsz = (unsigned int)snprintf(NULL, 0, LOG_ERROR_JSON, message);
		json = malloc(bufsz + 1);
		snprintf(json, bufsz + 1, LOG_ERROR_JSON, code, message);
		AzureIoT_SendMessage(json);
		free(json);*/
	}

	free(message);
}

/// <summary>
///		Log Method of type ERROR that will also send a message to IoT Hub if debug is activated.
/// </summary>
void Log_IoT_Error(const uint16_t code, const char *fmt, ...) {
	static state_variable *debugState;
	char code_str[6];
	char *message;
	//char *json;
	size_t bufsz;

	if (!debugState)
		debugState = State_GetVariable(FEATURE_VARNAME_DEBUG);

	va_list args;
	va_start(args, fmt);
	bufsz = (unsigned int)vsnprintf(NULL, 0, fmt, args);
	message = malloc(bufsz + 1);
	vsnprintf(message, bufsz + 1, fmt, args);
	va_end(args);

	Log_Debug(message);
	Utils_GetStringFromUINT16(code_str, sizeof(code_str), code);
	AzureIoT_TwinReportStateString(REPORTED_STATUSCODE, code_str);

	if (debugState->valBool) {
		/*bufsz = (unsigned int)snprintf(NULL, 0, LOG_ERROR_JSON, message);
		json = malloc(bufsz + 1);
		snprintf(json, bufsz + 1, LOG_ERROR_JSON, code, message);
		AzureIoT_SendMessage(json);
		free(json);*/
	}

	free(message);
}