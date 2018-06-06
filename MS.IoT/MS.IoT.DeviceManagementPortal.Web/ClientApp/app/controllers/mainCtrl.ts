import * as angular from "angular";
import { NotificationMessage } from "../models/models";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal");

    //Main Controller (abstract) used for main notifications
    class MainCtrl {
        static $inject: Array<string> = ["$scope"];
        private notifications: NotificationMessage[] = [];

        constructor($scope: ng.IScope) {
            $scope.$on('notificationsEvent', (event, data : NotificationMessage[]) => {
                this.notifications = data;
            });

            $scope.$on('$stateChangeSuccess',
                (event, toState, toParams, fromState, fromParams) => {
                    this.notifications = [];
                });
        };
    }

    app.controller('MainCtrl', MainCtrl as any);
}
