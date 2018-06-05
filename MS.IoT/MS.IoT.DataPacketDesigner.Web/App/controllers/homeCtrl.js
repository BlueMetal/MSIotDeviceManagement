var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Class for the view Home
    var HomeCtrl = /** @class */ (function () {
        //Main Constructor
        function HomeCtrl(currentUser, categories, userTemplates) {
            this.commonCategories = [];
            this.userTemplates = [];
            this.currentUser = currentUser;
            this.commonCategories = categories;
            this.resolveUserTemplates(userTemplates);
        }
        //Load user templates.
        HomeCtrl.prototype.resolveUserTemplates = function (userTemplates) {
            var _loop_1 = function () {
                var userTemplate = userTemplates[i];
                var filterCategory = this_1.commonCategories.filter(function (p) { return p.id == userTemplate.categoryId; });
                if (filterCategory != null && filterCategory.length > 0) {
                    userTemplate.category = filterCategory[0];
                    var filterSubcategory = userTemplate.category.subcategories.filter(function (p) { return p.id == userTemplate.subcategoryId; });
                    if (filterSubcategory != null && filterSubcategory.length > 0)
                        userTemplate.subcategory = filterSubcategory[0];
                }
            };
            var this_1 = this;
            for (var i = 0; i < userTemplates.length; i++) {
                _loop_1();
            }
            this.userTemplates = userTemplates;
        };
        HomeCtrl.$inject = ['currentUser', 'categories', 'userTemplates'];
        return HomeCtrl;
    }());
    app.controller('HomeCtrl', HomeCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=homeCtrl.js.map