var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Controller for the view ChooseTemplate
    var ChooseTemplateCtrl = /** @class */ (function () {
        //Main Constructor
        function ChooseTemplateCtrl($stateParams, $log, categories, commonTemplates) {
            this.$log = $log;
            this.commonTemplates = commonTemplates;
            this.currentCategory = $stateParams.categoryId;
            this.subCategories = this.resolveSubcategories(categories, $stateParams.categoryId);
            if (this.subCategories != null && this.subCategories.length > 0) {
                this.selectSubcategory(0);
            }
        }
        //Event Select a subcategory
        ChooseTemplateCtrl.prototype.selectSubcategory = function (index) {
            this.selectedSubcategory = this.subCategories[index];
            this.selectedSubcategoryIndex = index;
        };
        //Load subcategories
        ChooseTemplateCtrl.prototype.resolveSubcategories = function (categories, categoryId) {
            categoryId = categoryId;
            var tempGroups = [];
            var _loop_1 = function (i) {
                if (categories[i].id == categoryId) {
                    var subcategories_1 = categories[i].subcategories;
                    var _loop_2 = function (j) {
                        subcategories_1[j].templates = this_1.commonTemplates.filter(function (p) { return p.categoryId == categoryId && p.subcategoryId == subcategories_1[j].id; });
                    };
                    for (var j = 0; j < subcategories_1.length; j++) {
                        _loop_2(j);
                    }
                    return { value: subcategories_1 };
                }
            };
            var this_1 = this;
            for (var i = 0; i < categories.length; i++) {
                var state_1 = _loop_1(i);
                if (typeof state_1 === "object")
                    return state_1.value;
            }
            return [];
        };
        ChooseTemplateCtrl.$inject = ['$stateParams', '$log', 'categories', 'commonTemplates'];
        return ChooseTemplateCtrl;
    }());
    app.controller('ChooseTemplateCtrl', ChooseTemplateCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=chooseTemplateCtrl.js.map