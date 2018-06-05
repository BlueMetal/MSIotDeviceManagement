module msIoT {
    let app = angular.module("msIoT")

    export class UserService implements IUserService {
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
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'current'
                }).then((response) => {
                    this.cacheCurrentUser = response.data;
                    deferred.resolve(response.data);
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