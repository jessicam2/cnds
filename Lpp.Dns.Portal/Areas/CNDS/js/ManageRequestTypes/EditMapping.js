var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CNDS;
(function (CNDS) {
    var ManageRequestTypes;
    (function (ManageRequestTypes) {
        var EditMapping;
        (function (EditMapping) {
            var vm = null;
            var RequestTypeDefinitionDisplayItem = (function () {
                function RequestTypeDefinitionDisplayItem(item) {
                    this._item = item;
                }
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "ID", {
                    get: function () {
                        return this._item.ID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "DisplayName", {
                    get: function () {
                        return [this._item.Network, this._item.Project, this._item.RequestType, this._item.DataSource].join("//");
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "Network", {
                    get: function () {
                        return this._item.Network;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "NetworkID", {
                    get: function () {
                        return this._item.NetworkID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "Project", {
                    get: function () {
                        return this._item.Project;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "ProjectID", {
                    get: function () {
                        return this._item.ProjectID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "RequestType", {
                    get: function () {
                        return this._item.RequestType;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "RequestTypeID", {
                    get: function () {
                        return this._item.RequestTypeID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeDefinitionDisplayItem.prototype, "DataSource", {
                    get: function () {
                        return this._item.DataSource;
                    },
                    enumerable: true,
                    configurable: true
                });
                RequestTypeDefinitionDisplayItem.prototype.NetworkRequestTypeDefinition = function () {
                    return this._item;
                };
                return RequestTypeDefinitionDisplayItem;
            }());
            EditMapping.RequestTypeDefinitionDisplayItem = RequestTypeDefinitionDisplayItem;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, networks) {
                    var _this = _super.call(this, bindingControl) || this;
                    _this.SourceNetworkID = ko.observable(null);
                    _this.dsSourceProject = [];
                    _this.SourceProjectID = ko.observable(null);
                    _this.dsSourceRequestTypes = [];
                    _this.SourceRequestTypeID = ko.observable(null);
                    var self = _this;
                    var rawData = networks;
                    _this.MappingID = _this.Parameters.ID;
                    _this.SourceNetworkID = ko.observable(null);
                    _this.SourceProjectID = ko.observable(null);
                    _this.SourceRequestTypeID = ko.observable(null);
                    _this.SelectedRoutes = ko.observableArray([]);
                    _this.AllRoutes = ko.utils.arrayMap((_this.Parameters.Routes || []), function (item) { return new RequestTypeDefinitionDisplayItem(item); });
                    _this.AvailableRoutes = ko.pureComputed(function () {
                        return ko.utils.arrayFilter(self.AllRoutes, function (item) {
                            //cannot map routes between requesttypes in the same project
                            if (self.SourceNetworkID() == null || self.SourceProjectID() == null || self.SourceProjectID() == item.ProjectID)
                                return false;
                            return true;
                        });
                    });
                    _this.SourceProjectID.subscribe(function (value) {
                        self.SelectedRoutes([]);
                    });
                    _this.dsSourceNetwork = networks;
                    _this.CanSave = ko.pureComputed(function () {
                        var nullValues = ko.utils.arrayFilter([self.SourceNetworkID, self.SourceProjectID, self.SourceRequestTypeID], function (x) { return x() == null; });
                        return (nullValues || []).length == 0;
                    });
                    _this.SourceNetworkID.subscribe(function (value) {
                        self.SourceProjectID(null);
                        self.dsSourceProject = [];
                        if ((value || '').length > 0) {
                            var network = ko.utils.arrayFirst(rawData, function (n) { return n.ID == value; });
                            if (network) {
                                self.dsSourceProject = ko.utils.arrayMap(network.Children, function (c) { return c; });
                            }
                        }
                        var dropdownlist = $("#ddlSourceProject").data("kendoDropDownList");
                        dropdownlist.dataSource.data(self.dsSourceProject);
                        dropdownlist.enable(self.dsSourceProject.length > 0);
                    });
                    _this.SourceProjectID.subscribe(function (value) {
                        self.SourceRequestTypeID(null);
                        self.dsSourceRequestTypes = [];
                        if ((value || '').length > 0) {
                            var network = ko.utils.arrayFirst(rawData, function (n) { return n.ID == self.SourceNetworkID(); });
                            if (network) {
                                var project = ko.utils.arrayFirst(network.Children, function (p) { return p.ID == value; });
                                if (project) {
                                    self.dsSourceRequestTypes = project.Children || [];
                                }
                            }
                        }
                        var dropdownlist = $("#ddlSourceRequestType").data("kendoDropDownList");
                        dropdownlist.dataSource.data(self.dsSourceRequestTypes);
                        dropdownlist.enable(self.dsSourceRequestTypes.length > 0);
                    });
                    if (self.MappingID) {
                        Dns.WebApi.CNDSRequestTypes.GetNetworkRequestTypeMapping(self.MappingID).done(function (result) {
                            if (result) {
                                self.SourceNetworkID(result[0].NetworkID);
                                self.SourceProjectID(result[0].ProjectID);
                                self.SourceRequestTypeID(result[0].RequestTypeID);
                                self.SelectedRoutes(ko.utils.arrayMap(result[0].Routes, function (r) { return r.ID; }));
                            }
                        });
                    }
                    else {
                        self.MappingID = null;
                    }
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
                    var dtoVM = new Dns.ViewModels.CNDSNetworkRequestTypeMappingViewModel().toData();
                    dtoVM.ID = this.MappingID;
                    dtoVM.NetworkID = this.SourceNetworkID();
                    dtoVM.ProjectID = this.SourceProjectID();
                    dtoVM.RequestTypeID = this.SourceRequestTypeID();
                    dtoVM.Routes = ko.utils.arrayFilter(ko.utils.arrayMap(this.SelectedRoutes(), function (rid) {
                        var rt = ko.utils.arrayFirst(_this.AvailableRoutes(), function (r) { return r.ID == rid; });
                        if (rt) {
                            return rt.NetworkRequestTypeDefinition();
                        }
                        return null;
                    }), function (ii) { return ii != null; });
                    Dns.WebApi.CNDSRequestTypes.ValidateNetworkRequestTypeMappings([dtoVM]).done(function (validationResult) {
                        if (validationResult != null && validationResult.length > 0) {
                            //show validation error indicating duplicate mapping
                            Global.Helpers.ShowErrorAlert('Duplicate Mapping', "<p>A mapping with the same source RequestType already exists.</p>");
                        }
                        else {
                            Dns.WebApi.CNDSRequestTypes.CreateNetworkRequestTypeMapping(dtoVM).done(function () {
                                _this.Close(true);
                            });
                        }
                    });
                };
                return ViewModel;
            }(Global.DialogViewModel));
            EditMapping.ViewModel = ViewModel;
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
        })(EditMapping = ManageRequestTypes.EditMapping || (ManageRequestTypes.EditMapping = {}));
    })(ManageRequestTypes = CNDS.ManageRequestTypes || (CNDS.ManageRequestTypes = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=EditMapping.js.map