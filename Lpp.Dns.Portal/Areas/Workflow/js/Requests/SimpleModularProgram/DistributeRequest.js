var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/// <reference path="../../../../../js/requests/details.ts" />
var Workflow;
(function (Workflow) {
    var SimpleModularProgram;
    (function (SimpleModularProgram) {
        var DistributeRequest;
        (function (DistributeRequest) {
            var vm;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, screenPermissions, fieldOptions, uploadViewModel) {
                    var _this = _super.call(this, bindingControl, screenPermissions) || this;
                    _this.UploadViewModel = uploadViewModel;
                    var self = _this;
                    _this.CanSubmit = ko.computed(function () {
                        return self.HasPermission(Permissions.ProjectRequestTypeWorkflowActivities.CloseTask) && Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedDataMartIDs().length > 0;
                    });
                    _this.FieldOptions = fieldOptions;
                    self.IsFieldRequired = function (id) {
                        var options = ko.utils.arrayFirst(self.FieldOptions, function (item) { return item.FieldIdentifier == id; });
                        return options.Permission == Dns.Enums.FieldOptionPermissions.Required;
                    };
                    self.IsFieldVisible = function (id) {
                        var options = ko.utils.arrayFirst(self.FieldOptions, function (item) { return item.FieldIdentifier == id; });
                        return options.Permission != Dns.Enums.FieldOptionPermissions.Hidden;
                    };
                    Requests.Details.rovm.Request.ID.subscribe(function (newVal) {
                        $.when(newVal != null ? Dns.WebApi.Requests.GetCompatibleDataMarts({ TermIDs: [modularProgramTermID], ProjectID: Requests.Details.rovm.Request.ProjectID(), Request: "", RequestID: Requests.Details.rovm.Request.ID() }) : null, newVal != null ? Dns.WebApi.Requests.RequestDataMarts(Requests.Details.rovm.Request.ID()) : null).done(function (datamarts, selectedDataMarts) {
                            var query = (Requests.Details.rovm.Request.Query() == null || Requests.Details.rovm.Request.Query() === '') ? null : JSON.parse(Requests.Details.rovm.Request.Query());
                            var uploadViewModel = Controls.WFFileUpload.Index.init($('#mpupload'), query, modularProgramTermID);
                            self.UploadViewModel = uploadViewModel;
                            Plugins.Requests.QueryBuilder.DataMartRouting.vm.LoadDataMarts(Requests.Details.rovm.Request.ProjectID(), Requests.Details.rovm.Request.Query());
                        });
                    });
                    return _this;
                }
                ViewModel.prototype.PostComplete = function (resultID) {
                    var uploadViewModel = this.UploadViewModel;
                    $.when((uploadViewModel != null && uploadViewModel.DocumentsToDelete().length == 0) ? null : Dns.WebApi.Documents.Delete(ko.utils.arrayMap(uploadViewModel.DocumentsToDelete(), function (d) { return d.ID; }))).done(function () {
                        if (!Requests.Details.rovm.Validate())
                            return;
                        var selectedDataMartIDs = Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedDataMartIDs();
                        if (selectedDataMartIDs.length == 0 && resultID.toUpperCase() != "DFF3000B-B076-4D07-8D83-05EDE3636F4D") {
                            Global.Helpers.ShowAlert('Validation Error', '<div class="alert alert-warning" style="text-align:center;line-height:2em;"><p>A DataMart needs to be selected</p></div>');
                            return;
                        }
                        var selectedDataMarts = Plugins.Requests.QueryBuilder.DataMartRouting.vm.SelectedRoutings(resultID.toUpperCase() != "DFF3000B-B076-4D07-8D83-05EDE3636F4D");
                        var uploadCriteria = uploadViewModel.serializeCriteria();
                        Requests.Details.rovm.Request.Query(uploadCriteria);
                        var AdditionalInstructions = $('#DataMarts_AdditionalInstructions').val();
                        var dto = Requests.Details.rovm.Request.toData();
                        dto.AdditionalInstructions = AdditionalInstructions;
                        Dns.WebApi.Requests.CompleteActivity({
                            DemandActivityResultID: resultID,
                            Dto: dto,
                            DataMarts: selectedDataMarts,
                            Data: JSON.stringify(ko.utils.arrayMap(vm.UploadViewModel.Documents(), function (d) { return d.RevisionSetID; })),
                            Comment: null
                        }).done(function (results) {
                            var result = results[0];
                            if (result.Uri) {
                                Global.Helpers.RedirectTo(result.Uri);
                            }
                            else {
                                //Update the request etc. here 
                                Requests.Details.rovm.Request.ID(result.Entity.ID);
                                Requests.Details.rovm.Request.Timestamp(result.Entity.Timestamp);
                                Requests.Details.rovm.UpdateUrl();
                            }
                        });
                    });
                };
                return ViewModel;
            }(Global.WorkflowActivityViewModel));
            DistributeRequest.ViewModel = ViewModel;
            var modularProgramTermID = 'a1ae0001-e5b4-46d2-9fad-a3d8014fffd8';
            $.when(Requests.Details.rovm.Request.ID() != null ? Dns.WebApi.Requests.RequestDataMarts(Requests.Details.rovm.Request.ID()) : null).done(function (selectedDataMarts) {
                Requests.Details.rovm.SaveRequestID("DFF3000B-B076-4D07-8D83-05EDE3636F4D");
                var query = (Requests.Details.rovm.Request.Query() == null || Requests.Details.rovm.Request.Query() === '') ? null : JSON.parse(Requests.Details.rovm.Request.Query());
                var uploadViewModel = Requests.Details.rovm.Request.ID() != null ? Controls.WFFileUpload.Index.init($('#mpupload'), query, modularProgramTermID) : null;
                Plugins.Requests.QueryBuilder.DataMartRouting.init($('#DataMartsControl'), Requests.Details.rovm.FieldOptions, selectedDataMarts, Requests.Details.rovm.Request.DueDate(), Requests.Details.rovm.Request.Priority(), Requests.Details.rovm.Request.AdditionalInstructions());
                //Bind the view model for the activity
                var bindingControl = $("#MPDistributeRequest");
                vm = new ViewModel(bindingControl, Requests.Details.rovm.ScreenPermissions, Requests.Details.rovm.FieldOptions, uploadViewModel);
                $(function () {
                    ko.applyBindings(vm, bindingControl[0]);
                    Plugins.Requests.QueryBuilder.DataMartRouting.vm.LoadDataMarts(Requests.Details.rovm.Request.ProjectID(), Requests.Details.rovm.Request.Query());
                });
            });
        })(DistributeRequest = SimpleModularProgram.DistributeRequest || (SimpleModularProgram.DistributeRequest = {}));
    })(SimpleModularProgram = Workflow.SimpleModularProgram || (Workflow.SimpleModularProgram = {}));
})(Workflow || (Workflow = {}));
//# sourceMappingURL=DistributeRequest.js.map