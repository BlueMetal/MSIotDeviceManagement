var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //JSON Modal Controller, used to control the display of the JSON data in the JSON Modal
    var JsonModalCtrl = (function () {
        function JsonModalCtrl($uibModalInstance, device) {
            this.$uibModalInstance = $uibModalInstance;
            this.device = device;
        }
        JsonModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close();
        };
        return JsonModalCtrl;
    }());
    JsonModalCtrl.$inject = ['$uibModalInstance', 'device'];
    app.controller('JsonModalCtrl', JsonModalCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=jsonModalCtrl.js.map