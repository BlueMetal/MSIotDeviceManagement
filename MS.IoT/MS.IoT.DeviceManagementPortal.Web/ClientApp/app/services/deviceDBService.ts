import * as angular from "angular";
import { IDeviceDBService } from "../interfaces/interfaces";
import {
    DeviceTwin, DeviceQueryConfiguration, DeviceQueryResponse, DeviceQueryRuleGroup,
    DeviceMapGroupsInformation, DeviceMapQueryConfiguration, DeviceMapQueryResponse,
    DeviceMapAreaQueryResponse, DeviceTwinSummary, DeviceTwinDesiredFeatures, DeviceUpdateResult,
    DeviceTwinUpdateAsyncModel, DeviceTwinUpdateSyncModel, DeviceField
} from "../models/models";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal")

    export class DeviceDBService implements IDeviceDBService {
        static $inject: string[] = ['$q', '$http', '$log'];
        private readonly baseApi: string = 'api/devicetwin/';

        private $q: ng.IQService;
        private $http: ng.IHttpService;
        private $log: ng.ILogService;

        constructor($q: ng.IQService, $http: ng.IHttpService, $log : ng.ILogService) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }

		public getDeviceTwin(deviceId: string): ng.IPromise<DeviceTwin> {
            this.$log.debug('Retrieving device twin...', deviceId);
            var deferred = this.$q.defer<DeviceTwin>();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/" + deviceId
            }).then((response) => {
                deferred.resolve(response.data as DeviceTwin);
                this.$log.debug('Retrieved device twin...', deviceId, response);
            }, (error) => {
                this.$log.error('Error retrieving device twin.', deviceId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        } 

        public getDevicesTwinInfo(deviceQueryConfiguration: DeviceQueryConfiguration): ng.IPromise<DeviceQueryResponse> {
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer<DeviceQueryResponse>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceQueryConfiguration),
                url: this.baseApi + "devices/list"
            }).then((response : any) => {
                deferred.resolve(response.data);
                this.$log.debug('Retrieved devices twin...', response);
            }, (error) => {
                this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getDevicesTwinIds(where: DeviceQueryRuleGroup): ng.IPromise<string[]> {
            this.$log.debug('Retrieving devices twin ids...');
            var deferred = this.$q.defer<string[]>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(where),
                url: this.baseApi + "devices/listids"
            }).then((response: any) => {
                deferred.resolve(response.data);
                this.$log.debug('Retrieved devices twin ids...', response);
            }, (error) => {
                this.$log.error('Error retrieving devices twin ids.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getMapGroupsInformations(): ng.IPromise<DeviceMapGroupsInformation> {
            this.$log.debug('Retrieving map info...');
            var deferred = this.$q.defer<DeviceMapGroupsInformation>();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/mapinfo"
            }).then((response: any) => {
                deferred.resolve(response.data);
                this.$log.debug('Retrieved map info...', response);
            }, (error) => {
                this.$log.error('Error retrieving map info.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getDevicesTwinMap(deviceMapQueryConfiguration: DeviceMapQueryConfiguration): ng.IPromise<DeviceMapQueryResponse> {
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer<DeviceMapQueryResponse>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceMapQueryConfiguration),
                url: this.baseApi + "devices/map"
            }).then((response: any) => {
                deferred.resolve(response.data);
                this.$log.debug('Retrieved devices twin...', response);
            }, (error) => {
                this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getDevicesTwinMapArea(deviceMapQueryConfiguration: DeviceMapQueryConfiguration): ng.IPromise<DeviceMapAreaQueryResponse> {
            this.$log.debug('Retrieving devices twin...');
            var deferred = this.$q.defer<DeviceMapAreaQueryResponse>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(deviceMapQueryConfiguration),
                url: this.baseApi + "devices/maparea"
            }).then((response: any) => {
                deferred.resolve(response.data);
                this.$log.debug('Retrieved devices twin...', response);
            }, (error) => {
                this.$log.error('Error retrieving devices twin.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getDevicesTwinSummaryAggregations(topGroupActivatedGroupBy: string): ng.IPromise<DeviceTwinSummary> {
			this.$log.debug('Retrieving devices summary aggregations...');
            var deferred = this.$q.defer<DeviceTwinSummary>();
			this.$http({
				method: 'GET',
                url: this.baseApi + "devices/summary?groupBy=" + topGroupActivatedGroupBy
			}).then((response) => {
				deferred.resolve(response.data as DeviceTwinSummary);
				this.$log.debug('Retrieved devices summary..', response);
			}, (error) => {
				this.$log.error('Error retrieving devices summary.', error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

		public updateDevicesTwinLocations() {
			this.$log.debug('Updating devices twin locations...');
			var deferred = this.$q.defer();
			this.$http({
				method: 'PUT',
				url: this.baseApi + "devices/location"
			}).then((response) => {
				deferred.resolve(response.data);
				this.$log.debug('Updated devices twin locations..', response);
			}, (error) => {
				this.$log.error('Error updating devices locations..', error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

        public initializeDeviceTwinDesiredFeatures(features: DeviceTwinDesiredFeatures): ng.IPromise<boolean> {
			this.$log.debug('updating desired features..', features);
            var deferred = this.$q.defer<boolean>();
			this.$http({
				method: 'POST',
				data: JSON.stringify(features),
				url: this.baseApi + 'properties/features',
				//contentType: 'application/json'
			}).then((response) => {
				deferred.resolve(response.data as boolean);
				this.$log.debug('User features updated.', features, response);
			}, (error) => {
				this.$log.error('Error updating user features', features, error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

		public updateDeviceTwinDesiredFeature(feature: any): ng.IPromise<boolean> {
			this.$log.debug('updating desired feature..', feature);
            var deferred = this.$q.defer<boolean>();
			this.$http({
				method: 'PUT',
				data: JSON.stringify(feature),
				url: this.baseApi + 'properties/feature',
				//contentType: 'application/json'
			}).then((response) => {
				deferred.resolve(response.data as boolean);
				this.$log.debug('User features updated.', feature, response);
			}, (error) => {
				this.$log.error('Error updating user features', feature, error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

		public publishFeature(publishFeaturemodel: any): ng.IPromise<boolean> {
			this.$log.debug('publishing a feature..', publishFeaturemodel);
            var deferred = this.$q.defer<boolean>();
			this.$http({
				method: 'POST',
				data: JSON.stringify(publishFeaturemodel),
				url: this.baseApi + 'properties/feature/publish',
				//contentType: 'application/json'
			}).then((response) => {
				deferred.resolve(response.data as boolean);
				this.$log.debug('publishing a feature successfully.', publishFeaturemodel, response);
			}, (error) => {
				this.$log.error('Error publishing a feature', publishFeaturemodel, error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

		public deleteDevice(deviceId: string): ng.IPromise<boolean> {
			this.$log.debug('deleting device with id..', deviceId);
            var deferred = this.$q.defer<boolean>();
			this.$http({
				method: 'DELETE',
				url: this.baseApi + 'devices/' + deviceId,
			}).then((response) => {
				deferred.resolve(response.data as boolean);
				this.$log.debug('Deleted device successfully.', deviceId, response);
			}, (error) => {
				this.$log.error('Error deleting device', deviceId, error);
				deferred.reject([]);
			});
			return deferred.promise;
		}

		public deleteMultipleDevices(selection:string[]): ng.IPromise<boolean> {		
			this.$log.debug('deleting multiple devices...');
            var deferred = this.$q.defer<boolean>();
			this.$http({
				method: 'POST',
				data: JSON.stringify(selection),
				url: this.baseApi + 'devices',
				//contentType: 'application/json'
			}).then((response) => {
				deferred.resolve(response.data as boolean);
				this.$log.debug('Multiple devices deleted.', response);
			}, (error) => {
				this.$log.error('Error deleting multiple devices', error);
				deferred.reject([]);
			});
			return deferred.promise;
        }

        public updateDevicesAsync(selection: string[], jsonDesired: any, jsonTags: any): ng.IPromise<DeviceUpdateResult> {
            this.$log.debug('updating multiple devices...');
            var deferred = this.$q.defer<DeviceUpdateResult>();

            if (selection.length == 0 || (jsonDesired == null && jsonTags == null)) {
                this.$log.debug('nothing to update.');
                deferred.resolve({ hasSucceeded: false, errorReason: "Nothing to update" });
            }
            else {
                let jsonDesiredString = jsonDesired != null ? JSON.stringify(jsonDesired) : undefined;
                let jsonTagsString = jsonTags != null ? JSON.stringify(jsonTags) : undefined;
                let update: DeviceTwinUpdateAsyncModel = { deviceIds: selection, jsonDesired: jsonDesiredString, jsonTags: jsonTagsString };
                this.$http({
                    method: 'PUT',
                    data: JSON.stringify(update),
                    url: this.baseApi + 'devices/update',
                    //contentType: 'application/json'
                }).then((response) => {
                    deferred.resolve(response.data as DeviceUpdateResult);
                    this.$log.debug('Multiple devices update started.', response);
                }, (error) => {
                    this.$log.error('Error updating multiple devices', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        }

        public updateDeviceSync(deviceId: string, jsonDesired: any, jsonTags: any): ng.IPromise<boolean> {
            this.$log.debug('updating single device...');
            var deferred = this.$q.defer<boolean>();
            
            if (deviceId == null || deviceId == "" || (jsonDesired == null && jsonTags == null)) {
                this.$log.debug('nothing to update.');
                deferred.resolve(true);
            }
            else {
                let jsonDesiredString = jsonDesired != null ? JSON.stringify(jsonDesired) : undefined;
                let jsonTagsString = jsonTags != null ? JSON.stringify(jsonTags) : undefined;
                let update: DeviceTwinUpdateSyncModel = { deviceId: deviceId, jsonDesired: jsonDesiredString, jsonTags: jsonTagsString };
                this.$http({
                    method: 'PUT',
                    data: JSON.stringify(update),
                    url: this.baseApi + 'device/update',
                    //contentType: 'application/json'
                }).then((response) => {
                    deferred.resolve(response.data as boolean);
                    this.$log.debug('Single device update succeeded.', response);
                }, (error) => {
                    this.$log.error('Error updating single device', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        }

        public getDeviceFields(): ng.IPromise<DeviceField> {
            this.$log.debug('Retrieving device fields...');
            var deferred = this.$q.defer<DeviceField>();
            this.$http({
                method: 'GET',
                url: this.baseApi + "devices/fields"
            }).then((response) => {
                deferred.resolve(response.data as DeviceField);
                this.$log.debug('Retrieved device fields..', response);
            }, (error) => {
                this.$log.error('Error retrieving device fields.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }
    }

    app.service('DeviceDBService', DeviceDBService);
} 