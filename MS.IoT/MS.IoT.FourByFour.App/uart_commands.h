#include "simulator_structures.h"

#pragma once
/// <summary>
/// Main command processing method that analyzes the command name and redirect to the right command method.
/// </summary>
bool process_command(datapacket *p);