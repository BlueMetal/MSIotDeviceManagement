#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>
#include "azure_c_shared_utility/platform.h"
#include "azure_c_shared_utility/threadapi.h"
#include "iothub_client.h"
#include "iothubtransportmqtt.h"
#include "jsondecoder.h"
#include "jsonencoder.h"
#include "azure_iot_hub.h"
#include "constants.h"
#include "state.h"
#include "utils.h"
#include "log.h"
#include "network.h"

#include <applibs/log.h>
#include <applibs/config.h>

const char AzureIoTCertificatesX[] =
/* Baltimore */
"-----BEGIN CERTIFICATE-----\r\n"
"MIIDdzCCAl+gAwIBAgIEAgAAuTANBgkqhkiG9w0BAQUFADBaMQswCQYDVQQGEwJJ\r\n"
"RTESMBAGA1UEChMJQmFsdGltb3JlMRMwEQYDVQQLEwpDeWJlclRydXN0MSIwIAYD\r\n"
"VQQDExlCYWx0aW1vcmUgQ3liZXJUcnVzdCBSb290MB4XDTAwMDUxMjE4NDYwMFoX\r\n"
"DTI1MDUxMjIzNTkwMFowWjELMAkGA1UEBhMCSUUxEjAQBgNVBAoTCUJhbHRpbW9y\r\n"
"ZTETMBEGA1UECxMKQ3liZXJUcnVzdDEiMCAGA1UEAxMZQmFsdGltb3JlIEN5YmVy\r\n"
"VHJ1c3QgUm9vdDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKMEuyKr\r\n"
"mD1X6CZymrV51Cni4eiVgLGw41uOKymaZN+hXe2wCQVt2yguzmKiYv60iNoS6zjr\r\n"
"IZ3AQSsBUnuId9Mcj8e6uYi1agnnc+gRQKfRzMpijS3ljwumUNKoUMMo6vWrJYeK\r\n"
"mpYcqWe4PwzV9/lSEy/CG9VwcPCPwBLKBsua4dnKM3p31vjsufFoREJIE9LAwqSu\r\n"
"XmD+tqYF/LTdB1kC1FkYmGP1pWPgkAx9XbIGevOF6uvUA65ehD5f/xXtabz5OTZy\r\n"
"dc93Uk3zyZAsuT3lySNTPx8kmCFcB5kpvcY67Oduhjprl3RjM71oGDHweI12v/ye\r\n"
"jl0qhqdNkNwnGjkCAwEAAaNFMEMwHQYDVR0OBBYEFOWdWTCCR1jMrPoIVDaGezq1\r\n"
"BE3wMBIGA1UdEwEB/wQIMAYBAf8CAQMwDgYDVR0PAQH/BAQDAgEGMA0GCSqGSIb3\r\n"
"DQEBBQUAA4IBAQCFDF2O5G9RaEIFoN27TyclhAO992T9Ldcw46QQF+vaKSm2eT92\r\n"
"9hkTI7gQCvlYpNRhcL0EYWoSihfVCr3FvDB81ukMJY2GQE/szKN+OMY3EU/t3Wgx\r\n"
"jkzSswF07r51XgdIGn9w/xZchMB5hbgF/X++ZRGjD8ACtPhSNzkE1akxehi/oCr0\r\n"
"Epn3o0WC4zxe9Z2etciefC7IpJ5OCBRLbf1wbWsaY71k5h+3zvDyny67G7fyUIhz\r\n"
"ksLi4xaNmjICq44Y3ekQEe5+NauQrz4wlHrQMz2nZQ/1/I6eYs9HRCwBXbsdtTLS\r\n"
"R9I4LtD+gdwyah617jzV/OeBHRnDJELqYzmp\r\n"
"-----END CERTIFICATE-----\r\n"
/* MSIT */
"-----BEGIN CERTIFICATE-----\r\n"
"MIIFhjCCBG6gAwIBAgIEByeaqTANBgkqhkiG9w0BAQsFADBaMQswCQYDVQQGEwJJ\r\n"
"RTESMBAGA1UEChMJQmFsdGltb3JlMRMwEQYDVQQLEwpDeWJlclRydXN0MSIwIAYD\r\n"
"VQQDExlCYWx0aW1vcmUgQ3liZXJUcnVzdCBSb290MB4XDTEzMTIxOTIwMDczMloX\r\n"
"DTE3MTIxOTIwMDY1NVowgYsxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5n\r\n"
"dG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9y\r\n"
"YXRpb24xFTATBgNVBAsTDE1pY3Jvc29mdCBJVDEeMBwGA1UEAxMVTWljcm9zb2Z0\r\n"
"IElUIFNTTCBTSEEyMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA0eg3\r\n"
"p3aKcEsZ8CA3CSQ3f+r7eOYFumqtTicN/HJq2WwhxGQRlXMQClwle4hslAT9x9uu\r\n"
"e9xKCLM+FvHQrdswbdcaHlK1PfBHGQPifaa9VxM/VOo6o7F3/ELwY0lqkYAuMEnA\r\n"
"iusrr/466wddBvfp/YQOkb0JICnobl0JzhXT5+/bUOtE7xhXqwQdvDH593sqE8/R\r\n"
"PVGvG8W1e+ew/FO7mudj3kEztkckaV24Rqf/ravfT3p4JSchJjTKAm43UfDtWBpg\r\n"
"lPbEk9jdMCQl1xzrGZQ1XZOyrqopg3PEdFkFUmed2mdROQU6NuryHnYrFK7sPfkU\r\n"
"mYsHbrznDFberL6u23UykJ5jvXS/4ArK+DSWZ4TN0UI4eMeZtgzOtg/pG8v0Wb4R\r\n"
"DsssMsj6gylkeTyLS/AydGzzk7iWa11XWmjBzAx5ihne9UkCXgiAAYkMMs3S1pbV\r\n"
"S6Dz7L+r9H2zobl82k7X5besufIlXwHLjJaoKK7BM1r2PwiQ3Ov/OdgmyBKdHJqq\r\n"
"qcAWjobtZ1KWAH8Nkj092XA25epCbx+uleVbXfjQOsfU3neG0PyeTuLiuKloNwnE\r\n"
"OeOFuInzH263bR9KLxgJb95KAY8Uybem7qdjnzOkVHxCg2i4pd+/7LkaXRM72a1o\r\n"
"/SAKVZEhZPnXEwGgCF1ZiRtEr6SsxwUQ+kFKqPsCAwEAAaOCASAwggEcMBIGA1Ud\r\n"
"EwEB/wQIMAYBAf8CAQAwUwYDVR0gBEwwSjBIBgkrBgEEAbE+AQAwOzA5BggrBgEF\r\n"
"BQcCARYtaHR0cDovL2N5YmVydHJ1c3Qub21uaXJvb3QuY29tL3JlcG9zaXRvcnku\r\n"
"Y2ZtMA4GA1UdDwEB/wQEAwIBhjAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUH\r\n"
"AwIwHwYDVR0jBBgwFoAU5Z1ZMIJHWMys+ghUNoZ7OrUETfAwQgYDVR0fBDswOTA3\r\n"
"oDWgM4YxaHR0cDovL2NkcDEucHVibGljLXRydXN0LmNvbS9DUkwvT21uaXJvb3Qy\r\n"
"MDI1LmNybDAdBgNVHQ4EFgQUUa8kJpz0aCJXgCYrO0ZiFXsezKUwDQYJKoZIhvcN\r\n"
"AQELBQADggEBAHaFxSMxH7Rz6qC8pe3fRUNqf2kgG4Cy+xzdqn+I0zFBNvf7+2ut\r\n"
"mIx4H50RZzrNS+yovJ0VGcQ7C6eTzuj8nVvoH8tWrnZDK8cTUXdBqGZMX6fR16p1\r\n"
"xRspTMn0baFeoYWTFsLLO6sUfUT92iUphir+YyDK0gvCNBW7r1t/iuCq7UWm6nnb\r\n"
"2DVmVEPeNzPR5ODNV8pxsH3pFndk6FmXudUu0bSR2ndx80oPSNI0mWCVN6wfAc0Q\r\n"
"negqpSDHUJuzbEl4K1iSZIm4lTaoNKrwQdKVWiRUl01uBcSVrcR6ozn7eQaKm6ZP\r\n"
"2SL6RE4288kPpjnngLJev7050UblVUfbvG4=\r\n"
"-----END CERTIFICATE-----\r\n"
/* *.azure-devices.net */
"-----BEGIN CERTIFICATE-----\r\n"
"MIIGcjCCBFqgAwIBAgITWgABtrNbz7vBeV0QWwABAAG2szANBgkqhkiG9w0BAQsF\r\n"
"ADCBizELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcT\r\n"
"B1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEVMBMGA1UE\r\n"
"CxMMTWljcm9zb2Z0IElUMR4wHAYDVQQDExVNaWNyb3NvZnQgSVQgU1NMIFNIQTIw\r\n"
"HhcNMTUwODI3MDMxODA0WhcNMTcwODI2MDMxODA0WjAeMRwwGgYDVQQDDBMqLmF6\r\n"
"dXJlLWRldmljZXMubmV0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA\r\n"
"nXC/qBUdlnfIm5K3HYu0o/Mb5tNNcsr0xy4Do0Puwq2W1tz0ZHvIIS9VOANhkNCb\r\n"
"VyOncnP6dvmM/rYYKth/NQ8RUiZOYlROZ0SYC8cvxq9WOln4GXtEU8vNVqJbYrJj\r\n"
"rPMHfxqLzTE/0ZnQffnDT3iMUE9kFLHow0YgaSRU0KZsc9KAROmzBzu+QIB1WGKX\r\n"
"D7CN361tG1UuN68Bz7MSnbgk98Z+DjDxfusoDhiiy/Y9MLOJMt4WIy5BqL3lfLnn\r\n"
"r+JLqmpiFuyVUDacFQDprYJ1/AFgcsKYu/ydmASARPzqJhOGaC2sZP0U5oBOoBzI\r\n"
"bz4tfn8Bi0kJKmS53mQt+wIDAQABo4ICOTCCAjUwCwYDVR0PBAQDAgSwMB0GA1Ud\r\n"
"JQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAdBgNVHQ4EFgQUKpYehBSNA53Oxivn\r\n"
"aLCz3+eFUJ0wXQYDVR0RBFYwVIITKi5henVyZS1kZXZpY2VzLm5ldIIaKi5hbXFw\r\n"
"d3MuYXp1cmUtZGV2aWNlcy5uZXSCISouc3UubWFuYWdlbWVudC1henVyZS1kZXZp\r\n"
"Y2VzLm5ldDAfBgNVHSMEGDAWgBRRryQmnPRoIleAJis7RmIVex7MpTB9BgNVHR8E\r\n"
"djB0MHKgcKBuhjZodHRwOi8vbXNjcmwubWljcm9zb2Z0LmNvbS9wa2kvbXNjb3Jw\r\n"
"L2NybC9tc2l0d3d3Mi5jcmyGNGh0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kv\r\n"
"bXNjb3JwL2NybC9tc2l0d3d3Mi5jcmwwcAYIKwYBBQUHAQEEZDBiMDwGCCsGAQUF\r\n"
"BzAChjBodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL21zY29ycC9tc2l0d3d3\r\n"
"Mi5jcnQwIgYIKwYBBQUHMAGGFmh0dHA6Ly9vY3NwLm1zb2NzcC5jb20wTgYDVR0g\r\n"
"BEcwRTBDBgkrBgEEAYI3KgEwNjA0BggrBgEFBQcCARYoaHR0cDovL3d3dy5taWNy\r\n"
"b3NvZnQuY29tL3BraS9tc2NvcnAvY3BzADAnBgkrBgEEAYI3FQoEGjAYMAoGCCsG\r\n"
"AQUFBwMBMAoGCCsGAQUFBwMCMA0GCSqGSIb3DQEBCwUAA4ICAQCrjzOSW+X6v+UC\r\n"
"u+JkYyuypXN14pPLcGFbknJWj6DAyFWXKC8ihIYdtf/szWIO7VooplSTZ05u/JYu\r\n"
"ZYh7fAw27qih9CLhhfncXi5yzjgLMlD0mlbORvMJR/nMl7Yh1ki9GyLnpOqMmO+E\r\n"
"yTpOiE07Uyt2uWelLHjMY8kwy2bSRXIp7/+A8qHRaIIdXNtAKIK5jo068BJpo77h\r\n"
"4PljCb9JFdEt6sAKKuaP86Y+8oRZ7YzU4TLDCiK8P8n/gQXH0vvhOE/O0n7gWPqB\r\n"
"n8KxsnRicop6tB6GZy32Stn8w0qktmQNXOGU+hp8OL6irULWZw/781po6d78nmwk\r\n"
"1IFl2TB4+jgyblvJdTM0rx8vPf3F2O2kgsRNs9M5qCI7m+he43Bhue0Fj/h3oIIo\r\n"
"Qx7X/uqc8j3VTNE9hf2A4wksSRgRydjAYoo+bduNagC5s7Eucb4mBG0MMk7HAQU9\r\n"
"m/gyaxqth6ygDLK58wojSV0i4RiU01qZkHzqIWv5FhhMjbFwyKEc6U35Ps7kP/1O\r\n"
"fdGm13ONaYqDl44RyFsLFFiiDYxZFDSsKM0WDxbl9ULAlVc3WR85kEBK6I+pSQj+\r\n"
"7/Z5z2zTz9qOFWgB15SegTbjSR7uk9mEVnj9KDlGtG8W1or0EGrrEDP2CMsp0oEj\r\n"
"VTJbZAxEaZ3cVCKva5sQUxFMjwG32g==\r\n"
"-----END CERTIFICATE-----\r\n";

