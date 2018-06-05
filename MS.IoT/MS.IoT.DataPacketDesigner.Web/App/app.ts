module msIoT {
    var app = angular.module("msIoT", ['ui.router', 'ui.tree', 'ui.bootstrap']);

    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider: ng.ui.IStateProvider, $httpProvider: ng.IHttpProvider, $locationProvider: ng.ILocationProvider, $urlRouterProvider : ng.ui.IUrlRouterProvider) {
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
            var stateMainAbstract: ng.ui.IState = {
                name: 'app',
                abstract: true,
                url: '',
                template: '<div id="notifications" class="ng-hide" ng-show="rc.notifications.length > 0"><div class="fullwidth"><div class="container"><ul><li ng-repeat="notification in rc.notifications" class="notification-{{notification.Type}}" ng-bind="notification.Message"></li></ul></div></div></div><div ui-view></div>',
                controller: "MainCtrl",
                controllerAs: "rc",
                resolve: {
                    currentUser: resolveCurrentUser,
                    categories: resolveCategories
                }
            }

            var stateHome: ng.ui.IState = {
                name: 'app.home',
                url: '/',
                templateUrl: '/App/views/Home.html',
                controller: "HomeCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates,
                    userTemplates: resolveUserTemplates
                }
            }

            var stateStepAbstract: ng.ui.IState = {
                name: 'app.step',
                abstract: true,
                url: '',
                template: '<nav id="steps"><ul steps-menu></ul></nav><div ui-view></div>',
                controller: "StepCtrl",
                controllerAs: "st"
            }

            var stateChooseTemplate: ng.ui.IState = {
                name: 'app.step.choosetemplate',
                url: '/Template/ChooseTemplate/:categoryId',
                templateUrl: '/App/views/ChooseTemplate.html',
                controller: "ChooseTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates
                }
            }

            var stateCreateTemplate: ng.ui.IState = {
                name: 'app.step.createtemplate',
                url: '/Template/CreateTemplate/:categoryId/:templateId',
                /*templateUrl: (params: IStateTemplateParamsService) => {
                    return '/Angular/Template/ManageTemplate/' + params.templateId;
                },*/
                templateUrl: '/App/views/ManageTemplate.html',
                controller: "ManageTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    template: resolveTemplateById
                }
            }

            var stateManageTemplate: ng.ui.IState = {
                name: 'app.step.managetemplate',
                url: '/Template/ManageTemplate/:categoryId/:templateId',
                /*templateUrl: (params: IStateTemplateParamsService) => {
                    return '/Angular/Template/ManageTemplate/' + params.templateId;
                },*/
                templateUrl: '/App/views/ManageTemplate.html',
                controller: "ManageTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    template: resolveTemplateById
                }
            }

            var stateSimulateTemplate: ng.ui.IState = {
                name: 'app.step.simulatetemplate',
                url: '/Template/SimulateTemplate/:categoryId/:templateId',
                /*templateUrl: (params: IStateTemplateParamsService) => {
                    return '/Angular/Template/SimulateTemplate/' + params.templateId;
                },*/
                templateUrl: '/App/views/SimulateTemplate.html',
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