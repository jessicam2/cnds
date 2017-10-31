/// <reference path="../../../../js/_layout.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CNDS;
(function (CNDS) {
    var Search;
    (function (Search) {
        var SelectRequestType;
        (function (SelectRequestType) {
            var vm;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    self.ShowOnlyAvailable = ko.observable(false);
                    self.DataSources = ko.utils.arrayMap(_this.Parameters.DataSources, function (ds) { return new NetworkDataSource(ds); });
                    self.NetworkRequestTypes = ko.observableArray([]);
                    Dns.WebApi.CNDSRequestTypes.AvailableRequestTypesForNewRequest().done(function (results) {
                        self.NetworkRequestTypes(ko.utils.arrayMap(results || [], function (rt) { return new RequestTypeSelectionItem(rt, self.DataSources); }));
                        if (self.NetworkRequestTypes().length == 0) {
                            $('#txtLoadingMessage').text("No request types available.");
                            $('#loadingMessageContainer').removeClass('alert-success').addClass('alert-danger');
                        }
                    });
                    self.AvailableNetworkRequestTypes = ko.pureComputed(function () {
                        return ko.utils.arrayFilter(self.NetworkRequestTypes(), function (rt) { return rt.Disabled != self.ShowOnlyAvailable() || rt.Disabled == false; });
                    });
                    self.DataSourceNames = ko.pureComputed(function () {
                        return ko.utils.arrayMap(self.DataSources, function (ds) { return ds.Name; }).join(", ");
                    });
                    self.onSelectRequestType = function (item, evt) {
                        if (item.Disabled) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            return;
                        }
                        var selection = {
                            RequestType: item.RequestType,
                            RequestTypeID: item.RequestTypeID,
                            Project: item.Project,
                            ProjectID: item.RequestTypeProjectID,
                            Routes: item.ValidRoutes
                        };
                        var promise;
                        if (item.NotSupportedDataSources.length > 0) {
                            promise = Global.Helpers.ShowConfirm("Please Confirm:", '<p class="alert alert-warning" style="width:600px">One or more selected DataSources is not compatible with the selected RequestType. Incompatible DataSources will not be included as routes for the new Request.<br/><br />Do you wish to continue?</p>');
                        }
                        else {
                            promise = $.Deferred();
                            promise.resolve();
                        }
                        promise.done(function () {
                            self.Close(selection);
                        });
                    };
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    this.Close(null);
                };
                return ViewModel;
            }(Global.DialogViewModel));
            SelectRequestType.ViewModel = ViewModel;
            function init() {
                $(function () {
                    var bindingControl = $('#Content');
                    vm = new ViewModel(bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            }
            SelectRequestType.init = init;
            var RequestTypeSelectionItem = (function () {
                function RequestTypeSelectionItem(requestTypeItem, selectedDataSources) {
                    this.RequestTypeItem = requestTypeItem;
                    this._invalidRoutes = [];
                    this._validRoutes = [];
                    this._notSupportedDataSources = [];
                    var _loop_1 = function (i) {
                        var definition = requestTypeItem.MappingDefinitions[i];
                        var datasource = ko.utils.arrayFirst(selectedDataSources, function (ds) { return ds.ID == definition.DataMartID; });
                        if (datasource != null && ko.utils.arrayFirst(this_1._validRoutes, function (ds) { return ds.DefinitionID == definition.DefinitionID; }) == null) {
                            //the mapping has a matching route for the selected datasource.
                            this_1._validRoutes.push(definition);
                        }
                        else {
                            //the mapping does not have a matching route for the selected datasource.
                            this_1._invalidRoutes.push(definition);
                        }
                    };
                    var this_1 = this;
                    for (var i = 0; i < requestTypeItem.MappingDefinitions.length; i++) {
                        _loop_1(i);
                    }
                    if (requestTypeItem.MappingDefinitions != null && requestTypeItem.MappingDefinitions.length > 0) {
                        var _loop_2 = function (j) {
                            if (ko.utils.arrayFirst(requestTypeItem.MappingDefinitions, function (md) { return md.DataMartID == selectedDataSources[j].ID; }) == null) {
                                this_2._notSupportedDataSources.push(selectedDataSources[j]);
                            }
                        };
                        var this_2 = this;
                        //build the list of selected datasources that are not supported by this mapping definition
                        for (var j = 0; j < selectedDataSources.length; j++) {
                            _loop_2(j);
                        }
                    }
                    else {
                        //none of the selected datasource are valid since there are no routes for the mapping
                        this._notSupportedDataSources = selectedDataSources;
                    }
                }
                Object.defineProperty(RequestTypeSelectionItem.prototype, "Project", {
                    get: function () {
                        return this.RequestTypeItem.Project;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "RequestType", {
                    get: function () {
                        return this.RequestTypeItem.RequestType;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "RequestTypeID", {
                    get: function () {
                        return this.RequestTypeItem.RequestTypeID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "RequestTypeProjectID", {
                    get: function () {
                        return this.RequestTypeItem.ProjectID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "DisplayName", {
                    get: function () {
                        return this.RequestTypeItem.Project + " / " + this.RequestTypeItem.RequestType;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "DisplayDataSources", {
                    get: function () {
                        if (this._validRoutes.length == 0) {
                            return "* No Supported DataSources *";
                        }
                        return $.map(this._validRoutes, function (definition) { return definition.Network + " // " + definition.Project + " // " + definition.DataMart; }).join(" &bull; ");
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "ValidRoutes", {
                    get: function () {
                        return this._validRoutes;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "NotSupportedDataSources", {
                    get: function () {
                        if (this._validRoutes.length == 0)
                            return [];
                        return this._notSupportedDataSources;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "Disabled", {
                    get: function () {
                        return (this._validRoutes.length == 0);
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "CssClass", {
                    get: function () {
                        if (this._validRoutes.length == 0)
                            return "disabled";
                        var css = "";
                        if (this._notSupportedDataSources.length > 0 && this._validRoutes.length > 0) {
                            css = "list-group-item-warning";
                        }
                        else if (this._notSupportedDataSources.length == 0 && this._validRoutes.length > 0) {
                            css = "list-group-item-success";
                        }
                        //if (vm.SelectedRequestType() != null && vm.SelectedRequestType() == this) {
                        //    css += " SelectedRequestType";
                        //}
                        return css.trim();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "AnchorTitle", {
                    get: function () {
                        if (this._notSupportedDataSources.length > 0 && this._validRoutes.length > 0)
                            return "Partially supported, not all selected DataSources support this request type.";
                        return "";
                    },
                    enumerable: true,
                    configurable: true
                });
                return RequestTypeSelectionItem;
            }());
            SelectRequestType.RequestTypeSelectionItem = RequestTypeSelectionItem;
            function formatRouteName(item) {
                return item.Network + " | " + item.Project + " | " + item.DataMart + " - " + item.RequestType;
            }
            SelectRequestType.formatRouteName = formatRouteName;
            var NetworkDataSource = (function () {
                function NetworkDataSource(ds) {
                    this._ds = ds;
                }
                Object.defineProperty(NetworkDataSource.prototype, "ID", {
                    get: function () {
                        return this._ds.ID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "Name", {
                    get: function () {
                        return this._ds.Name;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "NetworkID", {
                    get: function () {
                        return this._ds.NetworkID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "Network", {
                    get: function () {
                        return this._ds.Network;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "OrganizationID", {
                    get: function () {
                        return this._ds.OrganizationID;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "Organization", {
                    get: function () {
                        return this._ds.Organization;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(NetworkDataSource.prototype, "NetworkName", {
                    get: function () {
                        return "Network: " + this._ds.Network + " // Organization: " + this._ds.Organization;
                    },
                    enumerable: true,
                    configurable: true
                });
                return NetworkDataSource;
            }());
            SelectRequestType.NetworkDataSource = NetworkDataSource;
        })(SelectRequestType = Search.SelectRequestType || (Search.SelectRequestType = {}));
    })(Search = CNDS.Search || (CNDS.Search = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=SelectRequestType.js.map