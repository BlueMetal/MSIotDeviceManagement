import * as angular from "angular";
import { IDeviceDBService, IGroupsService } from "../interfaces/interfaces";
import { LogicalOperators } from "../models/enums";
import { CustomGroupItem, DeviceField, DeviceQueryRuleGroup } from "../models/models";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

	//Controller that manages the group view.
	class GroupsCtrl {
        static $inject: Array<string> = ['listCustomGroups', 'listFields', '$uibModal', '$aside', '$q', 'DeviceDBService', 'GroupsService'];

		private $uibModal: ng.ui.bootstrap.IModalService;
        private $aside: any;
        private $q: ng.IQService;
        private groupsService: IGroupsService;
        private deviceDBService: IDeviceDBService;

        private isAddingGroup: boolean = false;
        public listCustomGroups: CustomGroupItem[] = [];
        public selectedCustomGroups: string[] = [];

        private listFields: DeviceField[];

		//Main Constructor
        constructor(listCustomGroups: CustomGroupItem[], listFields: DeviceField[], $uibModal: ng.ui.bootstrap.IModalService, $aside: any, $q: ng.IQService, deviceDBService: IDeviceDBService, groupsService: IGroupsService) {
			this.$aside = $aside;
			this.$uibModal = $uibModal;
            this.$q = $q;
            this.groupsService = groupsService;
            this.deviceDBService = deviceDBService;
            this.listCustomGroups = listCustomGroups;
            this.listFields = listFields;
		}
		
        public openAside(customGroupId: string, editMode: boolean) {
            var resolveCustomGroup = ['GroupsService', function (groupsService: IGroupsService) {
                return groupsService.getCustomGroupById(customGroupId);
            }];
            var listFields = this.listFields;
			this.$aside.open({
                template: require('../templates/side/sidePanelCustomGroup.html'),
				placement: 'right',
				size: 'md',
				backdrop: false,
                controller: 'SidePanelCustomGroupCtrl',
				controllerAs: 'vm',
				resolve: {
                    customGroup: resolveCustomGroup,
                    customGroupId: function () { return customGroupId },
                    listFields: function () { return listFields },
					editMode: function () { return editMode }
				}
			}).result.then((result : any) => {
                this.refresh();
			}, () => { });
        }

        //Popup to confirm the deletion of a devices
        public openPublishFeatureModal() {
            this.getMergedDevicesIds().then((deviceIds: string[]) => {
                this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    template: require('../templates/modals/publishFeatureModal.html'),
                    controller: 'PublishFeatureModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return "Publish a new Feature" },
                        content: function () { return "Select a feature and publish it to multiple devices" },
                        selectedDevices: function () { return deviceIds }
                    }
                }).result.then((result) => {
                    if (result) {

                    }
                }, () => { });
            });
        }

        public openUpdateDesiredProperties() {
            this.getMergedDevicesIds().then((deviceIds: string[]) => {
                this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    template: require('../templates/modals/updatePropertiesModal.html'),
                    controller: 'UpdatePropertiesModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return "Update desired properties" },
                        type: function () { return 'desired' },
                        selectedDevices: function () { return deviceIds }
                    }
                }).result.then((result) => {
                    if (result) {

                    }
                }, () => { });
            });
        }

        public openUpdateTagsProperties() {
            this.getMergedDevicesIds().then((deviceIds: string[]) => {
                this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    template: require('../templates/modals/updatePropertiesModal.html'),
                    controller: 'UpdatePropertiesModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return "Update tags properties" },
                        type: function () { return 'tags' },
                        selectedDevices: function () { return deviceIds }
                    }
                }).result.then((result) => {
                    if (result) {

                    }
                }, () => { });
            });
        }

        public getMergedDevicesIds(): ng.IPromise<string[]> {
            var deferred = this.$q.defer<string[]>();
            var queryGroup: DeviceQueryRuleGroup = { operator: LogicalOperators.Or, groups: [], rules: [] };
            this.listCustomGroups.forEach((customGroup: CustomGroupItem) => {
                var idx = this.selectedCustomGroups.indexOf(customGroup.id as string);
                if (idx > -1)
                    queryGroup.groups.push(customGroup.where);
            })

            this.deviceDBService.getDevicesTwinIds(queryGroup).then((deviceIds: string[]) => {
                console.log(deviceIds.length + " device ids retrieved.");
                deferred.resolve(deviceIds);
            }, (error) => {
                deferred.reject([]);
            });

            return deferred.promise;
        }

        public toggleSelectionItem(viewId: string) {
            var idx = this.selectedCustomGroups.indexOf(viewId);

            // Is currently selected
            if (idx > -1) {
                this.selectedCustomGroups.splice(idx, 1);
            }

            // Is newly selected
            else {
                this.selectedCustomGroups.push(viewId);
            }
        };

        public refresh() {
            this.groupsService.getCustomGroups().then((listCustomGroups: CustomGroupItem[]) => {
                this.listCustomGroups = listCustomGroups;
                this.selectedCustomGroups = [];
            });
        }

        public addGroup() {
            if (!this.isAddingGroup) {
                this.isAddingGroup = true;
                this.groupsService.createCustomGroup({ where: { operator: LogicalOperators.And, rules: [], groups: [], depth: 0 }, name: "New custom group", count: 0, order: 0 }).then((customGroupId: string) => {
                    this.groupsService.getCustomGroups().then((listCustomGroups: CustomGroupItem[]) => {
                        this.listCustomGroups = listCustomGroups;
                        this.openAside(customGroupId, true);
                        this.isAddingGroup = false;
                    });
                })
            }
        }

        public deleteMultipleCustomGroups() {
            if (this.selectedCustomGroups.length > 0) {
                this.groupsService.deleteCustomGroups(this.selectedCustomGroups).then((response: any) => {
                    console.log("multiple custom groups deleted");
                    this.selectedCustomGroups = [];
                    this.refresh();
                })
            }
        }

        public sortableOptions = {
            orderChanged: (event : any) => {
                let listCustomGroupsIds: string[] = [];
                this.listCustomGroups.forEach((customGroup: CustomGroupItem) => {
                    listCustomGroupsIds.push(customGroup.id as string);
                });

                this.groupsService.reorderCustomGroups(listCustomGroupsIds).then((response: any) => {
                    this.selectedCustomGroups = [];
                    this.refresh();
                });
            }
        };

		//Popup to confirm the deletion of a devices
		public confirmDelete() {
			this.$uibModal.open({
				animation: true,
				ariaLabelledBy: 'modal-title',
				ariaDescribedBy: 'modal-body',
				backdrop: false,
                template: require('../templates/modals/confirmModal.html'),
				controller: 'ConfirmModalCtrl',
				controllerAs: 'vm',
				resolve: {
					title: function () { return "Delete Custom Groups?" }
				}
			}).result.then((result) => {
				if (result) {
                    this.deleteMultipleCustomGroups();
				}
			}, () => { });
		}
	}

    app.controller('GroupsCtrl', GroupsCtrl as any);
}