import * as angular from "angular";
import { FieldTypes } from "../../models/enums";
import { IDeviceDBService, IGroupsService } from "../../interfaces/interfaces";
import {
    CustomGroupItem, DeviceInfoEntity, DeviceField, DeviceQueryConfiguration,
    DeviceQueryResponse
} from "../../models/models";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

	//side panel controller to display device Twin details
    class SidePanelCustomGroupCtrl {
        static $inject: Array<string> = ['$uibModalInstance', 'customGroup', 'listFields', 'customGroupId', 'editMode', 'DeviceDBService', 'GroupsService', '$scope', '$uibModal'];

        private $scope: any;
		private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;
        private groupsService: IGroupsService;
        private deviceDBService: IDeviceDBService;
        public customGroup: CustomGroupItem;
        private customGroupId: string;

        public listGroupDevices: DeviceInfoEntity[] = [];
        public listGroupDevicesIndex: number = 1;
        public listGroupDevicesItemCount: number = 0;
        private isLoadingDevices: boolean = false;

		public editMode: Boolean;
		public form: any;
		private $uibModal: ng.ui.bootstrap.IModalService;
        public isFormDirty = false;
        public hasFormErrors = false;

        public listFields: DeviceField[];
        public orderByTypes = [{ id: 0, name: "Ascending" }, { id: 1, name: "Descending" }];

        constructor($uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, customGroup: CustomGroupItem, listFields: DeviceField[],
            customGroupId: string, editMode: Boolean, deviceDBService: IDeviceDBService, groupsService: IGroupsService,
			$scope: ng.IRootScopeService, $uibModal: ng.ui.bootstrap.IModalService) {
            this.$uibModalInstance = $uibModalInstance;
            this.listFields = listFields;
            this.customGroup = customGroup;
            this.customGroupId = customGroupId;
			this.$scope = $scope;
            this.$uibModal = $uibModal;
            this.groupsService = groupsService;
            this.deviceDBService = deviceDBService;
            this.editMode = editMode;

            this.refreshDatatable();
        }

        public refreshDatatable() {
            if (this.isLoadingDevices)
                return;

            this.isLoadingDevices = true;
            this.listGroupDevices.splice(0, this.listGroupDevices.length);

            //Building query object
            var query: DeviceQueryConfiguration = {
                itemsPerPage: 10,
                where: this.customGroup.where,
                pageIndex: this.listGroupDevicesIndex - 1
            }

            this.deviceDBService.getDevicesTwinInfo(query).then((response: DeviceQueryResponse) => {
                this.listGroupDevices = response.items;
                this.listGroupDevicesItemCount = response.itemsCount;
                this.isLoadingDevices = false;
            })
        }

        public saveCustomGroup(formSideCustomGroupPanel: ng.IFormController) {
            this.hasFormErrors = formSideCustomGroupPanel.$invalid;
            if (!this.hasFormErrors) {
                this.groupsService.updateCustomGroup(this.customGroup).then((response: any) => {
                    this.editMode = false;
                    this.isFormDirty = false;
                    formSideCustomGroupPanel.$setPristine();
                    this.listGroupDevicesIndex = 1;
                    this.refreshDatatable();
                });
            }
        }

        public pageChanged() {
            this.refreshDatatable();
            console.log('Page changed to: ' + this.listGroupDevicesIndex);
        };

        public editCancel(formSideCustomGroupPanel: ng.IFormController) {
            if (formSideCustomGroupPanel.$dirty) {
                this.isFormDirty = true;
            } else {
                this.hasFormErrors = false;
                this.editMode = false;
                this.refreshCustomGroup();
            }
        }

        public noSaveCancel(formSideCustomGroupPanel: ng.IFormController) {
            this.refreshCustomGroup();
            formSideCustomGroupPanel.$setPristine();
            this.isFormDirty = false;
            this.editMode = false;
        }

        public refreshCustomGroup() {
            this.groupsService.getCustomGroupById(this.customGroupId).then((response: CustomGroupItem) => {
                this.customGroup = response;
            })
        }

		public cancel() {
			this.$uibModalInstance.close(false);
		}

		public ok() {

			this.$uibModalInstance.close(true);
		}
	}
    app.controller('SidePanelCustomGroupCtrl', SidePanelCustomGroupCtrl as any);
}
