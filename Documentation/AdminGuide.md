# Microsoft

# White Goods Solution

## Administrator Guide

**Table of Contents** 

- [1 Admin Guide](#1-admin-guide)
- [2 Performing DR Strategies](#2-performing-dr-strategies)
    - [2.1 Standard Solution Type](#21-standard-solution-type)
        - [2.1.1 IoT Hub Manual Failover](#211-iot-hub-manual-failover)
        - [2.1.2 Stop Stream Analytics Job in Primary Region](#212-stop-stream-analytics-job-in-primary-region)
        - [2.1.3 Accessing Traffic manager](#213-accessing-traffic-manager)
        - [2.1.4 Stopping the Web App](#214-stopping-the-web-app)
        - [2.1.5 Redeploy the Region 2 ARM Template](#215-redeploy-the-region-2-arm-template)
        - [2.1.6 Configure Region 2 web app to Traffic Manager](#216-configure-region-2-web-app-to-traffic-manager)
        - [2.1.7 Cosmos DB Geo replication](#217-cosmos-db-geo-replication)
        - [2.1.8 Accessing Web App](#218-accessing-web-app)
    - [2.2 Premium Solution Type](#22-premium-solution-type)
        - [2.2.1 IoT Hub Manual Failover](#221-iot-hub-manual-failover)
        - [2.2.2 Stop Stream Analytics Job in Primary Region](#222-stop-stream-analytics-job-in-primary-region)
        - [2.2.3 Accessing Traffic manager](#223-accessing-traffic-manager)
        - [2.2.4 Stopping the Web App](#224-stopping-the-web-app)
        - [2.2.5 Cosmos DB Geo replication](#225-cosmos-db-geo-replication)
        - [2.2.6 Accessing Web App](#226-accessing-web-app)

## 1 Admin Guide 

This document explains how to use the solution in addition to User Document. In this we are configuring, verifying and performing DR activities for Standard and Premium solutions.  

## 2 Performing DR Strategies

### 2.1 Standard Solution Type

In this scenario, there is again a primary and a secondary Azure region. All the traffic goes to the active deployment on the primary region. The secondary region is better prepared for disaster recovery because the database is running on both regions.  

Only the primary region has a deployed cloud service application. Both regions are synchronized with the contents of the database. When a disaster occurs, there are fewer activation requirements. You redeploy azure resources in the secondary region. 

Redeployment approach, you should have already stored the service packages. However, you don’t incur most of the overhead that database restore operation requires, because the database is ready and running.  This saves a significant amount of time, making this an affordable DR pattern. 

Standard Solution requires redeployment of azure resources in secondary region when the primary region is down. 

When user chooses Standard Solution type below azure resources will be deployed in primary region and Cosmos DB in both Regions. 

* App service
* Application insights
* Automation account
* Azure cosmos DB
* IoT Hub
* Log analytics
* Notification Hub
* Stream analytics job
* Traffic manager profile.

When Primary region is down, and user needs to redeploy azure resources to new region. Once redeployment gets completed below resources will get deployed.

* App service
* Application insights
* Notification hub
* Stream analytics Job

*Refer **4.1 and 4.2** Section in **Deployment Guide** for Standard Solution Deployment.*

#### 2.1.1 IoT Hub Manual Failover

Go to **Resource Group** -> Click on **IoT Hub**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/142.png">
</p>

Go to **Manual Failover (Preview)** from left side menu.

Click on **Initiate failover** to initiate manual failover of IoT Hub.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/143.png">
</p>

When failover process started, a pop up will be displayed on right top corner. 

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/144.png">
</p>

Once Manual Failover process completed, Primary Location and secondary location will interchange.

**Note**: This process will take around 15 mins. To initial failover again, user needs wait for 1 hour to run failover again.

#### 2.1.2 Stop Stream Analytics Job in Primary Region

Go to **Resource Group** and click on primary **Stream Analytics job**

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S1.png">
</p>

Stop the Stream analytics job by click on **Stop** and click on **Yes** for confirmation.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S2.png">
</p>

#### 2.1.3 Accessing Traffic manager 

Go to **Resource Group** and click on **Traffic manager** resource.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S3.png">
</p>

Now you can see the web app as the endpoint of the traffic manager.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/148.png">
</p>

#### 2.1.4 Stopping the Web App

Navigate to the **Web App** from resource group.

click on **Stop** then click on **Yes** to stop the Web App.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/149.png">
</p>

The Web App in the primary region has been stopped as failover.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/150.png">
</p>

Verify the same in traffic manager.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S4.png">
</p>

#### 2.1.5 Redeploy the Region 2 ARM Template

Go to **Github** and select **re-deploy.json** file from the **master** branch.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/152.png">
</p>

Click on **Raw**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/153.png">
</p>

 Copy the **re-deploy.json** template.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/154.png">
</p>

Click on **Add** in existing resource group and re-deploy the ARM template.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/155.png">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/156.png">
</p>

#### 2.1.6 Configure Region 2 web app to Traffic Manager

Go to **Resource Group** -> **Traffic manager profile** -> **Endpoints**, click on **add**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/157.png">
</p>

**Enter** the name for the **second End point** of traffic manager and select the second web app as the target resource.

Click on **Ok**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/158.png">
</p>

You can see the second region web app with online status under endpoint of traffic manager and **Copy** the **DNS Name**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S5.png">
</p>

#### 2.1.7 Cosmos DB Geo replication

Go to **Resource Group** -> click **Cosmos DB**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S6.png">
</p>

Navigate to **Replicate data globally** under Settings section then click **Manual failover**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/161.png">
</p>

**Select** the **Read Region** to become the **new write region**, check in the check box and click **ok**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/162.png">
</p>

#### 2.1.8 Accessing Web App

Go to **Resource Group** -> **Settings** -> **Deployments**, select **Microsoft Template**.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/163.png)

Select **output** and **copy** the **devicemanagement_trafficmanager** url.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/164.png)

**paste** the copied URL in new browser to access the Web App using your credentials.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/165.png)

You can see the **Device summary** as shown below.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/166.png)

### 2.2 Premium Solution Type

Both the primary region and the secondary region have a full deployment. This deployment includes the cloud services and a synchronized database. However, only the primary region is actively handling network requests. The secondary region becomes active only when the primary region experiences a service disruption. In that case, all new network requests route to the secondary region. Azure Traffic Manager can manage this failover automatically.

Failover occurs faster than the database-only variation because the services are already deployed. This topology provides a very low RTO. The secondary failover region must be ready to go immediately after failure of the primary region.

For the fastest response time with this model, you must have similar scale (number of role instances) in the primary and secondary regions. 

When user chooses premium as solution type below azure resource will be deployed in both regions.

* 2 app services
* 2 application insights
* 1 automation account
* 1 cosmos DB
* 1 IoT Hub
* 1 Log Analytics
* 2 Notification Hubs
* 2 Stream analytics job
* 1 Traffic manager

*Refer **4.1 and 4.2** Section in **Deployment Guide** for Premium Solution Deployment.*

#### 2.2.1 IoT Hub Manual Failover

Go to **Resource Group** and Click on **IoT Hub**.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S7.png)

Go to **Manual Failover(Preview)** from left side menu.

Click on **Initiate failover** to initiate manual failover of IoT Hub.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/168.png)

You can check the status of the failover.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/169.png)

When failover process started, a pop up will be displayed on right top corner. Once Manual Failover process completed, Primary Location and secondary location will interchange.

**Note**: This process will take around 15 mins. To initial failover again, user needs wait for 1 hour to run failover again.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/170.png">
</p>

#### 2.2.2 Stop Stream Analytics Job in Primary Region

Go to **Resource Group** and click on primary **Stream Analytics job**.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/S8.png)

Stop the Stream analytics job by click on **Stop** and click on **Yes** for confirmation.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/172.png)

You can see the stream analytics job has been stopped.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/173.png)

#### 2.2.3 Accessing the Traffic manager 

Go to **Resource Group** and click on **Traffic manager** resource.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/174.png)

Now you can see the web app as the endpoint of the traffic manager.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/175.png)

#### 2.2.4 Stopping the Web App

Navigate to the **Web App** from the resource group.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/176.png)

Select the **primary Web App** click on **Stop** then click on **Yes** to stop the Web App.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/177.png)

The Web App in the primary region has been stopped as failover.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/178.png)

Verify the Status of primary web app in traffic manager.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/179.png)

#### 2.2.5 Cosmos DB Geo replication

Go to **Resource Group** -> click **Cosmos DB**.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/180.png)

Navigate to **Replicate data globally** under Settings section then click **Manual failover**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/181.png">
</p>

**Select** the **Read Region** to become the **new write region**, check in the check box and click **ok**.

<p align="center">
  <img src="https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/182.png">
</p>

#### 2.2.6 Accessing Web App

Go to **Resource Group** -> **Settings** -> **Deployments**, select **Microsoft Template**.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/183.png)

Select **output** and **copy** the **devicemanagement_trafficmanager** url.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/184.png)

**paste** the copied URL in new browser to access the Web App using your credentials.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/185.png)

You can see the **Device summary** as shown below.

![alt text](https://github.com/sysgain/MSIotDeviceManagement/raw/master/Images/186.png)
