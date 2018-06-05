#include <time.h>
#include <stdlib.h>
#include <applibs/uart.h>
#include <applibs/log.h>
#include <stdio.h>
#include <stdarg.h>
#include <stdbool.h>
#include <string.h>
#include <limits.h>
#include <curl/curl.h>

#include "constants.h"
#include "state.h"
#include "utils.h"

static char val2char_buffer[TIME_STR_MAX_SIZE];

/// <summary>
///		Convert a 8 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT8(char* buffer, size_t buffer_size, uint8_t integer) {
	if (buffer_size < UINT8_STR_MAX_SIZE) {
		Log_Debug("ERROR: The size of the buffer was not big enough to store uint8_t\n");
		return;
	}
	memset(buffer, 0, buffer_size);
	sprintf(buffer, "%d", integer);
}

/// <summary>
///		Convert a 16 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT16(char* buffer, size_t buffer_size, uint16_t integer) {
	if (buffer_size < UINT16_STR_MAX_SIZE) {
		Log_Debug("ERROR: The size of the buffer was not big enough to store uint16_t\n");
		return;
	}
	memset(buffer, 0, buffer_size);
	sprintf(buffer, "%d", integer);
}

/// <summary>
///		Convert a 32 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT32(char* buffer, size_t buffer_size, uint32_t integer) {
	if (buffer_size < UINT32_STR_MAX_SIZE) {
		Log_Debug("ERROR: The size of the buffer was not big enough to store uint32_t\n");
		return;
	}
	memset(buffer, 0, buffer_size);
	sprintf(buffer, "%d", integer);
}

/// <summary>
///		Convert a time_t to a string
/// </summary>
void Utils_GetStringFromTime(char* buffer, size_t buffer_size, time_t integer) {
	if (buffer_size < TIME_STR_MAX_SIZE) {
		Log_Debug("ERROR: The size of the buffer was not big enough to store time_t\n");
		return;
	}
	memset(buffer, 0, buffer_size);
	sprintf(buffer, "%d", integer);
}

/// <summary>
///		Convert a time_t to a string with a specific format
/// </summary>
void Utils_GetFormattedDateFromTime(char* buffer, size_t buffer_size, time_t integer) {
	if (buffer_size < FORMAT_TIME_STR_MAX_SIZE) {
		Log_Debug("ERROR: The size of the buffer was not big enough to store formatted time\n");
		return;
	}
	memset(buffer, 0, buffer_size);
	strftime(buffer, buffer_size, "\"%Y-%m-%dT%H:%M:%S.0000000Z\"", gmtime(&integer));
}

/// <summary>
///		Get the string equivalent of a state_variable value
///		Warning: The buffer used to create the string is reused each time this function is called
/// </summary>
const char* Utils_GetVariableStringValue(const state_variable* stateVariable) {
	if (!stateVariable) {
		memset(val2char_buffer, 0, sizeof(val2char_buffer));
		return val2char_buffer;
	}

	switch (stateVariable->type) {
	case BYTE:
		Utils_GetStringFromUINT8(val2char_buffer, sizeof(val2char_buffer), stateVariable->valByte);
		break;
	case BOOL:
		Utils_GetStringFromUINT8(val2char_buffer, sizeof(val2char_buffer), stateVariable->valBool);
		break;
	case SHORT:
		Utils_GetStringFromUINT16(val2char_buffer, sizeof(val2char_buffer), stateVariable->valShort);
		break;
	case INT:
		Utils_GetStringFromUINT32(val2char_buffer, sizeof(val2char_buffer), stateVariable->valInt);
		break;
	case TIME:
		Utils_GetStringFromTime(val2char_buffer, sizeof(val2char_buffer), stateVariable->valTime);
		break;
	default:
		return NULL;
	};
	return val2char_buffer;
}