static char* retrieveKeys();
static void sendMessageCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* context);
static IOTHUBMESSAGE_DISPOSITION_RESULT receiveMessageCallback(IOTHUB_MESSAGE_HANDLE message, void* context);
static void twinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char* payLoad, size_t size, void* userContextCallback);
static int deviceMethodCallback(const char *methodName, const unsigned char* payload, size_t size, unsigned char** response, size_t *response_size, void* userContextCallback);

static IOTHUB_CLIENT_LL_HANDLE iothub_client_handle = NULL;

/// <summary>
///     Establishes a connection to Azure IoT Hub.
/// </summary>
bool AzureIoT_ClientConnect(void)
{
	if (platform_init() != 0)
	{
		Log_Debug("ERROR: Failed initializing platform.\n");
		return false;
	}

	char* updatedConnectionString = retrieveKeys();

	if (updatedConnectionString == NULL)
	{
		Log_Debug("ERROR: Failed to allocate buffer for connection string\n");
		return false;
	}

	iothub_client_handle = IoTHubClient_LL_CreateFromConnectionString(updatedConnectionString, MQTT_Protocol);

	free(updatedConnectionString);

	if (iothub_client_handle == NULL)
	{
		return false;
	}

	IOTHUB_CLIENT_RESULT azureRes = IoTHubClient_LL_SetOption(iothub_client_handle, "TrustedCerts", AzureIoTCertificatesX);

	if (azureRes != IOTHUB_CLIENT_OK)
	{
		Log_Debug("ERROR: Failure to set option \"TrustedCerts\"\n");
		return false;
	}

	// set callbacks
	IoTHubClient_LL_SetMessageCallback(iothub_client_handle, receiveMessageCallback, NULL);
	IoTHubClient_LL_SetDeviceMethodCallback(iothub_client_handle, deviceMethodCallback, NULL);
	IoTHubClient_LL_SetDeviceTwinCallback(iothub_client_handle, twinCallback, NULL);


	return true;
}

