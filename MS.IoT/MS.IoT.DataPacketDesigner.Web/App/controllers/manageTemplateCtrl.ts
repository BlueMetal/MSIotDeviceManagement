module msIoT {
    let app = angular.module("msIoT");

    type ManagementModes = 'create' | 'edit';
    type ModalType = 'info' | 'error';

    //Controller for the view ManageTemplate
    class ManageTemplateCtrl {
        static $inject: Array<string> = ['TemplateService', 'template', '$log', '$uibModal', '$state', '$scope'];
        
        private templateService: ITemplateService;
        private $log: ng.ILogService;
        private $uibModal: ng.ui.bootstrap.IModalService;
        private $state: ng.ui.IStateService;
        private $scope: ng.IScope;

        private $stateTo;
        private $stateToParams;

        private managementMode: ManagementModes = "create";
        private validateNbrPropertiesMax: number = 200;
        private validateNbrProperties: number = 0;

        public currentTemplateSaved : Template
        public currentTemplateState: Template;
        public isTextEditMode: boolean = false;

        //Main Constructor
        constructor(templateService: ITemplateService, template: Template, $log: ng.ILogService, $uibModal: ng.ui.bootstrap.IModalService, $state: ng.ui.IStateService, $scope : ng.IRootScopeService) {
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

            $scope.$on('$stateChangeStart',
                (event, toState, toParams, fromState, fromParams) => {
                    if (this.isModified()) {
                        this.$stateTo = toState;
                        this.$stateToParams = toParams;
                        event.preventDefault();
                        this.confirmLeave();
                    }
                })
        }

        //Popup to confirm the deletion of a user template
        public confirmDelete() {
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/confirm_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: function () { return "Delete template?" },
                    content: function () { return "Do you really want to delete this template?" }
                }
            }).result.then((result) => {
                if (result) {
                    this.templateService.deleteUserTemplate(this.currentTemplateState.id).then((result) => {
                        this.$state.go('app.home', {}, { notify: true })
                    }).catch((reason) => {
                        this.alertModal('Error', 'An unknown error has occured.', 'error');
                    });
                }
            }, () => { });
        }

        //Popup to confirm exiting the page without saving the changes.
        public confirmLeave() {
            this.$uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/confirm_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: () => { return "Cancel the changes?" },
                    content: () => { return "Changes that were not saved will be lost. Do you really want to leave this page?" }
                }
            }).result.then((result) => {
                if (result && this.$stateTo != null && this.$stateToParams != null) {
                    this.resetView();
                    this.$state.go(this.$stateTo, this.$stateToParams);
                }
            }, () => { });
        }

        //Popup alert to display important information
        public alertModal(title: string, content: string, type: ModalType) {
            this.$uibModal.open({
                animation: true,
                windowClass: 'modal-' + type,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'App/templates/alert_modal.html',
                controller: 'AlertModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    title: () => { return title },
                    content: () => { return content },
                }
            }).result.then(() => { }, () => { });
        }

        //Popup to display a JSON version of the template
        public openJSONModal() {
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
            }).result.then(() => { }, () => { });
        }

        //Enable/disable edit mode for the title/description of the template
        public toggleTextEdit() {
            this.isTextEditMode = !this.isTextEditMode;
        }

        //Compares the current state of the template with the reference template to figure out if the object was changed
        public isModified() : boolean
        {
            return !angular.equals(this.currentTemplateState, this.currentTemplateSaved);
        }

        //Reset the template (copy back the reference)
        public resetView()
        {
            //Cloning
            this.currentTemplateState = JSON.parse(JSON.stringify(this.currentTemplateSaved));
        }

        //Save the current changes, call the template api
        public save()
        {
            if (!this.validateSave())
                return;

            switch (this.managementMode)
            {
                case 'create':
                    this.templateService.createUserTemplate(this.currentTemplateState)
                        .then((newTemplateId: string) => {
                            if (newTemplateId) {
                                this.templateService.getTemplateById(newTemplateId).then((newTemplate: Template) => {
                                    this.currentTemplateSaved = newTemplate;
                                    this.$state.go('app.step.managetemplate', { categoryId: newTemplate.categoryId, templateId: newTemplate.id }, { notify: false })
                                    this.managementMode = "edit";
                                    this.resetView();
                                    this.alertModal('Information', 'Your template was saved. You can know proceed to the simulation.', 'info');
                                });
                            }
                        }).catch((result) => {
                            this.alertModal('Error', 'An unknown error has occured.', 'error');
                        });
                    break;
                case 'edit':
                    this.templateService.editUserTemplate(this.currentTemplateState)
                        .then((result: boolean) => {
                            if (result) {
                                this.resetView();
                                this.alertModal('Information', 'Your template was updated.', 'info');
                            }
                        }).catch((result) => {
                            this.alertModal('Error', 'An unknown error has occured.', 'error');
                        });
                    break;
            }
        }

        //Toggle the option "use as reusable template"
        public useAsReusableTemplate()
        {
            //We will update the update the last saved item instead of the currentState item.
            this.currentTemplateSaved.isReusableTemplate = !this.currentTemplateState.isReusableTemplate;
            this.templateService.editUserTemplateReusable(this.currentTemplateSaved)
                .then((result: boolean) => {
                    //If success, apply edited isUserTemplate to currentState item
                    this.currentTemplateState.isReusableTemplate = this.currentTemplateSaved.isReusableTemplate;
                }).catch((reason: any) => {
                    this.$log.error('Could not change the property isUserTemplate of this template.', reason);
                    this.alertModal('Error', 'An unknown error has occured.', 'error');
                });
        }

        //Perform a few checks to ensure that the template is in a saveable state:
        //- A title is given
        //- No property of the tree is empty
        private validateSave(): boolean
        {
            let notifications: NotificationMessage[] = [];

            if (this.currentTemplateState.name == null || this.currentTemplateState.name.length == 0)
            {
                notifications.push({ Type: 'error', Message: 'Please give a name to your template.' });
            }

            this.validateNbrProperties = 0;
            if (!this.validateProperties(this.currentTemplateState.properties)) {
                notifications.push({ Type: 'error', Message: 'At least one property\'s name is empty. Please make sure that all your properties are named.' });
            }

            if (this.validateNbrProperties > this.validateNbrPropertiesMax) {
                notifications.push({
                    Type: 'error', Message: 'The maximum number of properties allowed in your treeview is \'' + this.validateNbrPropertiesMax + '\'. Your treeview has \'' + this.validateNbrProperties + '\' properties. Please remove some properties and try again.'});
            }

            this.$scope.$emit('notificationsEvent', notifications);

            return notifications.length == 0;
        }

        //Recurcive event to check whether a property of the tree is empty
        private validateProperties(properties: CustomProperty[]): boolean
        {
            for (var i = 0; i < properties.length; i++)
            {
                this.validateNbrProperties++;
                let property = properties[i];
                if (property.name == null || property.name.length == 0)
                    return false;
                if (!this.validateProperties(property.properties))
                    return false;
            }
            return true;
        }
    }
    app.controller('ManageTemplateCtrl', ManageTemplateCtrl as any);
}