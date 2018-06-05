module MSIoTDeviceManagementPortal {
	let app = angular.module("MSIoTDeviceManagementPortal");

    //JSON Modal Controller, used to control the display of the JSON data in the JSON Modal
    class JsonModalCtrl {
        static $inject: Array<string> = ['$uibModalInstance', 'device'];

        private $uibModalInstance: any;
        private device: DeviceTwin;

        constructor($uibModalInstance, device: DeviceTwin) {
            this.$uibModalInstance = $uibModalInstance;
            this.device = device;
        }

        public ok() {
            this.$uibModalInstance.close();
        }
    }
    app.controller('JsonModalCtrl', JsonModalCtrl as any);
}
