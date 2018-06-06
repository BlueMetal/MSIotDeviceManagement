import * as angular from "angular";
import { FieldTypes } from "../../models/enums";
import { UpdatePropertyDefinition, DeviceUpdateResult } from "../../models/models";
import { IDeviceDBService } from "../../interfaces/interfaces";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

    //modal to confirm delete devices

	class UpdatePropertiesModalCtrl {
        static $inject: Array<string> = ['$uibModalInstance', '$uibModal', 'title', 'type', 'selectedDevices', 'DeviceDBService'];

        private $uibModal: ng.ui.bootstrap.IModalService;
		private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;
        private title: string;
        private type: string;
        private properties: UpdatePropertyDefinition[] = [];
		private selectedDevices: string[];
        private deviceDBService: IDeviceDBService;
        public isUpdateClicked: boolean = false;
        public isError: boolean = false;
        public errorMessage: string = "";

        constructor($uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, $uibModal: ng.ui.bootstrap.IModalService, title: string, type : string, selectedDevices: any, deviceDBService: IDeviceDBService) {
            this.$uibModalInstance = $uibModalInstance;
            this.$uibModal = $uibModal;
            this.deviceDBService = deviceDBService;
            this.title = title;
            this.type = type;
			this.selectedDevices = selectedDevices;

            if (type == 'desired') {
                this.properties = [
                    { name: "statusCode", type: FieldTypes.Number },
                    { name: "firmwareVersion", type: FieldTypes.String },
                    { name: "features", type: FieldTypes.Object, children: [], allowedChildrenTypes: [FieldTypes.Boolean] },
                    { name: "deviceState", type: FieldTypes.Object, children: [], allowedChildrenTypes: [FieldTypes.Boolean, FieldTypes.Number, FieldTypes.Double, FieldTypes.String, FieldTypes.Date] }
                ];
            } else if (type == 'tags') {
                this.properties = [
                    { name: "productName", type: FieldTypes.String },
                    { name: "productType", type: FieldTypes.String },
                    { name: "productFamily", type: FieldTypes.String },
                    { name: "retailerName", type: FieldTypes.String },
                    { name: "retailerRegion", type: FieldTypes.String },
                    { name: "userId", type: FieldTypes.String },
                    { name: "manufacturedDate", type: FieldTypes.Date },
                    { name: "shippedDate", type: FieldTypes.Date },
                    { name: "customTags", type: FieldTypes.Object, children: [], allowedChildrenTypes: [FieldTypes.Boolean, FieldTypes.Number, FieldTypes.Double, FieldTypes.String, FieldTypes.Date] }
                ];
            }
        }

        private buildPatch(updateProperties: UpdatePropertyDefinition[]): any {
            let patch: any = {};
            updateProperties.forEach((updateProperty: UpdatePropertyDefinition) => {
                if (updateProperty.isNull) {
                    patch[updateProperty.name] = null;
                } else if (updateProperty.value != null) {
                    patch[updateProperty.name] = updateProperty.value;
                } else if (updateProperty.children != null && updateProperty.children.length > 0) {
                    patch[updateProperty.name] = this.buildPatch(updateProperty.children);
                }
            })
            return patch;
        }

        public confirmUpdate() {
            let nbrDevices = this.selectedDevices.length;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                backdrop: false,
                template: require('../../templates/modals/confirmModal.html'),
                controller: 'ConfirmModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Update " + nbrDevices + " devices?" },
                    content: function () { return "Do you really want to update these devices? It can take a few minutes. Press OK to update." }
                }
            }).result.then((result) => {
                if (result) {
                    this.update();
                }
            }, () => { });
        }

        private update() {
            let patchDesired = this.type == 'desired' ? this.buildPatch(this.properties) : null;
            let patchTags = this.type == 'tags' ? this.buildPatch(this.properties) : null;

            this.isUpdateClicked = true;

            this.deviceDBService.updateDevicesAsync(this.selectedDevices, patchDesired, patchTags).then((response: DeviceUpdateResult) => {
                let title = response.hasSucceeded ? "Properties updated!" : "Error while updating the properties";
                let description = response.hasSucceeded ? 
                    "The properties are being updated. It might take a few minutes." ://"The properties have been updated.<br/>JobId: " + response.jobId :
                    "There was an error while updating the properties: " + response.errorReason + "<br />JobId: " + response.jobId
                this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    template: require('../../templates/modals/alertModal.html'),
                    controller: 'AlertModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: () => { return title },
                        content: () => { return description },
                    }
                }).result.then(() => {
                    this.$uibModalInstance.close(false);
                    }, () => {
                    });
            }).catch((error: any) => {
                this.isError = true;
                this.errorMessage = "There was an error while updating the properties.";
            })	
        }

        public addSubProperty(desiredField : any) {
            desiredField.children.push({});
        }

        public removeProperty(desiredField : any, subProperty : any) {
            let entityIndex = desiredField.children.indexOf(subProperty);
            if (entityIndex !== -1) {
                desiredField.children.splice(entityIndex, 1);
            }
        }

		public cancel() {
			this.$uibModalInstance.close(false);
		}

		public ok() {
			this.$uibModalInstance.close(true);
		}
	}
    app.controller('UpdatePropertiesModalCtrl', UpdatePropertiesModalCtrl);
}