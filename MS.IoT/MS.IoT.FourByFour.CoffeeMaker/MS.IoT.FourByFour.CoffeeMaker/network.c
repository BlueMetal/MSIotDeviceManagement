#include <time.h>
#include <stdlib.h>
#include <applibs/uart.h>
#include <applibs/config.h>
#include <applibs/log.h>
#include <applibs/networking.h>
#include <stdio.h>
#include <stdarg.h>
#include <stdbool.h>
#include <string.h>
#include <curl/curl.h>
#include "constants.h"
#include "utils.h"
#include "log.h"
#include "azure_iot_hub.h"

struct curl_response {
	size_t size;
	char* data;
};

/// <summary>
///		Internal method to copy the curl response into a curl_response structure
/// </summary>
size_t Networking_WriteData(void *ptr, size_t size, size_t nmemb, struct curl_response *data) {
	size_t index = data->size;
	size_t n = (size * nmemb);
	char* tmp;

	data->size += (size * nmemb);
	tmp = realloc(data->data, data->size + 1); /* +1 for '\0' */

	if (tmp) {
		data->data = tmp;
	}
	else {
		if (data->data) {
			free(data->data);
		}
		Log_Debug("ERROR: Failed to allocate memory.\n");
		return 0;
	}

	memcpy((data->data + index), ptr, n);
	data->data[data->size] = '\0';

	return size * nmemb;
}

/// <summary>
///		Initialize Curl
/// </summary>
bool Networking_InitCURL() {
	CURLcode res;
	Log_Debug("INFO: Initializing CURL...\n");
	res = curl_global_init(CURL_GLOBAL_DEFAULT);
	if (res != CURLE_OK) {
		Log_Debug("ERROR: CURL can not be initialized\n");
		return false;
	}
	return true;
}

/// <summary>
///		Release the resources used by Curl
/// </summary>
void Networking_DestroyCURL() {
	curl_global_cleanup();
}

/// <summary>
///		Send a POST request to an internal api endpoint of the 4x4 to setup a wifi connection
/// </summary>
bool Networking_SetWifi(const char *ssid, const char *securityState, const char *password)
{
	CURL *curl;
	CURLcode res;
	char request[512];
	struct curl_response data;
	struct curl_slist *chunk = NULL;
	chunk = curl_slist_append(chunk, "Content-Type: application/json");

	sprintf(request, API_WIFI_JSON, ssid, securityState, password);

	data.size = 0;
	data.data = malloc(4096); /* reasonable size initial buffer */
	if (NULL == data.data) {
		Log_Debug("ERROR: Failed to allocate memory.\n");
		return NULL;
	}
	data.data[0] = '\0';

	curl = curl_easy_init();
	if (curl) {
		curl_easy_setopt(curl, CURLOPT_URL, API_WIFI);
		curl_easy_setopt(curl, CURLOPT_POST, 1L);
		curl_easy_setopt(curl, CURLOPT_POSTFIELDS, request);
		curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, strlen(request));
		curl_easy_setopt(curl, CURLOPT_HTTPHEADER, chunk);
		//curl_easy_setopt(curl, CURLOPT_USERAGENT, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.5");
		curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
		curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, Networking_WriteData);
		curl_easy_setopt(curl, CURLOPT_WRITEDATA, &data);

		res = curl_easy_perform(curl);
		/* Check for errors */
		if (res != CURLE_OK)
			Log_IoT_Error(ERROR_CODE_CURL_ERROR, "ERROR: curl_easy_perform() failed: %s\n", curl_easy_strerror(res));

		curl_easy_cleanup(curl);
	}

	if (data.data) {
		Log_Debug("INFO: %s\n", data.data);
		free(data.data);
	}

	return true;
}

