var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    var UserService = /** @class */ (function () {
        function UserService($q, $http, $log) {
            this.baseApi = 'api/user/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        UserService.prototype.getCurrentUser = function () {
            var _this = this;
            this.$log.debug('Retrieving current user...');
            var deferred = this.$q.defer();
            if (this.cacheCurrentUser)
                deferred.resolve(this.cacheCurrentUser);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'current'
                }).then(function (response) {
                    _this.cacheCurrentUser = response.data;
                    deferred.resolve(response.data);
                    _this.$log.debug('Retrieved current user.', response);
                }, function (error) {
                    _this.$log.error('Error retrieving current user.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        UserService.$inject = ['$q', '$http', '$log'];
        return UserService;
    }());
    MSIoTDeviceManagementPortal.UserService = UserService;
    app.service('UserService', UserService);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=userService.js.map