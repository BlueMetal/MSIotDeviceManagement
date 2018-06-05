#include <stdbool.h>
#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <time.h>
#include <applibs/uart.h>
#include <unistd.h>
#include <string.h>

#include <applibs/applibs_result.h>
#include <applibs/uart.h>
#include <applibs/sopris_uarts.h>
#include <applibs/gpio.h>
#include <applibs/sopris_pins.h>
#include <applibs/sopris_cdvb2_pins.h>
#include <applibs/log.h>
#include <applibs/lifecycle.h>
#include <applibs/config.h>
#include <applibs/networking.h>
#include "jsondecoder.h"
#include "state.h"
#include "constants.h"
#include "debug.h"
#include "azure_iot_hub.h"
#include "uart_commands.h"
#include "network.h"
#include "log.h"
#include "utils.h"

static GPIO_Handle _Device_GPIOBrewStrength;
static GPIO_Handle _Device_GPIOGrind;
static GPIO_Handle _Device_GPIOBrew;
static GPIO_Handle _Device_GPIOWakeUp;
static time_t _Device_TimerAction;

/// <summary>
///		Initialize a GPIO handle to be used with the device. Also make sure the state of the button is unpressed.
/// </summary>
bool Device_InitGPIOHandle(Applibs_Pin pin, GPIO_Handle *gpioHandle) {
	Applibs_Result result;

	result = GPIO_CreateHandle(pin, gpioHandle);
	if (result != Result_Success) {
		Log_Debug("ERROR: GPIO_handle handle not opened\n");
		return false;
	}
	result = GPIO_SetAsOutput(*gpioHandle);
	if (result != Result_Success) {
		Log_Debug("ERROR: GPIO_handle handle not set as output\n");
		return false;
	}
	result = GPIO_SetOutputValue(*gpioHandle, GPIO_Value_High);
	if (result != Result_Success) {
		Log_Debug("ERROR: GPIO_handle handle not set to high\n");
		return false;
	}

	return true;
}

/// <summary>
///		Destroy a GPIO handle.
/// </summary>
bool Device_DestroyGPIOHandle(GPIO_Handle *gpioHandle) {
	Applibs_Result result;
	
	result = GPIO_SetOutputValue(*gpioHandle, GPIO_Value_High);
	if (result != Result_Success) {
		Log_Debug("ERROR: GPIO_handle handle not set to high\n");
		return false;
	}

	result = GPIO_DestroyHandle(*gpioHandle);
	if (result != Result_Success) {
		Log_Debug("ERROR: GPIO_handle handle could not be destroyed\n");
		return false;
	}

	return true;
}

/// <summary>
///		Initialize all GPIO handles to be used with the device.
/// </summary>
bool Device_InitGPIOHandles() {
	Log_Debug("INFO: Creating gpio handles for coffee maker\n");
	if (!Device_InitGPIOHandle(SOPRIS_CDVB2_PIO_1_4, &_Device_GPIOBrewStrength))
		return false;
	if (!Device_InitGPIOHandle(SOPRIS_CDVB2_PIO_2_20, &_Device_GPIOGrind))
		return false;
	if (!Device_InitGPIOHandle(SOPRIS_CDVB2_PIO_2_21, &_Device_GPIOBrew))
		return false;
	if (!Device_InitGPIOHandle(SOPRIS_CDVB2_PIO_2_31, &_Device_GPIOWakeUp))
		return false;

	return true;
}

/// <summary>
///		Destroy all GPIO handles
/// </summary>
bool Device_DestroyGPIOHandles() {
	Log_Debug("INFO: Destroying gpio handles for coffee maker\n");
	Device_DestroyGPIOHandle(&_Device_GPIOBrewStrength);
	Device_DestroyGPIOHandle(&_Device_GPIOGrind);
	Device_DestroyGPIOHandle(&_Device_GPIOBrew);
	Device_DestroyGPIOHandle(&_Device_GPIOWakeUp);

	return true;
}