/// <summary>
///     Generate a connection string based on the appconfig store properties
/// </summary>
static char* retrieveKeys()
{
	char* updated = (char*)malloc(MAX_CONNECTION_STRING_SIZE * 4);

	if (updated != NULL)
	{
		char* hostName = updated + MAX_CONNECTION_STRING_SIZE;
		char* deviceId = hostName + MAX_CONNECTION_STRING_SIZE;
		char* sharedAccessKey = deviceId + MAX_CONNECTION_STRING_SIZE;

		if (Config_ReadString(CONFIG_PARAM_HOSTNAME_IOT, hostName, MAX_CONNECTION_STRING_SIZE) == Result_Success)
		{
			if (Config_ReadString(CONFIG_PARAM_DEVICE_ID, deviceId, MAX_CONNECTION_STRING_SIZE) == Result_Success)
			{
				if (Config_ReadString(CONFIG_PARAM_SHARED_ACCESS_KEY, sharedAccessKey, MAX_CONNECTION_STRING_SIZE) == Result_Success)
				{
					snprintf(updated, MAX_CONNECTION_STRING_SIZE, CONFIG_CONNECTION_KEY, hostName, deviceId, sharedAccessKey);
					return updated;
				}
				else
				{
					Log_Debug("ERROR: failed to get %s\n", CONFIG_PARAM_SHARED_ACCESS_KEY);
				}
			}
			else
			{
				Log_Debug("ERROR: failed to get %s\n", CONFIG_PARAM_DEVICE_ID);
			}
		}
		else
		{
			Log_Debug("ERROR: failed to get %s\n", CONFIG_PARAM_HOSTNAME_IOT);
		}

		free(updated);
	}

	return NULL;
}

