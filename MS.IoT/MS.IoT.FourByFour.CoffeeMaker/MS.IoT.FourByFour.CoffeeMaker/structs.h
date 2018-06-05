#include <bits/alltypes.h>

#pragma once
typedef enum state_value_type {
	BYTE = 0,
	BOOL = 1,
	SHORT = 2,
	INT = 3,
	TIME = 4
} state_value_type;
typedef struct state_variable {
	const char varname[50];
	const bool persist;
	const bool report;
	const state_value_type type;
	union {
		uint8_t valByte;
		bool valBool;
		uint16_t valShort;
		uint32_t valInt;
		time_t valTime;
	};
} state_variable;

typedef struct feature {
	const char name[32];
	const char displayName[32];
	const char methods[256];
	const bool internal_use;
	state_variable state;
} feature;
typedef enum feature_type {
	BREW_STRENGTH = 0,
	BREW = 1,
	GRIND_AND_BREW = 2,
	WIFI = 3,
	DEBUG = 4
} feature_type;