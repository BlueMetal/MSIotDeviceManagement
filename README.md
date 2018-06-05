# BlueMetal Microsoft Azure Sphere IoT solution accelerator

The Azure Sphere Solution Accelerator ecosystem enables your company to setup connected device products quickly, securely, and cost-effectively. Connect your product to the cloud, define collected data, and simulate device-to-cloud communication. Monitor your connected products, analyze trends, gain insights in channel management and remotely activate new features, all while providing your customers with an elegant mobile app experience to manage their devices.

## Getting Started

Open the .sln files from MS.IoT directory to get started.

### Prerequisites

You will need Visual Studio 2015 or later.
A Microsoft Azure subscription account for deploying.
Azure Powershell modules.
check link https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps?view=azurermps-6.2.0


## This solution consists of 5 parts

* MSIoT deployment script - You can deploy the resources required for the Azure Sphere solution accelrator using this script.

* MSIoT solution consists - Marketing portal solution (This does the same job like the MSIoT deployment script)
    Data packet designer -  This portal helps to design and create the data packet(json) IoT device template. You can also download and install the simulator using this portal.
    Device Management Portal- This portal manages the Azure sphere IoT devices.  

* MsIoT.FourByFour.App solution - this contains the IoT coffe maker solution code to deploy in Azure Sphere (previously known by codename  4x4)  

* MS.IoT.Mobile.solution - This is a xamarin android native app to control and manage iot devices (e.g you can vrew a coffee using this mobile app and will switch on the actual coffe maker device connected with Azure Sphere)

* MS.IoT.Simulator solution- This is the Universal windows desktop app simulator to send messages to deployed IoT hub, stream analytics and cosmosdb

