import * as angular from "angular";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

    //Modal to confirm delete devices/group
	class ConfirmModalCtrl {
		static $inject: Array<string> = ['$uibModalInstance', 'title', 'content'];

		private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;
		private title: string;
		private content: string;

		constructor($uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, title: string, content: string) {
			this.$uibModalInstance = $uibModalInstance;
			this.title = title;
			this.content = content;
		}

		public cancel() {
			this.$uibModalInstance.close(false);
		}

		public ok() {
			this.$uibModalInstance.close(true);
		}
	}
    app.controller('ConfirmModalCtrl', ConfirmModalCtrl);
}
