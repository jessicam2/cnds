/// <reference path="common.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CNDS;
(function (CNDS) {
    var Search;
    (function (Search) {
        var CreateRequest;
        (function (CreateRequest) {
            var vm;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(request, requestTypeDetails, requesterCenterList, workplanList, reportAggregationLevelsList, activityTree, fieldOptions, allowEditRequestID, bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    _this.Request = request;
                    _this.ProjectName = requestTypeDetails.Project;
                    _this.RequestTypeName = requestTypeDetails.RequestType;
                    _this.FieldOptions = fieldOptions;
                    _this.RequestName = ko.observable(_this.Request.Name());
                    _this.DueDate = ko.observable(_this.Request.DueDate());
                    _this.Priority = ko.observable(_this.Request.Priority());
                    _this.Priorities = new Array({ Name: 'Low', Value: 0 }, { Name: 'Medium', Value: 1 }, { Name: 'High', Value: 2 }, { Name: 'Urgent', Value: 3 });
                    _this.ProjectID = ko.observable(self.Request.ProjectID());
                    _this.PurposeOfUse = ko.observable(self.Request.PurposeOfUse());
                    _this.PurposeOfUseOptions = new Array({ Name: 'Clinical Trial Research', Value: 'CLINTRCH' }, { Name: 'Healthcare Payment', Value: 'HPAYMT' }, { Name: 'Healthcare Operations', Value: 'HOPERAT' }, { Name: 'Healthcare Research', Value: 'HRESCH' }, { Name: 'Healthcare Marketing', Value: 'HMARKT' }, { Name: 'Observational Research', Value: 'OBSRCH' }, { Name: 'Patient Requested', Value: 'PATRQT' }, { Name: 'Public Health', Value: 'PUBHLTH' }, { Name: 'Prep-to-Research', Value: 'PTR' }, { Name: 'Quality Assurance', Value: 'QA' }, { Name: 'Treatment', Value: 'TREAT' });
                    _this.PhiDisclosureLevel = ko.observable(self.Request.PhiDisclosureLevel());
                    _this.PhiDisclosureLevelOptions = new Array({ Name: 'Aggregated', Value: 'Aggregated' }, { Name: 'Limited', Value: 'Limited' }, { Name: 'De-identified', Value: 'De-identified' }, { Name: 'PHI', Value: 'PHI' });
                    _this.RequesterCenterID = ko.observable(self.Request.RequesterCenterID());
                    _this.RequesterCenters = requesterCenterList;
                    _this.RequesterCentersList = requesterCenterList;
                    _this.WorkplanTypeID = ko.observable(self.Request.WorkplanTypeID());
                    _this.WorkplanTypes = workplanList;
                    _this.WorkplanTypesList = workplanList;
                    _this.ReportAggregationLevelID = ko.observable(self.Request.ReportAggregationLevelID());
                    _this.ReportAggregationLevels = reportAggregationLevelsList.filter(function (ral) { return ((ral.DeletedOn == undefined) || (ral.DeletedOn == null)); });
                    _this.MSRequestID = ko.observable(self.Request.MSRequestID());
                    _this.EditRequestIDAllowed = allowEditRequestID;
                    _this.Description = ko.observable(self.Request.Description());
                    _this.ProjectActivityTree = activityTree;
                    _this.SourceTaskOrderID = ko.observable(null);
                    _this.SourceActivityID = ko.observable(null);
                    _this.SourceActivityProjectID = ko.observable(null);
                    _this.BudgetTaskOrderID = ko.observable(null);
                    _this.BudgetActivityProjectID = ko.observable(null);
                    _this.BudgetActivityID = ko.observable(null);
                    _this.findActivity = function (id) {
                        if (id == null)
                            return null;
                        for (var i = 0; i < self.ProjectActivityTree.length; i++) {
                            var act = self.ProjectActivityTree[i];
                            if (act.ID == id) {
                                return act;
                            }
                            for (var j = 0; j < act.Activities.length; j++) {
                                var act2 = act.Activities[j];
                                if (act2.ID == id) {
                                    return act2;
                                }
                                for (var k = 0; k < act2.Activities.length; k++) {
                                    var act3 = act2.Activities[k];
                                    if (act3.ID == id) {
                                        return act3;
                                    }
                                }
                            }
                        }
                        return null;
                    };
                    //Task/Activity/Activity Project
                    _this.dsTaskOrders = new kendo.data.DataSource({
                        data: []
                    });
                    _this.dsActivities = new kendo.data.DataSource({
                        data: []
                    });
                    _this.dsActivityProjects = new kendo.data.DataSource({
                        data: []
                    });
                    _this.dsSourceActivities = new kendo.data.DataSource({
                        data: []
                    });
                    _this.dsSourceActivityProjects = new kendo.data.DataSource({
                        data: []
                    });
                    _this.RefreshActivitiesDataSources();
                    _this.isCheckedSource = ko.observable(self.BudgetTaskOrderID() == self.SourceTaskOrderID() && self.BudgetActivityID() == self.SourceActivityID() && self.BudgetActivityProjectID() == self.SourceActivityProjectID());
                    var mirrorActivities = function () {
                        self.BudgetTaskOrderID(self.SourceTaskOrderID());
                        self.BudgetActivityID(self.SourceActivityID());
                        self.BudgetActivityProjectID(self.SourceActivityProjectID());
                    };
                    var updatingFromSource = false;
                    self.isCheckedSource.subscribe(function (value) {
                        if (value) {
                            updatingFromSource = true;
                            mirrorActivities();
                            updatingFromSource = false;
                        }
                    });
                    self.SourceTaskOrderID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true) {
                            updatingFromSource = true;
                            mirrorActivities();
                            updatingFromSource = false;
                        }
                    });
                    self.SourceActivityID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true) {
                            updatingFromSource = true;
                            mirrorActivities();
                            updatingFromSource = false;
                        }
                    });
                    self.SourceActivityProjectID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true) {
                            updatingFromSource = true;
                            mirrorActivities();
                            updatingFromSource = false;
                        }
                    });
                    self.BudgetTaskOrderID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true && updatingFromSource != true) {
                            mirrorActivities();
                        }
                    });
                    self.BudgetActivityID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true && updatingFromSource != true) {
                            mirrorActivities();
                        }
                    });
                    self.BudgetActivityProjectID.subscribe(function (newValue) {
                        if (self.isCheckedSource() == true && updatingFromSource != true) {
                            mirrorActivities();
                        }
                    });
                    self.onSave = function () {
                        $('#EditRequestMetadataDialog').submit();
                    };
                    self.onSubmit = function () {
                        if (!self.Validate()) {
                            return;
                        }
                        if (self.RequestName() == (null || "")) {
                            Global.Helpers.ShowAlert('Validation Error', '<div class="alert alert-warning" style="text-align:center;line-height:2em;"><p>Request Name is required.</p></div>');
                            return;
                        }
                        self.Request.Name(self.RequestName());
                        self.Request.Description(self.Description());
                        self.Request.DueDate(self.DueDate());
                        self.Request.Priority(self.Priority());
                        self.Request.PhiDisclosureLevel(self.PhiDisclosureLevel());
                        self.Request.PurposeOfUse(self.PurposeOfUse());
                        self.Request.RequesterCenterID(self.RequesterCenterID());
                        self.Request.WorkplanTypeID(self.WorkplanTypeID());
                        self.Request.SourceActivityID(self.SourceActivityID());
                        self.Request.SourceActivityProjectID(self.SourceActivityProjectID());
                        self.Request.SourceTaskOrderID(self.SourceTaskOrderID());
                        self.Request.ActivityID(self.BudgetActivityID());
                        self.Request.MSRequestID(self.MSRequestID());
                        self.Request.ReportAggregationLevelID(self.ReportAggregationLevelID());
                        self.Close(self.Request.toData());
                    };
                    self.onCancel = function () {
                        self.Close(null);
                    };
                    self.IsFieldRequired = function (id) {
                        var options = ko.utils.arrayFirst(self.FieldOptions, function (item) { return item.FieldIdentifier == id; });
                        return options.Permission == Dns.Enums.FieldOptionPermissions.Required;
                    };
                    self.IsFieldVisible = function (id) {
                        var options = ko.utils.arrayFirst(self.FieldOptions, function (item) { return item.FieldIdentifier == id; });
                        return options.Permission != Dns.Enums.FieldOptionPermissions.Hidden;
                    };
                    if (self.IsFieldRequired("Budget-Source-CheckBox")) {
                        _this.isCheckedSource(true);
                    }
                    return _this;
                }
                ViewModel.prototype.RefreshActivitiesDataSources = function () {
                    this.dsTaskOrders.data(this.ProjectActivityTree);
                    var activities = [];
                    var activityProjects = [];
                    this.ProjectActivityTree.forEach(function (to) {
                        activities = activities.concat(to.Activities);
                        to.Activities.forEach(function (a) {
                            activityProjects = activityProjects.concat(a.Activities);
                        });
                    });
                    this.dsActivities.data(activities);
                    this.dsActivityProjects.data(activityProjects);
                    this.dsSourceActivities.data(activities);
                    this.dsSourceActivityProjects.data(activityProjects);
                };
                return ViewModel;
            }(Global.DialogViewModel));
            CreateRequest.ViewModel = ViewModel;
            function init() {
                var window = Global.Helpers.GetDialogWindow();
                var requestTypeDetails = (window.options).parameters;
                $.when(Dns.WebApi.Projects.GetActivityTreeByProjectID(requestTypeDetails.ProjectID), Dns.WebApi.Requests.ListRequesterCenters(null, "ID,Name", "Name"), Dns.WebApi.Requests.ListWorkPlanTypes(null, "ID,Name", "Name"), Dns.WebApi.Requests.ListReportAggregationLevels(null, "ID,Name,DeletedOn", "Name"), Dns.WebApi.Projects.GetFieldOptions(requestTypeDetails.ProjectID, User.ID), Dns.WebApi.RequestTypes.Get(requestTypeDetails.RequestTypeID), Dns.WebApi.Templates.GetByRequestType(requestTypeDetails.RequestTypeID), Dns.WebApi.Projects.GetPermissions([requestTypeDetails.ProjectID], [Permissions.Project.EditRequestID]), Dns.WebApi.Workflow.GetWorkflowEntryPointByRequestTypeID(requestTypeDetails.RequestTypeID)).done(function (activityTree, requestCenterList, workplanList, reportAggregationLevelsList, fieldOptions, requestTypes, templates, projectPermissions, workflowActivities) {
                    var allowEditRequestID = projectPermissions != null && projectPermissions.length > 0;
                    var workFlowActivity = workflowActivities[0];
                    var request = new Dns.ViewModels.RequestViewModel();
                    request.Name(requestTypes[0].Name);
                    request.CreatedByID(User.ID);
                    request.CreatedOn(new Date());
                    request.UpdatedByID(User.ID);
                    request.UpdatedOn(new Date());
                    request.CompletedOn(null);
                    request.Description("");
                    request.RequestTypeID(requestTypeDetails.RequestTypeID);
                    request.Priority(Dns.Enums.Priorities.Medium);
                    request.CurrentWorkFlowActivityID(workFlowActivity.ID);
                    request.ProjectID(requestTypeDetails.ProjectID);
                    request.OrganizationID(User.EmployerID);
                    request.WorkflowID(requestTypes[0].WorkflowID);
                    request.ParentRequestID(null);
                    if (templates != null && templates.length > 0 && templates[0].Type == Dns.Enums.TemplateTypes.Request && templates[0].Data != null) {
                        request.Query(templates[0].Data);
                    }
                    else {
                    }
                    var bindingControl = $('#Content');
                    vm = new ViewModel(request, requestTypeDetails, requestCenterList, workplanList, reportAggregationLevelsList, activityTree, fieldOptions, allowEditRequestID, bindingControl);
                    $(function () {
                        ko.applyBindings(vm, bindingControl[0]);
                    });
                });
            }
            CreateRequest.init = init;
            init();
        })(CreateRequest = Search.CreateRequest || (Search.CreateRequest = {}));
    })(Search = CNDS.Search || (CNDS.Search = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=CreateRequest.js.map