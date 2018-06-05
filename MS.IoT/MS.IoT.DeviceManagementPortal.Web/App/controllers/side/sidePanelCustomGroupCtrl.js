var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //side panel controller to display device Twin details
    var SidePanelCustomGroupCtrl = /** @class */ (function () {
        function SidePanelCustomGroupCtrl($uibModalInstance, customGroup, listFields, customGroupId, editMode, deviceDBService, groupsService, $scope, $uibModal) {
            this.listGroupDevices = [];
            this.listGroupDevicesIndex = 1;
            this.listGroupDevicesItemCount = 0;
            this.isLoadingDevices = false;
            this.isFormDirty = false;
            this.hasFormErrors = false;
            this.orderByTypes = [{ id: 0, name: "Ascending" }, { id: 1, name: "Descending" }];
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
        SidePanelCustomGroupCtrl.prototype.refreshDatatable = function () {
            var _this = this;
            if (this.isLoadingDevices)
                return;
            this.isLoadingDevices = true;
            this.listGroupDevices.splice(0, this.listGroupDevices.length);
            //Building query object
            var query = {
                itemsPerPage: 10,
                where: this.customGroup.where,
                pageIndex: this.listGroupDevicesIndex - 1
            };
            this.deviceDBService.getDevicesTwinInfo(query).then(function (response) {
                _this.listGroupDevices = response.items;
                _this.listGroupDevicesItemCount = response.itemsCount;
                _this.isLoadingDevices = false;
            });
        };
        SidePanelCustomGroupCtrl.prototype.saveCustomGroup = function (formSideCustomGroupPanel) {
            var _this = this;
            this.hasFormErrors = formSideCustomGroupPanel.$invalid;
            if (!this.hasFormErrors) {
                this.groupsService.updateCustomGroup(this.customGroup).then(function (response) {
                    _this.editMode = false;
                    _this.isFormDirty = false;
                    formSideCustomGroupPanel.$setPristine();
                    _this.listGroupDevicesIndex = 1;
                    _this.refreshDatatable();
                });
            }
        };
        SidePanelCustomGroupCtrl.prototype.pageChanged = function () {
            this.refreshDatatable();
            console.log('Page changed to: ' + this.listGroupDevicesIndex);
        };
        ;
        SidePanelCustomGroupCtrl.prototype.editCancel = function (formSideCustomGroupPanel) {
            if (formSideCustomGroupPanel.$dirty) {
                this.isFormDirty = true;
            }
            else {
                this.hasFormErrors = false;
                this.editMode = false;
                this.refreshCustomGroup();
            }
        };
        SidePanelCustomGroupCtrl.prototype.noSaveCancel = function (formSideCustomGroupPanel) {
            this.refreshCustomGroup();
            formSideCustomGroupPanel.$setPristine();
            this.isFormDirty = false;
            this.editMode = false;
        };
        SidePanelCustomGroupCtrl.prototype.refreshCustomGroup = function () {
            var _this = this;
            this.groupsService.getCustomGroupById(this.customGroupId).then(function (response) {
                _this.customGroup = response;
            });
        };
        SidePanelCustomGroupCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        SidePanelCustomGroupCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        SidePanelCustomGroupCtrl.$inject = ['$uibModalInstance', 'customGroup', 'listFields', 'customGroupId', 'editMode', 'DeviceDBService', 'GroupsService', '$scope', '$uibModal'];
        return SidePanelCustomGroupCtrl;
    }());
    app.controller('SidePanelCustomGroupCtrl', SidePanelCustomGroupCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=sidePanelCustomGroupCtrl.js.map