module MSIoTDeviceManagementPortal {
    var app = angular.module("MSIoTDeviceManagementPortal", ['ui.router', 'ui.bootstrap', 'ngAside', 'ngFileUpload', 'ngTagsInput', 'ui.select', 'ngSanitize', 'as.sortable']);

    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
		function ($stateProvider: ng.ui.IStateProvider, $httpProvider: ng.IHttpProvider, $locationProvider: ng.ILocationProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) {
            $urlRouterProvider.otherwise('app.listdashboard');
            $locationProvider.html5Mode(true);

            //Resolve calls
            var resolveCurrentUser = ['UserService', function (userService: IUserService) {
                return userService.getCurrentUser();
            }];

            var resolveAlerts = ['AlertService', function (alertService: IAlertService) {
                return alertService.getAlerts();
            }];

            var resolveCustomGroups = ['GroupsService', function (groupsService: IGroupsService) {
                return groupsService.getCustomGroups();
            }];

            var resolveDeviceFields = ['DeviceDBService', function (deviceDBService: IDeviceDBService) {
                return deviceDBService.getDeviceFields();
            }];

            //States
            var stateMainAbstract: ng.ui.IState = {
                name: 'app',
                abstract: true,
                url: '',
                templateUrl: '/App/views/Main.html',
                controller: "MainCtrl",
                controllerAs: "rc",
                resolve: {
                    currentUser: resolveCurrentUser
                }
            }

            var stateListDashboard: ng.ui.IState = {
                name: 'app.list',
                url: '/',
                templateUrl: '/App/views/ListDashboard.html',
                controller: "ListDashboardCtrl",
                controllerAs: "vm",
                resolve: {
                    alerts: resolveAlerts
                }
            }

            var stateMapDashboard: ng.ui.IState = {
                name: 'app.map',
                url: '/map',
                templateUrl: '/App/views/MapDashboard.html',
                controller: "MapDashboardCtrl",
                controllerAs: "vm"
            }

            var stateGroupsDashboard: ng.ui.IState = {
                name: 'app.groups',
                url: '/groups',
                templateUrl: '/App/views/Groups.html',
                controller: "GroupsCtrl",
                controllerAs: "vm",
                resolve: {
                    listCustomGroups: resolveCustomGroups,
                    listFields: resolveDeviceFields
                }
            }

            $stateProvider
                .state(stateMainAbstract)
                .state(stateListDashboard)
                .state(stateMapDashboard)
                .state(stateGroupsDashboard)
        }]
    );
}