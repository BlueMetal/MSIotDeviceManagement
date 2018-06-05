#pragma once
/// <summary>
///		Convert a 8 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT8(char* buffer, size_t buffer_size, uint8_t integer);

/// <summary>
///		Convert a 16 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT16(char* buffer, size_t buffer_size, uint16_t integer);

/// <summary>
///		Convert a 32 byte integer to a string
/// </summary>
void Utils_GetStringFromUINT32(char* buffer, size_t buffer_size, uint32_t integer);

/// <summary>
///		Convert a time_t to a string
/// </summary>
void Utils_GetStringFromTime(char* buffer, size_t buffer_size, time_t integer);

/// <summary>
///		Convert a time_t to a string with a specific format
/// </summary>
void Utils_GetFormattedDateFromTime(char* buffer, size_t buffer_size, time_t integer);

/// <summary>
///		Get the string equivalent of a state_variable value
///		Warning: The buffer used to create the string is reused each time this function is called
/// </summary>
const char* Utils_GetVariableStringValue(const state_variable* stateVariable);