/// <summary>
///		Check if the network is ready (via the Networking_IsNetworkingReady 
///		call) and initiates a connection to the IoT Hub if networking is up.
/// </summary>
Applibs_Result Networking_CheckNetworkConnection(bool *networkConnected, bool forceClientReconnection)
{
	static bool oldIsNetworkReady = false;
	static bool oldNetworkConnected = false;
	bool currentIsNetworkReady = false;

	*networkConnected = false;

	Applibs_Result  result = Networking_IsNetworkingReady(&currentIsNetworkReady);
	if (result != Result_Success) {
		Log_Debug("ERROR: Could not request network status\n");
		return result;
	}

	//If reconnection forced, make sure the client is disconnected = memory freed
	if (forceClientReconnection) {
		AzureIoT_ClientDisconnect();
		oldIsNetworkReady = false;
	}

	// If network state changed from 'not ready' to 'ready', attempt connection.
	if (!oldIsNetworkReady && currentIsNetworkReady) {
		oldNetworkConnected = AzureIoT_ClientConnect();
	}

	// If network is not ready, cancel connection flag.
	else if (!currentIsNetworkReady) {
		oldNetworkConnected = false;
	}

	oldIsNetworkReady = currentIsNetworkReady;
	*networkConnected = oldNetworkConnected;
	return result;
}

/// <summary>
///		Send a GET request to a specific website defined by the appconfig key CONFIG_CONNECTION_IP_CHECK_WEBSITE to retrieve the public IP of the network
///		Buffer must be 42 characters (length of ipv6 + quotes + /0)
/// </summary>
bool Networking_GetIPAddressFromPortalManager(char *buffer, size_t bufferSize)
{
	CURL *curl;
	CURLcode res;
	char request[512];
	struct curl_response data;
	bool useSSL = false;
	char* hostname;

	//Get Hostname from Config store
	hostname = (char*)malloc(MAX_CONNECTION_STRING_SIZE * 3);;
	if (hostname == NULL)
	{
		Log_Debug("ERROR: Failed to allocate buffer for connection string\n");
		return false;
	}

	Applibs_Result result_config = Config_ReadString(CONFIG_CONNECTION_IP_CHECK_WEBSITE, hostname, MAX_CONNECTION_STRING_SIZE);
	if (result_config == Result_Config_KeyNotFound) {
		Log_IoT_Error(ERROR_CODE_APPCONFIG_EXISTS, "ERROR: The key '%s' was not found in the config store.\n", (char*)CONFIG_CONNECTION_IP_CHECK_WEBSITE);
		return false;
	}
	else if (result_config != Result_Success) {
		Log_IoT_Error(ERROR_CODE_APPCONFIG_READ, "ERROR: Failure while attempting to read the config store.\n");
		return false;
	}

	//Merge Hostname to request URL
	sprintf(request, API_IP_ADDRESS, hostname);
	Log_Debug(request);

	//Test for SSL
	useSSL = strstr(hostname, "https") != NULL;

	//Free hostname malloc
	free(hostname);

	//Curl request
	data.size = 0;
	data.data = malloc(128); /* reasonable size initial buffer */
	if (NULL == data.data) {
		Log_Debug("ERROR: Failed to allocate memory.\n");
		return false;
	}
	data.data[0] = '\0';

	curl = curl_easy_init();
	if (curl) {
		curl_easy_setopt(curl, CURLOPT_URL, request);
		if (useSSL) {
			curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
			curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);
		}
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, Networking_WriteData);
		curl_easy_setopt(curl, CURLOPT_WRITEDATA, &data);

		res = curl_easy_perform(curl);

		if (res != CURLE_OK)
			Log_IoT_Error(ERROR_CODE_CURL_ERROR, "ERROR: curl_easy_perform() failed: %s\n", curl_easy_strerror(res));

		curl_easy_cleanup(curl);
	}

	if (data.data) {
		bool result = false;
		Log_Debug("INFO: %s\n", data.data);
		if (data.size <= IPV6_STR_MAX_SIZE) {
			memset(buffer, 0, sizeof(bufferSize));
			strcpy(buffer, data.data);
			//memcpy(buffer, data.data, data.size);
			result = true;
		}
		free(data.data);
		return result;
	}
	else
		return false;

	return true;
}