<#
   This script will deploy the resources required for MSIoT Solution in Azure
   parameters required are- 
    -applicationName
    -location
#>


param (
    [string][Parameter(Mandatory=$true)][ValidateNotNull()][ValidateScript({If ($_ -match '^[a-zA-Z0-9_-]*$') {
        $True
    } Else {
        Throw "$_ Application name should contain only alphanumeric characters, underscore and hyphen"
    }})]$applicationName, 
    [string][Parameter(Mandatory=$true)][ValidateSet("australiaeast","australiasoutheast","centralindia","centralus","eastasia",
                                                    "eastus","eastus2","japaneast","japanwest","northeurope","southindia",
                                                    "southeastasia","uksouth","ukwest","westcentralus","westeurope",
                                                    "westus","westus2")]$location
)

########################################
# Install and import required module
########################################
Install-Module AzureRM
Import-Module AzureRM
Install-Module AzureADPreview
Import-Module AzureADPreview

# constant variables required
$TemplateWebIotCosmos='https://msiotsolutiondev.blob.core.windows.net/template/MSIoTProvisionNetCore-WebIoTCosmos.json?st=2018-02-08T07%3A40%3A00Z&se=2020-03-09T07%3A40%3A00Z&sp=rl&sv=2017-04-17&sr=c&sig=PyYQCoEfx1j1yJ3tnEZjX8W3PVJzmJPbaBD2IS0dLFM%3D';
$TemplateStreamAnalytics='https://msiotsolutiondev.blob.core.windows.net/template/MSIoTProvisionNetCore-StreamAnalytics.json?st=2018-02-08T07%3A40%3A00Z&se=2020-03-09T07%3A40%3A00Z&sp=rl&sv=2017-04-17&sr=c&sig=PyYQCoEfx1j1yJ3tnEZjX8W3PVJzmJPbaBD2IS0dLFM%3D';
$dataPacketDesignerpPackageWebZipUri='https://msiotsolutiondev.blob.core.windows.net/webpublish/MS.IoT.DataPacketDesigner.Web.NetCore.zip?st=2018-02-08T01%3A40%3A00Z&se=2020-03-09T02%3A40%3A00Z&sp=rl&sv=2017-04-17&sr=c&sig=SHiZpyfwfeGQm4TvkBIHSOS7GhIdqoMb1qcyc2%2B9d%2Fc%3D';
$deviceManagementPortalPackageWebZipUri='https://msiotsolutiondev.blob.core.windows.net/webpublish/MS.IoT.DeviceManagementPortal.Web.NetCore.zip?st=2018-02-08T01%3A40%3A00Z&se=2020-03-09T02%3A40%3A00Z&sp=rl&sv=2017-04-17&sr=c&sig=SHiZpyfwfeGQm4TvkBIHSOS7GhIdqoMb1qcyc2%2B9d%2Fc%3D';

$resourcegroup=$applicationName

########################################
# Create Azure Active Directory in users tenant
# params required- 
    # identifier Uri
    # clientsecret
# returns Azure AD Application object
########################################
function CreateAzureADApplication {
    param([string]$URI, $ClientSecretSecure)
    Write-Host "`nCreating Application in Azure AD" -ForegroundColor Green   
    $endDate = [System.DateTime]::Now.AddYears(2)
    $azureAdApplication = New-AzureRmADApplication -DisplayName $applicationName -IdentifierUris $URI -Password $ClientSecretSecure -EndDate $endDate -Verbose
    return $azureAdApplication
}

########################################
# Create Client secret key required for Azure AD application
# returns clientsecret string key
########################################
function CreateClientSecret {
    Write-Host "`nCreating Client Secret Key" -ForegroundColor Green
    $bytes = New-Object Byte[] 32
    $rand = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $rand.GetBytes($bytes)
    $ClientSecret = [System.Convert]::ToBase64String($bytes)
    return $ClientSecret
}

########################################
# Set Windows Active directory signin permissions in AD application
# params required- 
    # tenant Id
    # Azure AD APplication objectID
