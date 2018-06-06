import * as angular from "angular";
import { Template, Category } from "../models/models";
import { ITemplateService } from "../interfaces/interfaces";

module msIoT {
    let app = angular.module("msIoT")

    export class MockTemplateService implements ITemplateService {
        static $inject: string[] = ['$q', '$log'];
        private readonly baseApi: string = 'api/templates/';

        private $q: ng.IQService;
        private $log: ng.ILogService;

        private cacheCommonTemplates?: any[] = undefined;
        private cacheCategories?: any[] = undefined;
        private cacheUserTemplates?: any[] = undefined;

        constructor($q: ng.IQService, $log : ng.ILogService) {
            this.$q = $q;
            this.$log = $log;
        }

        public getCommonTemplates(): ng.IPromise<Template[]> {
            this.$log.debug('Retrieving common templates...');
            var deferred = this.$q.defer<Template[]>();
            if (this.cacheCommonTemplates)
                deferred.resolve(this.cacheCommonTemplates);
            else {
                this.cacheCommonTemplates = this.mockTemplate.filter(p => p.docType == "common" || (p.docType == "user" && p.userId == "mockUser"));
                deferred.resolve(this.cacheCommonTemplates);
            }
            return deferred.promise;
        }

        public getUserTemplates(): ng.IPromise<Template[]> {
            this.$log.debug('Retrieving user templates...');
            var deferred = this.$q.defer<Template[]>();
            if (this.cacheUserTemplates)
                deferred.resolve(this.cacheUserTemplates);
            else {
                this.cacheUserTemplates = this.mockTemplate.filter(p => p.docType == "user" && p.userId == "mockUser");
                deferred.resolve(this.cacheUserTemplates);
            }
            return deferred.promise;
        } 

        public getCategories(): ng.IPromise<Category[]> {
            this.$log.debug('Retrieving categories...');
            var deferred = this.$q.defer<Category[]>();
            if (this.cacheCategories)
                deferred.resolve(this.cacheCategories);
            else {
                this.cacheCategories = this.mockCategory;
                deferred.resolve(this.mockCategory);
            }
            return deferred.promise;
        }

        public getTemplateById(templateId: string): ng.IPromise<Template> {
            this.$log.debug('Retrieving template...', templateId);
            var deferred = this.$q.defer<Template>();
            var template = this.mockTemplate.filter(p => p.id == templateId)[0];
            deferred.resolve(template);
            return deferred.promise;
        } 

        public createUserTemplate(template: Template): ng.IPromise<string> {
            this.$log.debug('Creating new template...', template);
            var deferred = this.$q.defer<string>();
            template.baseTemplateId = template.id;
            template.id = Math.random().toString();
            template.docType = "user";
            template.userId = "mockUser";
            this.mockTemplate.push(template);
            deferred.resolve(template.id);
            return deferred.promise;
        }

        public editUserTemplate(template: Template): ng.IPromise<boolean> {
            this.$log.debug('Editing user template...', template);
            var deferred = this.$q.defer<boolean>();
            template.docType = "user";
            template.userId = "mockUser";

            deferred.resolve(true);

            return deferred.promise;
        }

        public editUserTemplateReusable(template: Template): ng.IPromise<boolean> {
            this.$log.debug('Editing user reusable template...', template);
            var deferred = this.$q.defer<boolean>();
            this.cacheCommonTemplates = undefined;
            this.cacheUserTemplates = undefined;
            template.docType = "user";
            template.userId = "mockUser";

            deferred.resolve(true);
                
                
            return deferred.promise;
        }

        public deleteUserTemplate(templateId: string): ng.IPromise<boolean> {
            this.$log.debug('Deleting user template...', templateId);
            var deferred = this.$q.defer<boolean>();
            /*this.$http({
                method: 'DELETE',
                url: this.baseApi + 'usertemplates/' + templateId,
                contentType: 'application/json'
            }).then((response) => {
                this.cacheUserTemplates = null;
                this.cacheCommonTemplates = null;
                deferred.resolve(response.data);
                this.$log.debug('User template deleted.', templateId, response);
            }, (error) => {
                this.$log.error('Error deleting template.', templateId, error);
                deferred.reject([]);
            });*/
            deferred.resolve(true);
            return deferred.promise;
        }