/// <summary>
///     Disconnect and destroys all resources.
/// </summary>
void AzureIoT_ClientDisconnect(void)
{
	if (iothub_client_handle != NULL)
	{
		IoTHubClient_LL_Destroy(iothub_client_handle);
		iothub_client_handle = NULL;

		platform_deinit();
	}
}

/*static void PeriodicLog(time_t *lastInvokedTime, time_t periodInSecond, const char *format, ...)
{
	struct timespec ts;
	int timeOk = timespec_get(&ts, TIME_UTC);
	if (timeOk) {
		if (ts.tv_sec > *lastInvokedTime + periodInSecond) {
			char outString[128];
			va_list args;
			va_start(args, format);
			vsnprintf(outString, sizeof(outString), format, args);
			va_end(args);
			*lastInvokedTime = ts.tv_sec;
			Log_Debug(outString);
		}
	}
}*/

/// <summary>
///     Performs IoT Hub client processing.
/// </summary>
void AzureIoT_DoPeriodicTasks(void)
{
	//static time_t lastTimeLogged;
	//PeriodicLog(&lastTimeLogged, 5, "%s calls in progress...\n", __func__);

	IoTHubClient_LL_DoWork(iothub_client_handle);
}

/// <summary>
///     Sends single message to IoT Hub.
/// </summary>
bool AzureIoT_SendMessage(char* message)
{
	bool result;

	if (iothub_client_handle == NULL)
	{
		result = false;
		Log_Debug("ERROR: IoT Hub client not initialized\n");
	}
	else
	{
		IOTHUB_MESSAGE_HANDLE message_handle = IoTHubMessage_CreateFromByteArray(message, strlen(message));

		if (message_handle == 0)
		{
			result = false;
			Log_Debug("ERROR: Unable to create a new IoTHubMessage\n");
		}
		else
		{
			if (IoTHubClient_LL_SendEventAsync(iothub_client_handle, message_handle, sendMessageCallback, /*&callback_param*/0) != IOTHUB_CLIENT_OK)
			{
				result = false;
				Log_Debug("ERROR: Failed to hand over the message to IoTHubClient");
			}
			else
			{
				result = true;
				Log_Debug("INFO: IoTHubClient accepted the message for delivery\n");
			}
		}

		IoTHubMessage_Destroy(message_handle);
	}
	return result;
}

/// <summary>
///     Callback when device twin reported properties are accepted by IoT Hub.
/// </summary>
static void reportStatusCallback(int result, void* context)
{
	Log_Debug("INFO: Reported state accepted by IoT Hub. Result is: %d\n", result);
}

/// <summary>
///     Empty clone function for multitree.
/// </summary>
static int NOPCloneFunction(void** destination, const void* source)
{
	*destination = (void**)source;
	return 0;
}

/// <summary>
///     Empty free function for multitree.
/// </summary>
static void NoFreeFunction(void* value)
{
	(void)value;
}