########################################
function SetAzureADApplicationResourceAccessPermissions {
    param([string]$tenantId, $appObjId)
    Write-Host "`nSetting Windows Active directory signin permissions on AD application" -ForegroundColor Green   
    $authority="https://login.microsoftonline.com/$tenantId"

    $clientId = "1950a258-227b-4e31-a9cf-717495945fc2"  # Set well-known client ID for AzurePowerShell
    $redirectUri = "urn:ietf:wg:oauth:2.0:oob" # Set redirect URI for Azure PowerShell
    $resourceAppIdURI = "https://graph.windows.net/" # resource we want to use
    
    # Create Authentication Context tied to Azure AD Tenant
    $authContext = New-Object "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" -ArgumentList $authority
    # Acquire graph token
    $authResult = $authContext.AcquireToken($resourceAppIdURI, $clientId, $redirectUri, "Auto")
    Write-Host "Access token is " $authResult.AccessToken -ForegroundColor Magenta

    $authHeader = $authResult.CreateAuthorizationHeader()
    $headers = @{"Authorization" = $authHeader; "Content-Type"="application/json"}    

    $url = "https://graph.windows.net/$tenantId/applications/$($appObjId)?api-version=1.6"
    $postData = "{`"requiredResourceAccess`":[{`"resourceAppId`":`"00000002-0000-0000-c000-000000000000`",
    `"resourceAccess`":[{`"id`":`"311a71cc-e848-46a1-bdf8-97ff7156d8e6`",`"type`":`"Scope`"}]}]}";
    $result = Invoke-RestMethod -Uri $url -Method "PATCH" -Headers $headers -Body $postData 
    
    Write-Host "`nWindows Active directory signin permissions on AD application set Successfully...!" -ForegroundColor Green 
}

########################################
# Get subscriptions list and select subscription
# returns selected subscription object
########################################
function Select-Subscription {
      Write-Host "`nSelecting Subscription" -ForegroundColor Green
      $subscription = ""
      $allSubscriptions = Get-AzureRmSubscription -Verbose
      switch ($allSubscriptions.Count) {
             0 {Write-Error "No Operations Management Suite workspaces found"}
             1 {return $allSubscriptions}
        default {
            $uiPrompt = "Enter the number corresponding to the Azure subscription you would like to work with.`n"

            $count = 1
            foreach ($subscription in $allSubscriptions) {
                $uiPrompt += "$count. " + $subscription.Name + " (" + $subscription.SubscriptionId + ")`n"
                $count++
            }
            $answer = (Read-Host -Prompt $uiPrompt) - 1
            $subscription = $allSubscriptions[$answer]
            Write-Host $subscription.Name
        }  
    }
    return $subscription
}

