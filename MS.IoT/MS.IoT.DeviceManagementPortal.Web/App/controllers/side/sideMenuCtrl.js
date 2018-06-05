var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Main Controller (abstract) used for main notifications
    var SideMenuCtrl = /** @class */ (function () {
        function SideMenuCtrl($scope) {
            var _this = this;
            $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                _this.isMapActivated = toState.name == 'app.map';
                _this.isListActivated = toState.name == 'app.list';
                _this.isGroupsActivated = toState.name == 'app.groups';
            });
        }
        ;
        SideMenuCtrl.$inject = ["$scope"];
        return SideMenuCtrl;
    }());
    app.controller('SideMenuCtrl', SideMenuCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=sideMenuCtrl.js.map