var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal", ['ui.router', 'ui.bootstrap', 'ngAside', 'ngFileUpload', 'ngTagsInput', 'ui.select', 'ngSanitize', 'as.sortable']);
    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('app.listdashboard');
            $locationProvider.html5Mode(true);
            //Resolve calls
            var resolveCurrentUser = ['UserService', function (userService) {
                    return userService.getCurrentUser();
                }];
            var resolveAlerts = ['AlertService', function (alertService) {
                    return alertService.getAlerts();
                }];
            var resolveCustomGroups = ['GroupsService', function (groupsService) {
                    return groupsService.getCustomGroups();
                }];
            var resolveDeviceFields = ['DeviceDBService', function (deviceDBService) {
                    return deviceDBService.getDeviceFields();
                }];
            //States
            var stateMainAbstract = {
                name: 'app',
                abstract: true,
                url: '',
                templateUrl: '/App/views/Main.html',
                controller: "MainCtrl",
                controllerAs: "rc",
                resolve: {
                    currentUser: resolveCurrentUser
                }
            };
            var stateListDashboard = {
                name: 'app.list',
                url: '/',
                templateUrl: '/App/views/ListDashboard.html',
                controller: "ListDashboardCtrl",
                controllerAs: "vm",
                resolve: {
                    alerts: resolveAlerts
                }
            };
            var stateMapDashboard = {
                name: 'app.map',
                url: '/map',
                templateUrl: '/App/views/MapDashboard.html',
                controller: "MapDashboardCtrl",
                controllerAs: "vm"
            };
            var stateGroupsDashboard = {
                name: 'app.groups',
                url: '/groups',
                templateUrl: '/App/views/Groups.html',
                controller: "GroupsCtrl",
                controllerAs: "vm",
                resolve: {
                    listCustomGroups: resolveCustomGroups,
                    listFields: resolveDeviceFields
                }
            };
            $stateProvider
                .state(stateMainAbstract)
                .state(stateListDashboard)
                .state(stateMapDashboard)
                .state(stateGroupsDashboard);
        }]);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=app.js.map