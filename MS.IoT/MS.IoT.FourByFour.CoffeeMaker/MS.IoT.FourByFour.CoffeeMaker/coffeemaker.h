#pragma once
/// <summary>
///		Initialize all GPIO handles to be used with the device.
/// </summary>
bool Device_InitGPIOHandles();

/// <summary>
///		Destroy all GPIO handles
/// </summary>
bool Device_DestroyGPIOHandles();

/// <summary>
///		Action to change the "Brew Strengh" value of the device.
///		0: Regular (default), 1: Bold, 2: Strong
/// </summary>
bool Device_ChangeBrewStrength(uint8_t newState);

/// <summary>
///		Action to launch or stop a brew
///		Sets the grind value to 0
/// </summary>
bool Device_LaunchBrew();

/// <summary>
///		Action to launch or stop a grind and brew
///		Sets the grind value to 1 (4)
/// </summary>
bool Device_LaunchGrindBrew();

/// <summary>
///		Action to set Wifi
///		The parameter is a char sequence of SSID + 0x0 + Password
/// </summary>
bool Device_SetWifi(const char* data);

/// <summary>
///		Execute periodic tasks specific to the device
/// </summary>
void Device_DoPeriodicTasks(const bool connectionState);

/// <summary>
/// Device Twin update callback function, called when an update is received from the Azure IoT Hub.
/// </summary>
void Device_TwinUpdate(const char *childName, const char *propertyName, const char *valueString);

/// <summary>
/// Direct Method callback function, called when a Direct Method is received from the Azure IoT Hub.
/// </summary>
void Device_DirectMethodCall(const char* methodName, const char *payload, unsigned char **response, size_t *response_size);