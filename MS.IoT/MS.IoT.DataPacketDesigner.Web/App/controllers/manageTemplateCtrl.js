var msIoT;
(function (msIoT) {
    var app = angular.module("msIoT");
    //Controller for the view ManageTemplate
    var ManageTemplateCtrl = /** @class */ (function () {
        //Main Constructor
        function ManageTemplateCtrl(templateService, template, $log, $uibModal, $state, $scope) {
            var _this = this;
            this.managementMode = "create";
            this.validateNbrPropertiesMax = 200;
            this.validateNbrProperties = 0;
            this.isTextEditMode = false;
            this.templateService = templateService;
            this.$log = $log;
            this.$uibModal = $uibModal;
            this.$state = $state;
            this.$scope = $scope;
            if ($state.current.name == 'app.step.createtemplate')
                this.managementMode = "create";
            else if ($state.current.name == 'app.step.managetemplate')
                this.managementMode = "edit";
            this.currentTemplateSaved = template;
            this.resetView();
            $scope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                if (_this.isModified()) {
                    _this.$stateTo = toState;
                    _this.$stateToParams = toParams;
                    event.preventDefault();
                    _this.confirmLeave();
                }
            });
        }
        //Popup to confirm the deletion of a user template
        ManageTemplateCtrl.prototype.confirmDelete = function () {
            var _this = this;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/confirm_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Delete template?"; },
                    content: function () { return "Do you really want to delete this template?"; }
                }
            }).result.then(function (result) {
                if (result) {
                    _this.templateService.deleteUserTemplate(_this.currentTemplateState.id).then(function (result) {
                        _this.$state.go('app.home', {}, { notify: true });
                    }).catch(function (reason) {
                        _this.alertModal('Error', 'An unknown error has occured.', 'error');
                    });
                }
            }, function () { });
        };
        //Popup to confirm exiting the page without saving the changes.
        ManageTemplateCtrl.prototype.confirmLeave = function () {
            var _this = this;
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/confirm_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Cancel the changes?"; },
                    content: function () { return "Changes that were not saved will be lost. Do you really want to leave this page?"; }
                }
            }).result.then(function (result) {
                if (result && _this.$stateTo != null && _this.$stateToParams != null) {
                    _this.resetView();
                    _this.$state.go(_this.$stateTo, _this.$stateToParams);
                }
            }, function () { });
        };
        //Popup alert to display important information
        ManageTemplateCtrl.prototype.alertModal = function (title, content, type) {
            this.$uibModal.open({
                animation: true,
                windowClass: 'modal-' + type,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/alert_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return title; },
                    content: function () { return content; },
                }
            }).result.then(function () { }, function () { });
        };
        //Popup to display a JSON version of the template
        ManageTemplateCtrl.prototype.openJSONModal = function () {
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/json_modal.html',
                controller: 'JsonModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    template: this.currentTemplateState
                }
            }).result.then(function () { }, function () { });
        };
        //Enable/disable edit mode for the title/description of the template
        ManageTemplateCtrl.prototype.toggleTextEdit = function () {
            this.isTextEditMode = !this.isTextEditMode;
        };
        //Compares the current state of the template with the reference template to figure out if the object was changed
        ManageTemplateCtrl.prototype.isModified = function () {
            return !angular.equals(this.currentTemplateState, this.currentTemplateSaved);
        };
        //Reset the template (copy back the reference)
        ManageTemplateCtrl.prototype.resetView = function () {
            //Cloning
            this.currentTemplateState = JSON.parse(JSON.stringify(this.currentTemplateSaved));
        };
        //Save the current changes, call the template api
        ManageTemplateCtrl.prototype.save = function () {
            var _this = this;
            if (!this.validateSave())
                return;
            switch (this.managementMode) {
                case 'create':
                    this.templateService.createUserTemplate(this.currentTemplateState)
                        .then(function (newTemplateId) {
                        if (newTemplateId) {
                            _this.templateService.getTemplateById(newTemplateId).then(function (newTemplate) {
                                _this.currentTemplateSaved = newTemplate;
                                _this.$state.go('app.step.managetemplate', { categoryId: newTemplate.categoryId, templateId: newTemplate.id }, { notify: false });
                                _this.managementMode = "edit";
                                _this.resetView();
                                _this.alertModal('Information', 'Your template was saved. You can know proceed to the simulation.', 'info');
                            });
                        }
                    }).catch(function (result) {
                        _this.alertModal('Error', 'An unknown error has occured.', 'error');
                    });
                    break;
                case 'edit':
                    this.templateService.editUserTemplate(this.currentTemplateState)
                        .then(function (result) {
                        if (result) {
                            _this.resetView();
                            _this.alertModal('Information', 'Your template was updated.', 'info');
                        }
                    }).catch(function (result) {
                        _this.alertModal('Error', 'An unknown error has occured.', 'error');
                    });
                    break;
            }
        };
        //Toggle the option "use as reusable template"
        ManageTemplateCtrl.prototype.useAsReusableTemplate = function () {
            var _this = this;
            //We will update the update the last saved item instead of the currentState item.
            this.currentTemplateSaved.isReusableTemplate = !this.currentTemplateState.isReusableTemplate;
            this.templateService.editUserTemplateReusable(this.currentTemplateSaved)
                .then(function (result) {
                //If success, apply edited isUserTemplate to currentState item
                _this.currentTemplateState.isReusableTemplate = _this.currentTemplateSaved.isReusableTemplate;
            }).catch(function (reason) {
                _this.$log.error('Could not change the property isUserTemplate of this template.', reason);
                _this.alertModal('Error', 'An unknown error has occured.', 'error');
            });
        };
        //Perform a few checks to ensure that the template is in a saveable state:
        //- A title is given
        //- No property of the tree is empty
        ManageTemplateCtrl.prototype.validateSave = function () {
            var notifications = [];
            if (this.currentTemplateState.name == null || this.currentTemplateState.name.length == 0) {
                notifications.push({ Type: 'error', Message: 'Please give a name to your template.' });
            }
            this.validateNbrProperties = 0;
            if (!this.validateProperties(this.currentTemplateState.properties)) {
                notifications.push({ Type: 'error', Message: 'At least one property\'s name is empty. Please make sure that all your properties are named.' });
            }
            if (this.validateNbrProperties > this.validateNbrPropertiesMax) {
                notifications.push({
                    Type: 'error', Message: 'The maximum number of properties allowed in your treeview is \'' + this.validateNbrPropertiesMax + '\'. Your treeview has \'' + this.validateNbrProperties + '\' properties. Please remove some properties and try again.'
                });
            }
            this.$scope.$emit('notificationsEvent', notifications);
            return notifications.length == 0;
        };
        //Recurcive event to check whether a property of the tree is empty
        ManageTemplateCtrl.prototype.validateProperties = function (properties) {
            for (var i = 0; i < properties.length; i++) {
                this.validateNbrProperties++;
                var property = properties[i];
                if (property.name == null || property.name.length == 0)
                    return false;
                if (!this.validateProperties(property.properties))
                    return false;
            }
            return true;
        };
        ManageTemplateCtrl.$inject = ['TemplateService', 'template', '$log', '$uibModal', '$state', '$scope'];
        return ManageTemplateCtrl;
    }());
    app.controller('ManageTemplateCtrl', ManageTemplateCtrl);
})(msIoT || (msIoT = {}));
//# sourceMappingURL=manageTemplateCtrl.js.map