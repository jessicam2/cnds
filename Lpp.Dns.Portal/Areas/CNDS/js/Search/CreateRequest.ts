/// <reference path="common.ts" />

module CNDS.Search.CreateRequest {
    var vm: ViewModel;

    export class ViewModel extends Global.DialogViewModel {
        private Request: Dns.ViewModels.RequestViewModel;

        public FieldOptions: Dns.Interfaces.IBaseFieldOptionAclDTO[];

        public ProjectName: string;
        public RequestTypeName: string;
        public RequestName: KnockoutObservable<string>;
        public PurposeOfUse: KnockoutObservable<string>;
        public PurposeOfUseOptions: Array<any>;
        public PhiDisclosureLevel: KnockoutObservable<string>;
        public PhiDisclosureLevelOptions: Array<any>;
        public RequesterCenterID: KnockoutObservable<any>;
        public RequesterCenters: Array<any>;
        public RequesterCenter: KnockoutObservable<any>;
        public RequesterCentersList: Dns.Interfaces.IRequesterCenterDTO[];
        public ProjectID: KnockoutObservable<any>;
        public Description: KnockoutObservable<string>;

        public MSRequestID: KnockoutObservable<string>;
        public EditRequestIDAllowed: boolean;

        public DueDate: KnockoutObservable<Date>;//
        public Priority: KnockoutObservable<number>;//actually an enum
        public Priorities: Array<any>;

        public WorkplanTypeID: KnockoutObservable<any>;
        public WorkplanTypes: Array<any>;
        public WorkplanTypesList: Dns.Interfaces.IWorkplanTypeDTO[];

        public ReportAggregationLevelID: KnockoutObservable<any>;
        public ReportAggregationLevels: Array<any>;

        public ProjectActivityTree: Dns.Interfaces.IActivityDTO[];
        public isCheckedSource: KnockoutObservable<boolean>;

        public SourceTaskOrderID: KnockoutObservable<any>;
        public SourceActivityID: KnockoutObservable<any>;
        public SourceActivityProjectID: KnockoutObservable<any>;
        public BudgetTaskOrderID: KnockoutObservable<any>;
        public BudgetActivityID: KnockoutObservable<any>;
        public BudgetActivityProjectID: KnockoutObservable<any>;

        public dsTaskOrders: kendo.data.DataSource;
        public dsActivities: kendo.data.DataSource;
        public dsActivityProjects: kendo.data.DataSource;
        public dsSourceActivities: kendo.data.DataSource;
        public dsSourceActivityProjects: kendo.data.DataSource;

        private findActivity: (id: any) => Dns.Interfaces.IActivityDTO;

        private onSubmit: () => void;
        private onSave: () => void;
        private onCancel: () => void;

        public IsFieldVisible: (id: string) => boolean;
        public IsFieldRequired: (id: string) => boolean;

