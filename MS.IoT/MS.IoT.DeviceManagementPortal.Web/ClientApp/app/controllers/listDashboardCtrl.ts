import * as angular from "angular";
import { OrderByType, LogicalOperators, ComparisonOperators } from "../models/enums";
import { IDeviceDBService, IGroupsService } from "../interfaces/interfaces";
import {
    DatatableConfig, DatatableState, DeviceTwinSummary, TopActivatedGroup,
    Alert, DeviceAlertCount, DatatableField, NodeState, DeviceGroup, DeviceQueryRuleGroup,
    DeviceQueryRule, DeviceQueryConfiguration, DeviceQueryResponse, DeviceInfoEntity
} from "../models/models";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

	//Class for the view Home
	class ListDashboardCtrl {
        static $inject: Array<string> = ['alerts', 'DeviceDBService', '$uibModal', '$aside', '$state'];

        private $uibModal: ng.ui.bootstrap.IModalService;
        private $aside: any;
        private $state: any;

        private deviceDBService: IDeviceDBService;
        private deviceTwinSummary?: DeviceTwinSummary = undefined;
        private dbConfig: DatatableConfig;
        private dbState: DatatableState;
        private topGroupActivatedConfig: TopActivatedGroup[]
        private currentTopGroupActivated: TopActivatedGroup;
        private searchBox: any = [];
        private alerts: Alert[];
        private isDeviceSummaryLoaded = false;

		//Main Constructor
        constructor(alerts: Alert[], deviceDBService: IDeviceDBService, $uibModal: ng.ui.bootstrap.IModalService,
            $aside: any, $state: any) {
            this.deviceDBService = deviceDBService;
			this.$aside = $aside;
			this.$uibModal = $uibModal;		
            this.$state = $state;
            this.alerts = alerts;

            //List fields
            let fieldProductFamily: DatatableField = { name: "productFamily", displayName: "Product Family", class: "col-md-2" };
            let fieldProductName: DatatableField = { name: "productName", displayName: "Product Name", class: "col-md-2" };
            let fieldRetailerName: DatatableField = { name: "retailerName", displayName: "Retailer", class: "col-md-1" };
            let fieldRetailerLocation: DatatableField = { name: "retailerRegion", displayName: "Retailer Location", class: "col-md-2" };
            let fieldLocation: DatatableField = { name: "installedLocation", displayName: "Installed Location", class: "col-md-2" };
            let fieldStatus: DatatableField = { name: "connectionState", displayName: "Status", class: "col-md-1", rendering: this.renderStatus };
            let fieldCode: DatatableField = { name: "statusCode", displayName: "Alert", class: "col-md-1 col-alert", rendering: this.renderAlert };

            //Datatable config
            this.dbConfig = {
                itemsPerPage: 10,
                paginationMaxSize: 10,
                searchFields: ["productFamily", "productName", "retailerName", "retailerRegion", "installedLocation", "deviceId"],
                tabs: [
                    { title: 'All', groupBy: undefined, fields: [fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: '', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Product Family', groupBy: 'productFamily', fields: [fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'productFamily', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Product Name', groupBy: 'productName', fields: [fieldProductName, fieldProductFamily, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'productName', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Retailer', groupBy: 'retailerName', fields: [fieldRetailerName, fieldProductFamily, fieldProductName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'retailerName', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Retailer Location', groupBy: 'retailerRegion', fields: [fieldRetailerLocation, fieldProductFamily, fieldProductName, fieldRetailerName, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'retailerRegion', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Installed Location', groupBy: 'installedLocation', fields: [fieldLocation, fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldStatus, fieldCode], orderByDefault: 'installedLocation', orderBySortingDefault: OrderByType.Ascending },
                    { title: 'Status', groupBy: 'connectionState', fields: [fieldStatus, fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldCode], orderByDefault: 'connectionState', orderBySortingDefault: OrderByType.Ascending, rendering: this.renderStatus },
                ]
            };

            //Initialize current State
            this.dbState = {
                isDatabaseLoaded: false,
                isDatabaseLoading: false,
                isDatabaseLocationUpdating: false,
                itemsPerPage: this.dbConfig.itemsPerPage,
                tab: this.dbConfig.tabs[0],
                orderBy: 'productFamily',
                orderBySorting: OrderByType.Ascending,
                searchTerms: [],
                root: this.createBlankNodeState()
            };

            //List Top Activated
            this.topGroupActivatedConfig = [
                { title: 'Product Family', groupBy: 'productFamily' },
                { title: 'Product Name', groupBy: 'productName' },
                { title: 'Retailer', groupBy: 'retailerName' },
                { title: 'Retailer Location', groupBy: 'retailerRegion' }
            ];
            this.currentTopGroupActivated = this.topGroupActivatedConfig[0];
            this.refreshComponentSummary();
		}

        private createBlankNodeState(): NodeState {
            return { isOpened: false, isLoaded: false, depth: 0, groups: [], items: [], selectedItems: [], pageIndex: 1, itemsCount: 0, pageItemsCount: 0, parent: undefined, filters: [] };
        }

        public changeTab(tabIndex: number) {
            this.dbState.tab = this.dbConfig.tabs[tabIndex];
            this.dbState.root = this.createBlankNodeState();
            this.dbState.root.isOpened = true;
            this.dbState.orderBy = this.dbState.tab.orderByDefault;
            this.dbState.orderBySorting = this.dbState.tab.orderBySortingDefault != null ? this.dbState.tab.orderBySortingDefault : OrderByType.Ascending;
            this.refreshDatatable(this.dbState.root);
        }

        public changeOrder(fieldId: string) {
            if (this.dbState.orderBySorting == OrderByType.Ascending)
                this.dbState.orderBySorting = OrderByType.Descending;
            else
                this.dbState.orderBySorting = OrderByType.Ascending;
            this.dbState.orderBy = fieldId;

            if (this.dbState.tab.groupBy != "" && this.dbState.tab.fields[0].name == fieldId)
                this.refreshDatatable(this.dbState.root);
            else
                this.refreshDatatableItems(this.dbState.root);
            
        }

        public search() {
            this.dbState.searchTerms.splice(0, this.dbState.searchTerms.length);
            this.searchBox.forEach((searchTerm: any) => {
                this.dbState.searchTerms.push(searchTerm.text);
            });
            this.dbState.root.selectedItems.splice(0, this.dbState.root.selectedItems.length);
            this.refreshDatatable(this.dbState.root);
        }

        public refreshDatatableItems(nodeState: NodeState) {
            if (nodeState.groups.length == 0) {
                this.refreshDatatable(nodeState);
            } else {
                nodeState.groups.forEach((group: DeviceGroup) => {
                    this.refreshDatatableItems(group.state);
                });
            }
        }

        public refreshDatatable(nodeState: NodeState) {
            nodeState.isLoaded = false;
            nodeState.groups.splice(0, nodeState.groups.length);
            nodeState.items.splice(0, nodeState.items.length);

            if (!nodeState.isOpened)
                return;

            //Creating where clause
            var whereClause: DeviceQueryRuleGroup = { operator: LogicalOperators.And, rules: [], groups: [] };
            //Adding groupBy filter
            if (nodeState.filters != null && nodeState.filters.length > 0) {
                nodeState.filters.forEach((filter: DeviceQueryRule) => {
                    whereClause.rules.push(filter);
                })
            }
            //Adding search filters
            if (this.dbState.searchTerms != null && this.dbState.searchTerms.length > 0) {
                var whereSearchClause: DeviceQueryRuleGroup = { operator: LogicalOperators.Or, rules: [], groups: [] };
                whereClause.groups.push(whereSearchClause);
                this.dbState.searchTerms.forEach((searchTerm: string) => {
                    this.dbConfig.searchFields.forEach((searchField: string) => {
                        whereSearchClause.rules.push({ field: searchField, operator: ComparisonOperators.Contains, value: searchTerm });
                    });
                })
            }

            //Building query object
            var query: DeviceQueryConfiguration = {
                itemsPerPage: this.dbState.itemsPerPage,
                groupBy: nodeState.parent == null ? this.dbState.tab.groupBy : '',
                orderBy: this.dbState.orderBy,
                orderBySorting: this.dbState.orderBySorting,
                where: whereClause,
                pageIndex: nodeState.pageIndex - 1
            }

            this.deviceDBService.getDevicesTwinInfo(query).then((response: DeviceQueryResponse) => {
                this.processDatabaseStatus(response, nodeState);
                if (response.groupsCount > 0) {
                    nodeState.groups = response.groups;
                    nodeState.groups.forEach((group: DeviceGroup) => {
                        group.displayName = this.dbState.tab.rendering == null ? group.name : this.dbState.tab.rendering(group);
                    })
                    nodeState.pageItemsCount = response.groupsCount;
                    nodeState.groups.forEach((group : DeviceGroup) => {
                        var isOpened = false;
                        group.state = this.createBlankNodeState();
                        group.state.parent = nodeState;
                        group.state.depth = this.getDepthLevel(group.state);
                        group.state.filters = [{ field: this.dbState.tab.groupBy, operator: ComparisonOperators.Equals, value: group.name }];
                        Object.defineProperty(group.state, "isOpened", {
                            get: () => { return isOpened; },
                            set: (newValue) => { isOpened = newValue; this.toggleGroup(group); }
                        });
                    });
                } else if (response.itemsCount > 0) {
                    nodeState.items = response.items;
                    nodeState.pageItemsCount = response.itemsCount;
                } else if(response.isDatabaseLoaded) {
                    nodeState.items = [];
                    nodeState.pageItemsCount = 0;
                }
                nodeState.itemsCount = response.itemsCount;
                nodeState.isLoaded = true;

                this.refreshComponentSummary();
            })
        }

        public changeTopGroupActivated(groupIndex: number) {
            this.currentTopGroupActivated = this.topGroupActivatedConfig[groupIndex];
            this.refreshComponentSummary();
        }

        public refreshComponentSummary() {
            if (!this.dbState.isDatabaseLoaded)
                return;

            this.deviceDBService.getDevicesTwinSummaryAggregations(this.currentTopGroupActivated.groupBy).then((response: DeviceTwinSummary) => {
                this.deviceTwinSummary = response;
                this.deviceTwinSummary.alertCounts.forEach((alertCount: DeviceAlertCount) => {
                    let alertSearch = this.alerts.filter(p => p.code == alertCount.alert);
                    if (alertSearch != null && alertSearch.length == 1)
                        alertCount.description = alertSearch[0].message;
                })
                this.isDeviceSummaryLoaded = true;
            })
        }

        private processDatabaseStatus(response: DeviceQueryResponse, nodeState: NodeState, filters: DeviceQueryRule[] = []) {
            this.dbState.isDatabaseLoaded = response.isDatabaseLoaded;
            this.dbState.isDatabaseLoading = response.isDatabaseLoading;
            this.dbState.isDatabaseLocationUpdating = response.isDatabaseLocationUpdating;

            if (!this.dbState.isDatabaseLoaded)
                setTimeout(() => { this.refreshDatatable(nodeState); }, 4000)
        }

        private getDepthLevel(state: NodeState) : number {
            var depth: number = 0;
            while (state.parent != null) {
                depth++;
                state = state.parent;
            }
            return depth;
        }

        private toggleGroup(group: DeviceGroup) {
            group.state.pageIndex = 1;
            if (group.state.isOpened) {
                this.refreshDatatable(group.state);
            } else
                group.state.items = [];
        }

        public toggleSelectionItem(deviceId : string) {
            let idx = this.dbState.root.selectedItems.indexOf(deviceId);

			// Is currently selected
			if (idx > -1) {
                this.dbState.root.selectedItems.splice(idx, 1);
                if (this.dbState.root.groups != null)
                    this.removeSelectionFromGroups(this.dbState.root.groups, deviceId);
			}

			// Is newly selected
			else {
                this.dbState.root.selectedItems.push(deviceId);
                if (this.dbState.root.groups != null)
                    this.addSelectionToGroups(this.dbState.root.groups, deviceId);
			}
        };

        public toggleSelectionGroup(group: DeviceGroup) {
            if (group.itemsIds.length == group.state.selectedItems.length) {
                //Uncheck
                group.itemsIds.forEach((deviceId: string) => {
                    let idx = this.dbState.root.selectedItems.indexOf(deviceId);
                    if (idx > -1) {
                        this.dbState.root.selectedItems.splice(idx, 1);
                        this.removeSelectionFromGroups(this.dbState.root.groups, deviceId);
                    }
                });
            } else {
                //Check
                group.itemsIds.forEach((deviceId: string) => {
                    let idx = this.dbState.root.selectedItems.indexOf(deviceId);
                    if (idx == -1) {
                        this.dbState.root.selectedItems.push(deviceId);
                        this.addSelectionToGroups(this.dbState.root.groups, deviceId);
                    }
                });
            }
        };

        private addSelectionToGroups(groups: DeviceGroup[], deviceId: string) {
            if (groups == null || groups.length == 0)
                return;

            groups.forEach((group: DeviceGroup) => {
                if (group.itemsIds != null) {
                    let idx = group.itemsIds.indexOf(deviceId);
                    if (idx > -1) {
                        idx = group.state.selectedItems.indexOf(deviceId);
                        if (idx == -1)
                            group.state.selectedItems.push(deviceId);
                    }
                }
                //Recursivity
                if (group.state.groups != null && group.state.groups.length > 0)
                    this.addSelectionToGroups(group.state.groups, deviceId);
            });
        }

        private removeSelectionFromGroups(groups: DeviceGroup[], deviceId: string) {
            if (groups == null || groups.length == 0)
                return;

            groups.forEach((group: DeviceGroup) => {
                if (group.itemsIds != null) {
                    var idx = group.itemsIds.indexOf(deviceId);
                    if (idx > -1) {
                        idx = group.state.selectedItems.indexOf(deviceId);
                        if (idx > -1)
                            group.state.selectedItems.splice(idx, 1);
                    }
                }
                //Recursivity
                if (group.state.groups != null && group.state.groups.length > 0)
                    this.removeSelectionFromGroups(group.state.groups, deviceId);
            });
        }

		public deleteMultipleDevices() {
            if (this.dbState.root.selectedItems.length > 0)
			{
                this.deviceDBService.deleteMultipleDevices(this.dbState.root.selectedItems).then((response: any) => {
					console.log("multiple devices deleted");
					this.$state.reload();
				})
			}
		}

        public pageChanged(state : NodeState) {
            this.refreshDatatable(state);
            console.log('Page changed to: ' + state.pageIndex);
        };

		public setItemsPerPage(num : number) {
			//this.dbState.itemsPerPage = num;
            //this.dbState.pageIndex = 1; //reset to first page
		}
		
        public openAside(deviceId: string, editMode: boolean) {
			var resolveDeviceDB = ['DeviceDBService', function (DeviceDBService: IDeviceDBService) {
				return DeviceDBService.getDeviceTwin(deviceId);
			}];
			this.$aside.open({
                template: require('../templates/side/sidePanel.html'),
				placement: 'right',
				size: 'md',
				backdrop: false,
				controller: 'SidePanelCtrl',
				controllerAs: 'vm',
				resolve: {
                    deviceTwin: resolveDeviceDB,
                    alerts: () => this.alerts,
					deviceId: function () { return deviceId },
					editMode: function () { return editMode }
				}
			}).result.then((result : any) => {
				if (result) {
					// do 
				}
			}, () => { });
		}

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
					title: function () { return "Delete Devices?" },
					content: function () { return "Do you really want to delete these devices? Press OK to delete." }
				}
			}).result.then((result) => {
				if (result) {
					this.deleteMultipleDevices();
				}
			}, () => { });
		}

		//Popup to confirm the deletion of a devices
		public openPublishFeatureModal() {
            let selectedDevicesList = this.dbState.root.selectedItems;
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
					selectedDevices: function () { return selectedDevicesList}
				}
			}).result.then((result) => {
				if (result) {
					
				}
			}, () => { });
        }

        private renderAlert(e: DeviceInfoEntity) {
            if (e.statusCode != null && e.statusCode != 0) {
                return "<span><img src=\"/images/icon-alert.svg\" /> " + e.statusCode + "</span>";
            }
            return "";
        }

        private renderStatus(e: any) {///DeviceInfoEntity|DeviceGroup)
            let status: number = 0;
            if (e.connectionState != null)
                status = e.connectionState;
            else if (e.name != null)
                status = parseInt(e.name);

            switch (status) {
                case 0:
                    return "Connected";
                case 1:
                    return "Disconnected";
                case 2:
                    return "Not Activated";
            }
        }

        /*private renderInstalledLocation(e: DeviceInfoEntity) {
            if (e.productCity == null && e.productRegion == null)
                return "";
            if (e.productCity == null)
                return e.productRegion;
            if (e.productRegion == null)
                return e.productCity;
            return e.productRegion + ", " + e.productCity;
        }*/
	}

    app.controller('ListDashboardCtrl', ListDashboardCtrl as any);
}