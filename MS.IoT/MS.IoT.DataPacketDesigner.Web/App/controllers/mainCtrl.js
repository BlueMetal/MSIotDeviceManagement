var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Main Controller (abstract) used for main notifications
    var MainCtrl = /** @class */ (function () {
        function MainCtrl($scope) {
            var _this = this;
            $scope.$on('notificationsEvent', function (event, data) {
                _this.notifications = data;
            });
            $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                _this.notifications = [];
            });
        }
        ;
        MainCtrl.$inject = ["$scope"];
        return MainCtrl;
    }());
    app.controller('MainCtrl', MainCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=mainCtrl.js.map