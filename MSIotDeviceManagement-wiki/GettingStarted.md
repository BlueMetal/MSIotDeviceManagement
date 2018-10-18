# Microsoft

# White Goods Solution

## Getting Started Manual

**Table of Contents** 

 - [1 Introduction](#1-introduction)
     - [1.1 Overview of White Goods Solution](#11-overview-of-white-goods-solution)
     - [1.2 Overview of IOT Solution](#12-overview-of-iot-solution)
          - [1.2.1 Highlights](#121-highlights)
          - [1.2.2 Brief About the Solution](#122-brief-about-the-solution)         
- [2 IoT Solutions](#2-iot-solutions)
    - [2.1 Core Architecture (Current)](#21-core-architecture-(Current))
    - [2.2 Automated Solution](#22-automated-solution)
    - [2.3 Architectures](#23-architectures)
         - [2.3.1 Basic Architecture](#231-basic-architecture)
         - [2.3.2 Standard Architecture](#232-standard-architecture)
         - [2.3.3 Premium Architecture](#233-premium-architecture)
    - [2.4 Conventional Data work Flow Diagram](24#-conventional-data-work-flow-diagram)
    - [2.5 Azure Components Functionality](#25-azure-components-functionality)
         - [2.5.1 IoT Hub](#251-iothub)
         - [2.5.2 Stream Analytics](#252-stream-analytics)
         - [2.5.3 Azure Sphere Device](#253-azure-sphere-device)
         - [2.5.4 Notification Hub](#254-notification-hub)
         - [2.5.5 Web Application](#255-web-application)
         - [2.5.6 Azure Active Directory](#256-azure-active-directory)
         - [2.5.7 Azure Run book](#257-azure-run-book)
         - [2.5.8 Cosmos DB](#258-cosmos-db)
         - [2.5.9 OMS Log analytics](#259-oms-log-analytics)
         - [2.5.10 Application Insights](#2510-application-insights)
- [3 Solution Type & Cost Mechanism](3#-solution-deployment-type-&-cost-mechanism)
    - [3.1 Solutions and Associated Costs](#41-solutions-and-associated-costs)
         - [3.1.1 Basic](#311-Basic)
         - [3.1.2 Standard](#312-standard)
         - [3.1.3 Premium](#313-premium)
    - [3.2 Cost Comparison](#32-cost-comparison)
         - [3.2.1 In terms of features](#321-in-terms-of-features)
         - [3.2.2 Dollar Impact](#322-dollar-impact) 
         - [3.2.3 Estimated Monthly Cost for each Solution](#323-estimated-monthly-cost-for-each-solution)
- [4 What are paired regions?](#4-what-are-paired-regions?)
- [5 Deployment Guide for the Solution](#5-deployment-guide-for-the-solution) 
- [6 User Guide for the Solution](#6-user-guide-for-the-solution)
- [7 Administrator Guide for the Solution](#7-administrator-guide-for-the-solution)
   
## 1. Introduction

### 1.1 Overview of White Goods Solution

In White Goods solution we are monitoring the Household Appliances(Devices) like Washing machine, Dishwasher, Refrigerators, Coffee Maker etc. By using this solution User will access the multiple devices remotely and can get the acknowledgement from the Devices whether its turned On or Off. It also provides the device location and helps user for predictive maintenance of the Devices.

The Azure Sphere Solution Accelerator ecosystem enables your company to setup connected device products quickly, securely,and cost-effectively. Connect your product to the cloud, define collected data, and simulate device-to-cloud communication. Monitor your connected products, analyze trends, gain insights in channel management.

### 1.2 Overview of IOT Solution

#### 1.2.1 Highlights

The Rationale behind this IOT Solution for Whitegoods is to: 

1. **Blink Application establishes a connection between IoT Hub and Azure Sphere Device.***

2. **Based on the user selection on Activating one of the Blink Application feature, the nodes in the IoT Hub’s Device Twin gets updated.**

3. **Based on the status of the IoT Hub’s Device Twin, the Azure Sphere Device will activate the respective Blink Rate number (i.e. Blink Rate1, Blink Rate2 or Blink Rate3)**

#### 1.2.2 Brief About the Solution

* The Device Management Web Application manages the Azure sphere IoT devices and Azure Sphere Device contains the Blink Application code which communicates with IoT Hub.  

* A runbook which creates Database, Collections in Cosmos DB and update the reply URLs of the Application in Azure Active Directory to authenticate Device Management Web Application.

* This solution is helpful to monitor thousands of connected devices and access them remotely from Device Management Application.

## 2. IoT Solutions 

### 2.1 Core Architecture (Current)

Below Diagram explains the Core architecture for White Goods solution

<p align="center">
  <img src="https://github.com/sysgain/whitegoods/raw/master/Images/A1.png">
</p>

Core Architecture components:

* 4x4 Device
* IoT Hub
* Stream Analytics
* Data Packet Web Application
* Device Management
* Cosmos DB
* Notification Hub
* Run Book
* Azure Active Directory

### 2.2 Automated Solution	

Automated IOT Solution is designed on the top of current core architecture. In addition, this solution also provides Monitoring and High availability.

This solution is deployed through ARM template. This is a single click deployment which reduces manual effort when compared with the existing solution.

In addition, this solution consists

* Application Insights to provide monitoring for Web Application. Application Insights store the logs of the Web API which will be helpful to trace the web API working.

* Log analytics to provide monitoring for Stream Analytics, IoT hub, Cosmos DB. Log analytics store the logs, which will be helpful to trace the working of these resources.

* Geo-replication to provide high availability for Cosmos DB. Geo-replication is used to set the recovery database as the primary database whenever primary database is failed.

* This solution also provides Disaster Recovery activities. IoT Hub manual failover is helpful to make the IoT Hub available in another region, when IoT Hub of one region is failed.

* Traffic Manager delivers high availability for critical web applications by monitoring the endpoints and providing automatic failover when an endpoint goes down.

### 2.3 Architectures

#### 2.3.1 Basic Architecture:

Basic solution will have all core components, in addition this solution also consists monitoring components like Application Insights and OMS Log Analytics. 

* Application Insights provide monitoring for Web API.
* OMS Log Analytics provide monitoring for Stream Analytics, IoT hub, Cosmos DB.

The below diagram depicts the dataflow between azure components:

<p align="center">
  <img src="https://github.com/sysgain/whitegoods/raw/master/Images/D1.png">
</p>

Basic Architecture comprises of following components:

* 1-Azure Sphere
* 1-Web App
* 1-Cosmos DB
* 1-Application Insights
* 1-Stream Analytics
* 1-IoT HUB
* 1-Log analytics
* 1-Notification Hub
* 1-Runbook 
* 1-Azure Active Directory

#### 2.3.2 Standard Architecture:

Standard Architecture diagram will have two regions.

1. Primary Region(Deployment)
2. Secondary Region (Re – Deployment)

We have IoT Hub manual failover, Cosmos DB geo replication and redeployment components. The effect of these components will occur when primary Region goes down.

The main use of this solution is whenever disaster recovery occurs the redeployment components will deploy in another region, user need to manually add the Web application as an endpoint to the Traffic Manager and also start the Stream Analytics Job manually.

The below diagram depicts the dataflow between azure components in standard solution:

<p align="center">
  <img src="https://github.com/sysgain/whitegoods/raw/master/Images/A2.png">
</p>

Standard Architecture comprises of following components:

* 1-Web App
* 1-RunBook
* 1-Application Insights
* 1-Cosmos DB
* 1-IoT HUB
* 1-Log analytics
* 1-Notification Hub
* 1-Stream Analytics
* 1-Traffic Manager

When there is a Region failover, user needs to redeploy ARM Template provided in GIT Repo. When redeployment Completed Successfully, below azure resources will be deployed. 

Note:  Deployment process will take some time around 30mins to complete deployment Successfully.

* 1-Web App
* 1-Notification Hub
* 1-Application Insights
* 1-Stream Analytics job

#### 2.3.3 Premium Architecture:

Premium Architecture diagram also have two regions.

1. Primary Region
2. Secondary Region

All the components get deployed at once.

We have IoT Hub manual failover, Cosmos DB geo replication. Initially the Stream Analytics of the Secondary Region should be stopped before sending data to IoT Hub. The effect of these components will occur when primary Region goes down.

**Note: Make sure to run the stream Analytics job of the Secondary Region when the Primary Region goes down.**

The below diagram depicts the dataflow between azure components in premium solution:

![alt text](https://github.com/sysgain/whitegoods/raw/master/Images/A3.png)

Premium Architecture comprises of following components:

* 2-Web App
* 1-RunBook
* 2-Application Insights
* 1-Cosmos DB
* 1-IoT HUB
* 1-Log analytics
* 2-Notification Hub
* 2-Stream Analytics
* 1-Traffic Manager

In this type of solution all components will be deployed at initial deployment itself.

This type of solution reduces downtime of solution when region is down. In this solution there is redeployment approach which reduces downtime of the solution.

### 2.4 Conventional Data Work Flow 

<p align="center">
  <img src="https://github.com/sysgain/whitegoods/raw/master/Images/D2.png">
</p>

### 2.5 Azure Components Functionality

Microsoft Azure is a cloud computing service created by Microsoft for building, testing, deploying, and managing applications and services through a global network of Microsoft-managed data centers. It provides software as a service (SaaS), platform as a service (PaaS) and infrastructure as a service (IaaS) and supports many different programming languages, tools and frameworks, including both Microsoft-specific and third-party software and systems.

Microsoft lists over 600 Azure services, of which some are as below:

* Compute
* Storage services
* Data management
* Management
* Machine learning
* IoT

#### 2.5.1 IoT Hub

**Introduction:**

Azure IoT Hub is a fully managed service that enables reliable and secure bi-directional communications between millions of IoT devices and an application back end. 

Azure IoT Hub offers reliable device-to-cloud and cloud-to-device hyper-scale messaging, enables secure communications using per-device security credentials and access control, and includes device libraries for the most popular languages and platforms. Before you can communicate with IoT Hub from a gateway you must create an IoT Hub instance in your Azure subscription and then provision your device in your IoT hub. Some samples in this repository require that you have a usable IoT Hub instance.

The Azure IoT Hub offers several services for connecting IoT devices with Azure services, processing incoming messages or sending messages to the devices. From a device perspective, the functionalities of the Azure IoT Hub enable simple and safe connection of IoT devices with Azure services by facilitating bidirectional communication between the devices and the Azure IoT Hub.

**Implementation:**

IoT Hub is the core component of IoT Hub Solution. Azure Sphere device generates data and sends to IoT Hub. Based on the user selection on Activating one of the Blink Application feature, the nodes in the IoT Hub’s Device Twin gets updated.

#### 2.5.2 Steam Analytics

**Introduction:**

Stream Analytics is an event processing engine that can ingest events in real-time, whether from one data stream or multiple streams. Events can come from sensors, applications, devices, operational systems, websites, and a variety of other sources. Just about anything that can generate event data is fair game.

Stream Analytics provides high-throughput, low-latency processing, while supporting real-time stream computation operations. With a Stream Analytics solution, organizations can gain immediate insights into real-time data as well as detect anomalies in the data, set up alerts to be triggered under specific conditions, and make the data available to other applications and services for presentation or further analysis. Stream Analytics can also incorporate historical or reference data into the real-time streams to further enrich the information and derive better analytics.

To implement a streaming pipeline, developers create one or more jobs that define a stream’s inputs and outputs. The jobs also incorporate SQL-like queries that determine how the data should be transformed. In addition, developers can adjust a number of a job’s settings. For example, they can control when the job should start producing result output, how to handle events that do not arrive sequentially, and what to do when a partition lags other or does not contain data. Once a job is implemented, administrators can view the job’s status via the Azure portal.

**Implementation:**

Stream Analytics gets device telemetry data as input from IoT Hub and sends it to Cosmos DB’s messages collections. Stream analytics should always be in running state. 

#### 2.5.3 Azure Sphere Device

**Introduction:**

The Azure Sphere solution brings together the best of Microsoft’s expertise in cloud, software, and silicon—resulting in a unique approach to security that starts in the silicon and extends to the cloud. Azure Sphere contains three components that work together to keep devices protected and secured in today’s dynamic threat landscape: Azure Sphere certified MCUs, the Azure Sphere OS and the Azure Sphere Security Service.

The first Azure Sphere chip will be the MediaTek MT3620, which represents years of close collaboration and testing between MediaTek and Microsoft. This new cross-over class of MCU includes built-in Microsoft security technology, built-in connectivity, and combines the versatility and power of a Cortex-A processor with the low overhead and real-time guarantees of a Cortex-M class processor.

**Implementation:**

Installing Blink application on Azure sphere Device which establishes a connection with IoT Hub, creating a Device in IoT Hub and send Device status to IoT Hub’s Device Twin.

#### 2.5.4 Notification Hub

**Introduction:**

Azure Notification Hubs provides a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once, or tailor notifications to individual users. You can use Notification Hubs with any connected mobile application—whether it’s built on Azure Virtual Machines, Cloud Services, Web Sites, or Mobile Services.

Azure Notification Hubs are push notification software engines designed to alert users about new content for a given site,service or app. Azure Notification Hubs are part of Microsoft Azure’s public cloud service offerings.

**Implementation:**

Notification hub is used to send notifications to Mobile application whenever an event occurs beyond the defined metrics.

#### 2.5.5 Web Application 

**Introduction:**

A Web application (Web app) is an application program that is stored on a remote server and delivered over the Internet through a browser interface.

Azure Web Apps enables you to build and host web applications in the programming language of your choice without managing infrastructure. It offers auto-scaling and high availability, supports both Windows and Linux, and enables automated deployments from GitHub, Visual Studio Team Services, or any Git repo.

Web Apps not only adds the power of Microsoft Azure to your application, such as security, load balancing, auto scaling, and automated management. You can also take advantage of its DevOps capabilities, such as continuous deployment from VSTS,GitHub, Docker Hub, and other sources, package management, staging environments, custom domain, and SSL certificates.

In Solution we have a web application in one app service plan. 

* Device management web application

**Implementation:**

Device management web application is a dashboard where you can view the Summary of Devices such Device count, status of device connection. It also provides the detailed insights of a device. We can view the Alert summary, Device Activation status and date, Shipment date.

#### 2.5.6 Azure Active Directory

**Introduction:**

Microsoft Azure Active Directory (Azure AD) is a cloud service that provides administrators with the ability to manage end user identities and access privileges. The service gives administrators the freedom to choose which information will stay in the cloud, who can manage or use the information, what services or applications can access the information and which end users can have access.

**Implementation:**

Azure Active directory is used to authenticate users to login to Web Application. Azure active Directory enables secure authentications to web application

#### 2.5.7 Azure Run book
  
**Introduction:**

Azure Automation enables the users to automate the tasks, which are manual and repetitive in nature by using Runbooks. 
Runbooks in Azure Automation are based on Windows PowerShell or Windows PowerShell Workflow. We can code and implement the logic, which we want to automate, using PowerShell.

**Implementation:**

In this Solution Azure run books are used to create Database and collections in Document DB, it is also used to update reply URLs in Active Directory Application.

#### 2.5.8 Cosmos DB  

**Introduction:**

Azure Cosmos DB is a Microsoft cloud database that supports multiple ways of storing and processing data. As such, it is classified as a multi-model database. In multi-model databases, various database engines are natively supported and accessible via common APIs.

**Implementation:**

In this Solution, Cosmos DB have Templates, Messages and Groups Collections. The Messages collections will get updated with the telemetry data of the Device.
 
#### 2.5.9 OMS Log analytics

**Introduction:**

The Microsoft Operations Management Suite (OMS), previously known as Azure Operational Insights, is a software as a service platform that allows an administrator to manage on-premises and cloud IT assets from one console.

Microsoft OMS handles log analytics, IT automation, backup and recovery, and security and compliance tasks.

Log analytics will collect and store your data from various log sources and allow you to query over them using a custom query language.

**Implementation:**

Log analytics to provide monitoring for Stream Analytics, IoT hub, Cosmos DB. Log analytics store the logs, which will be helpful to trace the working of these resources. OMS log analytics provides in detailed insights using solutions.

#### 2.5.10 Application Insights

**Introduction:**

Application Insights is an extensible Application Performance Management (APM) service for web developers on multiple platforms. Use it to monitor live web application. It will automatically detect performance anomalies. It includes powerful analytics tools to help diagnose issues and to understand what users actually do with web application.

Application Insights monitor below:

* Request rates, response times, and failure rates
* Dependency rates, response times, and failure rates
* Exceptions 
* Page views and load performance
* AJAX calls
* User and session counts
* Performance counters
* Host diagnostics from Docker or Azure
* Diagnostic trace logs
* Custom events and metrics

**Implementation:**

Application Insights to provide monitoring for Web Application. Application Insights store the logs of the Web API which will be helpful to trace the web API working.

## 3. Solution Type & Cost Mechanism

### 3.1 Solutions and Associated Costs

The Automated solutions provided by us covers in Section …. Will have the following Cost associated. The solutions are created considering users requirements & have Cost effective measures. User have control on what Type of azure resources need to be deploy with respect to SKU And Cost. These options will let user to choose whether user wants to deploy azure resources with minimal SKU and Production ready SKU. The Cost Models per solutions are explained in future sections:


#### 3.1.1. Basic

The Basic solution requires minimum azure components with minimal available SKU’s. This Solution provides (Core + Monitoring) features such as application Insights & OMS Log Analytics. The details on components used in this solution is listed in Section: 

* The Estimated Monthly Azure cost is: **$190.51**

'Note: Refer below table for the optional component list & Features'

**Pricing Model for Basic Solution:**

Prices are calculated by Considering Location as East US and Pricing Model as **“PAYG”**.

| **Resource Name**           | **Size**           | **Azure Cost/month**                                                                                                                                 
| -------------              | -------------       | --------------------                                                                                                                                  
| **App Service Plan**       | Basic Tier; 1 B1 (1 Core(s), 1.75 GB RAM, 10 GB Storage)         | $54.75 
| **Cosmos DB**   | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50    | $25.86 
| **IoT HUB**        | Standard Tier: S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/da                      | $25.00    
| **Log Analytics**      | First 5GB of data storage is free. Per GB(Standalone). After finishing 5GB, $2.30 per GB.                          | $2.30  
| **Azure Automation Account**        | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month     | $0.00   
| **Notification Hub**       | Free                          | $0.00 
| **Application Insight**       | Basic, 1GB * $2.30 Region: East US first 5GB free per month          | $2.30 
| **Stream Analytics**   | Standard Streaming Unit, 1 unit(s) 1 * $80.30    | $80.30 
|                     | **Estimated monthly cost**          | **190.51** 

#### 3.1.2. Standard

This Solution provides (Core + Monitoring +Hardening) features such as application Insights, OMS Log Analytics, High Availability, Security & Disaster recovery. The details on components used in this solution is listed in Section: 

* The Estimated Monthly Azure cost is: **$364.90**

'Note: Refer below table for the optional component list & Features.'

**Pricing Model for Standard Solution:**

Prices are calculated by Location as East US and Pricing Model as **“PAYG”**.

| **Resource Name**           | **Size**           | **Azure Cost/month**                                                                                                                              
| -------------              | -------------       | --------------------                                                                                                                                
| **App Service Plan**       | Standard Tier; S1: 2 (Core(s), 1.75 GB RAM, 50 GB Storage) x 730 Hours; Windows OS    | $146.00  
| **Cosmos DB**   | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50   | $25.86
| **IoT HUB**        | Standard Tier: S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day          | $25.00    
| **Log Analytics**      | First 5GB of data storage is free. Per GB(Standalone) Region East US. After finishing 5GB, $2.30 per GB.     | $2.30   
| **Azure Automation Account**        | 2*Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00   
| **Notification Hub**       | 2*Free                          | $0.00 
| **Application Insight**       | 2 * Basic, 1GB * $2.30 Region: East US first 5GB free per month       | $4.60 
| **Stream Analytics**   | 2 * Standard Streaming Unit, 1 unit(s) 1 * $80.30 Region: East US         | $160.60  
| **Traffic Manager**     | DNS Query $0.54 + Azure Endpoint $0.36     | $0.90
|                     | **Estimated monthly cost**          | **$364.90**

**Note: When we redeploy the solution, there will not be any extra cost, since primary region is already paid.** 

#### 3.1.3. Premium

This solution also provides (Core + Monitoring +Hardening), the difference between Standard & Premium solution is under Premium - Both the regions can be deployed at same time, and however this is not possible under standard solution. The details on components used in this solution is listed in Section: 

* The Estimated Monthly Azure cost is: **$364.90**

**Pricing Model for Premium Solution:**

Prices are calculated by Considering Location as East US and Pricing Model as **“PAYG”**.

| **Resource Name**            	| **Size**                                                                                                              	| **Azure Cost/Month** 	|
|--------------------------	|-------------------------------------------------------------------------------------------------------------------	|------------------	|
| App Service Plan         	| Standard Tier; S1: 2 (Core(s), 1.75 GB RAM, 50 GB Storage) x 730 Hours; Windows OS                                	| $146.00          	|
| Cosmos DB                	| Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50      	| $25.86           	|
| IoT-Hub                  	| S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                                               	| $25.00           	|
| Log Analytics            	| First 5GB of data storage is free. Per GB(Standalone). After finishing 5GB, $2.30 per GB.                         	| $2.30            	|
| Azure Automation Account 	| 2*Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month. 	| $0.00            	|
| Notification Hub         	| 2 * Free                                                                                                          	| $0.00            	|
| Application Insight      	| 2 * Basic, 1GB * $2.30 First 5GB free per month                                                                   	| $4.60            	|
| Stream Analytics         	| 2 * Standard Streaming Unit, 1 unit(s) 1 * $80.30                                                                 	| $160.60          	|
| Traffic Manager          	| DNS Query $0.54 + Azure Endpoint $0.36                                                                            	| $0.90            	|
|                          	| **Estimated monthly cost**                                                                                            	| **$364.90**          	|


### 3.2 Cost Comparison: 

In this section we will be comparing the cost for all the solution provided in terms of Features & $ Impact:

#### 3.2.1 In terms of features:

The below table explain the distinctive features available across solution types.

| **Resource Name**            	| **Parameter**      	| **Basic**                                                                                              	| **Standard**                                                                                           	| **Premium**                                                                                            	|
|--------------------------	|----------------	|----------------------------------------------------------------------------------------------------	|----------------------------------------------------------------------------------------------------	|----------------------------------------------------------------------------------------------------	|
| App Service Plan         	| SKU            	| B1                                                                                                 	| S1                                                                                                 	| S1                                                                                                 	|
|                          	| Cores          	| 1 Core                                                                                             	| 1 Core                                                                                             	| 1 Core                                                                                             	|
|                          	| RAM            	| 1.75GB                                                                                             	| 1.75GB                                                                                             	| 1.75GB                                                                                             	|
|                          	| Storage        	| 10GB                                                                                               	| 50GB                                                                                               	| 50GB                                                                                               	|
|                          	| OS             	| Windows                                                                                            	| Windows                                                                                            	| Windows                                                                                            	|
| IoT-Hub                  	| SKU            	| S1                                                                                                 	| S1                                                                                                 	| S1                                                                                                 	|
|                          	| Devices        	| Unlimited devices                                                                                  	| Unlimited Devices                                                                                  	| Unlimited Devices                                                                                  	|
|                          	| Messages       	| 400,000 messages/day                                                                               	| 4,00,000 msgs/day                                                                                  	| 4,00,000 msgs/day                                                                                  	|
| Cosmos DB                	| SKU            	| Standard                                                                                           	| Standard                                                                                           	| Standard                                                                                           	|
|                          	| Database       	| 1                                                                                                  	| 1                                                                                                  	| 1                                                                                                  	|
|                          	| Storage        	| 10 GB                                                                                              	| 10 GB                                                                                              	| 10 GB                                                                                              	|
|                          	| Purchase model 	| 4 x 100 RU/s                                                                                       	| 4 x 100 RU/s                                                                                       	| 4 x 100 RU/s                                                                                       	|
| Stream Analytics         	| SKU            	| Standard                                                                                           	| Standard                                                                                           	| Standard                                                                                           	|
|                          	| Streaming Unit 	| 1 Units                                                                                            	| 1 Units                                                                                            	| 1 Units                                                                                            	|
| Application Insights     	| Logs collected 	| 6 GB, 5 GB of data is included for free.                                                           	| 6 GB, 5 GB of data is included for free.                                                           	| 6 GB, 5 GB of data is included for free.                                                           	|
| Log Analytics            	| Logs ingested  	| 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB                        	| 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB                        	| 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB                        	|
| Azure Automation Account 	| Capability     	| Process Automation 500 minutes of process automation and 744 hours of watchers are free each month 	| Process Automation 500 minutes of process automation and 744 hours of watchers are free each month 	| Process Automation 500 minutes of process automation and 744 hours of watchers are free each month 	|
| Notification Hub         	| SKU            	| Free                                                                                               	| Free                                                                                               	| Free                                                                                               	|
| Traffic Manager          	| DNS Query      	| -                                                                                                  	| 1 Million/ Month                                                                                   	| 1 Million/ Month                                                                                   	|
|                          	| Endpoint       	| -                                                                                                  	| Azure EndPoint 1 per month                                                                         	| Azure EndPoint 1 per month                                                                         	|

#### 3.2.2 Dollar Impact: 

The below Table explains the $ impact for the solutions by resources.

| Resource Name            	| Basic  	| Standard 	| Premium 	|
|--------------------------	|--------	|----------	|---------	|
| App Service Plan         	| $54.75 	| $146.00  	| $146.00 	|
| Cosmos DB                	| $25.86 	| $25.86   	| $25.86  	|
| Stream Analytics         	| $80.30 	| $160.60  	| $160.60 	|
| IoT-Hub                  	| $25.00 	| $25.00   	| $25.00  	|
| Application Insights     	| $2.30  	| $2.30    	| $2.30   	|
| Log Analytics            	| $2.30  	| $2.30    	| $2.30   	|
| Notification Hub         	| $0.00  	| $0.00    	| $0.00   	|
| Azure Automation Account 	| $0.00  	| $0.00    	| $0.00   	|
| Traffic Manager          	| $0.00  	| $0.54    	| $0.54   	|     

#### 3.2.3. Estimated Monthly Cost for each Solution:

| Resource Name          	| Basic   	| Standard 	| Premium 	|
|------------------------	|---------	|----------	|---------	|
| Estimated monthly cost 	| $190.51 	| $364.90  	| $364.90 	|

## 4. What are paired regions? 

Azure operates in multiple geographies around the world. An Azure geography is a defined area of the world that contains at least one Azure Region. An Azure region is an area within a geography, containing one or more datacenters. 

Each Azure region is paired with another region within the same geography, together making a regional pair. The exception is Brazil South, which is paired with a region outside its geography. 

**IoT Hub Manual Failover Support Geo-Paired Regions**

| **S.No**           | **Geography**           | **Paired Regions**                                                                                                                     
| -------------              | -------------       | -------------                                                                                                                    
| **1**       | North America       | East US 2 - Central US
| **2**   | North America    | Central US - East US 2  
| **3**        | North America      | West US 2 - West Central US   
| **4**      | North America      | West Central US - West US 2 
| **5**    | Canada   | Canada Central - Canada East
| **6**        | Canada      | Canada East - Canada Central   
| **7**       | Australia   | Australia East - Australia South East
| **8**       | Australia        | Australia South East - Australia East 
| **9**   | India    | Central India - South India  
| **10**       | India      | South India - Central India
| **11**   | Asia    | East Asia - South East Asia
| **12**        | Asia      | South East Asia - East Asia   
| **13**      | Japan      | Japan West - Japan East
| **14**    | Japan  | Japan East - Japan West
| **15**        | Korea      | Korea Central - Canada Central   
| **16**       | Korea        | Korea South - Korea Central
| **17**       | UK        | UK South - UK West 
| **18**   | UK    | UK West - UK South  

## 5. Deployment Guide for the Solution

To deploy the Basic, Standard or Premium Solution please refer **[Deployment Guide Documentation](https://github.com/ooha-m/MSIotDeviceManagement/blob/master/MSIotDeviceManagement-wiki/DeploymentGuide.md)**.

## 6. User Guide for the Solution

For Running Blink Application and verifying the Device Management Web application, please refer **[User Guide Documentation](https://github.com/ooha-m/MSIotDeviceManagement/blob/master/MSIotDeviceManagement-wiki/UserGuide.md)**.

## 7. Administrator Guide for the Solution

To configure and validate the Standard and Premium Solution, please refer the **[Administrator Guide Documentation](https://github.com/ooha-m/MSIotDeviceManagement/blob/master/MSIotDeviceManagement-wiki/AdminGuide.md)**.
