var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //modal to confirm delete devices
    var PublishFeatureModalCtrl = (function () {
        function PublishFeatureModalCtrl($uibModalInstance, title, content, selectedDevices, deviceDBService, form) {
            this.isPublishClicked = false;
            this.isError = "";
            this.formSubmitted = false;
            this.$uibModalInstance = $uibModalInstance;
            this.title = title;
            this.content = content;
            this.selectedDevices = selectedDevices;
            this.deviceDBService = deviceDBService;
            this.formPublish = form;
            this.features = [
                { "name": "Brew Strength", "value": "brewStrengthFeature", "methodName": "changeBrewStrength" },
                { "name": "Brew", "value": "brewFeature", "methodName": "launchBrew" },
                { "name": "Grind and Brew", "value": "grindAndBrewFeature", "methodName": "launchGrindAndBrew" }
            ];
        }
        PublishFeatureModalCtrl.prototype.publish = function (form) {
            var _this = this;
            this.formSubmitted = true;
            if (form.$valid) {
                this.isPublishClicked = true;
                var publishFeaturemodel = {
                    "deviceIdList": this.selectedDevices,
                    "featureName": this.selectedFeature.name,
                    "id": this.selectedFeature.value,
                    "featureDescription": this.featureDescription,
                    "methodName": this.selectedFeature.methodName,
                    "notificationType": "Feature"
                };
                this.deviceDBService.publishFeature(publishFeaturemodel).then(function (response) {
                    console.log("publish feature notification sent");
                    _this.featureResponse = "Published feature successfully";
                    _this.isPublishClicked = false;
                    _this.isError = "successText";
                }).catch(function (error) {
                    _this.featureResponse = "Error publishing Feature";
                    _this.isPublishClicked = false;
                    _this.isError = "errorText";
                });
            }
        };
        PublishFeatureModalCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        PublishFeatureModalCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        return PublishFeatureModalCtrl;
    }());
    PublishFeatureModalCtrl.$inject = ['$uibModalInstance', 'title', 'content', 'selectedDevices',
        'DeviceDBService'];
    app.controller('PublishFeatureModalCtrl', PublishFeatureModalCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=publishFeatureModalCtrl.js.map