/// <summary>
///     Updates device twin reported properties.
/// </summary>
void AzureIoT_TwinReportStateString(const char *propertyName, const char *propertyValue)
{
	if (iothub_client_handle == NULL)
	{
		Log_Debug("ERROR: Not initialized\n");
	}
	else
	{
		MULTITREE_HANDLE tree = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

		if (tree != NULL)
		{
			MULTITREE_RESULT mtreeResult = MULTITREE_OK;

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddLeaf(tree, propertyName, propertyValue);
			}


			if (MULTITREE_OK == mtreeResult)
			{
				STRING_HANDLE res = STRING_new();

				if (res != NULL)
				{
					if (JSON_ENCODER_OK == JSONEncoder_EncodeTree(tree, res, JSONEncoder_CharPtr_ToString))
					{
						const char* json = STRING_c_str(res);

						if (IoTHubClient_LL_SendReportedState(iothub_client_handle, (unsigned char*)json, strlen(json), reportStatusCallback, 0) != IOTHUB_CLIENT_OK)
						{
							Log_Debug("ERROR: Failed to set reported state\n");
						}
						else
						{
							Log_Debug("INFO: Reported state set\n");
						}
					}
					else
					{
						Log_Debug("ERROR: Failed to encode JSON\n");
					}

					STRING_delete(res);
				}
				else
				{
					Log_Debug("ERROR: Failed to allocate buffer for JSON\n");
				}
			}

			MultiTree_Destroy(tree);
		}
		else
		{
			Log_Debug("ERROR: Failed to allocate multitree\n");
		}
	}
}

/// <summary>
///     Updates device twin reported properties of a feature.
/// </summary>
void AzureIoT_TwinReportStateFeature(const char *featureName, const char *propertyName, const char *propertyValue)
{
	if (iothub_client_handle == NULL)
	{
		Log_Debug("ERROR: Not initialized\n");
	}
	else
	{
		MULTITREE_HANDLE tree = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

		if (tree != NULL)
		{
			MULTITREE_RESULT mtreeResult = MULTITREE_OK;
			MULTITREE_HANDLE features = MultiTree_Create(NOPCloneFunction, NoFreeFunction);
			MULTITREE_HANDLE child = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddChild(tree, REPORTED_FEATURES_DEFINITIONS, &features);
			}

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddChild(features, featureName, &child);
			}

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddLeaf(child, propertyName, propertyValue);

				if (MULTITREE_OK == mtreeResult)
				{
					STRING_HANDLE res = STRING_new();

					if (res != NULL)
					{
						if (JSON_ENCODER_OK == JSONEncoder_EncodeTree(tree, res, JSONEncoder_CharPtr_ToString))
						{
							const char* json = STRING_c_str(res);

							if (IoTHubClient_LL_SendReportedState(iothub_client_handle, (unsigned char*)json, strlen(json), reportStatusCallback, 0) != IOTHUB_CLIENT_OK)
							{
								Log_Debug("ERROR: Failed to set reported state\n");
							}
							else
							{
								Log_Debug("INFO: Reported state set\n");
							}
						}
						else
						{
							Log_Debug("ERROR: Failed to encode JSON\n");
						}

						STRING_delete(res);
					}
					else
					{
						Log_Debug("ERROR: Failed to allocate buffer for JSON\n");
					}
				}
				//MultiTree_Destroy(child);
			}
			else
			{
				Log_Debug("ERROR: Failed to create child node %s\n", featureName);
			}

			MultiTree_Destroy(tree);
		}
		else
		{
			Log_Debug("ERROR: Failed to allocate multitree\n");
		}
	}
}

/// <summary>
///     Updates device twin reported properties with a sub node.
/// </summary>
void AzureIoT_TwinReportStateWithChild(const char *childName, const char *propertyName, const char *propertyValue)
{
	if (iothub_client_handle == NULL)
	{
		Log_Debug("ERROR: Not initialized\n");
	}
	else
	{
		MULTITREE_HANDLE tree = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

		if (tree != NULL)
		{
			MULTITREE_RESULT mtreeResult = MULTITREE_OK;
			MULTITREE_HANDLE child = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddChild(tree, childName, &child);
			}

			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddLeaf(child, propertyName, propertyValue);

				if (MULTITREE_OK == mtreeResult)
				{
					STRING_HANDLE res = STRING_new();

					if (res != NULL)
					{
						if (JSON_ENCODER_OK == JSONEncoder_EncodeTree(tree, res, JSONEncoder_CharPtr_ToString))
						{
							const char* json = STRING_c_str(res);

							if (IoTHubClient_LL_SendReportedState(iothub_client_handle, (unsigned char*)json, strlen(json), reportStatusCallback, 0) != IOTHUB_CLIENT_OK)
							{
								Log_Debug("ERROR: Failed to set reported state\n");
							}
							else
							{
								Log_Debug("INFO: Reported state set\n");
							}
						}
						else
						{
							Log_Debug("ERROR: Failed to encode JSON\n");
						}

						STRING_delete(res);
					}
					else
					{
						Log_Debug("ERROR: Failed to allocate buffer for JSON\n");
					}
				}
				//MultiTree_Destroy(child);
			}
			else
			{
				Log_Debug("ERROR: Failed to create child node %s\n", childName);
			}

			MultiTree_Destroy(tree);
		}
		else
		{
			Log_Debug("ERROR: Failed to allocate multitree\n");
		}
	}
}