        constructor(
            request: Dns.ViewModels.RequestViewModel,
            requestTypeDetails: CNDS.Search.Interfaces.IRequestTypeSelection,
            requesterCenterList: Dns.Interfaces.IRequesterCenterDTO[],
            workplanList: Dns.Interfaces.IWorkplanTypeDTO[],
            reportAggregationLevelsList: Dns.Interfaces.IReportAggregationLevelDTO[],
            activityTree: Dns.Interfaces.IActivityDTO[],
            fieldOptions: Dns.Interfaces.IBaseFieldOptionAclDTO[],
            allowEditRequestID: boolean,
            bindingControl: JQuery) {
            super(bindingControl);

            var self = this;
            this.Request = request;
            this.ProjectName = requestTypeDetails.Project;
            this.RequestTypeName = requestTypeDetails.RequestType;

            this.FieldOptions = fieldOptions;

            this.RequestName = ko.observable(this.Request.Name());
            this.DueDate = ko.observable(this.Request.DueDate());
            this.Priority = ko.observable(this.Request.Priority());
            this.Priorities = new Array({ Name: 'Low', Value: 0 }, { Name: 'Medium', Value: 1 }, { Name: 'High', Value: 2 }, { Name: 'Urgent', Value: 3 });

            this.ProjectID = ko.observable(self.Request.ProjectID());
            this.PurposeOfUse = ko.observable(self.Request.PurposeOfUse());
            this.PurposeOfUseOptions = new Array({ Name: 'Clinical Trial Research', Value: 'CLINTRCH' }, { Name: 'Healthcare Payment', Value: 'HPAYMT' }, { Name: 'Healthcare Operations', Value: 'HOPERAT' }, { Name: 'Healthcare Research', Value: 'HRESCH' }, { Name: 'Healthcare Marketing', Value: 'HMARKT' }, { Name: 'Observational Research', Value: 'OBSRCH' }, { Name: 'Patient Requested', Value: 'PATRQT' }, { Name: 'Public Health', Value: 'PUBHLTH' }, { Name: 'Prep-to-Research', Value: 'PTR' }, { Name: 'Quality Assurance', Value: 'QA' }, { Name: 'Treatment', Value: 'TREAT' });

            this.PhiDisclosureLevel = ko.observable(self.Request.PhiDisclosureLevel());
            this.PhiDisclosureLevelOptions = new Array({ Name: 'Aggregated', Value: 'Aggregated' }, { Name: 'Limited', Value: 'Limited' }, { Name: 'De-identified', Value: 'De-identified' }, { Name: 'PHI', Value: 'PHI' });

            this.RequesterCenterID = ko.observable(self.Request.RequesterCenterID());
            this.RequesterCenters = requesterCenterList;
            this.RequesterCentersList = requesterCenterList;

            this.WorkplanTypeID = ko.observable(self.Request.WorkplanTypeID());
            this.WorkplanTypes = workplanList;
            this.WorkplanTypesList = workplanList;

            this.ReportAggregationLevelID = ko.observable(self.Request.ReportAggregationLevelID());
            this.ReportAggregationLevels = reportAggregationLevelsList.filter((ral) => ((ral.DeletedOn == undefined) || (ral.DeletedOn == null)));

            this.MSRequestID = ko.observable(self.Request.MSRequestID());
            this.EditRequestIDAllowed = allowEditRequestID;

            this.Description = ko.observable(self.Request.Description());

            this.ProjectActivityTree = activityTree;

            this.SourceTaskOrderID = ko.observable<any>(null);
            this.SourceActivityID = ko.observable<any>(null);
            this.SourceActivityProjectID = ko.observable<any>(null);
            this.BudgetTaskOrderID = ko.observable<any>(null);
            this.BudgetActivityProjectID = ko.observable<any>(null);
            this.BudgetActivityID = ko.observable<any>(null);

            this.findActivity = (id: any) => {
                if (id == null)
                    return null;

                for (var i = 0; i < self.ProjectActivityTree.length; i++) {
                    var act: Dns.Interfaces.IActivityDTO = self.ProjectActivityTree[i];
                    if (act.ID == id) {
                        return act;
                    }

                    for (var j = 0; j < act.Activities.length; j++) {
                        var act2: Dns.Interfaces.IActivityDTO = act.Activities[j];
                        if (act2.ID == id) {
                            return act2;
                        }

                        for (var k = 0; k < act2.Activities.length; k++) {
                            var act3: Dns.Interfaces.IActivityDTO = act2.Activities[k];
                            if (act3.ID == id) {
                                return act3;
                            }
                        }
                    }
                }

                return null;
            };

            //Task/Activity/Activity Project
            this.dsTaskOrders = new kendo.data.DataSource({
                data: []
            });
            this.dsActivities = new kendo.data.DataSource({
                data: []
            });
            this.dsActivityProjects = new kendo.data.DataSource({
                data: []
            });


            this.dsSourceActivities = new kendo.data.DataSource({
                data: []
            });
            this.dsSourceActivityProjects = new kendo.data.DataSource({
                data: []
            });

            this.RefreshActivitiesDataSources();

            this.isCheckedSource = ko.observable(self.BudgetTaskOrderID() == self.SourceTaskOrderID() && self.BudgetActivityID() == self.SourceActivityID() && self.BudgetActivityProjectID() == self.SourceActivityProjectID());

            var mirrorActivities = () => {
                self.BudgetTaskOrderID(self.SourceTaskOrderID());
                self.BudgetActivityID(self.SourceActivityID());
                self.BudgetActivityProjectID(self.SourceActivityProjectID());
            }

            var updatingFromSource = false;
            self.isCheckedSource.subscribe((value) => {
                if (value) {
                    updatingFromSource = true;
                    mirrorActivities();
                    updatingFromSource = false;
                }
            });

            self.SourceTaskOrderID.subscribe((newValue) => {
                if (self.isCheckedSource() == true) {
                    updatingFromSource = true;
                    mirrorActivities();
                    updatingFromSource = false;
                }
            });

            self.SourceActivityID.subscribe((newValue) => {
                if (self.isCheckedSource() == true) {
                    updatingFromSource = true;
                    mirrorActivities();
                    updatingFromSource = false;
                }
            });

            self.SourceActivityProjectID.subscribe((newValue) => {
                if (self.isCheckedSource() == true) {
                    updatingFromSource = true;
                    mirrorActivities();
                    updatingFromSource = false;
                }
            });

            self.BudgetTaskOrderID.subscribe((newValue) => {
                if (self.isCheckedSource() == true && updatingFromSource != true) {
                    mirrorActivities();
                }
            });

            self.BudgetActivityID.subscribe((newValue) => {
                if (self.isCheckedSource() == true && updatingFromSource != true) {
                    mirrorActivities();
                }
            });

            self.BudgetActivityProjectID.subscribe((newValue) => {
                if (self.isCheckedSource() == true && updatingFromSource != true) {
                    mirrorActivities();
                }
            });

            self.onSave = () => {
                $('#EditRequestMetadataDialog').submit();
            };
            self.onSubmit = () => {
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

            self.onCancel = () => {
                self.Close(null);
            };

            self.IsFieldRequired = (id: string) => {
                var options = ko.utils.arrayFirst(self.FieldOptions, (item) => { return item.FieldIdentifier == id; });
                return options.Permission == Dns.Enums.FieldOptionPermissions.Required;
            };

            self.IsFieldVisible = (id: string) => {
                var options = ko.utils.arrayFirst(self.FieldOptions, (item) => { return item.FieldIdentifier == id; });
                return options.Permission != Dns.Enums.FieldOptionPermissions.Hidden;
            };

            if (self.IsFieldRequired("Budget-Source-CheckBox")) {
                this.isCheckedSource(true);
            }
        }

        private RefreshActivitiesDataSources() {
            this.dsTaskOrders.data(this.ProjectActivityTree);

            var activities = [];
            var activityProjects = [];

            this.ProjectActivityTree.forEach((to) => {

                activities = activities.concat(to.Activities);

                to.Activities.forEach((a) => {
                    activityProjects = activityProjects.concat(a.Activities);
                });
            });

            this.dsActivities.data(activities);
            this.dsActivityProjects.data(activityProjects);

            this.dsSourceActivities.data(activities);
            this.dsSourceActivityProjects.data(activityProjects);
        }
    }

