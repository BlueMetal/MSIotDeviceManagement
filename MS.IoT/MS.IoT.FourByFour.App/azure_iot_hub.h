/// \file azure_iot_hub.h
/// \brief This header defines a sample interface for performing basic operations with Azure
/// IoT Hub
///
/// Call runDemo() to run entire demo or call specific function to perform specific operations.
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
void AzureIoT_TwinReportState(const char *propertyName, const char *propertyValue);

/// <summary>
///     Sends sample message.
/// </summary>
void AzureIoT_SendMessage(char* message);

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

typedef void(*TwinUpdateFromIoTHubFnType)(const char *payload);
/// <summary>
///     Set the function callback invoked whenever a Device Twin update from the IoT Hub is received.
/// </summary>
void AzureIoT_SetDeviceTwinUpdateFromIoTHubCallback(TwinUpdateFromIoTHubFnType fn);

typedef void(*DirectMethodCallFromIoTHubFnType)(const char *directMethodName, const char *payload);
/// <summary>
///     Set the function to be called whenever a Direct Method call from the IoT Hub is received.
/// </summary>
void AzureIoT_SetDirectMethodCallFromIoTHubCallback(DirectMethodCallFromIoTHubFnType fn);
