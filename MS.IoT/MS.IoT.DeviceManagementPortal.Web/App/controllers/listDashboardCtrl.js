var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Class for the view Home
    var ListDashboardCtrl = /** @class */ (function () {
        //Main Constructor
        function ListDashboardCtrl(alerts, deviceDBService, $uibModal, $aside, $state) {
            this.searchBox = [];
            this.isDeviceSummaryLoaded = false;
            this.deviceDBService = deviceDBService;
            this.$aside = $aside;
            this.$uibModal = $uibModal;
            this.$state = $state;
            this.alerts = alerts;
            //List fields
            var fieldProductFamily = { name: "productFamily", displayName: "Product Family", class: "col-md-2" };
            var fieldProductName = { name: "productName", displayName: "Product Name", class: "col-md-2" };
            var fieldRetailerName = { name: "retailerName", displayName: "Retailer", class: "col-md-1" };
            var fieldRetailerLocation = { name: "retailerRegion", displayName: "Retailer Location", class: "col-md-2" };
            var fieldLocation = { name: "installedLocation", displayName: "Installed Location", class: "col-md-2" };
            var fieldStatus = { name: "connectionStatus", displayName: "Status", class: "col-md-1", rendering: this.renderStatus };
            var fieldCode = { name: "statusCode", displayName: "Alert", class: "col-md-1 col-alert", rendering: this.renderAlert };
            //Datatable config
            this.dbConfig = {
                itemsPerPage: 10,
                paginationMaxSize: 10,
                searchFields: ["productFamily", "productName", "retailerName", "retailerRegion", "installedLocation", "deviceId"],
                tabs: [
                    { title: 'All', groupBy: null, fields: [fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: '', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Product Family', groupBy: 'productFamily', fields: [fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'productFamily', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Product Name', groupBy: 'productName', fields: [fieldProductName, fieldProductFamily, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'productName', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Retailer', groupBy: 'retailerName', fields: [fieldRetailerName, fieldProductFamily, fieldProductName, fieldRetailerLocation, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'retailerName', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Retailer Location', groupBy: 'retailerRegion', fields: [fieldRetailerLocation, fieldProductFamily, fieldProductName, fieldRetailerName, fieldLocation, fieldStatus, fieldCode], orderByDefault: 'retailerRegion', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Installed Location', groupBy: 'installedLocation', fields: [fieldLocation, fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldStatus, fieldCode], orderByDefault: 'installedLocation', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending },
                    { title: 'Status', groupBy: 'connectionStatus', fields: [fieldStatus, fieldProductFamily, fieldProductName, fieldRetailerName, fieldRetailerLocation, fieldLocation, fieldCode], orderByDefault: 'connectionStatus', orderBySortingDefault: MSIoTDeviceManagementPortal.OrderByType.Ascending, rendering: this.renderStatus },
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
                orderBySorting: MSIoTDeviceManagementPortal.OrderByType.Ascending,
                searchTerms: [],
                root: null
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
        ListDashboardCtrl.prototype.createBlankNodeState = function () {
            return { isOpened: false, isLoaded: false, depth: 0, groups: [], items: [], selectedItems: [], pageIndex: 1, itemsCount: 0, pageItemsCount: 0, parent: null, filters: [] };
        };
        ListDashboardCtrl.prototype.changeTab = function (tabIndex) {
            this.dbState.tab = this.dbConfig.tabs[tabIndex];
            this.dbState.root = this.createBlankNodeState();
            this.dbState.root.isOpened = true;
            this.dbState.orderBy = this.dbState.tab.orderByDefault;
            this.dbState.orderBySorting = this.dbState.tab.orderBySortingDefault != null ? this.dbState.tab.orderBySortingDefault : MSIoTDeviceManagementPortal.OrderByType.Ascending;
            this.refreshDatatable(this.dbState.root);
        };
        ListDashboardCtrl.prototype.changeOrder = function (fieldId) {
            if (this.dbState.orderBySorting == MSIoTDeviceManagementPortal.OrderByType.Ascending)
                this.dbState.orderBySorting = MSIoTDeviceManagementPortal.OrderByType.Descending;
            else
                this.dbState.orderBySorting = MSIoTDeviceManagementPortal.OrderByType.Ascending;
            this.dbState.orderBy = fieldId;
            if (this.dbState.tab.groupBy != "" && this.dbState.tab.fields[0].name == fieldId)
                this.refreshDatatable(this.dbState.root);
            else
                this.refreshDatatableItems(this.dbState.root);
        };
        ListDashboardCtrl.prototype.search = function () {
            var _this = this;
            this.dbState.searchTerms.splice(0, this.dbState.searchTerms.length);
            this.searchBox.forEach(function (searchTerm) {
                _this.dbState.searchTerms.push(searchTerm.text);
            });
            this.dbState.root.selectedItems.splice(0, this.dbState.root.selectedItems.length);
            this.refreshDatatable(this.dbState.root);
        };
        ListDashboardCtrl.prototype.refreshDatatableItems = function (nodeState) {
            var _this = this;
            if (nodeState.groups.length == 0) {
                this.refreshDatatable(nodeState);
            }
            else {
                nodeState.groups.forEach(function (group) {
                    _this.refreshDatatableItems(group.state);
                });
            }
        };
        ListDashboardCtrl.prototype.refreshDatatable = function (nodeState) {
            var _this = this;
            nodeState.isLoaded = false;
            nodeState.groups.splice(0, nodeState.groups.length);
            nodeState.items.splice(0, nodeState.items.length);
            if (!nodeState.isOpened)
                return;
            //Creating where clause
            var whereClause = { operator: MSIoTDeviceManagementPortal.LogicalOperators.And, rules: [], groups: [] };
            //Adding groupBy filter
            if (nodeState.filters != null && nodeState.filters.length > 0) {
                nodeState.filters.forEach(function (filter) {
                    whereClause.rules.push(filter);
                });
            }
            //Adding search filters
            if (this.dbState.searchTerms != null && this.dbState.searchTerms.length > 0) {
                var whereSearchClause = { operator: MSIoTDeviceManagementPortal.LogicalOperators.Or, rules: [], groups: [] };
                whereClause.groups.push(whereSearchClause);
                this.dbState.searchTerms.forEach(function (searchTerm) {
                    _this.dbConfig.searchFields.forEach(function (searchField) {
                        whereSearchClause.rules.push({ field: searchField, operator: MSIoTDeviceManagementPortal.ComparisonOperators.Contains, value: searchTerm });
                    });
                });
            }
            //Building query object
            var query = {
                itemsPerPage: this.dbState.itemsPerPage,
                groupBy: nodeState.parent == null ? this.dbState.tab.groupBy : '',
                orderBy: this.dbState.orderBy,
                orderBySorting: this.dbState.orderBySorting,
                where: whereClause,
                pageIndex: nodeState.pageIndex - 1
            };
            this.deviceDBService.getDevicesTwinInfo(query).then(function (response) {
                _this.processDatabaseStatus(response, nodeState);
                if (response.groupsCount > 0) {
                    nodeState.groups = response.groups;
                    nodeState.groups.forEach(function (group) {
                        group.displayName = _this.dbState.tab.rendering == null ? group.name : _this.dbState.tab.rendering(group);
                    });
                    nodeState.pageItemsCount = response.groupsCount;
                    nodeState.groups.forEach(function (group) {
                        var isOpened = false;
                        group.state = _this.createBlankNodeState();
                        group.state.parent = nodeState;
                        group.state.depth = _this.getDepthLevel(group.state);
                        group.state.filters = [{ field: _this.dbState.tab.groupBy, operator: MSIoTDeviceManagementPortal.ComparisonOperators.Equals, value: group.name }];
                        Object.defineProperty(group.state, "isOpened", {
                            get: function () { return isOpened; },
                            set: function (newValue) { isOpened = newValue; _this.toggleGroup(group); }
                        });
                    });
                }
                else if (response.itemsCount > 0) {
                    nodeState.items = response.items;
                    nodeState.pageItemsCount = response.itemsCount;
                }
                else if (response.isDatabaseLoaded) {
                    nodeState.items = [];
                    nodeState.pageItemsCount = 0;
                }
                nodeState.itemsCount = response.itemsCount;
                nodeState.isLoaded = true;
                _this.refreshComponentSummary();
            });
        };
        ListDashboardCtrl.prototype.changeTopGroupActivated = function (groupIndex) {
            this.currentTopGroupActivated = this.topGroupActivatedConfig[groupIndex];
            this.refreshComponentSummary();
        };
        ListDashboardCtrl.prototype.refreshComponentSummary = function () {
            var _this = this;
            if (!this.dbState.isDatabaseLoaded)
                return;
            this.deviceDBService.getDevicesTwinSummaryAggregations(this.currentTopGroupActivated.groupBy).then(function (response) {
                _this.deviceTwinSummary = response;
                _this.deviceTwinSummary.alertCounts.forEach(function (alertCount) {
                    var alertSearch = _this.alerts.filter(function (p) { return p.code == alertCount.alert; });
                    if (alertSearch != null && alertSearch.length == 1)
                        alertCount.description = alertSearch[0].message;
                });
                _this.isDeviceSummaryLoaded = true;
            });
        };
        ListDashboardCtrl.prototype.processDatabaseStatus = function (response, nodeState, filters) {
            var _this = this;
            if (filters === void 0) { filters = []; }
            this.dbState.isDatabaseLoaded = response.isDatabaseLoaded;
            this.dbState.isDatabaseLoading = response.isDatabaseLoading;
            this.dbState.isDatabaseLocationUpdating = response.isDatabaseLocationUpdating;
            if (!this.dbState.isDatabaseLoaded)
                setTimeout(function () { _this.refreshDatatable(nodeState); }, 4000);
        };
        ListDashboardCtrl.prototype.getDepthLevel = function (state) {
            var depth = 0;
            while (state.parent != null) {
                depth++;
                state = state.parent;
            }
            return depth;
        };
        ListDashboardCtrl.prototype.toggleGroup = function (group) {
            group.state.pageIndex = 1;
            if (group.state.isOpened) {
                this.refreshDatatable(group.state);
            }
            else
                group.state.items = [];
        };
        ListDashboardCtrl.prototype.toggleSelectionItem = function (deviceId) {
            var idx = this.dbState.root.selectedItems.indexOf(deviceId);
            // Is currently selected
            if (idx > -1) {
                this.dbState.root.selectedItems.splice(idx, 1);
                if (this.dbState.root.groups != null)
                    this.removeSelectionFromGroups(this.dbState.root.groups, deviceId);
            }
            else {
                this.dbState.root.selectedItems.push(deviceId);
                if (this.dbState.root.groups != null)
                    this.addSelectionToGroups(this.dbState.root.groups, deviceId);
            }
        };
        ;
        ListDashboardCtrl.prototype.toggleSelectionGroup = function (group) {
            var _this = this;
            if (group.itemsIds.length == group.state.selectedItems.length) {
                //Uncheck
                group.itemsIds.forEach(function (deviceId) {
                    var idx = _this.dbState.root.selectedItems.indexOf(deviceId);
                    if (idx > -1) {
                        _this.dbState.root.selectedItems.splice(idx, 1);
                        _this.removeSelectionFromGroups(_this.dbState.root.groups, deviceId);
                    }
                });
            }
            else {
                //Check
                group.itemsIds.forEach(function (deviceId) {
                    var idx = _this.dbState.root.selectedItems.indexOf(deviceId);
                    if (idx == -1) {
                        _this.dbState.root.selectedItems.push(deviceId);
                        _this.addSelectionToGroups(_this.dbState.root.groups, deviceId);
                    }
                });
            }
        };
        ;
        ListDashboardCtrl.prototype.addSelectionToGroups = function (groups, deviceId) {
            var _this = this;
            if (groups == null || groups.length == 0)
                return;
            groups.forEach(function (group) {
                if (group.itemsIds != null) {
                    var idx = group.itemsIds.indexOf(deviceId);
                    if (idx > -1) {
                        idx = group.state.selectedItems.indexOf(deviceId);
                        if (idx == -1)
                            group.state.selectedItems.push(deviceId);
                    }
                }
                //Recursivity
                if (group.state.groups != null && group.state.groups.length > 0)
                    _this.addSelectionToGroups(group.state.groups, deviceId);
            });
        };
        ListDashboardCtrl.prototype.removeSelectionFromGroups = function (groups, deviceId) {
            var _this = this;
            if (groups == null || groups.length == 0)
                return;
            groups.forEach(function (group) {
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
                    _this.removeSelectionFromGroups(group.state.groups, deviceId);
            });
        };
        ListDashboardCtrl.prototype.deleteMultipleDevices = function () {
            var _this = this;
            if (this.dbState.root.selectedItems.length > 0) {
                this.deviceDBService.deleteMultipleDevices(this.dbState.root.selectedItems).then(function (response) {
                    console.log("multiple devices deleted");
                    _this.$state.reload();
                });
            }
        };
        ListDashboardCtrl.prototype.pageChanged = function (state) {
            this.refreshDatatable(state);
            console.log('Page changed to: ' + state.pageIndex);
        };
        ;
        ListDashboardCtrl.prototype.setItemsPerPage = function (num) {
            //this.dbState.itemsPerPage = num;
            //this.dbState.pageIndex = 1; //reset to first page
        };
        ListDashboardCtrl.prototype.openAside = function (deviceId, editMode) {
            var _this = this;
            var resolveDeviceDB = ['DeviceDBService', function (DeviceDBService) {
                    return DeviceDBService.getDeviceTwin(deviceId);
                }];
            this.$aside.open({
                templateUrl: 'App/templates/side/sidePanel.html',
                placement: 'right',
                size: 'md',
                backdrop: false,
                controller: 'SidePanelCtrl',
                controllerAs: 'vm',
                resolve: {
                    deviceTwin: resolveDeviceDB,
                    alerts: function () { return _this.alerts; },
                    deviceId: function () { return deviceId; },
                    editMode: function () { return editMode; }
                }
            }).result.then(function (result) {
                if (result) {
                    // do 
                }
            }, function () { });
        };
        //Popup to confirm the deletion of a devices
        ListDashboardCtrl.prototype.confirmDelete = function () {
            var _this = this;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                backdrop: false,
                templateUrl: 'App/templates/modals/confirmModal.html',
                controller: 'ConfirmModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Delete Devices?"; },
                    content: function () { return "Do you really want to delete these devices? Press OK to delete."; }
                }
            }).result.then(function (result) {
                if (result) {
                    _this.deleteMultipleDevices();
                }
            }, function () { });
        };
        //Popup to confirm the deletion of a devices
        ListDashboardCtrl.prototype.openPublishFeatureModal = function () {
            var selectedDevicesList = this.dbState.root.selectedItems;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                backdrop: false,
                templateUrl: 'App/templates/modals/publishFeatureModal.html',
                controller: 'PublishFeatureModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Publish a new Feature"; },
                    content: function () { return "Select a feature and publish it to multiple devices"; },
                    selectedDevices: function () { return selectedDevicesList; }
                }
            }).result.then(function (result) {
                if (result) {
                }
            }, function () { });
        };
        ListDashboardCtrl.prototype.renderAlert = function (e) {
            if (e.statusCode != null && e.statusCode != 0) {
                return "<span><img src=\"/Content/images/icon-alert.svg\" /> " + e.statusCode + "</span>";
            }
            return "";
        };
        ListDashboardCtrl.prototype.renderStatus = function (e) {
            var status = 0;
            if (e.connectionStatus != null)
                status = e.connectionStatus;
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
        };
        ListDashboardCtrl.$inject = ['alerts', 'DeviceDBService', '$uibModal', '$aside', '$state'];
        return ListDashboardCtrl;
    }());
    app.controller('ListDashboardCtrl', ListDashboardCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=listDashboardCtrl.js.map