########################################
# function to check format validation of deployment template
# returns selected subscription object
########################################
function Format-ValidationOutput {
    param ($ValidationOutput, [int] $Depth = 0)
    Set-StrictMode -Off
    return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

########################################
# Get access token of the deployed application to call api
# params required-
    # tenantId
    # clientId
    #clientSecret
# returns accessToken
########################################
function Get-AccessTokenAPI{
param([string]$tenantId,[string]$clientId,[string]$clientSecret)

    Write-Host "`nRequesting access token.." -ForegroundColor Green

    $tokenParams = @{
	  client_id=$clientId;
  	  client_secret=$clientSecret;
      resource=$clientId;
	  grant_type='client_credentials';
	  scope='openid'
	}

    $baseUri="https://login.microsoftonline.com/"+$tenantId+"/oauth2/token"
    $response=Invoke-RestMethod -Uri $baseUri -Method POST -Body $tokenParams
 
    return $response.access_token
}


########################################
# Initialize cosmosDb with default starter templates
# params required-
    # apiBaseUrl
    # accessToken
# returns accessToken
########################################
function CosmosDbInit{
param([string]$apiBaseURL,[string]$accessToken)

    Write-Host "`nInitializing Cosmos DB..." -ForegroundColor Green
    $APIURL=[string]::Format("{0}/api/templates/generate",$apiBaseURL)
    $APIURL=$APIURL.Replace('https','http')

    $result=Invoke-RestMethod -Uri $APIURL -Method Post -ContentType "application/json" -Headers @{ "Authorization" = "Bearer $accessToken" }
    if($result -eq "true")
    {
        Write-Host "`nCosmos DB initialized successfully..!" -ForegroundColor Green    
        return $true;
    }
    else
    {
        Write-Host "`nError !! Cosmos DB not initialized!" -ForegroundColor Red  
        return $false;  
    }
}

#####################################################################
# Login user
#####################################################################
Write-Host "`nSigning in user" -ForegroundColor Green
Login-AzureRmAccount;
$aadContext=Get-AzureRmContext -Verbose

if(!$aadContext.Account)
{
    exit;
}
Write-Host "`nUser Signedin successfully..!" -ForegroundColor Green
# Get tenant Id
$tenantId=$aadContext.Tenant.Id
# form Azure AD identifier URI
$URI=[string]::Format("https://{0}/{1}",$tenantId,$applicationName)


#####################################################################
# Create Azure AD application and service principal
#####################################################################
$clientSecret=CreateClientSecret
Write-Host 'Client secret created successfully-' $clientSecret -ForegroundColor Green
$ClientSecretSecure = ConvertTo-SecureString $ClientSecret -AsPlainText -Force
$azureAdApplication=CreateAzureADApplication $URI $ClientSecretSecure
if(!$azureAdApplication)
{
    Write-Host "`nError..! Cannot create Azure AD application..!!" -ForegroundColor Red
    exit
}
Write-Host "`nAzure AD Application created successfully..!" -ForegroundColor Green

# Create a service principal
Write-Host 'Creating Service Principal' -ForegroundColor Green
New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId -Verbose


###################################################################
# Assign Windows Azure AD permissions to sign in to the application
# This enables all users in the tenant to login into application
###################################################################
SetAzureADApplicationResourceAccessPermissions $tenantId $azureAdApplication.ObjectId

#####################################################################
# Get and select subscription
#####################################################################
$subscription = Select-Subscription
if(!$subscription)
{
    Write-Host "`nSorry..! No Subscription found..!!" -ForegroundColor Red
    exit
}
$subscriptionId = $subscription.SubscriptionId
Select-AzureRmSubscription -SubscriptionId $subscriptionId
Write-Host "`nSubscription selected is- " $subscription.Name -ForegroundColor Green


#####################################################################
# Create Azure Resource Group
#####################################################################
Write-Host "`nCreating Resource Group.." -ForegroundColor Green
$resourceGroupResponse=New-AzureRmResourceGroup -Name $resourcegroup -Location $location -Verbose
if(!$resourceGroupResponse)
{
    exit;
}
Write-Host "`nResource group" $resourcegroup " created successfully..!" -ForegroundColor Green


#####################################################################
# Build parameters for ARM template
#####################################################################
$clientId=$azureAdApplication.ApplicationId
Write-Host "`nBuilding Parameters required for ARM template deployment" -ForegroundColor Green
$parameters = @{}
$parameters.Add(“clientId”, $clientId)
$parameters.Add(“clientSecret”, $clientSecret)
$parameters.Add(“tenantId”, $tenantId)
$parameters.Add(“dataPacketDesignerpPackageWebZipUri”, $dataPacketDesignerpPackageWebZipUri)
$parameters.Add(“deviceManagementPortalPackageWebZipUri”, $deviceManagementPortalPackageWebZipUri)
$parameters | ConvertTo-Json


#####################################################################
# Deploying Solution - DataPacket Designer, IoTHub, CosmosDB
#####################################################################
$ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourcegroup `
                                                                                  -TemplateFile $TemplateWebIotCosmos `
                                                                                  -TemplateParameterObject $parameters `
                                                                                  -Verbose
                                                                                  )
if ($ErrorMessages) {
        Write-Host '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.' -ForegroundColor Red
    }
    else {
        Write-Host '', 'Template is valid.' -ForegroundColor Green
        $result=New-AzureRmResourceGroupDeployment -ResourceGroupName $resourcegroup `
                                                    -TemplateFile $TemplateWebIotCosmos `
                                                    -TemplateParameterObject $parameters `
                                                    -Force -Verbose `
                                                    -ErrorVariable ErrorMessages

         Write-Host '', "`nDeployed resources to Azure successfully..!" -ForegroundColor Green
         Write-Host '', "`nJSON output..`n`n" -ForegroundColor Green
         $result.Outputs | ConvertTo-Json
         Write-Host '', "`nThe Data Packet Designer URL is - Please copy -" -ForegroundColor Green
         $datapacketUri=$result.Outputs.dataPacketDesignerUrl.Value
         Write-Host $datapacketUri -ForegroundColor Green
         Write-Host '', "`nThe Device Management Portal URL is - Please copy -" -ForegroundColor Green
         $deviceManagementUri=$result.Outputs.deviceManagementPortalUrl.Value
         Write-Host $deviceManagementUri -ForegroundColor Green
         $iotHubName=$result.Outputs.iotHubName.Value;
         $cosmosDBAccountName=$result.Outputs.cosmosDBAccountName.Value;


        #####################################################################
        # Update Azure AD applications reply url
        #####################################################################
         $datapacketUriOIDC=$datapacketUri+"/signin-oidc"      
         $deviceManagementUriOIDC=$deviceManagementUri+"/signin-oidc"   
         $replyURLList = @($datapacketUriOIDC,$deviceManagementUriOIDC);  
         Write-Host '', 'Configuring and setting the Azure AD reply URLs' -ForegroundColor Green
         Set-AzureRmADApplication -ApplicationId $azureAdApplication.ApplicationId -HomePage $datapacketUri -ReplyUrls $replyURLList -Verbose

        
        #####################################################################
        # Get Access token for calling API
        #####################################################################
         $accessToken=Get-AccessTokenAPI $tenantId $clientId $clientSecret
         Write-Host "Access token is " $accessToken -ForegroundColor Magenta
         #add sleep inorder to have API startup successfully
         Start-Sleep -s 10


        #####################################################################
        # Call API to initialize cosmos DB with default starter IoT templates
        # This templates will be loaded in the data packet designer
        #####################################################################
         $cosmosDBInitResult=CosmosDbInit $datapacketUri $accessToken         
         if($cosmosDBInitResult -eq $true)
         {               
                #####################################################################
                # Build Parameters to deploy stream analytics template
                #####################################################################
                Write-Host "`nBuilding Parameters required for Stream Analytics ARM template deployment" -ForegroundColor Green
                $parametersASAJob = @{}
                $parametersASAJob.Add(“iotHubName”, $iotHubName)
                $parametersASAJob.Add(“cosmosDbAccountName”, $cosmosDBAccountName)
                $parametersASAJob.Add(“cosmosDbName”,"MSIoT")
                $parametersASAJob.Add(“cosmosDBMessageCollectionName”,"Messages")
                $parametersASAJob | ConvertTo-Json


                #####################################################################
                # Deploying Stream analytics solution template
                #####################################################################
                $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourcegroup `
                                                                                                  -TemplateFile $TemplateStreamAnalytics `
                                                                                                  -TemplateParameterObject $parametersASAJob `
                                                                                                  -Verbose
                                                                                                  )
                if ($ErrorMessages) {
                        Write-Host '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.' -ForegroundColor Red
                    }
                    else {
                        Write-Host 'Template is valid.' -ForegroundColor Green
                         $result=New-AzureRmResourceGroupDeployment -ResourceGroupName $resourcegroup `
                                                                    -TemplateFile $TemplateStreamAnalytics `
                                                                    -TemplateParameterObject $parametersASAJob `
                                                                    -Force -Verbose `
                                                                    -ErrorVariable ErrorMessages
                    if ($ErrorMessages) {
                        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
                    }
                    else
                    {
                        Write-Host '', "`nDeployed Stream Analytics Job to Azure successfully..!" -ForegroundColor Green
                    }
                }

         }

    if ($ErrorMessages) {
        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
    }
    else
    {
        Write-Host "`nCongratulations..!! your MSIoT solution accelerator application is ready to use...! Please login into https://portal.azure.com for details" -ForegroundColor Green
        Write-Host "`nHere is the link to Data Packet Designer solution - " $datapacketUri -ForegroundColor Green
        Write-Host "`nHere is the link to Device Managemet Solution - " $deviceManagementUri -ForegroundColor Green
        Start-Process $datapacketUri
        Start-Process $deviceManagementUri
    }
}