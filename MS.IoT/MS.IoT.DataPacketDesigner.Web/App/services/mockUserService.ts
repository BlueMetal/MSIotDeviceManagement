module msIoT {
    let app = angular.module("msIoT")

    export class MockUserService implements IUserService {
        static $inject: string[] = ['$q', '$http', '$log'];
        private readonly baseApi: string = 'api/user/';

        private $q: ng.IQService;
        private $http: any;
        private $log: ng.ILogService;

        private cacheCurrentUser: User;

        constructor($q: ng.IQService, $http: any, $log : ng.ILogService) {
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
                this.cacheCurrentUser = { id: "mockUser", name: "John Doe" };
                deferred.resolve(this.cacheCurrentUser);
            }
            return deferred.promise;
        }
        
    }

    app.service('MockUserService', MockUserService);
} 