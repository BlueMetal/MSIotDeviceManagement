import * as angular from "angular";
import { PublishFeatureType } from "../../models/models";
import { IDeviceDBService } from "../../interfaces/interfaces";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

    //modal to confirm delete devices

	class PublishFeatureModalCtrl {
		static $inject: Array<string> = ['$uibModalInstance', 'title', 'content', 'selectedDevices',
			'DeviceDBService'];

		private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;
		private title: string;
		private content: string;
		private features: any;
		private selectedFeature?: PublishFeatureType = undefined;
		private featureDescription: any;
		private selectedDevices: any;
		private deviceDBService: IDeviceDBService;
		public featureResponse: string = "";
		public isPublishClicked = false;
		public isError = "";
		public formPublish: ng.IFormController;
		public formSubmitted = false;

		constructor($uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, title: string,
			content: string, selectedDevices: any, deviceDBService: IDeviceDBService, form: ng.IFormController) {
			this.$uibModalInstance = $uibModalInstance;
			this.title = title;
			this.content = content;
			this.selectedDevices = selectedDevices;
			this.deviceDBService = deviceDBService;
			this.formPublish = form;

			this.features = [
				{ "name": "Brew Strength", "value": "brewStrengthFeature", "methodName":"changeBrewStrength" },
				{ "name": "Brew", "value": "brewFeature", "methodName": "launchBrew" },
				{ "name": "Grind and Brew", "value": "grindAndBrewFeature", "methodName": "launchGrindAndBrew" }
			]
		}

		public publish(form: ng.IFormController) {
			this.formSubmitted = true;
			if (form.$valid && this.selectedFeature != undefined)
			{
				this.isPublishClicked = true;
				var publishFeaturemodel = {
					"deviceIdList": this.selectedDevices,
					"featureName": this.selectedFeature.name,
					"id": this.selectedFeature.value,
					"featureDescription": this.featureDescription,
					"methodName": this.selectedFeature.methodName,
					"notificationType":"Feature"
				}
				this.deviceDBService.publishFeature(publishFeaturemodel).then((response: any) => {
					console.log("publish feature notification sent");
					this.featureResponse = "Published feature successfully";
					this.isPublishClicked = false;
					this.isError = "successText";
				}).catch((error: any) => {
					this.featureResponse = "Error publishing Feature";
					this.isPublishClicked = false;
					this.isError = "errorText";
				})
			}			
		}

		public cancel() {
			this.$uibModalInstance.close(false);
		}

		public ok() {
			this.$uibModalInstance.close(true);
		}
	}
	app.controller('PublishFeatureModalCtrl', PublishFeatureModalCtrl);
}
