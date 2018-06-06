import {
    DeviceQueryConfiguration, DeviceQueryResponse, DeviceQueryRuleGroup, DeviceMapGroupsInformation,
    DeviceMapQueryConfiguration, DeviceMapQueryResponse, DeviceMapAreaQueryResponse,
    DeviceTwin, DeviceTwinSummary, DeviceTwinDesiredFeatures, DeviceUpdateResult, DeviceField,
    User, Alert, CustomGroupItem
} from "../models/models";

declare module MSIoTDeviceManagementPortalInterfaces {
    export interface IDeviceDBService {
        getDevicesTwinInfo(deviceQueryConfiguration: DeviceQueryConfiguration): ng.IPromise<DeviceQueryResponse>;
        getDevicesTwinIds(where: DeviceQueryRuleGroup): ng.IPromise<string[]>;
        getMapGroupsInformations(): ng.IPromise<DeviceMapGroupsInformation>;
        getDevicesTwinMap(deviceMapQueryConfiguration: DeviceMapQueryConfiguration): ng.IPromise<DeviceMapQueryResponse>;
        getDevicesTwinMapArea(deviceMapQueryConfiguration: DeviceMapQueryConfiguration): ng.IPromise<DeviceMapAreaQueryResponse>;
		getDeviceTwin(deviceId: string): ng.IPromise<DeviceTwin>;
		getDevicesTwinSummaryAggregations(topGroupActivatedGroupBy: string): ng.IPromise<DeviceTwinSummary>;
        initializeDeviceTwinDesiredFeatures(features: DeviceTwinDesiredFeatures): ng.IPromise<boolean>;//
		updateDeviceTwinDesiredFeature(feature: any): ng.IPromise<boolean>; // 
		publishFeature(publishFeaturemodel: any): ng.IPromise<boolean>;
		deleteDevice(deviceId: string): ng.IPromise<boolean>;
        deleteMultipleDevices(selection: string[]): ng.IPromise<boolean>;
        updateDevicesAsync(selection: string[], jsonDesired: any, jsonTags: any): ng.IPromise<DeviceUpdateResult>;
        updateDeviceSync(deviceId: string, jsonDesired: any, jsonTags: any): ng.IPromise<boolean>;
        getDeviceFields(): ng.IPromise<DeviceField>
    }

    export interface IUserService {
        getCurrentUser(): ng.IPromise<User>;
    }

    export interface IAlertService {
        getAlerts(): ng.IPromise<Alert[]>;
    }

    export interface IGroupsService {
        getCustomGroups(): ng.IPromise<CustomGroupItem[]>;
        getCustomGroupById(customGroupId: string): ng.IPromise<CustomGroupItem>;
        createCustomGroup(customGroup: CustomGroupItem): ng.IPromise<string>;
        updateCustomGroup(customGroup: CustomGroupItem): ng.IPromise<boolean>;
        reorderCustomGroups(listCustomGroupsIds: string[]): ng.IPromise<boolean>;
        deleteCustomGroups(listCustomGroupsIds: string[]): ng.IPromise<boolean>;
    }
}

export = MSIoTDeviceManagementPortalInterfaces;