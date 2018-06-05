var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //JSON Modal Controller, used to control the display of the JSON data in the JSON Modal
    var JsonModalCtrl = /** @class */ (function () {
        function JsonModalCtrl($uibModalInstance, template) {
            this.$uibModalInstance = $uibModalInstance;
            this.template = template;
        }
        JsonModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close();
        };
        JsonModalCtrl.$inject = ['$uibModalInstance', 'template'];
        return JsonModalCtrl;
    }());
    app.controller('JsonModalCtrl', JsonModalCtrl);
    //Alert Modal Controller, used to control the display of the data in the alert Modal
    var AlertModalCtrl = /** @class */ (function () {
        function AlertModalCtrl($uibModalInstance, title, content) {
            this.$uibModalInstance = $uibModalInstance;
            this.title = title;
            this.content = content;
        }
        AlertModalCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        AlertModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        AlertModalCtrl.$inject = ['$uibModalInstance', 'title', 'content'];
        return AlertModalCtrl;
    }());
    app.controller('AlertModalCtrl', AlertModalCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=modalCtrl.js.map