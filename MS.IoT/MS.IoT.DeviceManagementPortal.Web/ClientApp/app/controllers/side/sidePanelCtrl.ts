import * as angular from "angular";
import { FieldTypes } from "../../models/enums";
import { IDeviceDBService } from "../../interfaces/interfaces";
import { DeviceTwin, FeatureDefinition, Alert, ReportedProperty, Tags } from "../../models/models";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

	//side panel controller to display device Twin details
	class SidePanelCtrl {
		static $inject: Array<string> = ['$uibModalInstance', 'deviceTwin', 'deviceId', 'alerts', 'editMode', 'DeviceDBService', '$scope', '$uibModal'];

		private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;
		private deviceDBService: IDeviceDBService;
		private deviceTwin: DeviceTwin;
		private deviceId: string;
		private $scope: any;

        public reportedFeatures: any[] = [];
		public desireFeaturesList : any[] = [];
        public features: FeatureDefinition[] = [];
		public differenceTime: any;
		public editMode: Boolean;
		public datePopup: Boolean;
		public dateOptions: any;
		private $uibModal: ng.ui.bootstrap.IModalService;
        public isFormDirty = false;
        public alertDescription: string = "";


        constructor($uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, deviceTwin: DeviceTwin,
			deviceId: string, alerts : Alert[], editMode: Boolean, deviceDBService: IDeviceDBService,
			$scope: ng.IRootScopeService, $uibModal: ng.ui.bootstrap.IModalService) {
			this.$uibModalInstance = $uibModalInstance;
			this.deviceTwin = deviceTwin;
			this.deviceId = deviceId;
			this.$scope = $scope;
            this.$uibModal = $uibModal;

            //set up alert
            if (this.deviceTwin.reported.statusCode != 0 && alerts.length > 0) {
                var alertsFound: Alert[] = alerts.filter(p => p.code == this.deviceTwin.reported.statusCode);
                if (alertsFound.length > 0)
                    this.alertDescription = alertsFound[0].message;
            }

			this.deviceDBService = deviceDBService;
			this.editMode = editMode;
			this.datePopup = false;
			this.dateOptions = {
				formatYear: 'yy',
				maxDate: new Date(2100, 1, 1),
				minDate: new Date(2000, 1, 1),
				startingDay: 1
			};

			this.init();
		}

		public init() {

			console.log("Device Twin retrieved");
			this.deviceTwin.tags.shippedDate = new Date(this.deviceTwin.tags.shippedDate);

            this.$scope.$watch(this.deviceTwin.tags, function (newVal: Tags, oldVal: Tags) {
			}, true);

			if (this.deviceTwin.reported.featuresDefinitions !== null) {
				// convert object to list
                for (var x in this.deviceTwin.desired.features) {
                    this.deviceTwin.desired.features.hasOwnProperty(x) && this.desireFeaturesList.push(this.deviceTwin.desired.features[x])
                }
				for (var x in this.deviceTwin.reported.featuresDefinitions) {
					this.deviceTwin.reported.featuresDefinitions.hasOwnProperty(x) && this.reportedFeatures.push(this.deviceTwin.reported.featuresDefinitions[x])
                }

                this.features.splice(0, this.features.length);

                var needUpdateDesired = false;
                this.reportedFeatures.forEach((featureDefinition: FeatureDefinition) => {
                    var desiredFeatureStatus = this.deviceTwin.desired.features != null ? this.deviceTwin.desired.features[featureDefinition.name as any] : null;
                    if (desiredFeatureStatus == null)
                        needUpdateDesired = true;
                    var feature: FeatureDefinition = {
                        name: featureDefinition.name,
                        displayName: featureDefinition.displayName,
                        isActivated: desiredFeatureStatus != null ? desiredFeatureStatus : featureDefinition.isActivated,
                    }
                    this.features.push(feature);
                });

                if (needUpdateDesired) {
                    this.deviceDBService.initializeDeviceTwinDesiredFeatures({ "deviceId": this.deviceTwin.deviceId, "features": this.features }).then((response: any) => {
                        console.log("desired features synced");
                    })
                }
			}
        }

        public openJSONModal() {
            this.deviceDBService.getDeviceTwin(this.deviceId).then((deviceTwinRefresh: DeviceTwin) => {
                this.$uibModal.open({
                    animation: true,
                    backdrop: false,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    template: require('../../templates/modals/jsonModal.html'),
                    controller: 'JsonModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        device: deviceTwinRefresh
                    }
                }).result.then(() => { }, () => { });
            }, () => {
                console.log("ERROR loading the JSON Device Twin.");
            });
        }

		public updateDesiredFeatures(feature : FeatureDefinition, isActivated : boolean) {
			feature.isActivated = isActivated;
			var desiredFeatureUpdate = {
				"deviceId": this.deviceTwin.deviceId,
				"feature": feature
			};
			this.deviceDBService.updateDeviceTwinDesiredFeature(desiredFeatureUpdate).then((response: any) => {
				console.log("desired feature updated");
			})
		}

		public saveTags(formSidePanel : ng.IFormController) {
			this.deviceDBService.updateDeviceSync(this.deviceTwin.deviceId, null, this.deviceTwin.tags).then((response: any) => {
				console.log("device twin tags updated");
				this.editMode = false;
				this.isFormDirty = false;
				formSidePanel.$setPristine();
			})
		}

        public editCancel(formSidePanel: ng.IFormController) {
			console.log(formSidePanel)
			if (formSidePanel.$dirty)
			{
				this.isFormDirty = true;
			}
			this.editMode = false;
		}

        public noSaveCancel(formSidePanel: ng.IFormController) {
			this.refresh();
			formSidePanel.$setPristine();
			this.isFormDirty = false;
		}

		public refresh() {
			this.deviceDBService.getDeviceTwin(this.deviceTwin.deviceId).then((response: any) => {
				this.deviceTwin = response;
				this.deviceTwin.tags.shippedDate = new Date(this.deviceTwin.tags.shippedDate);
			})
		}
		public openDate() {
			this.datePopup = true;
		};

        public IsConnected(reported: ReportedProperty) {
			if (reported.ipAddress === null)
				return null;
			var devicedateTimeStamp = new Date(reported.heartbeat);
			var currentTimeStamp = new Date();

			this.differenceTime = currentTimeStamp.getTime() - devicedateTimeStamp.getTime();
			var minutes = Math.floor(this.differenceTime / 60000);

			if (minutes < 15)
				return true;
			else
				return false;
		}

		public cancel() {
			this.$uibModalInstance.close(false);
		}

		public ok() {

			this.$uibModalInstance.close(true);
		}
	}
    app.controller('SidePanelCtrl', SidePanelCtrl as any);
}