/// <summary>
///		Simulate the press of a button on the device
/// </summary>
bool Device_PressButton(GPIO_Handle gpioHandle) {
	Applibs_Result result;
	//struct timespec tim = { .tv_sec = 0,.tv_nsec = DEVICE_BUTTON_PRESS_TIME_NANO };
	//struct timespec timH = { .tv_sec = 0,.tv_nsec = DEVICE_BUTTON_PRESS_TIME_NANO };

	Log_Debug("INFO: Press button\n");
	result = GPIO_SetOutputValue(gpioHandle, GPIO_Value_Low);
	if (result != Result_Success) {
		Log_IoT_Error(ERROR_CODE_GPIO_ERROR, "ERROR: GPIO_handle handle not set to low\n");
		return false;
	}
	//nanosleep(&tim, NULL);
	sleep(1);
	result = GPIO_SetOutputValue(gpioHandle, GPIO_Value_High);
	//nanosleep(&timH, NULL);
	sleep(1);
	if (result != Result_Success) {
		Log_IoT_Error(ERROR_CODE_GPIO_ERROR, "ERROR: GPIO_handle handle not set to high\n");
		return false;
	}

	return true;
}

/// <summary>
///		Action to change the "Brew Strengh" value of the device.
///		0: Regular (default), 1: Bold, 2: Strong
/// </summary>
bool Device_ChangeBrewStrength(int newState) {
	state_variable *varBrewStrength;
	
	if (State_GetVariable(VALUE_STATE_BREW)->valBool || State_GetVariable(VALUE_STATE_BREW_GRIND)->valBool)
		return true; //Already an action in progress, ignore the call
	
	if (newState > 2)
		return false; //Invalid state

	varBrewStrength = State_GetVariable(VALUE_STATE_BREW_STRENGTH);

	//Wake up
	Device_PressButton(_Device_GPIOWakeUp);

	while (newState != varBrewStrength->valByte) {
		if (Device_PressButton(_Device_GPIOBrewStrength)) {
			varBrewStrength->valByte++;
			if (varBrewStrength->valByte > 2)
				varBrewStrength->valByte = 0;
		}
		else {
			return false;
		}
	}

	UART_CommandSendProperty(varBrewStrength);
	AzureIoT_TwinReportStateDevice(varBrewStrength);
	return true;
}

/// <summary>
///		Action to change the "Grind" value of the device.
///		0: Disabled, 1: 4, 2: 6, 3: 8, 4: 10, 5: 12 (default)
/// </summary>
bool Device_ChangeGrind(uint8_t newState) {
	if (newState > 5)
		return false; //Invalid state

	state_variable *varGrind = State_GetVariable(VALUE_STATE_GRIND);
	while (newState != varGrind->valByte) {
		if (Device_PressButton(_Device_GPIOGrind)) {
			varGrind->valByte++;
			if (varGrind->valByte > 5)
				varGrind->valByte = 0;
			UART_CommandSendProperty(varGrind);
		}
		else {
			return false;
		}
	}
	return true;
}

/// <summary>
///		Action to launch or stop a brew
///		Sets the grind value to 0
/// </summary>
bool Device_LaunchBrew() {
	state_variable *varBrew = State_GetVariable(VALUE_STATE_BREW);
	state_variable *varBrewETA;
	
	//Already an action in progress, ignore the call
	if (State_GetVariable(VALUE_STATE_BREW_GRIND)->valBool)
		return true; 

	//Wake up
	Device_PressButton(_Device_GPIOWakeUp);
	varBrewETA = State_GetVariable(VALUE_STATE_BREW_ETA);

	if (varBrew->valBool) {
		//Already an action in progress, stop the action
		varBrew->valBool = false;
		varBrewETA->valTime = 0;
	} else {
		//Deactivate Grind
		if (!Device_ChangeGrind(0))
			return false;

		//Activate Brew
		varBrew->valBool = true;
		varBrewETA->valTime = time(NULL) + METHOD_ACTION_BREW_TIME;
	}

	//Press Brew
	Device_PressButton(_Device_GPIOBrew);

	//Send confirmation
	_Device_TimerAction = varBrewETA->valTime;
	UART_CommandSendProperty(varBrew);
	UART_CommandSendProperty(varBrewETA);

	//Send IoT Report
	AzureIoT_TwinReportStateDevice(varBrew);
	AzureIoT_TwinReportStateDevice(varBrewETA);
	return true;
}

