var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var CNDS;
(function (CNDS) {
    var ManageRequestTypes;
    (function (ManageRequestTypes) {
        var EditDefinition;
        (function (EditDefinition) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, networks) {
                    var _this = _super.call(this, bindingControl) || this;
                    _this.dsProject = [];
                    _this.dsRequestTypes = [];
                    _this.dsDataSources = [];
                    var self = _this;
                    _this.dsNetwork = networks;
                    self.NetworkID = ko.observable(null);
                    self.ProjectID = ko.observable(null);
                    self.RequestTypeID = ko.observable(null);
                    self.DataSourceID = ko.observable(null);
                    self.NetworkID.subscribe(function (value) {
                        self.ProjectID(null);
                        self.dsProject = [];
                        if ((value || '').length > 0) {
                            var network = ko.utils.arrayFirst(self.dsNetwork, function (n) { return n.ID == value; });
                            if (network) {
                                self.dsProject = ko.utils.arrayMap(network.Children, function (c) { return c; });
                            }
                        }
                        var dropdownlist = $("#ddlProject").data("kendoDropDownList");
                        dropdownlist.dataSource.data(self.dsProject);
                        dropdownlist.enable(self.dsProject.length > 0);
                    });
                    self.ProjectID.subscribe(function (value) {
                        self.RequestTypeID(null);
                        self.dsRequestTypes = [];
                        if ((value || '').length > 0) {
                            var network = ko.utils.arrayFirst(self.dsNetwork, function (n) { return n.ID == self.NetworkID(); });
                            if (network) {
                                var project = ko.utils.arrayFirst(network.Children, function (p) { return p.ID == value; });
                                if (project) {
                                    self.dsRequestTypes = project.Children || [];
                                }
                            }
                        }
                        var dropdownlist = $("#ddlRequestType").data("kendoDropDownList");
                        dropdownlist.dataSource.data(self.dsRequestTypes);
                        dropdownlist.enable(self.dsRequestTypes.length > 0);
                    });
                    self.RequestTypeID.subscribe(function (value) {
                        self.DataSourceID(null);
                        self.dsDataSources = [];
                        if ((value || '').length > 0) {
                            var network = ko.utils.arrayFirst(self.dsNetwork, function (n) { return n.ID == self.NetworkID(); });
                            if (network) {
                                var project = ko.utils.arrayFirst(network.Children, function (p) { return p.ID == self.ProjectID(); });
                                if (project) {
                                    var requestType = ko.utils.arrayFirst(project.Children, function (rt) { return rt.ID == value; });
                                    if (requestType) {
                                        self.dsDataSources = requestType.Children || [];
                                    }
                                }
                            }
                        }
                        var dropdownlist = $("#ddlDataSource").data("kendoDropDownList");
                        dropdownlist.dataSource.data(self.dsDataSources);
                        dropdownlist.enable(self.dsDataSources.length > 0);
                    });
                    self.DefinitionID = self.Parameters.ID;
                    if (self.DefinitionID) {
                        Dns.WebApi.CNDSRequestTypes.GetNetworkRequestTypeDefinition(self.DefinitionID).done(function (result) {
                            var definition = result[0];
                            self.NetworkID(definition.NetworkID);
                            self.ProjectID(definition.ProjectID);
                            self.RequestTypeID(definition.RequestTypeID);
                            self.DataSourceID(definition.DataSourceID);
                        });
                    }
                    else {
                        self.DefinitionID = null;
                    }
                    _this.CanSave = ko.pureComputed(function () {
                        var nullValues = ko.utils.arrayFilter([self.NetworkID, self.ProjectID, self.RequestTypeID, self.DataSourceID], function (x) { return x() == null; });
                        return (nullValues || []).length == 0;
                    });
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    this.Close(null);
                };
                ViewModel.prototype.onSubmit = function () {
                    var _this = this;
                    if (this.Validate() == false) {
                        return;
                    }
                    //TODO: validate that the route doesn't already exist
                    var def = new Dns.ViewModels.CNDSNetworkRequestTypeDefinitionViewModel();
                    def.ID(this.DefinitionID);
                    def.NetworkID(this.NetworkID());
                    def.ProjectID(this.ProjectID());
                    def.RequestTypeID(this.RequestTypeID());
                    def.DataSourceID(this.DataSourceID());
                    Dns.WebApi.CNDSRequestTypes.ValidateNetworkRequestTypeDefinition([def.toData()]).done(function (validationResult) {
                        if (validationResult != null && validationResult.length > 0) {
                            //show validation error indicating duplicate mapping
                            Global.Helpers.ShowErrorAlert('Duplicate Network RequestType Definition', "<p>A Network RequestType Definition with the same Network/Project/RequestType/DataSource combination already exists.</p>", 700);
                        }
                        else {
                            Dns.WebApi.CNDSRequestTypes.CreateOrUpdateNetworkRequestTypeDefinition([def.toData()]).done(function (result) {
                                _this.Close(true);
                            });
                        }
                    });
                };
                return ViewModel;
            }(Global.DialogViewModel));
            EditDefinition.ViewModel = ViewModel;
            function init() {
                Dns.WebApi.CNDSRequestTypes.ListAvailableNetworkRoutes().done(function (networks) {
                    $(function () {
                        var bindingControl = $('#Content');
                        vm = new ViewModel(bindingControl, networks);
                        ko.applyBindings(vm, bindingControl[0]);
                    });
                });
            }
            init();
        })(EditDefinition = ManageRequestTypes.EditDefinition || (ManageRequestTypes.EditDefinition = {}));
    })(ManageRequestTypes = CNDS.ManageRequestTypes || (CNDS.ManageRequestTypes = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=EditDefinition.js.map