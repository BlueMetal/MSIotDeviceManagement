module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal");

    //Main Controller (abstract) used for main notifications
    class SideMenuCtrl {
        static $inject: Array<string> = ["$scope"];
        public isListActivated;
        public isMapActivated;
        public isGroupsActivated;

        constructor($scope: ng.IScope) {

            $scope.$on('$stateChangeSuccess',
                (event, toState, toParams, fromState, fromParams) => {
                    this.isMapActivated = toState.name == 'app.map';
                    this.isListActivated = toState.name == 'app.list';
                    this.isGroupsActivated = toState.name == 'app.groups';
                });
        };
    }

    app.controller('SideMenuCtrl', SideMenuCtrl as any);
}
