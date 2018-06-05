var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    var TemplateService = /** @class */ (function () {
        function TemplateService($q, $http, $log) {
            this.baseApi = 'api/templates/';
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }
        TemplateService.prototype.getCommonTemplates = function () {
            var _this = this;
            this.$log.debug('Retrieving common templates...');
            var deferred = this.$q.defer();
            if (this.cacheCommonTemplates)
                deferred.resolve(this.cacheCommonTemplates);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'common'
                }).then(function (response) {
                    _this.cacheCommonTemplates = response.data;
                    deferred.resolve(response.data);
                    _this.$log.debug('Retrieved common templates.', response);
                }, function (error) {
                    _this.$log.error('Error retrieving common templates.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        TemplateService.prototype.getUserTemplates = function () {
            var _this = this;
            this.$log.debug('Retrieving user templates...');
            var deferred = this.$q.defer();
            if (this.cacheUserTemplates)
                deferred.resolve(this.cacheUserTemplates);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'usertemplates'
                }).then(function (response) {
                    _this.cacheUserTemplates = response.data;
                    deferred.resolve(response.data);
                    _this.$log.debug('Retrieved user templates.', response);
                }, function (error) {
                    _this.$log.error('Error retrieving user templates.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        TemplateService.prototype.getCategories = function () {
            var _this = this;
            this.$log.debug('Retrieving categories...');
            var deferred = this.$q.defer();
            if (this.cacheCategories)
                deferred.resolve(this.cacheCategories);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'categories'
                }).then(function (response) {
                    _this.cacheCategories = response.data;
                    deferred.resolve(response.data);
                    _this.$log.debug('Retrieved categories.', response);
                }, function (error) {
                    _this.$log.error('Error retrieving categories.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        };
        TemplateService.prototype.getTemplateById = function (templateId) {
            var _this = this;
            this.$log.debug('Retrieving template...', templateId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'GET',
                url: this.baseApi + 'templates/' + templateId
            }).then(function (response) {
                deferred.resolve(response.data);
                _this.$log.debug('Retrieved template...', templateId, response);
            }, function (error) {
                _this.$log.error('Error retrieving template.', templateId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        TemplateService.prototype.createUserTemplate = function (template) {
            var _this = this;
            this.$log.debug('Creating new template...', template);
            var deferred = this.$q.defer();
            this.$http({
                method: 'POST',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                contentType: 'application/json'
            }).then(function (response) {
                _this.cacheUserTemplates = null;
                deferred.resolve(response.data);
                _this.$log.debug('New template created.', template, response);
            }, function (error) {
                _this.$log.error('Error creating template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        TemplateService.prototype.editUserTemplate = function (template) {
            var _this = this;
            this.$log.debug('Editing user template...', template);
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                contentType: 'application/json'
            }).then(function (response) {
                _this.cacheUserTemplates = null;
                deferred.resolve(response.data);
                _this.$log.debug('User template edited.', template, response);
            }, function (error) {
                _this.$log.error('Error editing template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        TemplateService.prototype.editUserTemplateReusable = function (template) {
            var _this = this;
            this.$log.debug('Editing user reusable template...', template);
            var deferred = this.$q.defer();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                contentType: 'application/json'
            }).then(function (response) {
                _this.cacheCommonTemplates = null;
                _this.cacheUserTemplates = null;
                deferred.resolve(response.data);
                _this.$log.debug('User template edited.', template, response);
            }, function (error) {
                _this.$log.error('Error editing template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        TemplateService.prototype.deleteUserTemplate = function (templateId) {
            var _this = this;
            this.$log.debug('Deleting user template...', templateId);
            var deferred = this.$q.defer();
            this.$http({
                method: 'DELETE',
                url: this.baseApi + 'usertemplates/' + templateId,
                contentType: 'application/json'
            }).then(function (response) {
                _this.cacheUserTemplates = null;
                _this.cacheCommonTemplates = null;
                deferred.resolve(response.data);
                _this.$log.debug('User template deleted.', templateId, response);
            }, function (error) {
                _this.$log.error('Error deleting template.', templateId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        };
        TemplateService.$inject = ['$q', '$http', '$log'];
        return TemplateService;
    }());
    msIoT.TemplateService = TemplateService;
    app.service('TemplateService', TemplateService);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=templateService.js.map