/// <summary>
///		Action to launch or stop a grind and brew
///		Sets the grind value to 1 (4)
/// </summary>
bool Device_LaunchGrindBrew() {
	state_variable *varBrewGrind = State_GetVariable(VALUE_STATE_BREW_GRIND);
	state_variable *varBrewGrindETA;
	
	//Already an action in progress, ignore the call
	if (State_GetVariable(VALUE_STATE_BREW)->valBool)
		return true;

	//Wake up
	Device_PressButton(_Device_GPIOWakeUp);
	varBrewGrindETA = State_GetVariable(VALUE_STATE_BREW_GRIND_ETA);
	varBrewGrindETA->valTime = time(NULL) + METHOD_ACTION_BREW_GRIND_TIME;

	if (varBrewGrind->valBool) {
		//Already an action in progress, stop the action
		varBrewGrind->valBool = false;
		varBrewGrindETA->valTime = 0;
	}
	else {
		//Deactivate Grind
		if (!Device_ChangeGrind(1))
			return false;

		//Activate Brew
		varBrewGrind->valBool = true;
		varBrewGrindETA->valTime = time(NULL) + METHOD_ACTION_BREW_TIME;
	}

	//Press Brew
	Device_PressButton(_Device_GPIOBrew);

	//Send confirmation
	_Device_TimerAction = varBrewGrindETA->valTime;
	UART_CommandSendProperty(varBrewGrind);
	UART_CommandSendProperty(varBrewGrindETA);

	//Send IoT Report
	AzureIoT_TwinReportStateDevice(varBrewGrind);
	AzureIoT_TwinReportStateDevice(varBrewGrindETA);
	return true;
}

/// <summary>
///		Action to set Wifi
///		The parameter is a char sequence of SSID + 0x0 + Password
/// </summary>
bool Device_SetWifi(const char* ssid, const char* password, const char* security) {
	return Networking_SetWifi(ssid, security, password);
}

