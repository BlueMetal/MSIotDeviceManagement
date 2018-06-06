import * as angular from "angular";
import { IAlertService } from "../interfaces/interfaces";
import { Alert } from "../models/models";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal")

    export class AlertService implements IAlertService {
        static $inject: string[] = ['$q', '$http', '$log'];

        private $q: ng.IQService;
        private $http: ng.IHttpService;
        private $log: ng.ILogService;

        constructor($q: ng.IQService, $http: ng.IHttpService, $log : ng.ILogService) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }

		public getAlerts(): ng.IPromise<Alert[]> {
            this.$log.debug('Retrieving alerts...');
            var deferred = this.$q.defer<Alert[]>();
            this.$http({
                method: 'GET',
                url: "/alerts/alerts.json"
            }).then((response) => {
                deferred.resolve(response.data as Alert[]);
                this.$log.debug('Retrieved alerts...', response);
            }, (error) => {
                this.$log.error('Error retrieving alerts.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        } 
    }

    app.service('AlertService', AlertService);
} 