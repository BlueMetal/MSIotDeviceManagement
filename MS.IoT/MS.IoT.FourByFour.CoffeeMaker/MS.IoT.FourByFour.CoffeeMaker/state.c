#include <stdbool.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include <time.h>
#include <applibs/uart.h>
#include <applibs/config.h>
#include <applibs/log.h>
#include "constants.h"
#include "utils.h"
#include "log.h"

//Features
feature FEATURES[] = {
	{.name = FEATURE_NAME_BREW_STRENGTH,.displayName = FEATURE_DISPLAY_BREW_STRENGTH,.methods = FEATURE_METHODS_BREW_STRENGTH,.internal_use = false,.state = {.varname = FEATURE_VARNAME_BREW_STRENGTH,.type = BOOL,.persist = true, .report = true} },
	{ .name = FEATURE_NAME_BREW, .displayName = FEATURE_DISPLAY_BREW, .methods = FEATURE_METHODS_BREW, .internal_use = false,.state = { .varname = FEATURE_VARNAME_BREW,.type = BOOL,.persist = true,.report = true } },
	{ .name = FEATURE_NAME_GRIND_AND_BREW, .displayName = FEATURE_DISPLAY_GRIND_AND_BREW, .methods = FEATURE_METHODS_GRIND_AND_BREW, .internal_use = false,.state = { .varname = FEATURE_VARNAME_GRIND_BREW,.type = BOOL,.persist = true,.report = true } },
	{ .name = FEATURE_NAME_WIFI, .displayName = FEATURE_DISPLAY_WIFI, .methods = FEATURE_METHODS_WIFI, .internal_use = true,.state = { .varname = FEATURE_VARNAME_WIFI,.type = BOOL,.persist = true,.report = true } },
	{ .name = FEATURE_NAME_DEBUG, .displayName = FEATURE_DISPLAY_DEBUG, .methods = FEATURE_METHODS_DEBUG, .internal_use = true,.state = { .varname = FEATURE_VARNAME_DEBUG,.type = BOOL,.persist = true,.report = true } }
};
const uint8_t FEATURES_NBR = sizeof(FEATURES) / sizeof(feature);

//States
state_variable STATE_VALUES[] = {
	{ .varname = VALUE_STATE_BREW_STRENGTH,.type = BYTE,.valByte = 0,.report = true },
	{ .varname = VALUE_STATE_GRIND,.type = BYTE,.valByte = 5,.report = true },
	{ .varname = VALUE_STATE_BREW,.type = BOOL,.valBool = 0,.report = true },
	{ .varname = VALUE_STATE_BREW_ETA,.type = TIME,.valTime = 0,.report = true },
	{ .varname = VALUE_STATE_BREW_GRIND,.type = BOOL,.valBool = 0,.report = true },
	{ .varname = VALUE_STATE_BREW_GRIND_ETA,.type = TIME,.valTime = 0,.report = true },
	{ .varname = VALUE_STAT_LAST_BREWED,.type = TIME,.valTime = 0,.persist = true,.report = true },
	{ .varname = VALUE_STAT_NBR_BREWED_TODAY,.type = SHORT,.valShort = 0,.persist = true,.report = true },
	{ .varname = VALUE_STAT_NBR_BREWED_WEEKLY,.type = SHORT,.valShort = 0,.persist = true,.report = true },
	{ .varname = VALUE_STAT_NBR_BREWED_TOTAL,.type = INT,.valByte = 0,.persist = true,.report = true },
	{ .varname = VALUE_UI_CONNECTION_STATE,.type = BOOL,.valBool = 0 }
};
const uint8_t STATE_VALUES_NBR = sizeof(STATE_VALUES) / sizeof(state_variable);

//Methods
const method_definition METHODS[] = {
	{ .name = METHOD_ACTION_CHANGE_BREW_STRENGTH },
	{ .name = METHOD_ACTION_BREW },
	{ .name = METHOD_ACTION_GRIND_AND_BREW },
	{ .name = METHOD_ACTION_SET_WIFI }
};
const uint8_t METHODS_NBR = sizeof(METHODS) / sizeof(method_definition);

/// <summary>
///		Retrieve the pointer of a state_variable given its variable name
/// </summary>
state_variable * State_GetVariable(const char* variableName) {
	for (uint8_t i = 0; i < FEATURES_NBR; i++) {
		if (strcmp(variableName, FEATURES[i].state.varname) == 0)
			return &FEATURES[i].state;
	}

	for (uint8_t i = 0; i < STATE_VALUES_NBR; i++)
		if (strcmp(variableName, STATE_VALUES[i].varname) == 0)
			return &STATE_VALUES[i];

	return NULL;
}

/// <summary>
///		Retrieve the pointer of a feature given its feature name
/// </summary>
feature * State_GetFeature(const char* featureName) {
	for (uint8_t i = 0; i < FEATURES_NBR; i++) {
		if (strcmp(featureName, FEATURES[i].name) == 0) {
			return &FEATURES[i];
		}
	}
	return NULL;
}

/// <summary>
///		Initialize a state_variable by reading its value in the appconfig
/// </summary>
bool State_InitVariable(state_variable *variable) {
	Applibs_Result result;
	char buffer[TIME_STR_MAX_SIZE];
	
	if (variable->persist) {
		result = Config_ReadString(variable->varname, buffer, TIME_STR_MAX_SIZE);
		if (result == Result_Success) {
			switch (variable->type) {
			case BOOL:
				variable->valBool = (bool)atoi(buffer);
				break;
			case BYTE:
				variable->valBool = (uint8_t)atoi(buffer);
				break;
			case SHORT:
				variable->valShort = (uint16_t)atoi(buffer);
				break;
			case INT:
				variable->valInt = (uint32_t)atoi(buffer);
				break;
			case TIME:
				variable->valTime = strtol(buffer, NULL, 10);
				break;
			}
		}
		else if (result != Result_Config_KeyNotFound) {
			Log_IoT_Error(ERROR_CODE_APPCONFIG_READ, "ERROR: There was a problem trying to read the AppConfig value %s\n", variable->varname);
			return false;
		}
	}
	return true;
}

/// <summary>
///		Initialize state_variable values
/// </summary>
bool State_InitVariables() {
	bool result;

	Log_Debug("INFO: Loading variable states\n");
	
	for (uint8_t i = 0; i < FEATURES_NBR; i++) {
		result = State_InitVariable(&FEATURES[i].state);
		if (!result)
			return false;
	}

	for (uint8_t i = 0; i < STATE_VALUES_NBR; i++) {
		result = State_InitVariable(&STATE_VALUES[i]);
		if (!result)
			return false;
	}
	return true;
}

/// <summary>
///		Save the value of a variable in the persisted appconfig store
/// </summary>
bool State_SaveVariable(state_variable *variable) {
	Applibs_Result result;
	if (!variable->persist)
		return false;

	const char* str = Utils_GetVariableStringValue(variable);
	result = Config_WriteString(variable->varname, str);
	if (result != Result_Success){
		Log_IoT_Error(ERROR_CODE_APPCONFIG_WRITE, "ERROR: There was a problem trying to write in the AppConfig %s\n", variable->varname);
		return false;
	}

	return true;
}