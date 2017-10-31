/// <reference path="../../../../../js/requests/details.ts" />
module Workflow.SimpleModularProgram.DistributeRequest {
    var vm: ViewModel;

    export class ViewModel extends Global.WorkflowActivityViewModel {
    
        private UploadViewModel: Controls.WFFileUpload.Index.ViewModel;
        private CanSubmit: KnockoutComputed<boolean>;
        public FieldOptions: Dns.Interfaces.IBaseFieldOptionAclDTO[];
        public IsFieldVisible: (id: string) => boolean;
        public IsFieldRequired: (id: string) => boolean;

        constructor(bindingControl: JQuery, screenPermissions: any[], fieldOptions: Dns.Interfaces.IBaseFieldOptionAclDTO[], uploadViewModel: Controls.WFFileUpload.Index.ViewModel) {
            super(bindingControl, screenPermissions);
            this.UploadViewModel = uploadViewModel;            

            var self = this;

            this.CanSubmit = ko.computed(() => {
                return self.HasPermission(Permissions.ProjectRequestTypeWorkflowActivities.CloseTask) && Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedDataMartIDs().length > 0;
            }); 

            this.FieldOptions = fieldOptions;

            self.IsFieldRequired = (id: string) => {
                var options = ko.utils.arrayFirst(self.FieldOptions, (item) => { return item.FieldIdentifier == id; });
                return options.Permission == Dns.Enums.FieldOptionPermissions.Required;
            };

            self.IsFieldVisible = (id: string) => {
                var options = ko.utils.arrayFirst(self.FieldOptions, (item) => { return item.FieldIdentifier == id; });
                return options.Permission != Dns.Enums.FieldOptionPermissions.Hidden;
            };

            Requests.Details.rovm.Request.ID.subscribe(newVal => {
                $.when<any>(
                    newVal != null ? Dns.WebApi.Requests.GetCompatibleDataMarts({ TermIDs: [modularProgramTermID], ProjectID: Requests.Details.rovm.Request.ProjectID(), Request: "", RequestID: Requests.Details.rovm.Request.ID() }) : null,
                    newVal != null ? Dns.WebApi.Requests.RequestDataMarts(Requests.Details.rovm.Request.ID()) : null
                ).done((datamarts: Dns.Interfaces.IDataMartListDTO[], selectedDataMarts: Dns.Interfaces.IRequestDataMartDTO[]) => {

                    let query = (Requests.Details.rovm.Request.Query() == null || Requests.Details.rovm.Request.Query() === '') ? null : JSON.parse(Requests.Details.rovm.Request.Query());
                    let uploadViewModel = Controls.WFFileUpload.Index.init($('#mpupload'), query, modularProgramTermID);
                    self.UploadViewModel = uploadViewModel;

                    Plugins.Requests.QueryBuilder.DataMartRouting.vm.LoadDataMarts(Requests.Details.rovm.Request.ProjectID(), Requests.Details.rovm.Request.Query());
                    
                });

            });
        }

        public PostComplete(resultID: string) {

            let uploadViewModel = this.UploadViewModel;
            $.when<any>(
                (uploadViewModel != null && uploadViewModel.DocumentsToDelete().length == 0) ? null : Dns.WebApi.Documents.Delete(ko.utils.arrayMap(uploadViewModel.DocumentsToDelete(), (d) => { return d.ID; }))
            ).done(() => {
                if (!Requests.Details.rovm.Validate())
                    return;

                let selectedDataMartIDs = Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedDataMartIDs();
                if (selectedDataMartIDs.length == 0 && resultID.toUpperCase() != "DFF3000B-B076-4D07-8D83-05EDE3636F4D") {
                    Global.Helpers.ShowAlert('Validation Error', '<div class="alert alert-warning" style="text-align:center;line-height:2em;"><p>A DataMart needs to be selected</p></div>');
                    return;
                }

                let selectedDataMarts = Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedRoutings(resultID.toUpperCase() != "DFF3000B-B076-4D07-8D83-05EDE3636F4D");

                let uploadCriteria = uploadViewModel.serializeCriteria();
                Requests.Details.rovm.Request.Query(uploadCriteria);
                let AdditionalInstructions = $('#DataMarts_AdditionalInstructions').val()
                let dto = Requests.Details.rovm.Request.toData()
                dto.AdditionalInstructions = AdditionalInstructions;

                Dns.WebApi.Requests.CompleteActivity({
                    DemandActivityResultID: resultID,
                    Dto: dto,
                    DataMarts: selectedDataMarts,
                    Data: JSON.stringify(ko.utils.arrayMap(vm.UploadViewModel.Documents(), (d) => { return d.RevisionSetID; })),
                    Comment: null
                }).done((results) => {
                    let result = results[0];
                    if (result.Uri) {
                        Global.Helpers.RedirectTo(result.Uri);
                    } else {
                        //Update the request etc. here 
                        Requests.Details.rovm.Request.ID(result.Entity.ID);
                        Requests.Details.rovm.Request.Timestamp(result.Entity.Timestamp);
                        Requests.Details.rovm.UpdateUrl();
                    }
                });

            });
            
        }


    }

    var modularProgramTermID = 'a1ae0001-e5b4-46d2-9fad-a3d8014fffd8';
    $.when<any>(
        Requests.Details.rovm.Request.ID() != null ? Dns.WebApi.Requests.RequestDataMarts(Requests.Details.rovm.Request.ID()): null
    ).done((selectedDataMarts) => {

        Requests.Details.rovm.SaveRequestID("DFF3000B-B076-4D07-8D83-05EDE3636F4D");
        var query = (Requests.Details.rovm.Request.Query() == null || Requests.Details.rovm.Request.Query() === '') ? null : JSON.parse(Requests.Details.rovm.Request.Query());
        var uploadViewModel = Requests.Details.rovm.Request.ID() != null ? Controls.WFFileUpload.Index.init($('#mpupload'), query, modularProgramTermID) : null;

        Plugins.Requests.QueryBuilder.DataMartRouting.init($('#DataMartsControl'), Requests.Details.rovm.FieldOptions, selectedDataMarts, Requests.Details.rovm.Request.DueDate(), Requests.Details.rovm.Request.Priority(), Requests.Details.rovm.Request.AdditionalInstructions());

        //Bind the view model for the activity
        var bindingControl = $("#MPDistributeRequest");
        vm = new ViewModel(bindingControl, Requests.Details.rovm.ScreenPermissions, Requests.Details.rovm.FieldOptions, uploadViewModel);

        $(() => {            
            ko.applyBindings(vm, bindingControl[0]);
            Plugins.Requests.QueryBuilder.DataMartRouting.vm.LoadDataMarts(Requests.Details.rovm.Request.ProjectID(), Requests.Details.rovm.Request.Query());         
        });
    });

}