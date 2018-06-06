import * as angular from "angular";
import { TransitionService } from '@uirouter/angularjs';
import { NotificationMessage } from "../models/models";

module msIoT {
    let app = angular.module("msIoT");

    //Main Controller (abstract) used for main notifications
    class MainCtrl {
        static $inject: Array<string> = ["$scope", "$transitions"];
        private notifications: NotificationMessage[] = [];

        constructor($scope: ng.IScope, $transitions: TransitionService) {
            $scope.$on('notificationsEvent', (event, data : NotificationMessage[]) => {
                this.notifications = data;
            });

            $transitions.onSuccess({}, ($transition) => {
                this.notifications = [];
            });
        };
    }

    app.controller('MainCtrl', MainCtrl);
}
