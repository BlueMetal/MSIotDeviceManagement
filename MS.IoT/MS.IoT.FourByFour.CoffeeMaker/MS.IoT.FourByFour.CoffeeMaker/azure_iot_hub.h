#include "structs.h"
#pragma once

/// <summary>
///     Establishes connection to Azure IoT Hub
/// </summary>
bool AzureIoT_ClientConnect();

/// <summary>
///     Disconnect from Azure IoT Hub and releases resources
/// </summary>
void AzureIoT_ClientDisconnect();

/// <summary>
///     Reports device state using device twin
/// </summary>
void AzureIoT_TwinReportStateString(const char *propertyName, const char *propertyValue);

/// <summary>
///     Updates device twin reported properties of a feature.
/// </summary>
void AzureIoT_TwinReportStateFeature(const char *childName, const char *propertyName, const char *propertyValue);

/// <summary>
///     Reports device state using device twin
/// </summary>
void AzureIoT_TwinReportStateDevice(const state_variable *variable);

/// <summary>
///     Updates device twin reported properties during initialization
/// </summary>
void AzureIoT_InitReportState();

/// <summary>
///     Sends sample message.
/// </summary>
bool AzureIoT_SendMessage(char* message);

/// <summary>
///     Keeps IoT Hub Client alive.
///     Call this function periodically.
/// </summary>
void AzureIoT_DoPeriodicTasks();

typedef void(*MessageReceivedFromIoTHubFnType)(const char *payload);
/// <summary>
///     Set a function callback called whenever a message is received from IoT Hub.
/// </summary>
void AzureIoT_SetMessageReceivedFromIoTHubCallback(MessageReceivedFromIoTHubFnType fn);

typedef void(*TwinUpdateFromIoTHubFnType)(const char *childNode, const char *featureName, const char *payload);
/// <summary>
///     Set the function callback invoked whenever a Device Twin update from the IoT Hub is received.
/// </summary>
void AzureIoT_SetDeviceTwinUpdateFromIoTHubCallback(TwinUpdateFromIoTHubFnType fn);

typedef void(*DirectMethodCallFromIoTHubFnType)(const char *directMethodName, const char *payload, unsigned char **response, size_t *response_size);
/// <summary>
///     Set the function to be called whenever a Direct Method call from the IoT Hub is received.
/// </summary>
void AzureIoT_SetDirectMethodCallFromIoTHubCallback(DirectMethodCallFromIoTHubFnType fn);