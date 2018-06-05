#include "constants.h"

#pragma once
//Features
extern feature FEATURES[];
extern const uint8_t FEATURES_NBR;

//States
extern state_variable STATE_VALUES[];
extern const uint8_t STATE_VALUES_NBR;

//Methods
extern method_definition METHODS[];
extern const uint8_t METHODS_NBR;

/// <summary>
///		Retrieve the pointer of a state_variable given its variable name
/// </summary>
state_variable *State_GetVariable(const char* variableName);

/// <summary>
///		Retrieve the pointer of a feature given its feature name
/// </summary>
state_variable *State_GetFeature(const char* featureName);

/// <summary>
///		Initialize state_variable values
/// </summary>
bool State_InitVariables();

/// <summary>
///		Save the value of a variable in the persisted appconfig store
/// </summary>
bool State_SaveVariable(state_variable *variable);