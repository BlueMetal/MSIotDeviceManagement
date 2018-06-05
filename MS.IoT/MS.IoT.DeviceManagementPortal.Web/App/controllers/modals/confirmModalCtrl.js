var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Modal to confirm delete devices/group
    var ConfirmModalCtrl = /** @class */ (function () {
        function ConfirmModalCtrl($uibModalInstance, title, content) {
            this.$uibModalInstance = $uibModalInstance;
            this.title = title;
            this.content = content;
        }
        ConfirmModalCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        ConfirmModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        ConfirmModalCtrl.$inject = ['$uibModalInstance', 'title', 'content'];
        return ConfirmModalCtrl;
    }());
    app.controller('ConfirmModalCtrl', ConfirmModalCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=confirmModalCtrl.js.map