        private mockCategory : Category[] = [
            {
                "id": "refrigeration",
                "name": "Refrigeration",
                "description": "Description for Refrigeration",
                "color": "#0070C0",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "refrigerators",
                        "name": "Refrigerators",
                        "description": "Description for Refrigerators"
                    },
                    {
                        "id": "freezers",
                        "name": "Freezers",
                        "description": "Description for Freezers"
                    },
                    {
                        "id": "watercooler",
                        "name": "Water Cooler",
                        "description": "Description for Water Cooler"
                    }
                ]
            },
            {
                "id": "kitchen",
                "name": "Kitchen",
                "description": "Description for Kitchen",
                "color": "#FF5800",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "stoves",
                        "name": "Stoves",
                        "description": "Description for Stoves"
                    },
                    {
                        "id": "ovens",
                        "name": "Ovens",
                        "description": "Description for Ovens"
                    },
                    {
                        "id": "dishwashers",
                        "name": "Dishwashers",
                        "description": "Description for Dishwashers"
                    }
                ]
            },
            {
                "id": "lighting",
                "name": "Lighting",
                "description": "Description for Lighting",
                "color": "#F5A623",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "bulbs",
                        "name": "Bulbs",
                        "description": "Description for Bulbs"
                    }
                ]
            },
            {
                "id": "laundry",
                "name": "Laundry",
                "description": "Description for Laundry",
                "color": "#CF0115",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "washing_machines",
                        "name": "Washing machines",
                        "description": "Description for Washing machines"
                    },
                    {
                        "id": "dryers",
                        "name": "Dryers",
                        "description": "Description for Dryers"
                    }
                ]
            },
            {
                "id": "heating",
                "name": "Heating",
                "description": "Description for Heating",
                "color": "#00B0F0",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "air_conditioners",
                        "name": "Air Conditioners",
                        "description": "Description for Air Conditioners"
                    }
                ]
            },
            {
                "id": "miscellaneous",
                "name": "Miscellaneous",
                "description": "Description for Miscellaneous",
                "color": "#99A900",
                "docType": "category",
                "subcategories": [
                    {
                        "id": "cars",
                        "name": "Cars",
                        "description": "Description for Car"
                    }
                ]
            }
        ];

        private mockTemplate : Template[] = [
            {
                "id": "refrigerators_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "refrigerators",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "freezers_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "freezers",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "watercooler_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "watercooler",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "stoves_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "stoves",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "ovens_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "ovens",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "dishwashers_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "dishwashers",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "bulbs_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "lighting",
                "subcategoryId": "bulbs",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "washing_machines_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "laundry",
                "subcategoryId": "washing_machines",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "dryers_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "laundry",
                "subcategoryId": "dryers",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "air_conditioners_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "heating",
                "subcategoryId": "air_conditioners",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "cars_cyo",
                "name": "Create your own",
                "description": "Create your own template",
                "userId": undefined,
                "categoryId": "miscellaneous",
                "subcategoryId": "cars",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "property1",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "refrigerator_simple",
                "name": "Refrigerator Simple",
                "description": "Simple refrigerator with one temperature probe.",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "refrigerators",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "refrigerator_smart",
                "name": "Refrigerator Smart",
                "description": "Refrigerator template with multiple temperature probes, programs and more.",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "refrigerators",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperatures",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "probe1",
                                        "type": "number",
                                        "properties": []
                                    },
                                    {
                                        "name": "probe2",
                                        "type": "number",
                                        "properties": []
                                    },
                                    {
                                        "name": "probe3",
                                        "type": "number",
                                        "properties": []
                                    }
                                ]
                            },
                            {
                                "name": "water_tank",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "electric_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "noise",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "cooling_program",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "freezer_simple",
                "name": "Freezer Simple",
                "description": "Simple freezer with temperature probes.",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "freezers",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "watercooler_simple",
                "name": "Watercooler Simple",
                "description": "Water Cooling with temperature probes.",
                "userId": undefined,
                "categoryId": "refrigeration",
                "subcategoryId": "watercooler",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "stove_electric",
                "name": "Stove Electric",
                "description": "Stove working with electricity.",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "stoves",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_min",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_max",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "electric_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "stove_gas",
                "name": "Stove Gas",
                "description": "Stove working with gas.",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "stoves",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_min",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_max",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "gas_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "oven_electric",
                "name": "Stove Electric",
                "description": "Stove working with electricity.",
                "userId": undefined,
                "categoryId": "kitchen",
                "subcategoryId": "ovens",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_min",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_max",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "electric_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "light_simple",
                "name": "Light hue",
                "description": "Light.",
                "userId": undefined,
                "categoryId": "lighting",
                "subcategoryId": "bulbs",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "light_rgb",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "light_intensity",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "washing_machine_simple",
                "name": "Washing machine Simple",
                "description": "Water, Temperature",
                "userId": undefined,
                "categoryId": "laundry",
                "subcategoryId": "washing_machines",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "water_temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "drying_machine_simple",
                "name": "Drying machine Simple",
                "description": "Temperature",
                "userId": undefined,
                "categoryId": "laundry",
                "subcategoryId": "dryers",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "laundry_weight",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "ac_simple",
                "name": "Air Conditioner Simple",
                "description": "Temperature",
                "userId": undefined,
                "categoryId": "heating",
                "subcategoryId": "air_conditioners",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "car_smart",
                "name": "Car",
                "description": "Car Location",
                "userId": undefined,
                "categoryId": "miscellaneous",
                "subcategoryId": "cars",
                "docType": "common",
                "baseTemplateId": undefined,
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "gps_value",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "user_stove_electric",
                "name": "User Stove",
                "description": "Stove working with electricity.",
                "userId": "mockUser",
                "categoryId": "kitchen",
                "subcategoryId": "stoves",
                "docType": "user",
                "isReusableTemplate": true,
                "baseTemplateId": "stove_electric",
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperature",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_min",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "temperature_max",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "electric_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "running_program",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "time_till_finished",
                                "type": "number",
                                "properties": []
                            }
                        ]
                    }
                ]
            },
            {
                "id": "user_refrigerator_smart",
                "name": "Refrigerator Smart",
                "description": "Refrigerator template with multiple temperature probes, programs and more.",
                "userId": "mockUser",
                "categoryId": "refrigeration",
                "subcategoryId": "refrigerators",
                "docType": "user",
                "baseTemplateId": "refrigerator_smart",
                "creationDate": "2017-01-01T00:00:00.000000-00:00",
                "modifiedDate": "2017-01-01T00:00:00.000000-00:00",
                "properties": [
                    {
                        "name": "header",
                        "type": "object",
                        "properties": [
                            {
                                "name": "message_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "device_id",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "date",
                                "type": "date",
                                "properties": []
                            },
                            {
                                "name": "model",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "brand",
                                "type": "text",
                                "properties": []
                            },
                            {
                                "name": "location",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "address",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "city",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "zipcode",
                                        "type": "text",
                                        "properties": []
                                    },
                                    {
                                        "name": "country",
                                        "type": "text",
                                        "properties": []
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "name": "body",
                        "type": "object",
                        "properties": [
                            {
                                "name": "temperatures",
                                "type": "object",
                                "properties": [
                                    {
                                        "name": "probe1",
                                        "type": "number",
                                        "properties": []
                                    },
                                    {
                                        "name": "probe2",
                                        "type": "number",
                                        "properties": []
                                    },
                                    {
                                        "name": "probe3",
                                        "type": "number",
                                        "properties": []
                                    }
                                ]
                            },
                            {
                                "name": "water_tank",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "electric_consumption",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "noise",
                                "type": "number",
                                "properties": []
                            },
                            {
                                "name": "cooling_program",
                                "type": "text",
                                "properties": []
                            }
                        ]
                    }
                ]
            }
        ];
    }

    app.service('MockTemplateService', MockTemplateService);
} 