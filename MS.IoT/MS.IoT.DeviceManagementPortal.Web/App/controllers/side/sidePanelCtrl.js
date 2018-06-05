var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //side panel controller to display device Twin details
    var SidePanelCtrl = /** @class */ (function () {
        function SidePanelCtrl($uibModalInstance, deviceTwin, deviceId, alerts, editMode, deviceDBService, $scope, $uibModal) {
            var _this = this;
            this.reportedFeatures = [];
            this.desireFeaturesList = [];
            this.features = [];
            this.isFormDirty = false;
            this.$uibModalInstance = $uibModalInstance;
            this.deviceTwin = deviceTwin;
            this.deviceId = deviceId;
            this.$scope = $scope;
            this.$uibModal = $uibModal;
            //set up alert
            if (this.deviceTwin.reported.statusCode != 0 && alerts.length > 0) {
                var alertsFound = alerts.filter(function (p) { return p.code == _this.deviceTwin.reported.statusCode; });
                if (alertsFound.length > 0)
                    this.alertDescription = alertsFound[0].message;
            }
            this.deviceDBService = deviceDBService;
            this.editMode = editMode;
            this.datePopup = false;
            this.dateOptions = {
                formatYear: 'yy',
                maxDate: new Date(2100, 1, 1),
                minDate: new Date(2000, 1, 1),
                startingDay: 1
            };
            this.init();
        }
        SidePanelCtrl.prototype.init = function () {
            var _this = this;
            console.log("Device Twin retrieved");
            this.deviceTwin.tags.shippedDate = new Date(this.deviceTwin.tags.shippedDate);
            this.$scope.$watch(this.deviceTwin.tags, function (newVal, oldVal) {
            }, true);
            if (this.deviceTwin.reported.featuresDefinitions !== null) {
                // convert object to list
                for (var x in this.deviceTwin.desired.features) {
                    this.deviceTwin.desired.features.hasOwnProperty(x) && this.desireFeaturesList.push(this.deviceTwin.desired.features[x]);
                }
                for (var x in this.deviceTwin.reported.featuresDefinitions) {
                    this.deviceTwin.reported.featuresDefinitions.hasOwnProperty(x) && this.reportedFeatures.push(this.deviceTwin.reported.featuresDefinitions[x]);
                }
                this.features.splice(0, this.features.length);
                var needUpdateDesired = false;
                this.reportedFeatures.forEach(function (featureDefinition) {
                    var desiredFeatureStatus = _this.deviceTwin.desired.features[featureDefinition.name];
                    if (desiredFeatureStatus == null)
                        needUpdateDesired = true;
                    var feature = {
                        name: featureDefinition.name,
                        displayName: featureDefinition.displayName,
                        isActivated: desiredFeatureStatus != null ? desiredFeatureStatus : featureDefinition.isActivated,
                    };
                    _this.features.push(feature);
                });
                if (needUpdateDesired) {
                    this.deviceDBService.initializeDeviceTwinDesiredFeatures({ "deviceId": this.deviceTwin.deviceId, "features": this.features }).then(function (response) {
                        console.log("desired features synced");
                    });
                }
            }
        };
        SidePanelCtrl.prototype.openJSONModal = function () {
            this.$uibModal.open({
                animation: true,
                backdrop: false,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/modals/jsonModal.html',
                controller: 'JsonModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    device: this.deviceTwin
                }
            }).result.then(function () { }, function () { });
        };
        SidePanelCtrl.prototype.updateDesiredFeatures = function (feature, isActivated) {
            feature.isActivated = isActivated;
            var desiredFeatureUpdate = {
                "deviceId": this.deviceTwin.deviceId,
                "feature": feature
            };
            this.deviceDBService.updateDeviceTwinDesiredFeature(desiredFeatureUpdate).then(function (response) {
                console.log("desired feature updated");
            });
        };
        SidePanelCtrl.prototype.saveTags = function (formSidePanel) {
            var _this = this;
            this.deviceDBService.updateDeviceSync(this.deviceTwin.deviceId, null, this.deviceTwin.tags).then(function (response) {
                console.log("device twin tags updated");
                _this.editMode = false;
                _this.isFormDirty = false;
                formSidePanel.$setPristine();
            });
        };
        SidePanelCtrl.prototype.editCancel = function (formSidePanel) {
            console.log(formSidePanel);
            if (formSidePanel.$dirty) {
                this.isFormDirty = true;
            }
            this.editMode = false;
        };
        SidePanelCtrl.prototype.noSaveCancel = function (formSidePanel) {
            this.refresh();
            formSidePanel.$setPristine();
            this.isFormDirty = false;
        };
        SidePanelCtrl.prototype.refresh = function () {
            var _this = this;
            this.deviceDBService.getDeviceTwin(this.deviceTwin.deviceId).then(function (response) {
                _this.deviceTwin = response;
                _this.deviceTwin.tags.shippedDate = new Date(_this.deviceTwin.tags.shippedDate);
            });
        };
        SidePanelCtrl.prototype.openDate = function () {
            this.datePopup = true;
        };
        ;
        SidePanelCtrl.prototype.IsConnected = function (reported) {
            if (reported.ipAddress === null)
                return null;
            var devicedateTimeStamp = new Date(reported.heartbeat);
            var currentTimeStamp = new Date();
            this.differenceTime = currentTimeStamp.getTime() - devicedateTimeStamp.getTime();
            var minutes = Math.floor(this.differenceTime / 60000);
            if (minutes < 15)
                return true;
            else
                return false;
        };
        SidePanelCtrl.prototype.cancel = function () {
            this.$uibModalInstance.close(false);
        };
        SidePanelCtrl.prototype.ok = function () {
            this.$uibModalInstance.close(true);
        };
        SidePanelCtrl.$inject = ['$uibModalInstance', 'deviceTwin', 'deviceId', 'alerts', 'editMode', 'DeviceDBService', '$scope', '$uibModal'];
        return SidePanelCtrl;
    }());
    app.controller('SidePanelCtrl', SidePanelCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=sidePanelCtrl.js.map