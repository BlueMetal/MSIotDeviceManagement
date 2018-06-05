#include <applibs/applibs_result.h>
#include <stdbool.h>

#pragma once
/// <summary>
///		Check if the network is ready (via the Networking_IsNetworkingReady 
///		call) and initiates a connection to the IoT Hub if networking is up.
/// </summary>
Applibs_Result Networking_CheckNetworkConnection(bool *network_connected, bool force_client_reconnection);

/// <summary>
///		Send a POST request to an internal api endpoint of the 4x4 to setup a wifi connection
/// </summary>
bool Networking_SetWifi(const char *ssid, const char *securityState, const char *password);

/// <summary>
///		Send a GET request to a specific website defined by the appconfig key CONFIG_CONNECTION_IP_CHECK_WEBSITE to retrieve the public IP of the network
///		Buffer must be 42 characters (length of ipv6 + quotes + /0)
/// </summary>
bool Networking_GetIPAddressFromPortalManager(char *buffer, size_t bufferSize);

/// <summary>
///		Initialize Curl
/// </summary>
bool Networking_InitCURL();

/// <summary>
///		Release the resources used by Curl
/// </summary>
void Networking_DestroyCURL();