#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include <time.h>

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

#include "constants.h"
#include "state.h"
#include "log.h"
#include "azure_iot_hub.h"
#include "uart_processor.h"
#include "uart_commands.h"
#include "network.h"

/// <summary>
/// Device Twin update callback function, called when an update is received from the Azure IoT Hub.
// <param name="featureName">A pointer to a string containing a value enclosed in double quotes, e.g. "3".</param>
// <param name="valueString">A pointer to a string containing a value enclosed in double quotes, e.g. "3".</param>
/// </summary>
void DeviceTwinUpdate(const char *childName, const char *propertyName, const char *valueString)
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

	//Changing device value Avg
	if (strcmp(childName, DESIRED_DEVICE_STATE) == 0) {
		if (strcmp(propertyName, VALUE_STAT_AVG_BREWED_TODAY) == 0) {
			state_variable *varAvgBrewedToday = State_GetVariable(VALUE_STAT_AVG_BREWED_TODAY);
			varAvgBrewedToday->valShort = (uint16_t)atoi(valueString);
			State_SaveVariable(varAvgBrewedToday);
			AzureIoT_TwinReportStateDevice(varAvgBrewedToday);
			UART_CommandSendProperty(varAvgBrewedToday);
		}
		if (strcmp(propertyName, VALUE_STAT_AVG_BREW_STRENGTH) == 0) {
			state_variable *varAvgBrewStrength = State_GetVariable(VALUE_STAT_AVG_BREW_STRENGTH);
			varAvgBrewStrength->valByte = (uint8_t)atoi(valueString);
			State_SaveVariable(varAvgBrewStrength);
			AzureIoT_TwinReportStateDevice(varAvgBrewStrength);
			UART_CommandSendProperty(varAvgBrewStrength);
		}
	}
}

/// <summary>
/// Direct Method callback function, called when a Direct Method is received from the Azure IoT Hub.
/// </summary>
void DirectMethodCall(const char* methodName, const char *payload)
{
	Log_Debug("INFO: DirectMethod called: %s.\n", methodName);
}