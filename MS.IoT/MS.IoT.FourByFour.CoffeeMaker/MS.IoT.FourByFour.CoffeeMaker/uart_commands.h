#include <sys/time.h>
#include <sys/types.h>
#include <unistd.h>
#include "uart_processor.h"
#pragma once

/// <summary>
///		Send a Ping Command to the UI
/// </summary>
bool UART_CommandSendPing();

/// <summary>
///		Send a Send Property Command to UI.
/// </summary>
bool UART_CommandSendPropertyString(const char *propertyName, const char *propertyValue);

/// <summary>
///		Send a Send Property Command to UI using a state_variable.
/// </summary>
bool UART_CommandSendProperty(const state_variable *property);

/// <summary>
///		Main command processing method that analyzes the command name and redirect to the right command method.
/// </summary>
bool UART_ProcessCommand(datapacket *p);