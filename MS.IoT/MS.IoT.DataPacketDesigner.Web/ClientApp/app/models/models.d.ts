declare module msIoTModules {
        export type CustomPropertyType = 'text' | 'boolean' | 'number' | 'date' | 'list' | 'object';
        export type TemplateType = 'common' | 'user' | 'category';
        export type NotificationType = 'info' | 'warning' | 'error';

        export type CustomPropertyTypeOption = {
            id: CustomPropertyType,
            name: string
        }

        export type Template = {
            id: string;
            name: string;
            description?: string;
            userId?: string;
            categoryId: string;
            subcategoryId: string;
            category?: Category;
            subcategory?: Subcategory;
            docType: TemplateType;
            creationDate?: string;
            modifiedDate?: string;
            isReusableTemplate?: boolean;
            baseTemplateId?: string;
            properties: CustomProperty[];
        }

        export type User = {
            readonly id: string;
            readonly name: string;
        }

        export type Category = {
            id: string;
            name: string;
            description: string;
            color: string;
            readonly docType: TemplateType;
            subcategories: Subcategory[];
        }

        export type Subcategory = {
            id: string;
            name: string;
            description: string;
            templates?: Template[];
        }

        export type CustomProperty = {
            name: string;
            type: CustomPropertyType;
            properties: CustomProperty[];
        }

        export type NotificationMessage = {
            Message: string;
            Type: NotificationType
        }
}

export = msIoTModules;