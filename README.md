#Introduction 
This is an IoT solution accelerator for Microsoft Azure sphere.

#Prerequisites
You will need Visual Studio 2015 or later
Microsoft Azure subscription account to deploy. 

#Getting Started
 This solution consists of 5 parts- 
  
  MSIoT deployment script- You can deploy the resources required for the Azure Sphere solution accelrator using this script.
  
  MSIoT solution consists-
    Marketing portal solution (This does the same job like the MSIoT deployment script)
    Data packet designer -  This portal helps to design and create the data packet(json) IoT device template. You can also download and install the simulator using this portal.
    Device Management Portal- This portal manages the Azure sphere IoT devices.  
    
  MsIoT.FourByFour.App solution - this contains the IoT coffe maker solution code to deploy in Azure Sphere (previously known by codename  4x4)  
  
  MS.IoT.Mobile.solution - This is a xamarin android native app to control and manage iot devices (e.g you can vrew a coffee using this mobile app and will switch on the actual coffe maker device connected with Azure Sphere)
  
  MS.IoT.Simulator solution - This is the Universal windows desktop app simulator to send messages to deployed IoT hub, stream analytics and cosmosdb
  
