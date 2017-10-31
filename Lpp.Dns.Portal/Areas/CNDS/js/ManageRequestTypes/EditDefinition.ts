module CNDS.ManageRequestTypes.EditDefinition {
    var vm = null;
    export class ViewModel extends Global.DialogViewModel {
        private DefinitionID: any;
        public CanSave: KnockoutComputed<boolean>;

        public dsNetwork: Dns.Interfaces.ICNDSMappingItemDTO[];
        public NetworkID: KnockoutObservable<any>;

        public dsProject: Dns.Interfaces.ICNDSMappingItemDTO[] = [];
        public ProjectID: KnockoutObservable<any>;

        public dsRequestTypes: Dns.Interfaces.ICNDSMappingItemDTO[] = [];
        public RequestTypeID: KnockoutObservable<any>;

        public dsDataSources: Dns.Interfaces.ICNDSMappingItemDTO[] = [];
        public DataSourceID: KnockoutObservable<any>;


        constructor(bindingControl: JQuery, networks: Dns.Interfaces.ICNDSMappingItemDTO[]) {
            super(bindingControl);
            var self = this;
            
            this.dsNetwork = networks;

            self.NetworkID = ko.observable(null);
            self.ProjectID = ko.observable(null);
            self.RequestTypeID = ko.observable(null);
            self.DataSourceID = ko.observable(null);

            self.NetworkID.subscribe((value) => {
                self.ProjectID(null);
                self.dsProject = [];

                if ((value || '').length > 0) {
                    let network = ko.utils.arrayFirst(self.dsNetwork, (n) => n.ID == value);
                    if (network) {
                        self.dsProject = ko.utils.arrayMap(network.Children, (c) => c);
                    }
                }

                let dropdownlist = $("#ddlProject").data("kendoDropDownList");
                dropdownlist.dataSource.data(self.dsProject);
                dropdownlist.enable(self.dsProject.length > 0);
            });

            self.ProjectID.subscribe((value) => {
                self.RequestTypeID(null);
                self.dsRequestTypes = [];

                if ((value || '').length > 0) {
                    let network = ko.utils.arrayFirst(self.dsNetwork, (n) => n.ID == self.NetworkID())
                    if (network) {
                        var project = ko.utils.arrayFirst(network.Children, (p) => p.ID == value);
                        if (project) {
                            self.dsRequestTypes = project.Children || [];
                        }
                    }
                }

                let dropdownlist = $("#ddlRequestType").data("kendoDropDownList");
                dropdownlist.dataSource.data(self.dsRequestTypes);
                dropdownlist.enable(self.dsRequestTypes.length > 0);
            });

            self.RequestTypeID.subscribe((value) => {
                self.DataSourceID(null);
                self.dsDataSources = [];
                
                if ((value || '').length > 0) {
                    let network = ko.utils.arrayFirst(self.dsNetwork, (n) => n.ID == self.NetworkID())
                    if (network) {
                        let project = ko.utils.arrayFirst(network.Children, (p) => p.ID == self.ProjectID());
                        if (project) {
                            
                            let requestType = ko.utils.arrayFirst(project.Children, (rt) => rt.ID == value);
                            if (requestType) {
                                self.dsDataSources = requestType.Children || [];
                            }
                        }
                    }
                }

                let dropdownlist = $("#ddlDataSource").data("kendoDropDownList");
                dropdownlist.dataSource.data(self.dsDataSources);
                dropdownlist.enable(self.dsDataSources.length > 0);
            });

            self.DefinitionID = self.Parameters.ID;

            if (self.DefinitionID) {
                Dns.WebApi.CNDSRequestTypes.GetNetworkRequestTypeDefinition(self.DefinitionID).done((result: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO[]) => {
                    let definition = result[0];                    
                    self.NetworkID(definition.NetworkID);
                    self.ProjectID(definition.ProjectID);
                    self.RequestTypeID(definition.RequestTypeID);
                    self.DataSourceID(definition.DataSourceID);
                });

            } else {
                self.DefinitionID = null;
            }

            this.CanSave = ko.pureComputed(() => {
                var nullValues = ko.utils.arrayFilter([self.NetworkID, self.ProjectID, self.RequestTypeID, self.DataSourceID], (x) => x() == null);
                return (nullValues || []).length == 0;
            });
        }

        public onCancel() {
            this.Close(null);
        }

        public onSubmit() {
            
            if (this.Validate() == false) {
                return;
            }

            //TODO: validate that the route doesn't already exist
            let def = new Dns.ViewModels.CNDSNetworkRequestTypeDefinitionViewModel();
            def.ID(this.DefinitionID);
            def.NetworkID(this.NetworkID());
            def.ProjectID(this.ProjectID());
            def.RequestTypeID(this.RequestTypeID());
            def.DataSourceID(this.DataSourceID());

            Dns.WebApi.CNDSRequestTypes.ValidateNetworkRequestTypeDefinition([def.toData()]).done((validationResult) => {
                if (validationResult != null && validationResult.length > 0) {
                    //show validation error indicating duplicate mapping
                    Global.Helpers.ShowErrorAlert('Duplicate Network RequestType Definition', "<p>A Network RequestType Definition with the same Network/Project/RequestType/DataSource combination already exists.</p>", 700);
                } else {
                    Dns.WebApi.CNDSRequestTypes.CreateOrUpdateNetworkRequestTypeDefinition([def.toData()]).done((result) => {
                        this.Close(true);
                    });
                }
            });

            
        }
    }

    function init() {

        Dns.WebApi.CNDSRequestTypes.ListAvailableNetworkRoutes().done((networks) => {
            $(() => {

                var bindingControl = $('#Content');
                vm = new ViewModel(bindingControl, networks);
                ko.applyBindings(vm, bindingControl[0]);
            });
        });

    }

    init();
}