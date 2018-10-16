# Microsoft

# White Goods Solution

## User Guide

**Table of Contents** 

- [1 User Guide](#1-user-guide)
- [2 Login to Device Management Application](#2-login-to-device-management-application)
- [3 Run the Blink Applications](#3-run-the-blink-applications)
- [4 Verify data in IoT Hub Device Twin](#4-verify-data-in-iot-hub-device-twin)
- [5 Verify Device Management application](#5-verify-device-management-application)
     - [5.1 Validating Blink App code with Azure Sphere](#51-validating-blin-app-code-with-azure-sphere)
- [6 Monitoring Components](#6-monitoring-components)
    - [6.1 Application Insights](#61-application-insights)
    - [6.2 OMS Log Analytics](#62-oms-log-analytics)

## 1 User Guide

This Document explains about how to use the solution. In this we are configuring and validating the Blink Application with Device Management Application and also monitoring the resources of the solution.  

## 2 Login to Device Management Application

Once the solution is deployed successfully, navigate to the resource group and select the created resource group to view the list of resources that are created in the Resource Group as shown in the following figure.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u1.png">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u2.png">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u3.png">
</p>

Go to **Resource Group** -> Click on the deployed **App Service**.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u4.png">
</p>

Now you can see the app service **overview** page, copy the **URL** and browse it in new web browser.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u5.png)

To access the Device Management portal, login with your credentials.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u6.png">
 </p>
 
 <p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u7.png">
 </p>
 
<p align="center">  
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/q.png">
</p>

Now you can view the **Device Management Portal** Dashboard.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/u9.png)

## 3 Run the Blink Applications 

In Blink app Solution Explorer, select **azure_iot_hub.c** in your solution and update the IoT Hub connection string which was copied from above step and **save** it.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/85.png)

Now **open** the **app_manifest.json** file and update the IoT Hub Host name from the connection string.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/86.png)

Now you can build the application and use the Azure IoT Hub. In this walkthrough, we will use existing IoT Hub tools to monitor and communicate with your device.

Now **click on Remote GDB Debugger** and click on **Yes** to build the Blink App.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/87.png)

The output in the below screen shot shows that the build has been completed successfully and the Wi-Fi has been connected.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/88.png)

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/89.png)

Once the application is running the output also shows the IoT Hub connected Status and Report Status of the device as below.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/90.png)

## 4 Verify data in IoT Hub Device Twin

Go to **Resource Group** -> Click on **IoT Hub**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/100.png)

Click on **IoT devices** in left side menu and click **created device**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/101.png)

Here you can see the **Device Details** page. Click on **Device Twin**

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/102.png">
</p>

Below is the Device Twin json file with Blink Application features as false.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/103.png)

## 5 Verify Device Management application 

Go to **Resource Group** -> click **Device Management**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/104.png)

After running the blink App the connected Status of the device will get reflected in the device management web app.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/105.png">
</p>

Click on **connected**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/106.png)

The features of the blink app show disabled. Similarly the same status can be seen by Clicking on **View JSON**. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/107.png">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/108.png">
</p>

Click on **Activate** button on any of the blink app feature like **blinkrate1**. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/109.png">
</p>

Once it is activated the same will be reflected in IoT Hub’s device twin.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/devicetwin.PNG">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/110.png">
</p>

### 5.1 Validating Blink App code with Azure Sphere

To validate the blink rate, go to **resource group** -> go to **device management web app**. Select the device to which Azure Sphere Device is associated.

Under features section of the device, select Blink Rate1 as active.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b1.png">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/111.png">
</p>

After selecting blink rate1 feature we can see LED light blinking slowly on Azure Sphere device like below.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b2.png">
</p>

For video reference on Blink Rate1 go through the below link.

**https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Documentation/BlinkRate1.mp4** 

Uncheck Blink Rate1 and select Blink Rate2 as active.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b3.png">
</p>

When you select blink rate2 the LED light will blink a little faster on Azure Sphere device as shown in below screen shot.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b4.png">
</p>

For video reference on Blink Rate2 go through the below link.

