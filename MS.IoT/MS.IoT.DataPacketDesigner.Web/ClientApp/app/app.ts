import * as angular from "angular";
import "angular-ui-bootstrap";
import { StateProvider, Ng1StateDeclaration, UrlRouterProvider } from "@uirouter/angularjs";
import { IUserService, ITemplateService, IStateTemplateParamsService } from "./interfaces/interfaces";

module msIoT {
    var app = angular.module("msIoT", ['ui.router', 'ui.tree', 'ui.bootstrap']);

    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider: StateProvider, $httpProvider: ng.IHttpProvider, $locationProvider: ng.ILocationProvider, $urlRouterProvider: UrlRouterProvider) {
            $urlRouterProvider.otherwise('app.home');
            $locationProvider.html5Mode(true);

            //Resolve calls
            var resolveCurrentUser = ['UserService', function (UserService: IUserService) {
                return UserService.getCurrentUser();
            }];
            var resolveCategories = ['TemplateService', function (TemplateService: ITemplateService) {
                return TemplateService.getCategories();
            }];
            var resolveCommonTemplates = ['TemplateService', function (TemplateService: ITemplateService) {
                return TemplateService.getCommonTemplates();
            }];
            var resolveUserTemplates = ['TemplateService', function (TemplateService: ITemplateService) {
                return TemplateService.getUserTemplates();
            }]
            var resolveTemplateById = ['TemplateService', '$stateParams', function (TemplateService: ITemplateService, $stateParams: IStateTemplateParamsService) {
                return TemplateService.getTemplateById($stateParams.templateId);
            }]

            //States
            var stateMainAbstract: Ng1StateDeclaration = {
                name: 'app',
                abstract: true,
                url: '',
                template: require('./views/Main.html'),
                controller: "MainCtrl",
                controllerAs: "rc",
                resolve: {
                    currentUser: resolveCurrentUser,
                    categories: resolveCategories
                }
            }

            var stateHome: Ng1StateDeclaration = {
                name: 'app.home',
                url: '/',
                template: require('./views/Home.html'),
                controller: "HomeCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates,
                    userTemplates: resolveUserTemplates
                }
            }

            var stateStepAbstract: Ng1StateDeclaration = {
                name: 'app.step',
                abstract: true,
                url: '',
                template: require('./views/Step.html'),
                controller: "StepCtrl",
                controllerAs: "st"
            }

            var stateChooseTemplate: Ng1StateDeclaration = {
                name: 'app.step.choosetemplate',
                url: '/Template/ChooseTemplate/:categoryId',
                template: require('./views/ChooseTemplate.html'),
                controller: "ChooseTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates
                }
            }

            var stateCreateTemplate: Ng1StateDeclaration = {
                name: 'app.step.createtemplate',
                url: '/Template/CreateTemplate/:categoryId/:templateId',
                template: require('./views/ManageTemplate.html'),
                controller: "ManageTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    template: resolveTemplateById
                }
            }

            var stateManageTemplate: Ng1StateDeclaration = {
                name: 'app.step.managetemplate',
                url: '/Template/ManageTemplate/:categoryId/:templateId',
                template: require('./views/ManageTemplate.html'),
                controller: "ManageTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    template: resolveTemplateById
                }
            }

            var stateSimulateTemplate: Ng1StateDeclaration = {
                name: 'app.step.simulatetemplate',
                url: '/Template/SimulateTemplate/:categoryId/:templateId',
                template: require('./views/SimulateTemplate.html'),
                controller: "SimulateTemplateCtrl",
                controllerAs: "vm"
            }

            $stateProvider
                .state(stateMainAbstract)
                .state(stateHome)
                .state(stateStepAbstract)
                .state(stateChooseTemplate)
                .state(stateCreateTemplate)
                .state(stateManageTemplate)
                .state(stateSimulateTemplate)
        }]
    ); 
}