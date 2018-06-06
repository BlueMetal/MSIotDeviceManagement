import * as angular from "angular";
import { Subcategory, Category, Template } from "../models/models";
import { IStateTemplateParamsService } from "../interfaces/interfaces";

module msIoT {
    let app = angular.module("msIoT");

    //Controller for the view ChooseTemplate
    class ChooseTemplateCtrl {
        static $inject: Array<string> = ['$stateParams', '$log', 'categories', 'commonTemplates'];

        private $log: ng.ILogService;
        private commonTemplates: Template[];

        public currentCategory: string;
        public subCategories: Subcategory[] = [];
        public selectedSubcategory?: Subcategory = undefined;
        public selectedSubcategoryIndex: number = 0;

        //Main Constructor
        constructor($stateParams: IStateTemplateParamsService, $log: ng.ILogService, categories : Category[], commonTemplates : Template[]) {
            this.$log = $log;
            this.commonTemplates = commonTemplates;
            this.currentCategory = $stateParams.categoryId;
            this.subCategories = this.resolveSubcategories(categories, $stateParams.categoryId);
            if (this.subCategories != null && this.subCategories.length > 0) {
                this.selectSubcategory(0);
            }
        }

        //Event Select a subcategory
        public selectSubcategory(index: number)
        {
            this.selectedSubcategory = this.subCategories[index];
            this.selectedSubcategoryIndex = index;
        }

        //Load subcategories
        private resolveSubcategories(categories: Category[], categoryId: string): Subcategory[] {
            categoryId = categoryId;
            var tempGroups = [];
            for (let i = 0; i < categories.length; i++) {
                if (categories[i].id == categoryId) {
                    let subcategories = categories[i].subcategories;
                    for (let j = 0; j < subcategories.length; j++) {
                        subcategories[j].templates = this.commonTemplates.filter(p => p.categoryId == categoryId && p.subcategoryId == subcategories[j].id);
                    }
                    return subcategories;
                }
            }
            return [];
        }
    }
    app.controller('ChooseTemplateCtrl', ChooseTemplateCtrl);
}