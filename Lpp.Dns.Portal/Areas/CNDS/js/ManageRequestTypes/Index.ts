/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManageRequestTypes.Index {
    var vm = null;

    export class ViewModel extends Global.PageViewModel {
    
        public RequestTypeMappings: kendo.data.DataSource;
        public SelectedMappings: KnockoutObservableArray<any>;
        public HasSelectedMappingRows: KnockoutComputed<boolean>;

        public RouteDefinitions: kendo.data.DataSource;
        public SelectedRouteDefinitions: KnockoutObservableArray<any>;
        public HasSelectedRouteDefinitions: KnockoutComputed<boolean>;

        public onMappingRowSelectionChange: (e) => void;
        public onRouteDefinitionRowSelectionChange: (e) => void;

        constructor(bindingControl: JQuery) {
            super(bindingControl, null);
            var self = this;
            this.SelectedMappings = ko.observableArray<any>([]);
            this.HasSelectedMappingRows = ko.pureComputed(() => self.SelectedMappings().length > 0);

            self.RequestTypeMappings = new kendo.data.DataSource({
                type: 'webapi',
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 1000,
                transport: {
                    read: {
                        url: Global.Helpers.GetServiceUrl('cndsrequesttypes/listmappings')
                    }
                },
                schema: {
                    model: Dns.Interfaces.KendoModelCNDSNetworkRequestTypeMappingDTO
                }
            });

            this.onMappingRowSelectionChange = (e) => {

                let selectedRows = [];
                let grid = $(e.sender.wrapper).data('kendoGrid');
                let rows = grid.select();

                if (rows.length > 0) {
                    for (let i = 0; i < rows.length; i++) {
                        let item: Dns.Interfaces.ICNDSNetworkRequestTypeMappingDTO = (<any>grid.dataItem(rows[i])) as Dns.Interfaces.ICNDSNetworkRequestTypeMappingDTO;
                        selectedRows.push(item);
                    }
                }
                self.SelectedMappings(selectedRows);
            };

            var gMappings = $('#gMappings').kendoGrid({
                dataSource: self.RequestTypeMappings,
                pageable: false,
                sortable: true,
                height: 600,
                resizable: true,
                reorderable: false,
                scrollable: { virtual: false },
                filterable: true,
                noRecords: true,
                selectable: 'row',
                change: self.onMappingRowSelectionChange,
                columns: [
                    { field: 'Network', title: 'Network' },
                    { field: 'Project', title: 'Project' },
                    { field: 'RequestType', title: 'Request Type' }
                ],
                columnMenu: true,
                dataBound: (e) => {
                    //e.sender.table.on('dblclick', 'tbody > tr > td', () => {
                    //    self.onEditMapping();
                    //})
                }
            }).data("kendoGrid");
            

            this.SelectedRouteDefinitions = ko.observableArray<any>([]);
            this.HasSelectedRouteDefinitions = ko.pureComputed(() => self.SelectedRouteDefinitions().length > 0);

            self.RouteDefinitions = new kendo.data.DataSource({
                type: 'webapi',
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 1000,
                transport: {
                    read: {
                        url: Global.Helpers.GetServiceUrl('cndsrequesttypes/listnetworkrequesttypedefinitions')
                    }
                },
                schema: {
                    model: Dns.Interfaces.KendoModelCNDSNetworkRequestTypeDefinitionDTO
                }
            });

            this.onRouteDefinitionRowSelectionChange = (e) => {
                
                let selectedRows = [];
                let grid = $(e.sender.wrapper).data('kendoGrid');
                let rows = grid.select();

                if (rows.length > 0) {
                    for (let i = 0; i < rows.length; i++) {
                        let item: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO = (<any>grid.dataItem(rows[i])) as Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO;
                        selectedRows.push(item);
                    }
                }
                self.SelectedRouteDefinitions(selectedRows);
            };

            var gDefinitions = $('#gDefinitions').kendoGrid({
                dataSource: self.RouteDefinitions,
                pageable: false,
                sortable: true,
                height: 600,
                resizable: true,
                reorderable: false,
                scrollable: { virtual: false },
                filterable: true,
                noRecords: true,
                selectable: 'row',
                change: self.onRouteDefinitionRowSelectionChange,
                columns: [
                    { field: 'Network', title: 'Network' },
                    { field: 'Project', title: 'Project' },
                    { field: 'RequestType', title: 'Request Type' },
                    { field: 'DataSource', title: 'DataMart' }
                ],
                columnMenu: true
            }).data("kendoGrid");

            

            let tabstrip = $('#tStrip').kendoTabStrip().data('kendoTabStrip');
        }

        public onNewMapping() {
            let routes = this.RouteDefinitions.data().map((d: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO) => <Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO>{
                ID: d.ID,
                NetworkID: d.NetworkID,
                Network: d.Network,
                ProjectID: d.ProjectID,
                Project: d.Project,
                RequestTypeID: d.RequestTypeID,
                RequestType: d.RequestType,
                DataSourceID: d.DataSourceID,
                DataSource: d.DataSource,
                Timestamp: d.Timestamp
            });

            Global.Helpers.ShowDialog('New RequestType Mapping Definition', '/cnds/managerequesttypes/editmapping', [], 850, 750, { ID: null, Routes: routes }).done((result) => {
                if (result) {
                    this.SelectedMappings([]);
                    $('#gMappings').data('kendoGrid').clearSelection();
                    this.RequestTypeMappings.fetch();
                }

                window.location.reload();
            });
        }

        public onEditMapping() {
            let selectedMapping = this.SelectedMappings()[0] as Dns.Interfaces.ICNDSNetworkRequestTypeMappingDTO;
            let routes = this.RouteDefinitions.data().map((d: Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO) => <Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO>{
                ID: d.ID,
                NetworkID: d.NetworkID,
                Network: d.Network,
                ProjectID: d.ProjectID,
                Project: d.Project,
                RequestTypeID: d.RequestTypeID,
                RequestType: d.RequestType,
                DataSourceID: d.DataSourceID,
                DataSource: d.DataSource,
                Timestamp: d.Timestamp
            });
            Global.Helpers.ShowDialog('Edit RequestType Mapping Definition', '/cnds/managerequesttypes/editmapping', [], 850, 750, { ID: selectedMapping.ID, Routes: routes }).done((result) => {
                if (result) {
                    this.RequestTypeMappings.fetch();
                }
            });
        }

        public onDeleteMapping() {
            let selectedMapping = this.SelectedMappings()[0];
            
            Global.Helpers.ShowConfirm('Confirm Delete Mapping Definition', "<p>Please confirm that you wish to delete this Cross-Network RequestType Mapping definition.</p>").done((r) => {

                Dns.WebApi.CNDSRequestTypes.DeleteMapping([this.SelectedMappings()[0].ID]).done(() => {
                    //remove the mapping from the grid after delete from CNDS.                    
                    var grid = $('#gMappings').data("kendoGrid");
                    let rows = grid.select();
                    if (rows.length > 0) {
                        grid.clearSelection();
                        grid.dataSource.remove(grid.dataItem(rows[0]));
                        this.SelectedMappings([]);
                    }

                });
                
            });
        }

        public onDeleteDefinition() {
            let selectedDefinition = this.SelectedRouteDefinitions()[0] as CNDS.Interfaces.INetworkRequestTypeDefinitionDTO;

            Global.Helpers.ShowConfirm('Confirm Delete RequestType Definition', "<p>Please confirm that you wish to delete this Cross-Network RequestType Route definition.</p>").done((r) => {

                Dns.WebApi.CNDSRequestTypes.DeleteNetworkRequestTypeDefinitions([selectedDefinition.ID]).done(() => {
                    //remove the mapping from the grid after delete from CNDS.                    
                    var grid = $('#gDefinitions').data("kendoGrid");
                    let rows = grid.select();
                    if (rows.length > 0) {
                        grid.clearSelection();
                        grid.dataSource.remove(grid.dataItem(rows[0]));
                        this.SelectedRouteDefinitions([]);
                    }

                });

            });
        }

        public onNewRequestTypeDefinition() {
            Global.Helpers.ShowDialog('New RequestType Route Definition', '/cnds/managerequesttypes/editdefinition', [], 750, 450, { ID: null }).done((result) => {
                if (result) {
                    this.SelectedRouteDefinitions([]);
                    $('#gDefinitions').data('kendoGrid').clearSelection();
                    this.RouteDefinitions.fetch();
                }
                window.location.reload();
            });
        }

        public onEditRequestTypeDefinition() {
            var selectedDefinition = this.SelectedRouteDefinitions()[0] as Dns.Interfaces.ICNDSNetworkRequestTypeDefinitionDTO;
            
            Global.Helpers.ShowDialog('Edit RequestType Route Definition', '/cnds/managerequesttypes/editdefinition', [], 750, 450, { ID: selectedDefinition.ID }).done((result) => {
                if (result) {
                    this.RouteDefinitions.fetch();
                }
            });
        }
    }

    function init() {
        $.when<any>(
            Users.GetSetting("CNDS.ManageRequestTypes.gMappings.User:" + User.ID),
            Users.GetSetting("CNDS.ManageRequestTypes.gDefinitions.User:" + User.ID)
        ).done((gridMappingsSettings, gridRequestTypeDefinitionsSettings, m) => {
            $(() => {
                let mappings = [];
                let bindingControl = $('#Content');
                vm = new ViewModel(bindingControl);
                ko.applyBindings(vm, bindingControl[0]);
                
                $(window).unload(() => {
                    try {
                        Users.SetSetting("CNDS.ManageRequestTypes.gMappings.User:" + User.ID, Global.Helpers.GetGridSettings($("#gMappings").data("kendoGrid")));
                        Users.SetSetting("CNDS.ManageRequestTypes.gDefinitions.User:" + User.ID, Global.Helpers.GetGridSettings($("#gDefinitions").data("kendoGrid")));
                    } catch (ex) {
                        //ignore the error 
                    };
                });

                try {
                    Global.Helpers.SetGridFromSettings($("#gMappings").data("kendoGrid"), gridMappingsSettings);
                    Global.Helpers.SetGridFromSettings($("#gDefinitions").data("kendoGrid"), gridRequestTypeDefinitionsSettings);
                } catch (ex) {
                    //ignore the error
                };

            });
        });
        
    }
    init();
}