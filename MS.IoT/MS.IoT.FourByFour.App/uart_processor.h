#include "simulator_structures.h"

#pragma once
/// <summary>
/// Init UART
/// Create a valid handle for UART.
/// </summary>
bool init_uart();

/// <summary>
/// Destroy UART Handle
/// </summary>
void destroy_uart();

/// <summary>
/// Init Data Processor.
/// This method sets the initial values of the datapacket processor
/// variables, and initialize the array of processed packets to 1.
/// </summary>
bool init_data_processor(datapacket_processor *pproc);

/// <summary>
/// Destroy Data Processor
/// Make sure that the memory of the current packet being processed is freed if necessary.
/// Free the memory of the array of processed packets.
/// </summary>
void destroy_data_processor(datapacket_processor *pproc);

/// <summary>
/// Handles the reception of packets through UART. Waits to receive the specific header_tag, 
/// then allocate memory for the data and waits for the whole package to be sent.
/// </summary>
bool handle_uart_reception(datapacket_processor *pproc);

/// <summary>
/// Free memory of the processed packets
/// </summary>
void clean_packets(datapacket_processor *pproc);

/// <summary>
/// Send a Datapacket back to the device through UART
/// </summary>
bool send_packet(datapacket *p);