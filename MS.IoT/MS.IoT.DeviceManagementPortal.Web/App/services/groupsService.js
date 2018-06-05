var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    var GroupsService = /** @class */ (function () {
        function GroupsService($q, $http, $log) {
            this.baseApi = 'api/groups/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        GroupsService.prototype.getCustomGroups = function () {
            var _this = this;
            this.$log.debug('Retrieving custom groups...');
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "groups"
            }).then(function (response) {
                response.data.sort(function (a, b) { return (a.order > b.order) ? 1 : ((b.order > a.order) ? -1 : 0); });
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved custom groups...', response);
            }, function (error) {
                _this.$log.error('Error retrieving custom groups.', error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.prototype.getCustomGroupById = function (customGroupId) {
            var _this = this;
            this.$log.debug('Retrieving custom group...', customGroupId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + "group/" + customGroupId
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved custom group...', customGroupId, response);
            }, function (error) {
                _this.$log.error('Error retrieving custom group.', customGroupId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.prototype.createCustomGroup = function (customGroup) {
            var _this = this;
            this.$log.debug('Creating new custom group...', customGroup);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(customGroup),
                url: this.baseApi + 'groups',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('New custom group created.', customGroup, response);
            }, function (error) {
                _this.$log.error('Error creating custom group.', customGroup, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.prototype.updateCustomGroup = function (customGroup) {
            var _this = this;
            this.$log.debug('Editing custom group...', customGroup);
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(customGroup),
                url: this.baseApi + 'groups',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('User custom group edited.', customGroup, response);
            }, function (error) {
                _this.$log.error('Error editing custom group.', customGroup, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.prototype.reorderCustomGroups = function (listCustomGroupsIds) {
            var _this = this;
            this.$log.debug('Reordering custom groups...', listCustomGroupsIds);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(listCustomGroupsIds),
                url: this.baseApi + 'groups/reorder',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Custom groups reordered.', listCustomGroupsIds, response);
            }, function (error) {
                _this.$log.error('Error reordering custom groups.', listCustomGroupsIds, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.prototype.deleteCustomGroups = function (listCustomGroupsIds) {
            var _this = this;
            this.$log.debug('Deleting custom groups...', listCustomGroupsIds);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(listCustomGroupsIds),
                url: this.baseApi + 'groups/delete',
                contentType: 'application/json'
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Custom groups deleted.', listCustomGroupsIds, response);
            }, function (error) {
                _this.$log.error('Error deleting custom groups.', listCustomGroupsIds, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        GroupsService.$inject = ['$q', '$http', '$log'];
        return GroupsService;
    }());
    MSIoTDeviceManagementPortal.GroupsService = GroupsService;
    app.service('GroupsService', GroupsService);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=groupsService.js.map