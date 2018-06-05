var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    var DeviceDBService = /** @class */ (function () {
        function DeviceDBService($q, $http, $log) {
            this.baseApi = 'api/devicetwin/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        DeviceDBService.prototype.getDeviceTwin = function (deviceId) {
            var _this = this;
            this.$log.debug('Retrieving device twin...', deviceId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/" + deviceId
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved device twin...', deviceId, response);
            }, function (error) {
                _this.$log.error('Error retrieving device twin.', deviceId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getDevicesTwinInfo = function (deviceQueryConfiguration) {
            var _this = this;
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceQueryConfiguration),
                url: this.baseApi + "devices/list"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved devices twin...', response);
            }, function (error) {
                _this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getDevicesTwinIds = function (where) {
            var _this = this;
            this.$log.debug('Retrieving devices twin ids...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(where),
                url: this.baseApi + "devices/listids"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved devices twin ids...', response);
            }, function (error) {
                _this.$log.error('Error retrieving devices twin ids.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getMapGroupsInformations = function () {
            var _this = this;
            this.$log.debug('Retrieving map info...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/mapinfo"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved map info...', response);
            }, function (error) {
                _this.$log.error('Error retrieving map info.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getDevicesTwinMap = function (deviceMapQueryConfiguration) {
            var _this = this;
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceMapQueryConfiguration),
                url: this.baseApi + "devices/map"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved devices twin...', response);
            }, function (error) {
                _this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getDevicesTwinMapArea = function (deviceMapQueryConfiguration) {
            var _this = this;
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceMapQueryConfiguration),
                url: this.baseApi + "devices/maparea"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved devices twin...', response);
            }, function (error) {
                _this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.getDevicesTwinSummaryAggregations = function (topGroupActivatedGroupBy) {
            var _this = this;
            this.$log.debug('Retrieving devices summary aggregations...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/summary?groupBy=" + topGroupActivatedGroupBy
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved devices summary..', response);
            }, function (error) {
                _this.$log.error('Error retrieving devices summary.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.updateDevicesTwinLocations = function () {
            var _this = this;
            this.$log.debug('Updating devices twin locations...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                url: this.baseApi + "devices/location"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Updated devices twin locations..', response);
            }, function (error) {
                _this.$log.error('Error updating devices locations..', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.initializeDeviceTwinDesiredFeatures = function (features) {
            var _this = this;
            this.$log.debug('updating desired features..', features);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(features),
                url: this.baseApi + 'properties/features',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('User features updated.', features, response);
            }, function (error) {
                _this.$log.error('Error updating user features', features, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.updateDeviceTwinDesiredFeature = function (feature) {
            var _this = this;
            this.$log.debug('updating desired feature..', feature);
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(feature),
                url: this.baseApi + 'properties/feature',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('User features updated.', feature, response);
            }, function (error) {
                _this.$log.error('Error updating user features', feature, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.publishFeature = function (publishFeaturemodel) {
            var _this = this;
            this.$log.debug('publishing a feature..', publishFeaturemodel);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(publishFeaturemodel),
                url: this.baseApi + 'properties/feature/publish',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('publishing a feature successfully.', publishFeaturemodel, response);
            }, function (error) {
                _this.$log.error('Error publishing a feature', publishFeaturemodel, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.deleteDevice = function (deviceId) {
            var _this = this;
            this.$log.debug('deleting device with id..', deviceId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'DELETE',
                url: this.baseApi + 'devices/' + deviceId,
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Deleted device successfully.', deviceId, response);
            }, function (error) {
                _this.$log.error('Error deleting device', deviceId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.deleteMultipleDevices = function (selection) {
            var _this = this;
            this.$log.debug('deleting multiple devices...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(selection),
                url: this.baseApi + 'devices',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Multiple devices deleted.', response);
            }, function (error) {
                _this.$log.error('Error deleting multiple devices', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.prototype.updateDevicesAsync = function (selection, jsonDesired, jsonTags) {
            var _this = this;
            this.$log.debug('updating multiple devices...');
            var deferred = this.$q.defer();
            if (selection.length == 0 || (jsonDesired == null && jsonTags == null)) {
                this.$log.debug('nothing to update.');
                deferred.resolve({ hasSucceeded: false, errorReason: "Nothing to update" });
            }
            else {
                var jsonDesiredString = jsonDesired != null ? JSON.stringify(jsonDesired) : null;
                var jsonTagsString = jsonTags != null ? JSON.stringify(jsonTags) : null;
                var update = { deviceIds: selection, jsonDesired: jsonDesiredString, jsonTags: jsonTagsString };
                this.$http({
                    method: 'PUT',
                    data: JSON.stringify(update),
                    url: this.baseApi + 'devices/update',
                    contentType: 'application/json'
                }).then(function (response) {
                    deferred.resolve(response.data);
                    _this.$log.debug('Multiple devices update started.', response);
                }, function (error) {
                    _this.$log.error('Error updating multiple devices', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        DeviceDBService.prototype.updateDeviceSync = function (deviceId, jsonDesired, jsonTags) {
            var _this = this;
            this.$log.debug('updating single device...');
            var deferred = this.$q.defer();
            if (deviceId == null || deviceId == "" || (jsonDesired == null && jsonTags == null)) {
                this.$log.debug('nothing to update.');
                deferred.resolve(true);
            }
            else {
                var jsonDesiredString = jsonDesired != null ? JSON.stringify(jsonDesired) : null;
                var jsonTagsString = jsonTags != null ? JSON.stringify(jsonTags) : null;
                var update = { deviceId: deviceId, jsonDesired: jsonDesiredString, jsonTags: jsonTagsString };
                this.$http({
                    method: 'PUT',
                    data: JSON.stringify(update),
                    url: this.baseApi + 'device/update',
                    contentType: 'application/json'
                }).then(function (response) {
                    deferred.resolve(response.data);
                    _this.$log.debug('Single device update succeeded.', response);
                }, function (error) {
                    _this.$log.error('Error updating single device', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        DeviceDBService.prototype.getDeviceFields = function () {
            var _this = this;
            this.$log.debug('Retrieving device fields...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/fields"
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved device fields..', response);
            }, function (error) {
                _this.$log.error('Error retrieving device fields.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        DeviceDBService.$inject = ['$q', '$http', '$log'];
        return DeviceDBService;
    }());
    MSIoTDeviceManagementPortal.DeviceDBService = DeviceDBService;
    app.service('DeviceDBService', DeviceDBService);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=deviceDBService.js.map