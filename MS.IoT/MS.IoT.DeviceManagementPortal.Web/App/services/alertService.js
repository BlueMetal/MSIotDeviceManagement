var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    var AlertService = /** @class */ (function () {
        function AlertService($q, $http, $log) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        AlertService.prototype.getAlerts = function () {
            var _this = this;
            this.$log.debug('Retrieving alerts...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: "/Content/alerts/alerts.json"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved alerts...', response);
            }, function (error) {
                _this.$log.error('Error retrieving alerts.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        AlertService.$inject = ['$q', '$http', '$log'];
        return AlertService;
    }());
    MSIoTDeviceManagementPortal.AlertService = AlertService;
    app.service('AlertService', AlertService);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=alertService.js.map