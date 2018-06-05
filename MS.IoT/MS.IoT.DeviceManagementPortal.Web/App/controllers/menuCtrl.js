var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Menu controller used to collapse the menu in phone view
    var MenuCtrl = /** @class */ (function () {
        function MenuCtrl($uibModal, $state) {
            this.isNavCollapsed = true;
            this.$uibModal = $uibModal;
            this.$state = $state;
            this.active = "menuactive";
        }
        ;
        MenuCtrl.prototype.importPanel = function () {
            var _this = this;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/modals/importModal.html',
                controller: 'ImportModalCtrl',
                controllerAs: 'vm',
                resolve: {}
            }).result.then(function (areDevicesImported) {
                console.log("modal left");
                if (areDevicesImported) {
                    _this.$state.reload();
                }
            }, function () {
                console.log("modal left another");
            });
        };
        MenuCtrl.$inject = ["$uibModal", "$state"];
        return MenuCtrl;
    }());
    app.controller('MenuCtrl', MenuCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=menuCtrl.js.map