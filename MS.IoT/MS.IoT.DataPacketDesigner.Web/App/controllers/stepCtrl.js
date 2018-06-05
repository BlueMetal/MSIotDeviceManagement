var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Step Controller (abstract) used to display the breadcrumb
    var StepCtrl = /** @class */ (function () {
        function StepCtrl($state) {
            this.stepsCreate = [
                {
                    title: "Home",
                    stateView: "app.home"
                },
                {
                    title: "Select Your Template",
                    stateView: "app.step.choosetemplate"
                },
                {
                    title: "Define Your Data Types",
                    stateView: "app.step.createtemplate"
                },
                {
                    title: "Simulate Your Data",
                    stateView: "app.step.simulatetemplate"
                }
            ];
            this.stepsEdit = [
                {
                    title: "Home",
                    stateView: "app.home"
                },
                {
                    title: "Select Your Template",
                    stateView: "app.step.choosetemplate"
                },
                {
                    title: "Define Your Data Types",
                    stateView: "app.step.managetemplate"
                },
                {
                    title: "Simulate Your Data",
                    stateView: "app.step.simulatetemplate"
                }
            ];
            this.steps = this.stepsCreate;
            this.currentState = $state.current;
            this.currentStateParams = $state.params;
            if (this.currentState.name == "app.step.createtemplate")
                this.steps = this.stepsCreate;
            else if (this.currentState.name == "app.step.managetemplate" || this.currentState.name == "app.step.simulatetemplate")
                this.steps = this.stepsEdit;
            this.currentIndex = 0;
            for (var i = 0; i < this.steps.length; i++) {
                if (this.steps[i].stateView == this.currentState.name) {
                    this.currentIndex = i;
                    break;
                }
            }
        }
        ;
        StepCtrl.$inject = ['$state'];
        return StepCtrl;
    }());
    app.controller('StepCtrl', StepCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=stepCtrl.js.map