/// <summary>
///     Updates device twin reported properties.
/// </summary>
void AzureIoT_TwinReportStateDevice(const state_variable *variable) {
	if (!variable->report)
		return;

	const char* propertyValue = Utils_GetVariableStringValue(variable);
	AzureIoT_TwinReportStateWithChild(DESIRED_DEVICE_STATE, variable->varname, propertyValue);
}

/// <summary>
///     Updates device twin reported properties features during initialization
/// </summary>
MULTITREE_RESULT AzureIoT_InitFeatureReportState(MULTITREE_HANDLE treeNode, const char *featureName, const char *featureDisplayName, const char *featureMethods, const bool featureIsActivated, const bool featureIsInternalOnly)
{
	MULTITREE_RESULT mtreeResult = MULTITREE_OK;
	MULTITREE_HANDLE child = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

	mtreeResult = MultiTree_AddChild(treeNode, featureName, &child);

	if (mtreeResult == MULTITREE_OK) {
		mtreeResult = MultiTree_AddLeaf(child, REPORTED_FEATURES_DEFINITIONS_DISPLAY_NAME, featureDisplayName);

		if (mtreeResult == MULTITREE_OK)
			mtreeResult = MultiTree_AddLeaf(child, REPORTED_FEATURES_DEFINITIONS_METHODS, featureMethods);

		if (mtreeResult == MULTITREE_OK)
			mtreeResult = MultiTree_AddLeaf(child, REPORTED_FEATURES_DEFINITIONS_IS_ACTIVATED, featureIsActivated ? "true" : "false");

		if (mtreeResult == MULTITREE_OK)
			mtreeResult = MultiTree_AddLeaf(child, REPORTED_FEATURES_DEFINITIONS_INTERNAL_USE, featureIsInternalOnly ? "true" : "false");
	}
	else
	{
		Log_Debug("ERROR: Failed to create child node %s\n", featureName);
	}

	return mtreeResult;
}