    export function init() {

        let window: kendo.ui.Window = Global.Helpers.GetDialogWindow();
        let requestTypeDetails = (<any>(window.options)).parameters as CNDS.Search.Interfaces.IRequestTypeSelection;
        
        $.when<any>(
            Dns.WebApi.Projects.GetActivityTreeByProjectID(requestTypeDetails.ProjectID),
            Dns.WebApi.Requests.ListRequesterCenters(null, "ID,Name", "Name"),
            Dns.WebApi.Requests.ListWorkPlanTypes(null, "ID,Name", "Name"),
            Dns.WebApi.Requests.ListReportAggregationLevels(null, "ID,Name,DeletedOn", "Name"),
            Dns.WebApi.Projects.GetFieldOptions(requestTypeDetails.ProjectID, User.ID),
            Dns.WebApi.RequestTypes.Get(requestTypeDetails.RequestTypeID),
            Dns.WebApi.Templates.GetByRequestType(requestTypeDetails.RequestTypeID),
            Dns.WebApi.Projects.GetPermissions([requestTypeDetails.ProjectID], [Permissions.Project.EditRequestID]),
            Dns.WebApi.Workflow.GetWorkflowEntryPointByRequestTypeID(requestTypeDetails.RequestTypeID)
        ).done((
            activityTree: Dns.Interfaces.IActivityDTO[],
            requestCenterList: Dns.Interfaces.IRequesterCenterDTO[],
            workplanList: Dns.Interfaces.IWorkplanTypeDTO[],
            reportAggregationLevelsList: Dns.Interfaces.IReportAggregationLevelDTO[],
            fieldOptions: Dns.Interfaces.IBaseFieldOptionAclDTO[],
            requestTypes: Dns.Interfaces.IRequestTypeDTO[],
            templates: Dns.Interfaces.ITemplateDTO[],
            projectPermissions: any[],
            workflowActivities: Dns.Interfaces.IWorkflowActivityDTO[]
        ) => {
        
            let allowEditRequestID = projectPermissions != null && projectPermissions.length > 0;
            let workFlowActivity = workflowActivities[0];

            let request = new Dns.ViewModels.RequestViewModel();
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
            } else {
            }
            
            var bindingControl = $('#Content');
            vm = new ViewModel(request, requestTypeDetails, requestCenterList, workplanList, reportAggregationLevelsList, activityTree, fieldOptions, allowEditRequestID, bindingControl);
            $(() => {
                ko.applyBindings(vm, bindingControl[0]);
            });

        });
    }

    init();
}