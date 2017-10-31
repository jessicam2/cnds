var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/// <reference path="../../../../js/_layout.ts" />
var CNDS;
(function (CNDS) {
    var ManageRequestTypes;
    (function (ManageRequestTypes) {
        var Index;
        (function (Index) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl, null) || this;
                    var self = _this;
                    _this.SelectedMappings = ko.observableArray([]);
                    _this.HasSelectedMappingRows = ko.pureComputed(function () { return self.SelectedMappings().length > 0; });
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
                    _this.onMappingRowSelectionChange = function (e) {
                        var selectedRows = [];
                        var grid = $(e.sender.wrapper).data('kendoGrid');
                        var rows = grid.select();
                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++) {
                                var item = grid.dataItem(rows[i]);
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
                        dataBound: function (e) {
                            //e.sender.table.on('dblclick', 'tbody > tr > td', () => {
                            //    self.onEditMapping();
                            //})
                        }
                    }).data("kendoGrid");
                    _this.SelectedRouteDefinitions = ko.observableArray([]);
                    _this.HasSelectedRouteDefinitions = ko.pureComputed(function () { return self.SelectedRouteDefinitions().length > 0; });
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
                    _this.onRouteDefinitionRowSelectionChange = function (e) {
                        var selectedRows = [];
                        var grid = $(e.sender.wrapper).data('kendoGrid');
                        var rows = grid.select();
                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++) {
                                var item = grid.dataItem(rows[i]);
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
                    var tabstrip = $('#tStrip').kendoTabStrip().data('kendoTabStrip');
                    return _this;
                }
                ViewModel.prototype.onNewMapping = function () {
                    var _this = this;
                    var routes = this.RouteDefinitions.data().map(function (d) { return ({
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
                    }); });
                    Global.Helpers.ShowDialog('New RequestType Mapping Definition', '/cnds/managerequesttypes/editmapping', [], 850, 750, { ID: null, Routes: routes }).done(function (result) {
                        if (result) {
                            _this.SelectedMappings([]);
                            $('#gMappings').data('kendoGrid').clearSelection();
                            _this.RequestTypeMappings.fetch();
                        }
                        window.location.reload();
                    });
                };
                ViewModel.prototype.onEditMapping = function () {
                    var _this = this;
                    var selectedMapping = this.SelectedMappings()[0];
                    var routes = this.RouteDefinitions.data().map(function (d) { return ({
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
                    }); });
                    Global.Helpers.ShowDialog('Edit RequestType Mapping Definition', '/cnds/managerequesttypes/editmapping', [], 850, 750, { ID: selectedMapping.ID, Routes: routes }).done(function (result) {
                        if (result) {
                            _this.RequestTypeMappings.fetch();
                        }
                    });
                };
                ViewModel.prototype.onDeleteMapping = function () {
                    var _this = this;
                    var selectedMapping = this.SelectedMappings()[0];
                    Global.Helpers.ShowConfirm('Confirm Delete Mapping Definition', "<p>Please confirm that you wish to delete this Cross-Network RequestType Mapping definition.</p>").done(function (r) {
                        Dns.WebApi.CNDSRequestTypes.DeleteMapping([_this.SelectedMappings()[0].ID]).done(function () {
                            //remove the mapping from the grid after delete from CNDS.                    
                            var grid = $('#gMappings').data("kendoGrid");
                            var rows = grid.select();
                            if (rows.length > 0) {
                                grid.clearSelection();
                                grid.dataSource.remove(grid.dataItem(rows[0]));
                                _this.SelectedMappings([]);
                            }
                        });
                    });
                };
                ViewModel.prototype.onDeleteDefinition = function () {
                    var _this = this;
                    var selectedDefinition = this.SelectedRouteDefinitions()[0];
                    Global.Helpers.ShowConfirm('Confirm Delete RequestType Definition', "<p>Please confirm that you wish to delete this Cross-Network RequestType Route definition.</p>").done(function (r) {
                        Dns.WebApi.CNDSRequestTypes.DeleteNetworkRequestTypeDefinitions([selectedDefinition.ID]).done(function () {
                            //remove the mapping from the grid after delete from CNDS.                    
                            var grid = $('#gDefinitions').data("kendoGrid");
                            var rows = grid.select();
                            if (rows.length > 0) {
                                grid.clearSelection();
                                grid.dataSource.remove(grid.dataItem(rows[0]));
                                _this.SelectedRouteDefinitions([]);
                            }
                        });
                    });
                };
                ViewModel.prototype.onNewRequestTypeDefinition = function () {
                    var _this = this;
                    Global.Helpers.ShowDialog('New RequestType Route Definition', '/cnds/managerequesttypes/editdefinition', [], 750, 450, { ID: null }).done(function (result) {
                        if (result) {
                            _this.SelectedRouteDefinitions([]);
                            $('#gDefinitions').data('kendoGrid').clearSelection();
                            _this.RouteDefinitions.fetch();
                        }
                        window.location.reload();
                    });
                };
                ViewModel.prototype.onEditRequestTypeDefinition = function () {
                    var _this = this;
                    var selectedDefinition = this.SelectedRouteDefinitions()[0];
                    Global.Helpers.ShowDialog('Edit RequestType Route Definition', '/cnds/managerequesttypes/editdefinition', [], 750, 450, { ID: selectedDefinition.ID }).done(function (result) {
                        if (result) {
                            _this.RouteDefinitions.fetch();
                        }
                    });
                };
                return ViewModel;
            }(Global.PageViewModel));
            Index.ViewModel = ViewModel;
            function init() {
                $.when(Users.GetSetting("CNDS.ManageRequestTypes.gMappings.User:" + User.ID), Users.GetSetting("CNDS.ManageRequestTypes.gDefinitions.User:" + User.ID)).done(function (gridMappingsSettings, gridRequestTypeDefinitionsSettings, m) {
                    $(function () {
                        var mappings = [];
                        var bindingControl = $('#Content');
                        vm = new ViewModel(bindingControl);
                        ko.applyBindings(vm, bindingControl[0]);
                        $(window).unload(function () {
                            try {
                                Users.SetSetting("CNDS.ManageRequestTypes.gMappings.User:" + User.ID, Global.Helpers.GetGridSettings($("#gMappings").data("kendoGrid")));
                                Users.SetSetting("CNDS.ManageRequestTypes.gDefinitions.User:" + User.ID, Global.Helpers.GetGridSettings($("#gDefinitions").data("kendoGrid")));
                            }
                            catch (ex) {
                            }
                            ;
                        });
                        try {
                            Global.Helpers.SetGridFromSettings($("#gMappings").data("kendoGrid"), gridMappingsSettings);
                            Global.Helpers.SetGridFromSettings($("#gDefinitions").data("kendoGrid"), gridRequestTypeDefinitionsSettings);
                        }
                        catch (ex) {
                        }
                        ;
                    });
                });
            }
            init();
        })(Index = ManageRequestTypes.Index || (ManageRequestTypes.Index = {}));
    })(ManageRequestTypes = CNDS.ManageRequestTypes || (CNDS.ManageRequestTypes = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=Index.js.map