/// <summary>
///     Updates device twin reported properties during initialization
/// </summary>
void AzureIoT_InitReportState()
{
	if (iothub_client_handle == NULL)
	{
		Log_Debug("ERROR: Not initialized\n");
	}
	else
	{
		MULTITREE_HANDLE tree = MultiTree_Create(NOPCloneFunction, NoFreeFunction);

		if (tree != NULL)
		{
			char str_heartbeat[FORMAT_TIME_STR_MAX_SIZE];
			char str_ip_address[IPV6_STR_MAX_SIZE];
			char **str_device_properties;
			MULTITREE_RESULT mtreeResult = MULTITREE_OK;
			MULTITREE_HANDLE child_device = MultiTree_Create(NOPCloneFunction, NoFreeFunction);
			MULTITREE_HANDLE child_feature = MultiTree_Create(NOPCloneFunction, NoFreeFunction);
			
			str_device_properties = malloc(STATE_VALUES_NBR * sizeof(char*));
			if (str_device_properties == NULL) {
				Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
				return;
			}

			//Init heartbeat
			if (mtreeResult == MULTITREE_OK) {
				Utils_GetFormattedDateFromTime(str_heartbeat, sizeof(str_heartbeat), time(NULL));
				mtreeResult = MultiTree_AddLeaf(tree, REPORTED_HEARTBEAT, str_heartbeat);
			}
			//Init activation date
			if (mtreeResult == MULTITREE_OK) {
				char activationDate[FORMAT_TIME_STR_MAX_SIZE];

				if (Config_ReadString(REPORTED_ACTIVATION_DATE, activationDate, FORMAT_TIME_STR_MAX_SIZE) == Result_Config_KeyNotFound) {
					mtreeResult = MultiTree_AddLeaf(tree, REPORTED_ACTIVATION_DATE, str_heartbeat);
					Config_WriteString(REPORTED_ACTIVATION_DATE, str_heartbeat);
				}
			}
			//Init status code
			if (mtreeResult == MULTITREE_OK)
				mtreeResult = MultiTree_AddLeaf(tree, REPORTED_STATUSCODE, STATUSCODE_INIT);
			//Init firmware
			if (mtreeResult == MULTITREE_OK)
				mtreeResult = MultiTree_AddLeaf(tree, REPORTED_FIRMWARE_VERSION, FIRM_VERSION);
			//Init IP Address
			if (mtreeResult == MULTITREE_OK) {
				if(Networking_GetIPAddressFromPortalManager(str_ip_address, sizeof(str_ip_address)))
					mtreeResult = MultiTree_AddLeaf(tree, REPORTED_IP_ADDRESS, str_ip_address);
			}

			//Device Specific node
			if (mtreeResult == MULTITREE_OK)
			{
				mtreeResult = MultiTree_AddChild(tree, REPORTED_DEVICE_STATE, &child_device);
				for (uint8_t i = 0; i < STATE_VALUES_NBR; i++) {
					if (!STATE_VALUES[i].report)
						continue;

					if (mtreeResult == MULTITREE_OK) {
						switch (STATE_VALUES[i].type) {
						case BYTE:
							str_device_properties[i] = malloc(UINT8_STR_MAX_SIZE * sizeof(char));
							if (str_device_properties[i] == NULL) {
								Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
								return;
							}
							Utils_GetStringFromUINT8(str_device_properties[i], UINT8_STR_MAX_SIZE, STATE_VALUES[i].valByte);
							break;
						case BOOL:
							str_device_properties[i] = malloc(UINT8_STR_MAX_SIZE * sizeof(char));
							if (str_device_properties[i] == NULL) {
								Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
								return;
							}
							Utils_GetStringFromUINT8(str_device_properties[i], UINT8_STR_MAX_SIZE, STATE_VALUES[i].valBool);
							break;
						case SHORT:
							str_device_properties[i] = malloc(UINT16_STR_MAX_SIZE * sizeof(char));
							if (str_device_properties[i] == NULL) {
								Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
								return;
							}
							Utils_GetStringFromUINT16(str_device_properties[i], UINT16_STR_MAX_SIZE, STATE_VALUES[i].valShort);
							break;
						case INT:
							str_device_properties[i] = malloc(UINT32_STR_MAX_SIZE * sizeof(char));
							if (str_device_properties[i] == NULL) {
								Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
								return;
							}
							Utils_GetStringFromUINT32(str_device_properties[i], UINT32_STR_MAX_SIZE, STATE_VALUES[i].valInt);
							break;
						case TIME:
							str_device_properties[i] = malloc(TIME_STR_MAX_SIZE * sizeof(char));
							if (str_device_properties[i] == NULL) {
								Log_Debug("ERROR: Could not allocate memory for deviceProperties.\n");
								return;
							}
							Utils_GetStringFromTime(str_device_properties[i], TIME_STR_MAX_SIZE, STATE_VALUES[i].valTime);
							break;
						}
						mtreeResult = MultiTree_AddLeaf(child_device, STATE_VALUES[i].varname, str_device_properties[i]);
					}
				}
			}
			else {
				Log_Debug("ERROR: Failed to create child node %s\n", REPORTED_DEVICE_STATE);
			}
			//MultiTree_Destroy(child_device);

			//Features Definitions node
			if (mtreeResult == MULTITREE_OK) {
				mtreeResult = MultiTree_AddChild(tree, REPORTED_FEATURES_DEFINITIONS, &child_feature);

				for (uint8_t i = 0; i < FEATURES_NBR; i++)
				{
					if (mtreeResult == MULTITREE_OK)
						mtreeResult = AzureIoT_InitFeatureReportState(child_feature, FEATURES[i].name, FEATURES[i].displayName, FEATURES[i].methods, FEATURES[i].state.valBool, FEATURES[i].internal_use);
				}
			}
			else
			{
				Log_Debug("ERROR: Failed to create child node %s\n", REPORTED_FEATURES_DEFINITIONS);
			}
			//MultiTree_Destroy(child_feature);


			if (MULTITREE_OK == mtreeResult)
			{
				STRING_HANDLE res = STRING_new();

				if (res != NULL)
				{
					if (JSON_ENCODER_OK == JSONEncoder_EncodeTree(tree, res, JSONEncoder_CharPtr_ToString))
					{
						const char* json = STRING_c_str(res);

						if (IoTHubClient_LL_SendReportedState(iothub_client_handle, (unsigned char*)json, strlen(json), reportStatusCallback, 0) != IOTHUB_CLIENT_OK)
						{
							Log_Debug("ERROR: Failed to set reported state\n");
						}
						else
						{
							Log_Debug("INFO: Reported state set\n");
						}
					}
					else
					{
						Log_Debug("ERROR: Failed to encode JSON\n");
					}

					STRING_delete(res);
				}
				else
				{
					Log_Debug("ERROR: Failed to allocate buffer for JSON\n");
				}
			}

			MultiTree_Destroy(tree);

			for (uint8_t i = 0; i < STATE_VALUES_NBR; i++) {
				if (!STATE_VALUES[i].report)
					continue;
				free(str_device_properties[i]);
			}
			free(str_device_properties);
		}
		else
		{
			Log_Debug("ERROR: Failed to allocate multitree\n");
		}
	}
}

static MessageReceivedFromIoTHubFnType messageReceivedFromIoTHubCb = 0;

void AzureIoT_SetMessageReceivedFromIoTHubCallback(MessageReceivedFromIoTHubFnType fn)
{
	messageReceivedFromIoTHubCb = fn;
}


/// <summary>
///     Callback when sending message to IoT Hub is completed.
/// </summary>
static void sendMessageCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* context)
{
	Log_Debug("INFO: Message received by IoT Hub. Result is: %d\n", result);
}

/// <summary>
///     Callback when message is received from IoT Hub.
/// </summary>
static IOTHUBMESSAGE_DISPOSITION_RESULT receiveMessageCallback(IOTHUB_MESSAGE_HANDLE message, void* context)
{
	const unsigned char* buffer;
	size_t size;
	if (IoTHubMessage_GetByteArray(message, &buffer, &size) != IOTHUB_MESSAGE_OK)
	{
		Log_Debug("ERROR: Unable to IoTHubMessage_GetByteArray\n");
	}
	else
	{
		/*buffer is not zero terminated*/
		unsigned char* str_msg = malloc(size + 1);

		if (str_msg != NULL)
		{
			memcpy(str_msg, buffer, size);
			str_msg[size] = '\0';

			if (messageReceivedFromIoTHubCb != 0)
				messageReceivedFromIoTHubCb(str_msg);
			else
				Log_Debug("INFO: No user callback set up for event 'message received from IoT Hub' \n");

			Log_Debug("INFO: Received message '%s' from IoT Hub\n", str_msg);
			free(str_msg);
		}
		else
		{
			Log_Debug("ERROR: Couldn't allocate buffer for received message\n");
		}
	}

	return IOTHUBMESSAGE_ACCEPTED;
}

