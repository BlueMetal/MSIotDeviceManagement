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
#include "features.h"
#include "coffeemaker.h"

/// <summary>
///		Init Application
///		Initialize UART
///		Initialize Data Processor
///		Initialize GPIO for Coffee Maker
///		Initialize Curl
/// </summary>
bool InitApplication(datapacket_processor *pproc)
{
	Log_Debug("INFO: Application starting\n");

	// Initialize UART/Data processor/Coffee Maker GPIO/CURL
	if (!UART_InitUART() || !UART_InitDataProcessor(pproc) || !Device_InitGPIOHandles() || !Networking_InitCURL())
		return false;

	Log_Debug("INFO: Initialization complete\n");
	return true;
}

/// <summary>
///		Exit Application
///		Method executing before exiting the application to clean the memory
/// </summary>
void ExitApplication(datapacket_processor *pproc)
{
	Networking_DestroyCURL();
	AzureIoT_ClientDisconnect();
	Device_DestroyGPIOHandles();
	UART_DestroyDataProcessor(pproc);
	UART_DestroyUART();
	Log_Debug("INFO: Application exiting\n");
}

/// <summary>
///		Main method.
///		- Initialize UART, Data Processor, GPIO and Curl
///		- Load previously saved device state
///		- Set the callback for device twin and direct methods
///		- In the loop:
///		* Retrieve incoming UART data
///		* If packets are ready to be processed, process them based on their command name
///		* Destroy the processed packets
///		* Ensure the connection is working and execute the IoT Hub sending/receiving jobs
///		* At the first run in a loop with a working connection, initialize the reported properties
///		* Run the device specific periodic tasks
///		- At the end of the loop, disconnect from IoT Hub, clean the processor, UART, GPIO, Curl and exit the program
/// </summary>
int main(void)
{
	struct timespec sleepTime = { 0, 1000000 }; // 1 millisecond
	bool initializatedReport = false;
	datapacket_processor pproc;

	// Initialize UART/Data processor
	if (!InitApplication(&pproc)) {
		ExitApplication(&pproc);
		return 0;
	}
	
	//Initialize variable
	State_InitVariables();

	// Set the Azure IoT Hub library callbacks.
	AzureIoT_SetDeviceTwinUpdateFromIoTHubCallback(&Device_TwinUpdate);
	AzureIoT_SetDirectMethodCallFromIoTHubCallback(&Device_DirectMethodCall);

	// Main loop
	while (!Lifecycle_TerminationRequested()) {
		bool networkConnectionOn = false;
		bool resultCommand;

		// Listen to UART port and retrieve packets
		if (!UART_HandleReception(&pproc))
			break;

		// Packet found and fully retrieved
		if (pproc.nbr_packets_ready > 0) {
			for (uint16_t p = 0; p < pproc.nbr_packets_ready; ++p) {
				resultCommand = UART_ProcessCommand(&pproc.packets[p]);

				// If error detected, leave loop immediately
				if (!resultCommand)
					break;
			}

			// Free memory
			UART_CleanPackets(&pproc);

			// If error detected, leave loop immediately
			if (!resultCommand)
				break;
		}

		// Check network and connect to IoT if possible
		if (Networking_CheckNetworkConnection(&networkConnectionOn, false) != Result_Success) {
			Log_Debug("ERROR: Failure during network status check\n");
			break;
		}

		// Periodic tasks
		if (networkConnectionOn) {
			if (!initializatedReport) {
				AzureIoT_InitReportState();
				initializatedReport = true;
			}
			AzureIoT_DoPeriodicTasks();
		}
		Device_DoPeriodicTasks(networkConnectionOn);

		// Wait for next iteration of the loop
		nanosleep(&sleepTime, NULL);
	}

	ExitApplication(&pproc);
	return 0;
}