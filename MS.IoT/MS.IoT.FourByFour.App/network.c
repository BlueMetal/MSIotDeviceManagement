#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#include <applibs/applibs_result.h>
#include <applibs/log.h>
#include <applibs/networking.h>

#include "azure_iot_hub.h"

/// <summary>
/// check_network_connection checks if the network is ready (via the Networking_IsNetworkingReady 
/// call) and initiates a connection to the IoT Hub if networking is up.
/// </summary>
Applibs_Result check_network_connection(bool *network_connected, bool force_client_reconnection)
{
	static bool old_is_network_ready = false;
	static bool old_network_connected = false;
	bool current_is_network_ready = false;

	*network_connected = false;

	Applibs_Result  result = Networking_IsNetworkingReady(&current_is_network_ready);
	if (result != Result_Success) {
		Log_Debug("ERROR: could not request network status\n");
		return result;
	}

	//If reconnection forced, make sure the client is disconnected = memory freed
	if (force_client_reconnection) {
		AzureIoT_ClientDisconnect();
		old_is_network_ready = false;
	}

	// If network state changed from 'not ready' to 'ready', attempt connection.
	if (!old_is_network_ready && current_is_network_ready) {
		old_network_connected = AzureIoT_ClientConnect();
	}

	// If network is not ready, cancel connection flag.
	else if (!current_is_network_ready) {
		old_network_connected = false;
	}

	old_is_network_ready = current_is_network_ready;
	*network_connected = old_network_connected;
	return result;
}