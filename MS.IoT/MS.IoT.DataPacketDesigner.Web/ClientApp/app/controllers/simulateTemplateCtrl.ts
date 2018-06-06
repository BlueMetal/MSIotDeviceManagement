import * as angular from "angular";
import { StateService } from "@uirouter/angularjs";
import {  } from "../models/models";
import { ITemplateService, IStateTemplateParamsService } from "../interfaces/interfaces";

module msIoT {
    let app = angular.module("msIoT");

    //Controller for the view SimulateTemplate
    class SimulateTemplateCtrl {
        static $inject: Array<string> = ['TemplateService', '$stateParams', '$log', '$uibModal', '$state', '$http'];

        private templateService: ITemplateService;
        private $log: ng.ILogService;
        private $http: ng.IHttpService;
        private $uibModal: ng.ui.bootstrap.IModalService;
        private $state: StateService;

        public currentTemplateId: string;

        //Main Controller
        constructor(templateService: ITemplateService, $stateParams: IStateTemplateParamsService, $log: ng.ILogService, $uibModal: ng.ui.bootstrap.IModalService, $state: StateService, $http: ng.IHttpService) {
            this.templateService = templateService;
            this.$log = $log;
            this.$http = $http;
            this.$uibModal = $uibModal;
            this.$state = $state;

            this.currentTemplateId = $stateParams.templateId;
        }

        //Call the endpoint to download the simulator
		public downloadBlobZip(path : string) {
			this.$log.info("Downloading blob zip -"+path);			
            window.location.href = "/api/blob/download/"+path;
		}
    }
    app.controller('SimulateTemplateCtrl', SimulateTemplateCtrl);
}