import * as angular from "angular";
import { Template, Category } from "../models/models";
import { ITemplateService } from "../interfaces/interfaces";

module msIoT {
    let app = angular.module("msIoT")

    export class TemplateService implements ITemplateService {
        static $inject: string[] = ['$q', '$http', '$log'];
        private readonly baseApi: string = 'api/templates/';

        private $q: ng.IQService;
        private $http: ng.IHttpService;
        private $log: ng.ILogService;

        private cacheCommonTemplates?: Template[] = undefined;
        private cacheCategories?: Category[] = undefined;
        private cacheUserTemplates?: Template[] = undefined;

        constructor($q: ng.IQService, $http: ng.IHttpService, $log : ng.ILogService) {
            this.$q = $q;
            this.$http = $http;
            this.$log = $log;
        }

        public getCommonTemplates(): ng.IPromise<Template[]> {
            this.$log.debug('Retrieving common templates...');
            var deferred = this.$q.defer<Template[]>();
            if (this.cacheCommonTemplates)
                deferred.resolve(this.cacheCommonTemplates);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'common'
                }).then((response) => {
                    this.cacheCommonTemplates = response.data as Template[];
                    deferred.resolve(response.data as Template[]);
                    this.$log.debug('Retrieved common templates.', response);
                }, (error) => {
                    this.$log.error('Error retrieving common templates.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        }

        public getUserTemplates(): ng.IPromise<Template[]> {
            this.$log.debug('Retrieving user templates...');
            var deferred = this.$q.defer<Template[]>();
            if (this.cacheUserTemplates)
                deferred.resolve(this.cacheUserTemplates);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'usertemplates'
                }).then((response) => {
                    this.cacheUserTemplates = response.data as Template[];
                    deferred.resolve(response.data as Template[]);
                    this.$log.debug('Retrieved user templates.', response);
                }, (error) => {
                    this.$log.error('Error retrieving user templates.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        } 

        public getCategories(): ng.IPromise<Category[]> {
            this.$log.debug('Retrieving categories...');
            var deferred = this.$q.defer<Category[]>();
            if (this.cacheCategories)
                deferred.resolve(this.cacheCategories);
            else {
                this.$http({
                    method: 'GET',
                    url: this.baseApi + 'categories'
                }).then((response) => {
                    this.cacheCategories = response.data as Category[];
                    deferred.resolve(response.data as Category[]);
                    this.$log.debug('Retrieved categories.', response);
                }, (error) => {
                    this.$log.error('Error retrieving categories.', error);
                    deferred.reject([]);
                });
            }
            return deferred.promise;
        }

        public getTemplateById(templateId: string): ng.IPromise<Template> {
            this.$log.debug('Retrieving template...', templateId);
            var deferred = this.$q.defer<Template>();
            this.$http({
                method: 'GET',
                url: this.baseApi + 'templates/' + templateId
            }).then((response) => {
                deferred.resolve(response.data as Template);
                this.$log.debug('Retrieved template...', templateId, response);
            }, (error) => {
                this.$log.error('Error retrieving template.', templateId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        } 

        public createUserTemplate(template: Template): ng.IPromise<string> {
            this.$log.debug('Creating new template...', template);
            var deferred = this.$q.defer<string>();
            this.$http({
                method: 'POST',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                //contentType: 'application/json'
            }).then((response) => {
                this.cacheUserTemplates = undefined;
                deferred.resolve(response.data as string);
                this.$log.debug('New template created.', template, response);
            }, (error) => {
                this.$log.error('Error creating template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public editUserTemplate(template: Template): ng.IPromise<boolean> {
            this.$log.debug('Editing user template...', template);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                //contentType: 'application/json'
            }).then((response) => {
                this.cacheUserTemplates = undefined;
                deferred.resolve(response.data as boolean);
                this.$log.debug('User template edited.', template, response);
            }, (error) => {
                this.$log.error('Error editing template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public editUserTemplateReusable(template: Template): ng.IPromise<boolean> {
            this.$log.debug('Editing user reusable template...', template);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'PUT',
                data: JSON.stringify(template),
                url: this.baseApi + 'usertemplates/',
                //contentType: 'application/json'
            }).then((response) => {
                this.cacheCommonTemplates = undefined;
                this.cacheUserTemplates = undefined;
                deferred.resolve(response.data as boolean);
                this.$log.debug('User template edited.', template, response);
            }, (error) => {
                this.$log.error('Error editing template.', template, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }

        public deleteUserTemplate(templateId: string): ng.IPromise<boolean> {
            this.$log.debug('Deleting user template...', templateId);
            var deferred = this.$q.defer<boolean>();
            this.$http({
                method: 'DELETE',
                url: this.baseApi + 'usertemplates/' + templateId,
                //contentType: 'application/json'
            }).then((response) => {
                this.cacheUserTemplates = undefined;
                this.cacheCommonTemplates = undefined;
                deferred.resolve(response.data as boolean);
                this.$log.debug('User template deleted.', templateId, response);
            }, (error) => {
                this.$log.error('Error deleting template.', templateId, error);
                deferred.reject([]);
            });
            return deferred.promise;
        }
    }

    app.service('TemplateService', TemplateService);
} 