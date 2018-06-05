#include <applibs/applibs_result.h>

#pragma once
/// <summary>
/// check_network_connection checks if the network is ready (via the Networking_IsNetworkingReady 
/// call) and initiates a connection to the IoT Hub if networking is up.
/// </summary>
Applibs_Result check_network_connection(bool *network_connected, bool force_client_reconnection);