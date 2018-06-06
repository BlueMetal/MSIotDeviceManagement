import * as angular from "angular";
import { CustomProperty } from "../models/models";

module msIoT {
    let app = angular.module("msIoT")

    app.directive('stepsMenu', () => {
        return {
            restrict: 'A',
            template: require('../directives/steps-menu.html')
            //scope: { currentState: '=', steps: '=' } //No need for isolated scope
        };
    });

    app.directive('treeviewNode', () => {
        return {
            restrict: 'A',
            template: require('../directives/nodes-renderer.html')
            //scope: { currentState: '=', steps: '=' } //No need for isolated scope
        };
    });

    app.directive('treeviewBlock', () => {
        return {
            restrict: 'AE',
            template: require('../directives/treeview-block.html'),
            scope: { templates: '=?' },
            link: function ($scope : any, element, attrs) {
                $scope.propertyTypes = [
                    { id: 'text', name: 'Text' },
                    { id: 'boolean', name: 'Boolean' },
                    { id: 'number', name: 'Number' },
                    { id: 'date', name: 'Date' },
                    { id: 'list', name: 'List' }
                ];

                $scope.newSubItem = function (scope : any) {
                    var nodeData : CustomProperty = scope.$modelValue;
                    nodeData.properties.push({
                        name: '',
                        type: 'text',
                        properties: []
                    });
                    nodeData.type = 'object';
                };

                $scope.removeItem = function (scope : any) {
                    scope.remove();
                    var parentContext = scope.$parentNodeScope;
                    if (parentContext != null) {
                        var nodeData: CustomProperty = parentContext.$modelValue;
                        if (nodeData.properties == null || nodeData.properties.length == 0)
                            nodeData.type = 'text';
                    }
                    
                }
            }
        };
    });

    app.directive('editableField', () => {
        return {
            restrict: 'E',
            template: require('../directives/editable-field.html'),
            scope: { field: '=', maxlength: '@', inputType: '@', required: '@' },
            link: function ($scope: any, element, attrs) {
                $scope.isEditMode = false;

                $scope.toggle = function () {
                    $scope.isEditMode = !$scope.isEditMode;
                }
            }
        };
    })

    //https://stackoverflow.com/questions/28975337/angularjs-stop-user-from-typing-invalid-input
    app.directive("regExInput", function () {
        "use strict";
        return {
            restrict: "A",
            require: "?regEx",
            scope: {},
            replace: false,
            link: function (scope, element, attrs : any, ctrl) {
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
}