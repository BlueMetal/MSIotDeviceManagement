import * as angular from "angular";
import { User } from "../models/models";
import { IUserService } from "../interfaces/interfaces";

module msIoT {
    let app = angular.module("msIoT")

    export class MockUserService implements IUserService {
        static $inject: string[] = ['$q', '$log'];
        private readonly baseApi: string = 'api/user/';

        private $q: ng.IQService;
        private $log: ng.ILogService;

        private cacheCurrentUser?: User = undefined;

        constructor($q: ng.IQService, $log : ng.ILogService) {
            this.$q = $q;
            this.$log = $log;
        }

        public getCurrentUser(): ng.IPromise<User> {
            this.$log.debug('Retrieving current user...');
            var deferred = this.$q.defer<User>();
            if (this.cacheCurrentUser)
                deferred.resolve(this.cacheCurrentUser);
            else {
                this.cacheCurrentUser = { id: "mockUser", name: "John Doe" };
                deferred.resolve(this.cacheCurrentUser);
            }
            return deferred.promise;
        }
        
    }

    app.service('MockUserService', MockUserService);
} 