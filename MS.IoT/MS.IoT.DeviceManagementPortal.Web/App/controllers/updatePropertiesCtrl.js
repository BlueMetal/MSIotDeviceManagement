var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //modal to confirm delete devices
    var UpdatePropertiesCtrl = (function () {
        function UpdatePropertiesCtrl($uibModalInstance, $uibModal, title, type, selectedDevices, deviceDBService) {
            this.$uibModalInstance = $uibModalInstance;
            this.$uibModal = $uibModal;
            this.deviceDBService = deviceDBService;
            this.title = title;
            this.type = type;
            this.selectedDevices = selectedDevices;
            if (type == 'desired') {
                this.properties = [
                    { name: "statusCode", type: MSIoTDeviceManagementPortal.FieldTypes.Number },
                    { name: "firmwareVersion", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "features", type: MSIoTDeviceManagementPortal.FieldTypes.Object, children: [], allowedChildrenTypes: [MSIoTDeviceManagementPortal.FieldTypes.Boolean] },
                    { name: "deviceState", type: MSIoTDeviceManagementPortal.FieldTypes.Object, children: [], allowedChildrenTypes: [MSIoTDeviceManagementPortal.FieldTypes.Boolean, MSIoTDeviceManagementPortal.FieldTypes.Number, MSIoTDeviceManagementPortal.FieldTypes.Double, MSIoTDeviceManagementPortal.FieldTypes.String, MSIoTDeviceManagementPortal.FieldTypes.Date] }
                ];
            }
            else if (type == 'tags') {
                this.properties = [
                    { name: "productName", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "productType", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "productFamily", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "retailerName", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "retailerRegion", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "userId", type: MSIoTDeviceManagementPortal.FieldTypes.String },
                    { name: "manufacturedDate", type: MSIoTDeviceManagementPortal.FieldTypes.Date },
                    { name: "shippedDate", type: MSIoTDeviceManagementPortal.FieldTypes.Date },
                    { name: "customTags", type: MSIoTDeviceManagementPortal.FieldTypes.Object, children: [], allowedChildrenTypes: [MSIoTDeviceManagementPortal.FieldTypes.Boolean, MSIoTDeviceManagementPortal.FieldTypes.Number, MSIoTDeviceManagementPortal.FieldTypes.Double, MSIoTDeviceManagementPortal.FieldTypes.String, MSIoTDeviceManagementPortal.FieldTypes.Date] }
                ];
            }
        }
        UpdatePropertiesCtrl.prototype.buildPatch = function (updateProperties) {
            var _this = this;
            var patch = {};
            updateProperties.forEach(function (updateProperty) {
                if (updateProperty.isNull) {
                    patch[updateProperty.name] = null;
                }
                else if (updateProperty.value != null) {
                    patch[updateProperty.name] = updateProperty.value;
                }
                else if (updateProperty.children != null && updateProperty.children.length > 0) {
                    patch[updateProperty.name] = _this.buildPatch(updateProperty.children);
                }
            });
            return patch;
        };
        UpdatePropertiesCtrl.prototype.confirmUpdate = function () {
            var _this = this;
            var nbrDevices = this.selectedDevices.length;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                backdrop: false,
                templateUrl: 'App/templates/confirmModal.html',
                controller: 'ConfirmModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Update " + nbrDevices + " devices?"; },
                    content: function () { return "Do you really want to update these devices? It can take a few minutes. Press OK to update."; }
                }
            }).result.then(function (result) {
                if (result) {
                    _this.update();
                }
            }, function () { });
        };
        UpdatePropertiesCtrl.prototype.update = function () {
            var _this = this;
            var patchDesired = this.type == 'desired' ? this.buildPatch(this.properties) : null;
            var patchTags = this.type == 'tags' ? this.buildPatch(this.properties) : null;
            this.isUpdateClicked = true;
            this.deviceDBService.updateDevicesAsync(this.selectedDevices, patchDesired, patchTags).then(function (response) {
                var title = response.hasSucceeded ? "Properties updated!" : "Error while updating the properties";
                var description = response.hasSucceeded ?
                    "The properties are being updated. It might take a few minutes." :
                    "There was an error while updating the properties: " + response.errorReason + "<br />JobId: " + response.jobId;
                _this.$uibModal.open({
                    animation: true,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    backdrop: false,
                    templateUrl: 'App/templates/alertModal.html',
                    controller: 'AlertModalCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        title: function () { return title; },
                        content: function () { return description; },
                    }
                }).result.then(function () {
                    _this.$uibModalInstance.close(false);
                }, function () {
                });
            }).catch(function (error) {
                _this.isError = true;
                _this.errorMessage = "There was an error while updating the properties.";
            });
        };
        UpdatePropertiesCtrl.prototype.addSubProperty = function (desiredField) {
            desiredField.children.push({});
        };
        UpdatePropertiesCtrl.prototype.removeProperty = function (desiredField, subProperty) {
            var entityIndex = desiredField.children.indexOf(subProperty);
            if (entityIndex !== -1) {
                desiredField.children.splice(entityIndex, 1);
            }
        };
        UpdatePropertiesCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        UpdatePropertiesCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        return UpdatePropertiesCtrl;
    }());
    UpdatePropertiesCtrl.$inject = ['$uibModalInstance', '$uibModal', 'title', 'type', 'selectedDevices', 'DeviceDBService'];
    app.controller('UpdatePropertiesCtrl', UpdatePropertiesCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=updatePropertiesCtrl.js.map