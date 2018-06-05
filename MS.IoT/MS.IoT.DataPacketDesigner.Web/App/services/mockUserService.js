var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    var MockUserService = /** @class */ (function () {
        function MockUserService($q, $http, $log) {
            this.baseApi = 'api/user/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        MockUserService.prototype.getCurrentUser = function () {
            this.$log.debug('Retrieving current user...');
            var deferred = this.$q.defer();
            if (this.cacheCurrentUser)
                deferred.resolve(this.cacheCurrentUser);
            else {
                this.cacheCurrentUser = { id: "mockUser", name: "John Doe" };
                deferred.resolve(this.cacheCurrentUser);
            }
            return deferred.promise;
        };
        MockUserService.$inject = ['$q', '$http', '$log'];
        return MockUserService;
    }());
    msIoT.MockUserService = MockUserService;
    app.service('MockUserService', MockUserService);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=mockUserService.js.map