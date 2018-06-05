module msIoT {
    let app = angular.module("msIoT");

    //Controller for the view SimulateTemplate
    class SimulateTemplateCtrl {
        static $inject: Array<string> = ['TemplateService', '$stateParams', '$log', '$uibModal', '$state', '$http'];

        private templateService: ITemplateService;
        private $log: ng.ILogService;
        private $http: any;
        private $uibModal: ng.ui.bootstrap.IModalService;
        private $state: ng.ui.IStateService;

        public currentTemplateId: string;

        //Main Controller
        constructor(templateService: ITemplateService, $stateParams: IStateTemplateParamsService, $log: ng.ILogService, $uibModal: ng.ui.bootstrap.IModalService, $state: ng.ui.IStateService, $http: any) {
            this.templateService = templateService;
            this.$log = $log;
            this.$http = $http;
            this.$uibModal = $uibModal;
            this.$state = $state;

            this.currentTemplateId = $stateParams.templateId;
        }

        //Call the endpoint to download the simulator
		public downloadBlobZip(path) {
			this.$log.info("Downloading blob zip -"+path);			
            window.location.href = "/api/blob/download/"+path;
		}
    }
    app.controller('SimulateTemplateCtrl', SimulateTemplateCtrl as any);
}