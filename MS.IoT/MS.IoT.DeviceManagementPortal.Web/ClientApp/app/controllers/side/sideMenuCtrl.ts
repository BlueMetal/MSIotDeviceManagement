import * as angular from "angular";
import { TransitionService, StateObject } from "@uirouter/angularjs";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal");

    //Main Controller (abstract) used for main notifications
    class SideMenuCtrl {
        static $inject: Array<string> = ['$transitions'];
        public isListActivated : boolean = true;
        public isMapActivated : boolean = false;
        public isGroupsActivated : boolean = false;

        constructor($transitions: TransitionService) {

            $transitions.onStart({}, ($transition, ) => {
                let toState: StateObject = $transition.$to();
                this.isMapActivated = toState.name == 'app.map';
                this.isListActivated = toState.name == 'app.list';
                this.isGroupsActivated = toState.name == 'app.groups';
            });
        };
    }

    app.controller('SideMenuCtrl', SideMenuCtrl as any);
}
