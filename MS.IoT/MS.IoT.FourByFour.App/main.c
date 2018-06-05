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
#include "simulator_structures.h"
#include "azure_iot_hub.h"
#include "uart_processor.h"
#include "uart_commands.h"
#include "network.h"

/// <summary>
/// Init Application
/// Initialize UART and the Data Processor
/// </summary>
bool init_application(datapacket_processor *pproc)
{
	Log_Debug("Application starting\n");

	// Initialize UART/Data processor
	if (!init_uart() || !init_data_processor(pproc))
		return false;
	return true;
}

/// <summary>
/// Exit Application
/// Method executing before exiting the application to clean the memory
/// </summary>
void exit_application(datapacket_processor *pproc)
{
	AzureIoT_ClientDisconnect();
	destroy_data_processor(pproc);
	destroy_uart();
	Log_Debug("Application exiting\n");
}

/// <summary>
/// Main method.
/// - Initialize UART and Data Processor
/// - In the loop:
/// * Retrieve incoming UART data
/// * If packets are ready to be processed, process them based on their command name
/// * Destroy the processed packets
/// * Ensure the connection is working and execute the IoT Hub sending/receiving jobs
/// - At the end of the loop, disconnect from IoT Hub, clean the processor, UART and exit the program
/// </summary>
int main(void)
{
	datapacket_processor pproc;

	// Initialize UART/Data processor
	if (!init_application(&pproc)) {
		exit_application(&pproc);
		return 0;
	}

	// Main loop
	while (!Lifecycle_TerminationRequested()) {
		bool network_connection_on = false;
		bool result_command;

		// Listen to UART port and retrieve packets
		if (!handle_uart_reception(&pproc))
			break;

		// Packet found and fully retrieved
		if (pproc.nbr_packets_ready > 0) {
			for (uint16_t p = 0; p < pproc.nbr_packets_ready; ++p) {
				result_command = process_command(&pproc.packets[p]);

				// If error detected, leave loop immediately
				if (!result_command)
					break;
			}

			// Free memory
			clean_packets(&pproc);

			// If error detected, leave loop immediately
			if (!result_command)
				break;
		}

		// Check network and connect to IoT if possible
		if (check_network_connection(&network_connection_on, false) != Result_Success) {
			Log_Debug("ERROR: failure during network status check\n");
			break;
		}

		// Send any queued messages, receive any messages if the network connection is up
		if (network_connection_on)
			AzureIoT_DoPeriodicTasks();

		// Wait for next iteration of the loop
		nanosleep(&SLEEP_TIME, NULL);
	}

	exit_application(&pproc);
	return 0;
}