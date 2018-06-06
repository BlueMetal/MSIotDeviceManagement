import * as angular from "angular";
import { LogicalOperators, FieldTypes } from "../models/enums";
import { DeviceQueryRuleGroup, DeviceQueryRule } from "../models/models";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal")

    app.directive('datatableRow', () => {
        return {
            restrict: 'AE',
            template: require('../templates/directives/datatable.html'),
            scope: { state: '=state', fields: '=fields', openModal: '&', pageChanged: '&', toggleSelectionItem: '&', toggleSelectionGroup: '&', itemsPerPage: '=', paginationMaxSize: '=' },
            link: function ($scope: any, element, attrs) {
                $scope.range = function (n : any) {
                    return new Array(n);
                };
            }
        };
    });

    app.directive('ruleGroup', () => {
        return {
            restrict: 'AE',
            template: require('../templates/directives/ruleGroup.html'),
            scope: { group: '=group', fields: '=fields',parent: '=parent', editMode: '=editMode' },
            link: function ($scope: any, element, attrs) {
                $scope.operators =[
                    { name: "All these conditions", id: 0 },
                    { name: "Any of these conditions", id: 1 }
                ]
                $scope.addRuleGroup = () => {
                    let newGroup: DeviceQueryRuleGroup = { rules: [{}], groups: [], operator: LogicalOperators.And, depth: $scope.group.depth+1 };
                    $scope.group.groups.push(newGroup);
                };
                $scope.addRule = () => {
                    let newFilter: DeviceQueryRule = { };
                    $scope.group.rules.push(newFilter);
                };
                $scope.removeRuleGroup = () => {
                    let groupIndex = $scope.parent.groups.indexOf($scope.group);
                    if (groupIndex !== -1) {
                        $scope.parent.groups.splice(groupIndex, 1);
                    }
                };
            }
        };
    });

    app.directive('ruleEntry', () => {
        return {
            restrict: 'AE',
            template: require('../templates/directives/ruleEntry.html'),
            scope: { entry: '=entry', fields: '=fields', parent: '=parent', editMode: '=editMode' },
            link: function ($scope: any, element, attrs) {
                $scope.operatorsList = [
                    { name: "is", id: 0 },
                    { name: "is not", id: 1 },
                    { name: "contains", id: 2 },
                    { name: "does not contain", id: 3 },
                    { name: "starts with", id: 4 },
                    { name: "ends with", id: 5 },
                    { name: "is greater than", id: 6 },
                    { name: "is greater or equal than", id: 7 },
                    { name: "is lesser than", id: 8 },
                    { name: "is lesser or equal than", id: 9 }
                ];
                $scope.operators = [];
                $scope.operators[FieldTypes.String] = [
                    $scope.operatorsList[0],
                    $scope.operatorsList[1],
                    $scope.operatorsList[2],
                    $scope.operatorsList[3],
                    $scope.operatorsList[4],
                    $scope.operatorsList[5]
                ];
                $scope.operators[FieldTypes.Number] = [
                    $scope.operatorsList[0],
                    $scope.operatorsList[1],
                    $scope.operatorsList[6],
                    $scope.operatorsList[7],
                    $scope.operatorsList[8],
                    $scope.operatorsList[9]
                ];
                $scope.operators[FieldTypes.Double] = [
                    $scope.operatorsList[0],
                    $scope.operatorsList[1],
                    $scope.operatorsList[6],
                    $scope.operatorsList[7],
                    $scope.operatorsList[8],
                    $scope.operatorsList[9]
                ];
                $scope.operators[FieldTypes.Date] = [
                    $scope.operatorsList[0],
                    $scope.operatorsList[1],
                    $scope.operatorsList[6],
                    $scope.operatorsList[7],
                    $scope.operatorsList[8],
                    $scope.operatorsList[9]
                ];
                $scope.operators[FieldTypes.Boolean] = [
                    $scope.operatorsList[0],
                    $scope.operatorsList[1]
                ];

                $scope.getFieldType = (field: string) => {
                    for (let i = 0; i < $scope.fields.length; i++) {
                        if ($scope.fields[i].name == field)
                            return $scope.fields[i].type;
                    }
                    return 0;
                }

                $scope.removeRule = () => {
                    let entityIndex = $scope.parent.rules.indexOf($scope.entry);
                    if (entityIndex !== -1) {
                        $scope.parent.rules.splice(entityIndex, 1);
                    }
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
            link: function (scope, element, attrs: any, ctrl) {
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