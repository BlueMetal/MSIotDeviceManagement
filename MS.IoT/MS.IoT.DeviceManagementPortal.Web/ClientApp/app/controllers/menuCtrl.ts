import * as angular from "angular";

module MSIoTDeviceManagementPortal {
    let app = angular.module("MSIoTDeviceManagementPortal");

    //Menu controller used to collapse the menu in phone view
    class MenuCtrl {
		static $inject: Array<string> = ["$uibModal","$state"];
        public isNavCollapsed: boolean;

		private $uibModal: ng.ui.bootstrap.IModalService;
		public $state: any;
		public active: string;

		constructor($uibModal: ng.ui.bootstrap.IModalService,$state:any) {
			this.isNavCollapsed = true;
			this.$uibModal = $uibModal;
			this.$state = $state;
			this.active = "menuactive";
        };

		public importPanel() {
			this.$uibModal.open({
				animation: true,
				ariaLabelledBy: 'modal-title',
				ariaDescribedBy: 'modal-body',
                template: require('../templates/modals/importModal.html'),
				controller: 'ImportModalCtrl',
				controllerAs: 'vm',
				resolve: {
					//template: this.currentTemplateState
				}
			}).result.then((areDevicesImported) => {
				console.log("modal left");
				if (areDevicesImported){
					this.$state.reload();
				}
			}, function () {
				console.log("modal left another");
			});
		}
    }

    app.controller('MenuCtrl', MenuCtrl as any);
}
