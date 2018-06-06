import * as angular from "angular";

module msIoT {
    let app = angular.module("msIoT");

    //Menu controller used to collapse the menu in phone view
    class MenuCtrl {
        static $inject: Array<string> = [];
        public isNavCollapsed: boolean;

        constructor() {
            this.isNavCollapsed = true;
        };


    }

    app.controller('MenuCtrl', MenuCtrl);
}