/// <summary>
///		Execute periodic tasks specific to the device to be executed every few seconds
///		- Monitor the connection states, and sends an update to the UI if the connection state changed
///		- Monitor the brewing state, and stop the brewing or grind and brewed when the timer is expired, and update the values
///		- Send a log message to IoT Hub
/// </summary>
void Device_DoPeriodicTaskSeconds(const bool connectionState) {
	static bool previousConnectionState = false;
	state_variable *varBrew = State_GetVariable(VALUE_STATE_BREW);
	state_variable *varBrewGrind = State_GetVariable(VALUE_STATE_BREW_GRIND);
	char *message;
	size_t bufsz;

	//Debug - Memory leak test
	#if defined DEBUG
	read_off_memory_status();
	#endif

	//Connection State UI report
	if (connectionState != previousConnectionState) {
		state_variable *connectionState = State_GetVariable(VALUE_UI_CONNECTION_STATE);
		connectionState->valBool = connectionState;
		UART_CommandSendProperty(connectionState);
		previousConnectionState = connectionState;
	}

	//Action timer
	if (varBrew->valBool || varBrewGrind->valBool) {
		if (_Device_TimerAction < time(NULL)) {
			_Device_TimerAction = 0;
			state_variable *varNbrBrewToday = State_GetVariable(VALUE_STAT_NBR_BREWED_TODAY);
			state_variable *varNbrBrewWeek = State_GetVariable(VALUE_STAT_NBR_BREWED_WEEKLY);
			state_variable *varNbrBrewTotal = State_GetVariable(VALUE_STAT_NBR_BREWED_TOTAL);
			state_variable *varLastBrew = State_GetVariable(VALUE_STAT_LAST_BREWED);
			state_variable *varBrewStrength = State_GetVariable(VALUE_STATE_BREW_STRENGTH);

			varNbrBrewToday->valShort++;
			varNbrBrewWeek->valShort++;
			varNbrBrewTotal->valInt++;
			varLastBrew->valTime = time(NULL);

			State_SaveVariable(varNbrBrewToday);
			UART_CommandSendProperty(varNbrBrewToday);
			AzureIoT_TwinReportStateDevice(varNbrBrewToday);
			State_SaveVariable(varNbrBrewWeek);
			UART_CommandSendProperty(varNbrBrewWeek);
			AzureIoT_TwinReportStateDevice(varNbrBrewWeek);
			State_SaveVariable(varNbrBrewTotal);
			UART_CommandSendProperty(varNbrBrewTotal);
			AzureIoT_TwinReportStateDevice(varNbrBrewTotal);
			State_SaveVariable(varLastBrew);
			UART_CommandSendProperty(varLastBrew);
			AzureIoT_TwinReportStateDevice(varLastBrew);

			if (varBrew->valBool) {
				state_variable *varBrewETA = State_GetVariable(VALUE_STATE_BREW_ETA);

				varBrew->valBool = false;
				varBrewETA->valTime = 0;
				//Send confirmation
				UART_CommandSendProperty(varBrew);
				UART_CommandSendProperty(varBrewETA);

				//Send IoT Report
				AzureIoT_TwinReportStateDevice(varBrew);
				AzureIoT_TwinReportStateDevice(varBrewETA);
			}
			else if (varBrewGrind->valBool) {
				state_variable *varBrewGrindETA = State_GetVariable(VALUE_STATE_BREW_GRIND_ETA);

				varBrewGrind->valBool = false;
				varBrewGrindETA->valTime = 0;
				//Send confirmation
				UART_CommandSendProperty(varBrewGrind);
				UART_CommandSendProperty(varBrewGrindETA);

				//Send IoT Report
				AzureIoT_TwinReportStateDevice(varBrewGrind);
				AzureIoT_TwinReportStateDevice(varBrewGrindETA);
			}

			//Send Log message
			bufsz = (unsigned int)snprintf(NULL, 0, API_COFFEE_LOG_JSON, varBrewStrength->valByte, varLastBrew->valTime, varBrewGrind->valBool);
			message = malloc(bufsz + 1);
			snprintf(message, bufsz + 1, API_COFFEE_LOG_JSON, varBrewStrength->valByte, varLastBrew->valTime, varBrewGrind->valBool);
			if (!AzureIoT_SendMessage(message))
				Log_IoT_Error(ERROR_CODE_IOT_MESSAGE, "ERROR: The brew logs could not be sent to IoT Hub\n");
			free(message);
		}
	}
}

