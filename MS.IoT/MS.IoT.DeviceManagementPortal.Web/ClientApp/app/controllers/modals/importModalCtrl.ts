import * as angular from "angular";

module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

    //modal to import devices

	class ImportModalCtrl {
		static $inject: Array<string> = ['$scope','$uibModalInstance', 'Upload', '$timeout'];

		public $scope: any;
		public Upload: any;
		public $timeout: any;
		public progress: any;
		public errorFile: any;
		public importSuccessResp: any;
		public importErrorResp: any;

		public areDevicesImported =true;
		
        private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance;

		constructor($scope: ng.IScope, $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, Upload: any, $timeout: any) {
			this.$scope = $scope;
			this.$uibModalInstance = $uibModalInstance;
			this.Upload = Upload;
			this.importSuccessResp = false;
			this.$timeout = $timeout;

			//$scope.$watch('file', () => {
			//	this.uploadFile($scope.file,$scope.errFile);
			//});	
        }

		public uploadFile(file : any, errFile : any) {
			this.areDevicesImported = false;
			var f = file;
			this.errorFile = errFile && errFile[0];
			if (this.errorFile)
			{
				this.areDevicesImported = true;
			}
			if (file) {
				file.upload = this.Upload.upload({
					url: 'api/devicetwin/devices/import',
					data: { file: file }
				})

				file.upload.then((response: any) => {
					this.importSuccessResp = response.data;
					this.areDevicesImported = true;
				}).catch((error: any) => {
					this.importErrorResp = error.data.Message;
					this.areDevicesImported = true;
				});
			}
			else
			{
				this.areDevicesImported = true;
			}
		}	
		
        public cancel() {
			this.$uibModalInstance.close(this.importSuccessResp);
        }

		public ok() {
			this.$uibModalInstance.close(this.importSuccessResp);			
        }
    }
    app.controller('ImportModalCtrl', ImportModalCtrl);
}
