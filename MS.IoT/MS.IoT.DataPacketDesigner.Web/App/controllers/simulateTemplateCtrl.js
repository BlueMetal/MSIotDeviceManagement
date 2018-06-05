var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Controller for the view SimulateTemplate
    var SimulateTemplateCtrl = /** @class */ (function () {
        //Main Controller
        function SimulateTemplateCtrl(templateService, $stateParams, $log, $uibModal, $state, $http) {
            this.templateService = templateService;
            this.$log = $log;
            this.$http = $http;
            this.$uibModal = $uibModal;
            this.$state = $state;
            this.currentTemplateId = $stateParams.templateId;
        }
        //Call the endpoint to download the simulator
        SimulateTemplateCtrl.prototype.downloadBlobZip = function (path) {
            this.$log.info("Downloading blob zip -" + path);
            window.location.href = "/api/blob/download/" + path;
        };
        SimulateTemplateCtrl.$inject = ['TemplateService', '$stateParams', '$log', '$uibModal', '$state', '$http'];
        return SimulateTemplateCtrl;
    }());
    app.controller('SimulateTemplateCtrl', SimulateTemplateCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=simulateTemplateCtrl.js.map