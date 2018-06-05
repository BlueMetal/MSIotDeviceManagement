module msIoT {
    export interface ITemplateService {
        getCommonTemplates(): ng.IPromise<Template[]>; 
        getUserTemplates(): ng.IPromise<Template[]>; 
        getCategories(): ng.IPromise<Category[]>; 
        getTemplateById(templateid: string): ng.IPromise<Template>; 
        createUserTemplate(template: Template): ng.IPromise<string>;
        editUserTemplate(template: Template): ng.IPromise<boolean>;
        editUserTemplateReusable(template: Template): ng.IPromise<boolean>;
        deleteUserTemplate(templateid: string): ng.IPromise<boolean>;
    }

    export interface IUserService {
        getCurrentUser(): ng.IPromise<User>;
    }

    export interface IStateTemplateParamsService extends ng.ui.IStateParamsService {
        templateId: string;
        categoryId: string;
    }
} 