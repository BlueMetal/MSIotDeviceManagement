var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Class for the view Home
    var ListDashboardCtrl = (function () {
        //Main Constructor
        function ListDashboardCtrl(currentUser) {
            this.currentUser = currentUser;
        }
        return ListDashboardCtrl;
    }());
    ListDashboardCtrl.$inject = ['currentUser'];
    app.controller('ListDashboardCtrl', ListDashboardCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=homeCtrl.js.map