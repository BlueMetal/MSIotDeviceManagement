#pragma once
/// <summary>
///		Log Method of type INFO that will also send a message to IoT Hub if debug is activated.
/// </summary>
void Log_IoT_Info(const char *fmt, ...);

/// <summary>
///		Log Method of type ERROR that will also send a message to IoT Hub if debug is activated.
/// </summary>
void Log_IoT_Error(const uint16_t code, const char *fmt, ...);