/// <summary>
///		Execute periodic tasks specific to the device to be executed every few minutes
///		- Monitor the time to reset brewed today and weekly when appropriate
///		- Report a hearbeat to IoT Hub
/// </summary>
void Device_DoPeriodicTaskMinutes(const bool connectionState) {
	char str_heartbeat[FORMAT_TIME_STR_MAX_SIZE];
	state_variable *varLastBrew = State_GetVariable(VALUE_STAT_LAST_BREWED);
	state_variable *varNbrBrewToday = State_GetVariable(VALUE_STAT_NBR_BREWED_TODAY);
	state_variable *varNbrBrewWeek = State_GetVariable(VALUE_STAT_NBR_BREWED_WEEKLY);
	time_t currentTime = time(NULL);
	struct tm td_now;
	struct tm td_last_brew;
	char buffer_week[4];
	uint8_t td_now_week_nbr = 0;
	uint8_t td_last_brew_week_nbr = 0;

	//Reset today
	td_now = *localtime(&currentTime);
	td_last_brew = *localtime(&(varLastBrew->valTime));

	//Get week of the year
	strftime(buffer_week, 4, "%W", &td_now);
	td_now_week_nbr = (uint8_t)atoi(buffer_week);
	strftime(buffer_week, 4, "%W", &td_last_brew);
	td_last_brew_week_nbr = (uint8_t)atoi(buffer_week);
	
	if (td_now.tm_year != 70 && td_last_brew.tm_year != 70 && (td_now.tm_year != td_last_brew.tm_year || td_now.tm_yday != td_last_brew.tm_yday)) {
		varNbrBrewToday->valShort = 0;
		State_SaveVariable(varNbrBrewToday);
		UART_CommandSendProperty(varNbrBrewToday);
		AzureIoT_TwinReportStateDevice(varNbrBrewToday);
	}

	if (td_now.tm_year != 70 && td_last_brew.tm_year != 70 && (td_now.tm_year != td_last_brew.tm_year || td_now_week_nbr != td_last_brew_week_nbr)) {
		varNbrBrewWeek->valShort = 0;
		State_SaveVariable(varNbrBrewWeek);
		UART_CommandSendProperty(varNbrBrewWeek);
		AzureIoT_TwinReportStateDevice(varNbrBrewWeek);
	}

	if (connectionState) {
		Utils_GetFormattedDateFromTime(str_heartbeat, sizeof(str_heartbeat), currentTime);
		AzureIoT_TwinReportStateString(REPORTED_HEARTBEAT, str_heartbeat);
	}
}

/// <summary>
///		Execute periodic tasks specific to the device
/// </summary>
void Device_DoPeriodicTasks(const bool connectionState) {
	static time_t timerSecond = 0;
	static time_t timerMinute = 0;
	time_t currentTime = time(NULL);

	if (timerSecond < currentTime) {
		timerSecond = currentTime + DEVICE_PERIODIC_TASKS_TIMER_SEC;
		Device_DoPeriodicTaskSeconds(connectionState);
	}

	if (timerMinute < currentTime) {
		timerMinute = currentTime + DEVICE_PERIODIC_TASKS_TIMER_MIN;
		Device_DoPeriodicTaskMinutes(connectionState);
	}
}

/// <summary>
/// Device Twin update callback function, called when an update is received from the Azure IoT Hub.
/// </summary>
void Device_TwinUpdate(const char *childName, const char *propertyName, const char *valueString)
{
	//Changing feature state
	if (strcmp(childName, DESIRED_FEATURES) == 0) {
		for (uint8_t i = 0; i < FEATURES_NBR; i++) {
			if (strcmp(propertyName, FEATURES[i].name) == 0) {
				FEATURES[i].state.valBool = strcmp(valueString, "true") == 0;
				State_SaveVariable(&FEATURES[i].state);
				AzureIoT_TwinReportStateFeature(FEATURES[i].name, REPORTED_FEATURES_DEFINITIONS_IS_ACTIVATED, FEATURES[i].state.valBool ? "true" : "false");
				UART_CommandSendPropertyString(FEATURES[i].state.varname, FEATURES[i].state.valBool ? "1" : "0");
			}
		}
	}
}

