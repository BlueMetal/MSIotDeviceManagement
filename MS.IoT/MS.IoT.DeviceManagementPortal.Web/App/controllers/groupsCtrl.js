var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Controller that manages the group view.
    var GroupsCtrl = /** @class */ (function () {
        //Main Constructor
        function GroupsCtrl(listCustomGroups, listFields, $uibModal, $aside, $q, deviceDBService, groupsService) {
            var _this = this;
            this.isAddingGroup = false;
            this.listCustomGroups = [];
            this.selectedCustomGroups = [];
            this.sortableOptions = {
                orderChanged: function (event) {
                    var listCustomGroupsIds = [];
                    _this.listCustomGroups.forEach(function (customGroup) {
                        listCustomGroupsIds.push(customGroup.id);
                    });
                    _this.groupsService.reorderCustomGroups(listCustomGroupsIds).then(function (response) {
                        _this.selectedCustomGroups = [];
                        _this.refresh();
                    });
                }
            };
            this.$aside = $aside;
            this.$uibModal = $uibModal;
            this.$q = $q;
            this.groupsService = groupsService;
            this.deviceDBService = deviceDBService;
            this.listCustomGroups = listCustomGroups;
            this.listFields = listFields;
        }
        GroupsCtrl.prototype.openAside = function (customGroupId, editMode) {
            var _this = this;
            var resolveCustomGroup = ['GroupsService', function (groupsService) {
                    return groupsService.getCustomGroupById(customGroupId);
                }];
            var listFields = this.listFields;
            this.$aside.open({
                templateUrl: 'App/templates/side/sidePanelCustomGroup.html',
                placement: 'right',
                size: 'md',
                backdrop: false,
                controller: 'SidePanelCustomGroupCtrl',
                controllerAs: 'vm',
                resolve: {
                    customGroup: resolveCustomGroup,
                    customGroupId: function () { return customGroupId; },
                    listFields: function () { return listFields; },
                    editMode: function () { return editMode; }
                }
            }).result.then(function (result) {
                _this.refresh();
            }, function () { });
        };
        //Popup to confirm the deletion of a devices
        GroupsCtrl.prototype.openPublishFeatureModal = function () {
            var _this = this;
            this.getMergedDevicesIds().then(function (deviceIds) {
                _this.$uibModal.open({
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
                        selectedDevices: function () { return deviceIds; }
                    }
                }).result.then(function (result) {
                    if (result) {
                    }
                }, function () { });
            });
        };
        GroupsCtrl.prototype.openUpdateDesiredProperties = function () {
            var _this = this;
            this.getMergedDevicesIds().then(function (deviceIds) {
                _this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    templateUrl: 'App/templates/modals/updatePropertiesModal.html',
                    controller: 'UpdatePropertiesModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return "Update desired properties"; },
                        type: function () { return 'desired'; },
                        selectedDevices: function () { return deviceIds; }
                    }
                }).result.then(function (result) {
                    if (result) {
                    }
                }, function () { });
            });
        };
        GroupsCtrl.prototype.openUpdateTagsProperties = function () {
            var _this = this;
            this.getMergedDevicesIds().then(function (deviceIds) {
                _this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    templateUrl: 'App/templates/modals/updatePropertiesModal.html',
                    controller: 'UpdatePropertiesModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return "Update tags properties"; },
                        type: function () { return 'tags'; },
                        selectedDevices: function () { return deviceIds; }
                    }
                }).result.then(function (result) {
                    if (result) {
                    }
                }, function () { });
            });
        };
        GroupsCtrl.prototype.getMergedDevicesIds = function () {
            var _this = this;
            var deferred = this.$q.defer();
            var queryGroup = { operator: MSIoTDeviceManagementPortal.LogicalOperators.Or, groups: [], rules: [] };
            this.listCustomGroups.forEach(function (customGroup) {
                var idx = _this.selectedCustomGroups.indexOf(customGroup.id);
                if (idx > -1)
                    queryGroup.groups.push(customGroup.where);
            });
            this.deviceDBService.getDevicesTwinIds(queryGroup).then(function (deviceIds) {
                console.log(deviceIds.length + " device ids retrieved.");
                deferred.resolve(deviceIds);
            }, function (error) {
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsCtrl.prototype.toggleSelectionItem = function (viewId) {
            var idx = this.selectedCustomGroups.indexOf(viewId);
            // Is currently selected
            if (idx > -1) {
                this.selectedCustomGroups.splice(idx, 1);
            }
            else {
                this.selectedCustomGroups.push(viewId);
            }
        };
        ;
        GroupsCtrl.prototype.refresh = function () {
            var _this = this;
            this.groupsService.getCustomGroups().then(function (listCustomGroups) {
                _this.listCustomGroups = listCustomGroups;
                _this.selectedCustomGroups = [];
            });
        };
        GroupsCtrl.prototype.addGroup = function () {
            var _this = this;
            if (!this.isAddingGroup) {
                this.isAddingGroup = true;
                this.groupsService.createCustomGroup({ where: { operator: MSIoTDeviceManagementPortal.LogicalOperators.And, rules: [], groups: [], depth: 0 }, name: "New custom group", count: 0 }).then(function (customGroupId) {
                    _this.groupsService.getCustomGroups().then(function (listCustomGroups) {
                        _this.listCustomGroups = listCustomGroups;
                        _this.openAside(customGroupId, true);
                        _this.isAddingGroup = false;
                    });
                });
            }
        };
        GroupsCtrl.prototype.deleteMultipleCustomGroups = function () {
            var _this = this;
            if (this.selectedCustomGroups.length > 0) {
                this.groupsService.deleteCustomGroups(this.selectedCustomGroups).then(function (response) {
                    console.log("multiple custom groups deleted");
                    _this.selectedCustomGroups = [];
                    _this.refresh();
                });
            }
        };
        //Popup to confirm the deletion of a devices
        GroupsCtrl.prototype.confirmDelete = function () {
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
                    title: function () { return "Delete Custom Groups?"; }
                }
            }).result.then(function (result) {
                if (result) {
                    _this.deleteMultipleCustomGroups();
                }
            }, function () { });
        };
        GroupsCtrl.$inject = ['listCustomGroups', 'listFields', '$uibModal', '$aside', '$q', 'DeviceDBService', 'GroupsService'];
        return GroupsCtrl;
    }());
    app.controller('GroupsCtrl', GroupsCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=groupsCtrl.js.map