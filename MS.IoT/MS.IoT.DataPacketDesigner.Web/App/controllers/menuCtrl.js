var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Menu controller used to collapse the menu in phone view
    var MenuCtrl = /** @class */ (function () {
        function MenuCtrl() {
            this.isNavCollapsed = true;
        }
        ;
        MenuCtrl.$inject = [];
        return MenuCtrl;
    }());
    app.controller('MenuCtrl', MenuCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=menuCtrl.js.map