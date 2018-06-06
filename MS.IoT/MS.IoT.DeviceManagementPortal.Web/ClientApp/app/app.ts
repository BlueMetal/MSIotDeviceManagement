import * as angular from "angular";
import "angular-ui-bootstrap";
import { StateProvider, Ng1StateDeclaration, UrlRouterProvider } from "@uirouter/angularjs";
import { IUserService, IAlertService, IGroupsService, IDeviceDBService } from "./interfaces/interfaces";

module MSIoTDeviceManagementPortal {
    var app = angular.module("MSIoTDeviceManagementPortal", ['ui.router', 'ui.bootstrap', 'ngAside', 'ngFileUpload', 'ngTagsInput', 'ui.select', 'ngSanitize', 'as.sortable']);

    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider: StateProvider, $httpProvider: ng.IHttpProvider, $locationProvider: ng.ILocationProvider, $urlRouterProvider: UrlRouterProvider) {
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
            var stateMainAbstract: Ng1StateDeclaration = {
                name: 'app',
                abstract: true,
                url: '',
                template: require('./views/Main.html'),
                controller: "MainCtrl",
                controllerAs: "rc",
                resolve: {
                    currentUser: resolveCurrentUser
                }
            }

            var stateListDashboard: Ng1StateDeclaration = {
                name: 'app.list',
                url: '/',
                template: require('./views/ListDashboard.html'),
                controller: "ListDashboardCtrl",
                controllerAs: "vm",
                resolve: {
                    alerts: resolveAlerts
                }
            }

            var stateMapDashboard: Ng1StateDeclaration = {
                name: 'app.map',
                url: '/map',
                template: require('./views/MapDashboard.html'),
                controller: "MapDashboardCtrl",
                controllerAs: "vm"
            }

            var stateGroupsDashboard: Ng1StateDeclaration = {
                name: 'app.groups',
                url: '/groups',
                template: require('./views/Groups.html'),
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