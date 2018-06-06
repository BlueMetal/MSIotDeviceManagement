import * as angular from "angular";
import { User } from "../models/models";
import { IUserService } from "../interfaces/interfaces";

module msIoT {

    let app = angular.module("msIoT")

    export class UserService implements IUserService {
        static $inject: string[] = ['$q', '$http', '$log'];
        private readonly baseApi: string = 'api/user/';

        private $q: ng.IQService;
        private $http: ng.IHttpService;
        private $log: ng.ILogService;

        private cacheCurrentUser?: User = undefined;

        constructor($q: ng.IQService, $http: ng.IHttpService, $log : ng.ILogService) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }

        public getCurrentUser(): ng.IPromise<User> {
            this.$log.debug('Retrieving current user...');
            var deferred = this.$q.defer<User>();
            if (this.cacheCurrentUser)
                deferred.resolve(this.cacheCurrentUser);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'current'
                }).then((response) => {
                    this.cacheCurrentUser = response.data as User;
                    deferred.resolve(response.data as User);
                    this.$log.debug('Retrieved current user.', response);
                }, (error) => {
                    this.$log.error('Error retrieving current user.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        }
        
    }

    app.service('UserService', UserService);
} 