/// <summary>
/// Direct Method callback function, called when a Direct Method is received from the Azure IoT Hub.
/// </summary>
void Device_DirectMethodCall(const char* methodName, const char *payload, unsigned char **response, size_t *response_size)
{
	bool result = false;
	char str_buffer[FORMAT_TIME_STR_MAX_SIZE];
	MULTITREE_HANDLE tree = NULL;

	Log_Debug("INFO: DirectMethod called: %s.\n", methodName);

	//Process Change Brew Strength
	if (strcmp(methodName, METHOD_ACTION_CHANGE_BREW_STRENGTH) == 0) {
		const void *value = NULL;
		if (JSON_DECODER_OK == JSONDecoder_JSON_To_MultiTree((char*)payload, &tree))
		{
			if (MULTITREE_OK == MultiTree_GetLeafValue(tree, "strength", &value))
			{
				const char* valueChar = (const char*)value;
				result = Device_ChangeBrewStrength(atoi(valueChar));
			}
			else {
				result = Device_ChangeBrewStrength(0);
			}
			MultiTree_Destroy(tree);
		}
		else {
			result = Device_ChangeBrewStrength(0);
		}
		if (response != NULL) {
			Utils_GetStringFromUINT8(str_buffer, sizeof(str_buffer), State_GetVariable(VALUE_STATE_BREW_STRENGTH)->valByte);
			*response_size = (unsigned int)snprintf(NULL, 0, METHOD_RESPONSE_CHANGE_BREW_STRENGTH, "success", str_buffer) + 1;
			*response = (unsigned char *)malloc(*response_size);
			snprintf(*response, *response_size, METHOD_RESPONSE_CHANGE_BREW_STRENGTH, "success", str_buffer);
		}
	}
	//Process Brew
	else if (strcmp(methodName, METHOD_ACTION_BREW) == 0) {
		result = Device_LaunchBrew();
		if (response != NULL) {
			Utils_GetFormattedDateFromTime(str_buffer, sizeof(str_buffer), State_GetVariable(VALUE_STATE_BREW_ETA)->valTime);
			*response_size = (unsigned int)snprintf(NULL, 0, METHOD_RESPONSE_BREW, "success", str_buffer) + 1;
			*response = (unsigned char *)malloc(*response_size);
			snprintf(*response, *response_size, METHOD_RESPONSE_BREW, "success", str_buffer);
		}
	}

	//Process Grind and Brew
	else if (strcmp(methodName, METHOD_ACTION_GRIND_AND_BREW) == 0) {
		result = Device_LaunchGrindBrew();
		if (response != NULL) {
			Utils_GetFormattedDateFromTime(str_buffer, sizeof(str_buffer), State_GetVariable(VALUE_STATE_BREW_GRIND_ETA)->valTime);
			*response_size = (unsigned int)snprintf(NULL, 0, METHOD_RESPONSE_BREW, "success", str_buffer) + 1;
			*response = (unsigned char *)malloc(*response_size);
			snprintf(*response, *response_size, METHOD_RESPONSE_BREW, "success", str_buffer);
		}
	}

	else if (strcmp(methodName, METHOD_ACTION_SET_WIFI) == 0) {
		const void *valueSSID = NULL;
		const void *valuePassword = NULL;
		const void *valueSecurity = NULL;
		if (JSON_DECODER_OK == JSONDecoder_JSON_To_MultiTree((char*)payload, &tree))
		{
			if (MULTITREE_OK == MultiTree_GetLeafValue(tree, "ssid", &valueSSID))
			{
				if (MULTITREE_OK == MultiTree_GetLeafValue(tree, "password", &valuePassword))
				{
					if (MULTITREE_OK == MultiTree_GetLeafValue(tree, "security", &valueSecurity))
					{
						const char* valueSSIDChar = (const char*)valueSSID;
						const char* valuePasswordChar = (const char*)valuePassword;
						const char* valueSecurityChar = (const char*)valueSecurity;
						result = Device_SetWifi(valueSSIDChar, valuePasswordChar, valueSecurityChar);
					}
				}
			}
			MultiTree_Destroy(tree);
		}
	}

	if (!result)
		Log_IoT_Error(ERROR_CODE_IOT_METHOD, "ERROR: There was a problem running DirectMethod %s.\n", methodName);
}