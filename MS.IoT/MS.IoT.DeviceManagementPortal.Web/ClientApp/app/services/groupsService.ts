import * as angular from "angular";
import { IGroupsService } from "../interfaces/interfaces";
import { CustomGroupItem } from "../models/models";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal")

    export class GroupsService implements IGroupsService {
        static $inject: string[] = ['$q', '$http', '$log'];
        private readonly baseApi: string = 'api/groups/';

        private $q: ng.IQService;
        private $http: ng.IHttpService;
        private $log: ng.ILogService;

        constructor($q: ng.IQService, $http: ng.IHttpService, $log : ng.ILogService) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }

        public getCustomGroups(): ng.IPromise<CustomGroupItem[]> {
            this.$log.debug('Retrieving custom groups...');
            var deferred = this.$q.defer<CustomGroupItem[]>();
            this.$http({
                method: 'GET',
                url: this.baseApi + "groups"
            }).then((response) => {
                let groupItems = response.data as CustomGroupItem[];
                if (groupItems != undefined && groupItems.length > 0)
                    groupItems.sort(function (a, b) { return (a.order > b.order) ? 1 : ((b.order > a.order) ? -1 : 0); });
                deferred.resolve(groupItems);
                this.$log.debug('Retrieved custom groups...', response);
            }, (error) => {
                this.$log.error('Error retrieving custom groups.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public getCustomGroupById(customGroupId: string): ng.IPromise<CustomGroupItem> {
            this.$log.debug('Retrieving custom group...', customGroupId);
            var deferred = this.$q.defer<CustomGroupItem>();
            this.$http({
                method: 'GET',
                url: this.baseApi + "group/" + customGroupId
            }).then((response) => {
                deferred.resolve(response.data as CustomGroupItem);
                this.$log.debug('Retrieved custom group...', customGroupId, response);
            }, (error) => {
                this.$log.error('Error retrieving custom group.', customGroupId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        } 

        public createCustomGroup(customGroup: CustomGroupItem): ng.IPromise<string> {
            this.$log.debug('Creating new custom group...', customGroup);
            var deferred = this.$q.defer<string>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(customGroup),
                url: this.baseApi + 'groups',
                //contentType: 'application/json'
            }).then((response) => {
                deferred.resolve(response.data as string);
                this.$log.debug('New custom group created.', customGroup, response);
            }, (error) => {
                this.$log.error('Error creating custom group.', customGroup, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public updateCustomGroup(customGroup: CustomGroupItem): ng.IPromise<boolean> {
            this.$log.debug('Editing custom group...', customGroup);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(customGroup),
                url: this.baseApi + 'groups',
                //contentType: 'application/json'
            }).then((response) => {
                deferred.resolve(response.data as boolean);
                this.$log.debug('User custom group edited.', customGroup, response);
            }, (error) => {
                this.$log.error('Error editing custom group.', customGroup, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public reorderCustomGroups(listCustomGroupsIds: string[]): ng.IPromise<boolean> {
            this.$log.debug('Reordering custom groups...', listCustomGroupsIds);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(listCustomGroupsIds),
                url: this.baseApi + 'groups/reorder',
                //contentType: 'application/json'
            }).then((response) => {
                deferred.resolve(response.data as boolean);
                this.$log.debug('Custom groups reordered.', listCustomGroupsIds, response);
            }, (error) => {
                this.$log.error('Error reordering custom groups.', listCustomGroupsIds, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public deleteCustomGroups(listCustomGroupsIds: string[]): ng.IPromise<boolean> {
            this.$log.debug('Deleting custom groups...', listCustomGroupsIds);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(listCustomGroupsIds),
                url: this.baseApi + 'groups/delete',
                //contentType: 'application/json'
            }).then((response) => {
                deferred.resolve(response.data as boolean);
                this.$log.debug('Custom groups deleted.', listCustomGroupsIds, response);
            }, (error) => {
                this.$log.error('Error deleting custom groups.', listCustomGroupsIds, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }
    }

    app.service('GroupsService', GroupsService);
} 