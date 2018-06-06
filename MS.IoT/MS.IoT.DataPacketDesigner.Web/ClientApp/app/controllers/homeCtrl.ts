import * as angular from "angular";
import { Category, Template, User } from "../models/models";

module msIoT {
    let app = angular.module("msIoT");

    //Class for the view Home
    class HomeCtrl {
        static $inject: Array<string> = ['currentUser', 'categories', 'userTemplates'];

        public commonCategories: Category[] = [];
        public userTemplates: Template[] = [];
        public currentUser: User;

        //Main Constructor
        constructor(currentUser: User, categories: Category[], userTemplates: Template[]) {
            this.currentUser = currentUser;
            this.commonCategories = categories;
            this.resolveUserTemplates(userTemplates);
        }

        //Load user templates.
        private resolveUserTemplates(userTemplates: Template[])
        {
            for (var i = 0; i < userTemplates.length; i++) {
                let userTemplate = userTemplates[i];
                let filterCategory = this.commonCategories.filter(p => p.id == userTemplate.categoryId);
                if (filterCategory != null && filterCategory.length > 0) {
                    userTemplate.category = filterCategory[0];

                    let filterSubcategory = userTemplate.category.subcategories.filter(p => p.id == userTemplate.subcategoryId);
                    if (filterSubcategory != null && filterSubcategory.length > 0)
                        userTemplate.subcategory = filterSubcategory[0];
                }
            }
            this.userTemplates = userTemplates;
        }
    }

    app.controller('HomeCtrl', HomeCtrl);
}