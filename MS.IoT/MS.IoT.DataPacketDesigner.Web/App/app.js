var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT", ['ui.router', 'ui.tree', 'ui.bootstrap']);
    app.config(['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('app.home');
            $locationProvider.html5Mode(true);
            //Resolve calls
            var resolveCurrentUser = ['UserService', function (UserService) {
                    return UserService.getCurrentUser();
                }];
            var resolveCategories = ['TemplateService', function (TemplateService) {
                    return TemplateService.getCategories();
                }];
            var resolveCommonTemplates = ['TemplateService', function (TemplateService) {
                    return TemplateService.getCommonTemplates();
                }];
            var resolveUserTemplates = ['TemplateService', function (TemplateService) {
                    return TemplateService.getUserTemplates();
                }];
            var resolveTemplateById = ['TemplateService', '$stateParams', function (TemplateService, $stateParams) {
                    return TemplateService.getTemplateById($stateParams.templateId);
                }];
            //States
            var stateMainAbstract = {
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
            };
            var stateHome = {
                name: 'app.home',
                url: '/',
                templateUrl: '/App/views/Home.html',
                controller: "HomeCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates,
                    userTemplates: resolveUserTemplates
                }
            };
            var stateStepAbstract = {
                name: 'app.step',
                abstract: true,
                url: '',
                template: '<nav id="steps"><ul steps-menu></ul></nav><div ui-view></div>',
                controller: "StepCtrl",
                controllerAs: "st"
            };
            var stateChooseTemplate = {
                name: 'app.step.choosetemplate',
                url: '/Template/ChooseTemplate/:categoryId',
                templateUrl: '/App/views/ChooseTemplate.html',
                controller: "ChooseTemplateCtrl",
                controllerAs: "vm",
                resolve: {
                    commonTemplates: resolveCommonTemplates
                }
            };
            var stateCreateTemplate = {
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
            };
            var stateManageTemplate = {
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
            };
            var stateSimulateTemplate = {
                name: 'app.step.simulatetemplate',
                url: '/Template/SimulateTemplate/:categoryId/:templateId',
                /*templateUrl: (params: IStateTemplateParamsService) => {
                    return '/Angular/Template/SimulateTemplate/' + params.templateId;
                },*/
                templateUrl: '/App/views/SimulateTemplate.html',
                controller: "SimulateTemplateCtrl",
                controllerAs: "vm"
            };
            $stateProvider
                .state(stateMainAbstract)
                .state(stateHome)
                .state(stateStepAbstract)
                .state(stateChooseTemplate)
                .state(stateCreateTemplate)
                .state(stateManageTemplate)
                .state(stateSimulateTemplate);
        }]);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=app.js.map