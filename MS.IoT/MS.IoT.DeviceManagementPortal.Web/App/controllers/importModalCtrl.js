var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //modal to import devices
    var ImportModalCtrl = (function () {
        function ImportModalCtrl($scope, $uibModalInstance, Upload, $timeout) {
            this.areDevicesImported = true;
            this.$scope = $scope;
            this.$uibModalInstance = $uibModalInstance;
            this.Upload = Upload;
            this.importSuccessResp = false;
            this.$timeout = $timeout;
            //$scope.$watch('file', () => {
            //	this.uploadFile($scope.file,$scope.errFile);
            //});	
        }
        ImportModalCtrl.prototype.uploadFile = function (file, errFile) {
            var _this = this;
            this.areDevicesImported = false;
            var f = file;
            this.errorFile = errFile && errFile[0];
            if (this.errorFile) {
                this.areDevicesImported = true;
            }
            if (file) {
                file.upload = this.Upload.upload({
                    url: 'api/devicetwin/devices/import',
                    data: { file: file }
                });
                file.upload.then(function (response) {
                    _this.importSuccessResp = response.data;
                    _this.areDevicesImported = true;
                }).catch(function (error) {
                    _this.importErrorResp = error.data.Message;
                    _this.areDevicesImported = true;
                });
            }
            else {
                this.areDevicesImported = true;
            }
        };
        ImportModalCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(this.importSuccessResp);
        };
        ImportModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(this.importSuccessResp);
        };
        return ImportModalCtrl;
    }());
    ImportModalCtrl.$inject = ['$scope', '$uibModalInstance', 'Upload', '$timeout'];
    app.controller('ImportModalCtrl', ImportModalCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=importModalCtrl.js.map