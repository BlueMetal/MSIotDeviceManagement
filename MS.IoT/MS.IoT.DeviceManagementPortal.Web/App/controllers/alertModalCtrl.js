var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Alert Modal Controller, used to control the content of the data in the alert Modal
    var AlertModalCtrl = (function () {
        function AlertModalCtrl($uibModalInstance, title, content) {
            this.$uibModalInstance = $uibModalInstance;
            this.title = title;
            this.content = content;
        }
        AlertModalCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        AlertModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        return AlertModalCtrl;
    }());
    AlertModalCtrl.$inject = ['$uibModalInstance', 'title', 'content'];
    app.controller('AlertModalCtrl', AlertModalCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=alertModalCtrl.js.map