**https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Documentation/BlinkRate2.mp4** 

Uncheck Blink Rate2 and select Blink Rate 3 as active.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b5.png">
</p>

when you select Blink Rate3 the LED Light will continuously blink on Azure Sphere device as shown in below screenshot.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/b6.png">
</p>

For video reference on Blink Rate3 go through the below link.

**https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Documentation/BlinkRate3.mp4**

## 6 Monitoring Components

### 6.1 Application Insights

Go to **Azure Portal**, select your **Resource Group** and select **Application Insights** as shown below. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/112.png)

On **Overview** page, Summary details are displayed as shown in the following figure.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/113.png)

Click on **Live Metric Stream**, which is available at left side menu to check the live requests of **Device Management Application**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/114.png)

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/115.png)

Go back to the **Application Insights Overview** page in **Azure Portal**.

Then click **Metrics Explorer** on the left side of the page as shown below.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/116.png">
</p>

Click **Edit** as shown in the following figure.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/117.png)

You can select any of the listed **Metrics** to view application logs.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/118.png)

If you want to **add new chart** click Add new chart as shown in the following figure and click **Edit** to add the specific metrics.

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/119.png">
</p>

Go back to **Application Insights**, in the **Overview** page click **Analytics** icon in the **Health** section as shown in the following figure.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/120.png)

The **Application Insights** page is displayed and double click on **requests**. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/121.png)

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/122.png)

You can click **run** to see the specific requests of application as below.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/123.png)

Now click on **chart** then click **Yes** to see the graph.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/124.png)

After click on chart you can see the **requests graph** like below.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/125.png)

Now do the same process for checking graph for custom metrics.

Double click on **custom metrics**.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/126.png)

You can click **run** to see the specific custom metrics of application as below.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/127.png)

Now click on **chart** then click **Yes** to see the graph. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/128.png)

Similarly, you can run the other common Pre-defined queries by navigating back to the **Home Page**.

### 6.2 OMS Log Analytics

Open **Azure Portal** -> **Resource Group** -> Click the **OMS Workspace** in resource group to view **OMS Overview** Section. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/129.png">
</p>

Click **Azure Resources** on left side menu to view available Azure Resources. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/130.png)

Select your **Resource Group** name from the dropdown list.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/131.png)

Go to overview blade and click on **OMS Portal** as shown below.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/132.png)

Once you click **OMS Workspace**, OMS Home Page will take few seconds to load and displayed as below. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/133.png">
</p>

Click **Search tab** to search the IoT hub, Stream Analytics, Cosmos DB. 

Click **Show legacy language converter**. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/134.png)

Copy **IoT Hub** resource name, paste it in the **Converter** box and click **RUN**. 

The IoT Hub information is displayed below the page as shown in the following figure. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/135.png)

Copy **Cusmos DB** resource name, paste it in the **Converter** box and click **RUN**. 

The cosmos db information is displayed in the below page as shown in the following figure. 

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/136.png)

For **Stream Analytics logs**, first you need to enable the **Diagnostics logs**. 

Go to **Azure Portal**, click **STREAM ANALYTICS JOB** as shown below. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/137.png">
</p>

Click **Diagnostics logs** on the left pane and select **Add diagnostics setting** as shown in the following figure.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/138.png)

In the **Diagnostics settings** page, enter the name in the **Name** field. 

Select **Send to Log Analytics** checkbox. 

Select the **Execution** and **Authoring** checkboxes under the **LOG** section. 

Select **Allmetrics** checkbox under the **METRIC** section. 

Configure your workspace from the **OMS Workspaces page** as shown in the following figure.  

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/139.png">
</p>

Click **Save**. 

<p align="center">
  <img src="https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/140.png">
</p>

Go to **OMS Portal** and Click Search tab for **Stream Analytics logs**.

Copy **Stream Analytics Job** resource name, paste it in the **Converter** box and click **RUN**. 

The Stream Analytics Job information is displayed below the page as shown in the following figure.

![alt text](https://github.com/ooha-m/MSIotDeviceManagement/raw/master/Images/141.png) 