/// <summary>
///     Function to be called whenever a Direct Method call is received from the IoT Hub.
/// </summary>
static DirectMethodCallFromIoTHubFnType directMethodCallFromIoTHubCb = 0;

void AzureIoT_SetDirectMethodCallFromIoTHubCallback(DirectMethodCallFromIoTHubFnType fn)
{
	directMethodCallFromIoTHubCb = fn;
}

/// <summary>
///     Function to be called whenever a Twin Device update is received from the IoT Hub.
/// </summary>
static TwinUpdateFromIoTHubFnType twinUpdateFromIoTHubCb = 0;

void AzureIoT_SetDeviceTwinUpdateFromIoTHubCallback(TwinUpdateFromIoTHubFnType fn)
{
	twinUpdateFromIoTHubCb = fn;
}


/// <summary>
///     Callback when device method is called.
/// </summary>
static int deviceMethodCallback(const char *methodName,
	const unsigned char *payload,
	size_t size,
	unsigned char **response,
	size_t *response_size,
	void *userContextCallback)
{
	bool method_found = false;
	*response_size = 0;
	char* payload_clean = (unsigned char *)malloc(size + 1);
	strncpy(payload_clean, payload, size);
	payload_clean[size] = '\0';

	Log_Debug("INFO: Try to invoke method %s\n", methodName);
	const char *responseMessage = "{}";
	int result = 200;

	for (uint8_t i = 0; i < METHODS_NBR; i++) {
		if (strcmp(methodName, METHODS[i].name) == 0)
		{
			if (directMethodCallFromIoTHubCb)
				directMethodCallFromIoTHubCb(methodName, payload_clean, response, response_size);
			Log_Debug("INFO: Method %s called...\n", METHODS[i].name);
			method_found = true;
		}
	}

	if(!method_found)
	{
		Log_Debug("ERROR: No method %s found\r\n", methodName);
		responseMessage = METHOD_RESPONSE_FAIL;
		result = 404;
	}

	free(payload_clean);

	if (*response_size == 0) {
		*response_size = strlen(responseMessage);
		*response = (unsigned char *)malloc(*response_size);
		if (*response != NULL)
		{
			strncpy((char*)(*response), responseMessage, *response_size);
		}
	}

	return result;
}

/// <summary>
///     Process to the firing of a desired property (features or device) 
/// </summary>
static void twinDesiredPropertyProcess(const char *childName, const char *propertyName, MULTITREE_HANDLE subtree) {
	const void *value = NULL;
	if (MULTITREE_OK == MultiTree_GetLeafValue(subtree, propertyName, &value))
	{
		const char *valueString = (const char*)value;
		if (twinUpdateFromIoTHubCb)
			twinUpdateFromIoTHubCb(childName, propertyName, valueString);
		Log_Debug("INFO: Property %s changed, new value is %s\n", propertyName, valueString);
	}
}

/// <summary>
///     Callback when device twin update is received from IoT Hub.
/// </summary>
static void twinCallback(DEVICE_TWIN_UPDATE_STATE updateState,
	const unsigned char *payLoad,
	size_t size,
	void *userContextCallback)
{
	char *temp = (char *)malloc(size + 1);

	if (temp == NULL)
	{
		Log_Debug("ERROR: Couldn't allocate buffer for twin update payload\n");
		return;
	}

	for (int i = 0; i < size; i++)
	{
		temp[i] = (char)(payLoad[i]);
	}
	temp[size] = '\0';

	MULTITREE_HANDLE tree = NULL;

	if (JSON_DECODER_OK == JSONDecoder_JSON_To_MultiTree(temp, &tree))
	{
		MULTITREE_HANDLE child = NULL;
		MULTITREE_HANDLE features = NULL;
		MULTITREE_HANDLE device = NULL;

		if (MULTITREE_OK != MultiTree_GetChildByName(tree, DESIRED, &child))
		{
			Log_Debug("INFO: This device twin message contains desired message only\n");
			child = tree;
		}

		if (MULTITREE_OK == MultiTree_GetChildByName(child, DESIRED_FEATURES, &features))
		{
			for (uint8_t i = 0; i < FEATURES_NBR; i++)
				twinDesiredPropertyProcess(DESIRED_FEATURES, FEATURES[i].name, features);
			//MultiTree_Destroy(features);
		}

		if (MULTITREE_OK == MultiTree_GetChildByName(child, DESIRED_DEVICE_STATE, &device))
		{
			for (uint8_t i = 0; i < STATE_VALUES_NBR; i++)
				twinDesiredPropertyProcess(DESIRED_DEVICE_STATE, STATE_VALUES[i].varname, device);
			//MultiTree_Destroy(device);
		}
		//MultiTree_Destroy(child);
	}
	MultiTree_Destroy(tree);
	free(temp);
}