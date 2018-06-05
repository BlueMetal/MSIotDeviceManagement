var app = angular.module('MSIoT', []);

// Home view controller
app.controller("homectrl", function ($scope, $http, $location) {
    $('#videoModal').on('shown.bs.modal', function () {
        $('#videoShow')[0].play();
    })
    $('#videoModal').on('hidden.bs.modal', function () {
        $('#videoShow')[0].pause();
    })
});


// Get started view controller
app.controller("getStartedController", function ($scope, $http, $location) {
    
});


//Deploy View Controller
app.controller("deployController", function ($scope, $http, $interval) {
    $scope.isError = false;
    $scope.isPageLoading = true;
    $scope.isSubmitDisabled = false;
    $scope.isLoading = false;
    $scope.isDeployed = false;
    $scope.isAnySubscription = false;

    $scope.isAppCreating = false;
    $scope.isAppCreated = false;

    $scope.isAppDeploying = false;
    $scope.isAppDeployed = false;

    $scope.isAppConfiguring = false;
    $scope.isAppConfigured = false;

	var stop;

	// Get subscriptions
    $http.get("/api/resourcemanager/subscriptions")
        .then(function (response) {
            $scope.isPageLoading = false;
            $scope.subscriptions = response.data;
            if ($scope.subscriptions.SubscriptionList.length === 0) {
                $scope.isAnySubscription = true;
                $scope.isDeployed = true;
            }
        }, function (error) {
            $scope.errorMessage = error.data.Message;
            $scope.isError = true;
            $scope.isPageLoading = false;
        });

	// get locations
    $scope.getLocations = function () {
        $http.get("/api/resourcemanager/subscriptions/" + $scope.deploy.subscription.SubscriptionId + "/locations")
            .then(function (response) {
                $scope.locations = response.data;
            });
    }

	// Function call to provsion resources
    $scope.deployTemplate = function () {
        $scope.submitted = true;

        if ($scope.provision.$valid === true) {

            $scope.isSubmitDisabled = true;
            $scope.isLoading = true;
            $scope.isAppCreating = true;
            $scope.isAppCreatingText = true;

            // create application in Azure AD
            $scope.appData = {
                "ApplicationName": $scope.deploy.applicationName,
                "SubscriptionId": $scope.deploy.subscription.SubscriptionId,
                "Location": $scope.deploy.location.Name,
            }

			$http.post("/api/serviceprincipal/create", $scope.appData)
				.then(function (response) {
                    $scope.aadApplication = response.data;
                    $scope.isAppCreating = false;
                    $scope.isAppCreated = true;

                    $scope.isAppDeploying = true;
                    $scope.isAppDeployingText = true;

                    var deployData = {
                        "ApplicationName": $scope.deploy.applicationName,
                        "SubscriptionId": $scope.deploy.subscription.SubscriptionId,
                        "Location": $scope.deploy.location.Name,
                        "ClientId": $scope.aadApplication.ClientId,
                        "ClientSecret": $scope.aadApplication.ClientSecret,
                        "TenantId": $scope.aadApplication.TenantId,
                    }

                    // deploy main data packet designer iot solution template
                    $http.post("/api/resourcemanager/deploy", deployData)
						.then(function (response) {
                            $scope.deploySuccess = response.data;                      

							$scope.deployStatusRequest = {
								"ApplicationName": $scope.appData.ApplicationName,
								"SubscriptionId": $scope.appData.SubscriptionId,
							}

							// timer to check status of deployment every 10 seconds
							stop=$interval(checkDeploymentStatus, 10000);
                            
                        },function (error) {
                            $scope.errorMessage = error.data.Message;
                            $scope.isError = true;
                            $scope.isLoading = false;
                            $scope.isAppCreatingText = false;
                            $scope.isAppDeploying = false;
                            $scope.isAppDeployingText = false;
                            $scope.isAppCreated = false;
                        });
                });
        }
	}

	// check status of solution deployment
	function checkDeploymentStatus() {
			
		$http.post("/api/resourcemanager/deploy/status", $scope.deployStatusRequest)
			.then(function (response) {
				$scope.deployStatusResponse = response.data;
				if ($scope.deployStatusResponse.properties.provisioningState === "Succeeded")
				{
					$scope.stopInterval();
					$scope.ConfigureApplication();
				}
			});
	}


	// stop status check after deployment succeeds
	$scope.stopInterval = function () {
		if (angular.isDefined(stop)) {
			$interval.cancel(stop);
			stop = undefined;
		}
	};


	// Update application reply url and setup cosmos collection and stream analytics
	$scope.ConfigureApplication = function () {

		$scope.isAppDeploying = false;
		$scope.isAppDeployed = true;

		// update application reply url
		var updateApplicationData = {
			"AppObjectId": $scope.aadApplication.AppObjectId,
			"HomePage": $scope.deployStatusResponse.dataPacketDesignerUrl.value,
			"ReplyUrls": [
				$scope.deployStatusResponse.dataPacketDesignerUrl.value,
				$scope.deployStatusResponse.deviceManagementPortalUrl.value
			]
		}

		$scope.isAppConfiguring = true;
		$scope.isAppConfiguringText = true;
		// Set reply url
		$http.put("/api/serviceprincipal/application/update", updateApplicationData)
			.then(function (response) {

				$scope.updateSuccess = response.data;
				$scope.isAppConfigured = false;
				$scope.isAppConfiguring = false;
				$scope.isAppDeployed = false;
				$scope.isAppCreated = false;
				$scope.isAppConfigured = false;
				$scope.isSubmitDisabled = false;
				$scope.isLoading = false;
				$scope.isDeployed = true;
				$scope.isAppCreatingText = false;
				$scope.isAppDeployingText = false;
				$scope.isAppConfiguringText = false;
			});

		var cosmosModel = {
			"CosmosDBAccountEndPoint": $scope.deployStatusResponse.cosmosDBAccountEndPoint.value,
			"CosmosDBAccountKey": $scope.deployStatusResponse.cosmosDBAccountKey.value
		}

		// cosmos db init
		$http.post("/api/commonTemplates/generate", cosmosModel)
			.then(function (response) {
				$scope.templateSuccess = response.data;

				// deploy stream analytics connecting Iot hub and cosmos db
				var deployASAData = {
					"ResourceGroupName": $scope.deploy.applicationName,
					"SubscriptionId": $scope.deploy.subscription.SubscriptionId,
					"Location": $scope.deploy.location.Name,
					"IoTHubName": $scope.deployStatusResponse.iotHubName.value,
					"CosmosDBAccountName": $scope.deployStatusResponse.cosmosDBAccountName.value,
					"CosmosDBName": "MSIoT",
					"CosmosDBMessageCollectionName": "Messages"
				}

				$http.post("/api/resourcemanager/deploystreamanalytics", deployASAData)
					.then(function (response) {
						$scope.templateASASuccess = response.data;

					});
			});	
	}
});