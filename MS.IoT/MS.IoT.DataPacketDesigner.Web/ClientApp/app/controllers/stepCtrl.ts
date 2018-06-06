import * as angular from "angular";
import { StateService, Ng1StateDeclaration } from "@uirouter/angularjs";
import {  } from "../models/models";

module msIoT {
    let app = angular.module("msIoT");

    //Step Controller (abstract) used to display the breadcrumb
    class StepCtrl {
        static $inject: Array<string> = ['$state'];
        public currentState: Ng1StateDeclaration;
        public currentStateParams: any;
        public currentIndex: number;
        public stepsCreate: any = [
            {
                title: "Home",
                stateView: "app.home"
            },
            {
                title: "Select Your Template",
                stateView: "app.step.choosetemplate"
            },
            {
                title: "Define Your Data Types",
                stateView: "app.step.createtemplate"
            },
            {
                title: "Simulate Your Data",
                stateView: "app.step.simulatetemplate"
            }
        ];
        public stepsEdit: any = [
            {
                title: "Home",
                stateView: "app.home"
            },
            {
                title: "Select Your Template",
                stateView: "app.step.choosetemplate"
            },
            {
                title: "Define Your Data Types",
                stateView: "app.step.managetemplate"
            },
            {
                title: "Simulate Your Data",
                stateView: "app.step.simulatetemplate"
            }
        ];
        public steps: any = this.stepsCreate;
        

        constructor($state : StateService) {
            this.currentState = $state.current;
            this.currentStateParams = $state.params;

            if (this.currentState.name == "app.step.createtemplate")
                this.steps = this.stepsCreate;
            else if (this.currentState.name == "app.step.managetemplate" || this.currentState.name == "app.step.simulatetemplate")
                this.steps = this.stepsEdit;

            this.currentIndex = 0;
            for (var i = 0; i < this.steps.length; i++)
            {
                if (this.steps[i].stateView == this.currentState.name) {
                    this.currentIndex = i;
                    break;
                }
            }
        };


    }

    app.controller('StepCtrl', StepCtrl);
}
