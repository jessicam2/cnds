module CNDS.ManageRequestTypes.EditMapping {
    var vm = null;

    export class RequestTypeDefinitionDisplayItem {
        private _item: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO;
        get ID(): any {
            return this._item.ID;
        }

        get DisplayName(): string {
            return [this._item.Network, this._item.Project, this._item.RequestType, this._item.DataSource].join("//");
        }

        get Network(): string {
            return this._item.Network;
        }

        get NetworkID(): any {
            return this._item.NetworkID;
        }

        get Project(): string {
            return this._item.Project;
        }

        get ProjectID(): any {
            return this._item.ProjectID;
        }

        get RequestType(): string {
            return this._item.RequestType;
        }

        get RequestTypeID(): any {
            return this._item.RequestTypeID;
        }

        get DataSource(): string {
            return this._item.DataSource;
        }
        

        constructor(item: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO) {
            this._item = item;
        }

        public NetworkRequestTypeDefinition() {
            return this._item;
        }
    }

    export class ViewModel extends Global.DialogViewModel {

        private MappingID: any;
        public dsSourceNetwork: Dns.Interfaces.ICNDSMappingItemDTO[];
        public SourceNetworkID: KnockoutObservable<any> = ko.observable(null);

        public dsSourceProject: Dns.Interfaces.ICNDSMappingItemDTO[] = [];
        public SourceProjectID: KnockoutObservable<any> = ko.observable(null);

        public dsSourceRequestTypes: Dns.Interfaces.ICNDSMappingItemDTO[] = [];
        public SourceRequestTypeID: KnockoutObservable<any> = ko.observable(null);

        public AllRoutes: RequestTypeDefinitionDisplayItem[];
        public SelectedRoutes: KnockoutObservableArray<any>;
        public AvailableRoutes: KnockoutComputed<RequestTypeDefinitionDisplayItem[]>;

        public CanSave: KnockoutComputed<boolean>;

        constructor(bindingControl: JQuery, networks: Dns.Interfaces.ICNDSMappingItemDTO[]) {
            super(bindingControl);   
            var self = this;

            let rawData = networks;

            this.MappingID = this.Parameters.ID;
            this.SourceNetworkID = ko.observable(null);
            this.SourceProjectID = ko.observable(null);
            this.SourceRequestTypeID = ko.observable(null);
            this.SelectedRoutes = ko.observableArray([]);

            this.AllRoutes = ko.utils.arrayMap((this.Parameters.Routes as Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO[] || []), (item) => new RequestTypeDefinitionDisplayItem(item));
            this.AvailableRoutes = ko.pureComputed(() => {
                return ko.utils.arrayFilter(self.AllRoutes, (item) => {
                    //cannot map routes between requesttypes in the same project
                    if (self.SourceNetworkID() == null || self.SourceProjectID() == null || self.SourceProjectID() == item.ProjectID )
                        return false

                    return true;
                });
            });

            this.SourceProjectID.subscribe((value) => {
                self.SelectedRoutes([]);
            });
            
            this.dsSourceNetwork = networks;

            this.CanSave = ko.pureComputed(() => {
                var nullValues = ko.utils.arrayFilter([self.SourceNetworkID, self.SourceProjectID, self.SourceRequestTypeID], (x) => x() == null);
                return (nullValues || []).length == 0;
            });

            this.SourceNetworkID.subscribe((value) => {
                self.SourceProjectID(null);
                self.dsSourceProject = [];

                if ((value || '').length > 0) {
                    let network = ko.utils.arrayFirst(rawData, (n) => n.ID == value);
                    if (network) {
                        self.dsSourceProject = ko.utils.arrayMap(network.Children, (c) => c);
                    }
                }

                let dropdownlist = $("#ddlSourceProject").data("kendoDropDownList");
                dropdownlist.dataSource.data(self.dsSourceProject);                
                dropdownlist.enable(self.dsSourceProject.length > 0);
            });

            this.SourceProjectID.subscribe((value) => {
                self.SourceRequestTypeID(null);
                self.dsSourceRequestTypes = [];

                if ((value || '').length > 0) {
                    let network = ko.utils.arrayFirst(rawData, (n) => n.ID == self.SourceNetworkID())
                    if (network) {
                        var project = ko.utils.arrayFirst(network.Children, (p) => p.ID == value);
                        if (project) {
                            self.dsSourceRequestTypes = project.Children || [];
                        }
                    }
                }

                let dropdownlist = $("#ddlSourceRequestType").data("kendoDropDownList");
                dropdownlist.dataSource.data(self.dsSourceRequestTypes);
                dropdownlist.enable(self.dsSourceRequestTypes.length > 0);
            });

            if (self.MappingID) {
                Dns.WebApi.CNDSRequestTypes.GetNetworkRequestTypeMapping(self.MappingID).done((result) => {
                    
                    if (result) {
                        self.SourceNetworkID(result[0].NetworkID);
                        self.SourceProjectID(result[0].ProjectID);
                        self.SourceRequestTypeID(result[0].RequestTypeID);
                        self.SelectedRoutes(ko.utils.arrayMap(result[0].Routes, (r) => r.ID));
                    }
                });
            } else {
                self.MappingID = null;
            }
        }

        public onCancel() {
            this.Close(null);
        }

        public onSubmit() {
            
            if (this.Validate() == false) {
                return;
            }

            let dtoVM = new Dns.ViewModels.CNDSNetworkRequestTypeMappingViewModel().toData();
            dtoVM.ID = this.MappingID;
            dtoVM.NetworkID = this.SourceNetworkID();
            dtoVM.ProjectID = this.SourceProjectID();
            dtoVM.RequestTypeID = this.SourceRequestTypeID();
            dtoVM.Routes = ko.utils.arrayFilter(ko.utils.arrayMap(this.SelectedRoutes(), (rid) => {
                var rt = ko.utils.arrayFirst(this.AvailableRoutes(), (r) => r.ID == rid);
                if (rt) {
                    return rt.NetworkRequestTypeDefinition();
                }
                return null;
            }), (ii) => ii != null);

            Dns.WebApi.CNDSRequestTypes.ValidateNetworkRequestTypeMappings([dtoVM]).done((validationResult) => {
                if (validationResult != null && validationResult.length > 0) {
                    //show validation error indicating duplicate mapping
                    Global.Helpers.ShowErrorAlert('Duplicate Mapping', "<p>A mapping with the same source RequestType already exists.</p>");
                } else {
                    Dns.WebApi.CNDSRequestTypes.CreateNetworkRequestTypeMapping(dtoVM).done(() => {
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