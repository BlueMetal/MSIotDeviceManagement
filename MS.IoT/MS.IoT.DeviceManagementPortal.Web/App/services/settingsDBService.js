var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    var SettingsDBService = (function () {
        function SettingsDBService($q, $http, $log) {
            this.baseApi = 'api/settings/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        SettingsDBService.prototype.getListViews = function () {
            var _this = this;
            this.$log.debug('Retrieving list views...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "views/"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved list views...', response);
            }, function (error) {
                _this.$log.error('Error retrieving list views.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        SettingsDBService.prototype.getListViewById = function (listViewId) {
            var _this = this;
            this.$log.debug('Retrieving list view...', listViewId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "view/" + listViewId
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved list view...', listViewId, response);
            }, function (error) {
                _this.$log.error('Error retrieving list view.', listViewId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        SettingsDBService.prototype.createListView = function (listView) {
            var _this = this;
            this.$log.debug('Creating new list view...', listView);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(listView),
                url: this.baseApi + 'views/',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('New list view created.', listView, response);
            }, function (error) {
                _this.$log.error('Error creating list view.', listView, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        SettingsDBService.prototype.editListView = function (listView) {
            var _this = this;
            this.$log.debug('Editing list view...', listView);
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(listView),
                url: this.baseApi + 'views/',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('User list view edited.', listView, response);
            }, function (error) {
                _this.$log.error('Error editing list view.', listView, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        SettingsDBService.prototype.deleteListView = function (listViewId) {
            var _this = this;
            this.$log.debug('Deleting list view...', listViewId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'DELETE',
                url: this.baseApi + 'view/' + listViewId,
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('List view deleted.', listViewId, response);
            }, function (error) {
                _this.$log.error('Error deleting list view.', listViewId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        return SettingsDBService;
    }());
    SettingsDBService.$inject = ['$q', '$http', '$log'];
    MSIoTDeviceManagementPortal.SettingsDBService = SettingsDBService;
    app.service('SettingsDBService', SettingsDBService);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=settingsDBService.js.map