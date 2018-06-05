var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    app.directive('stepsMenu', function () {
        return {
            restrict: 'A',
            templateUrl: 'App/directives/steps-menu.html',
        };
    });
    app.directive('treeviewBlock', function () {
        return {
            restrict: 'AE',
            templateUrl: 'App/directives/treeview-block.html',
            scope: { templates: '=' },
            link: function ($scope, element, attrs) {
                $scope.propertyTypes = [
                    { id: 'text', name: 'Text' },
                    { id: 'boolean', name: 'Boolean' },
                    { id: 'number', name: 'Number' },
                    { id: 'date', name: 'Date' },
                    { id: 'list', name: 'List' }
                ];
                $scope.newSubItem = function (scope) {
                    var nodeData = scope.$modelValue;
                    nodeData.properties.push({
                        name: '',
                        type: 'text',
                        properties: []
                    });
                    nodeData.type = 'object';
                };
                $scope.removeItem = function (scope) {
                    scope.remove();
                    var parentContext = scope.$parentNodeScope;
                    if (parentContext != null) {
                        var nodeData = parentContext.$modelValue;
                        if (nodeData.properties == null || nodeData.properties.length == 0)
                            nodeData.type = 'text';
                    }
                };
            }
        };
    });
    app.directive('editableField', function () {
        return {
            restrict: 'E',
            templateUrl: 'App/directives/editable-field.html',
            scope: { field: '=', maxlength: '@', inputType: '@', required: '@' },
            link: function ($scope, element, attrs) {
                $scope.isEditMode = false;
                $scope.toggle = function () {
                    $scope.isEditMode = !$scope.isEditMode;
                };
            }
        };
    });
    //https://stackoverflow.com/questions/28975337/angularjs-stop-user-from-typing-invalid-input
    app.directive("regExInput", function () {
        "use strict";
        return {
            restrict: "A",
            require: "?regEx",
            scope: {},
            replace: false,
            link: function (scope, element, attrs, ctrl) {
                element.bind('keypress', function (event) {
                    var regex = new RegExp(attrs.regEx);
                    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
                    if (!regex.test(key)) {
                        event.preventDefault();
                        return false;
                    }
                });
            }
        };
    });
})(msIoT || (msIoT = {}));
//# sourceMappingURL=directives.js.map