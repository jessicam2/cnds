/// <reference path="../../../../js/_layout.ts" />
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
                    self.DataSources = ko.utils.arrayMap(_this.Parameters.DataSources, function (ds) { return new NetworkDataSource(ds); });
                    self.AvailableNetworkRequestTypes = ko.observableArray([]);
                    Dns.WebApi.CNDSRequestTypes.AvailableRequestTypesForNewRequest(self.DataSources.map(function (ds) { return ds.ID; })).done(function (requestTypes) {
                        self.AvailableNetworkRequestTypes((requestTypes || []).map(function (rt) { return new RequestTypeSelectionItem(rt); }));
                        if (self.AvailableNetworkRequestTypes().length == 0) {
                            $('#txtLoadingMessage').text("No Request Types available for the selected Data Sources.");
                            $('#txtLoadingMessage').removeClass('alert-success').addClass('alert-danger');
                        }
                        else {
                            $('#LoadingBox').hide();
                        }
                    });
                    self.onSelectRequestType = function (item, evt) {
                        if (!item.HasValidRoutes) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            return;
                        }
                        var promise;
                        if (item.InvalidRoutes().length > 0) {
                            promise = Global.Helpers.ShowConfirm("Please Confirm:", '<p class="alert alert-warning" style="width:600px">One or more selected DataSources is not compatible with the selected RequestType. Incompatible DataSources will not be included as routes for the new Request.<br/><br />Do you wish to continue?</p>');
                        }
                        else {
                            promise = $.Deferred();
                            promise.resolve();
                        }
                        promise.done(function () {
                            self.Close(item.toData());
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
            var RequestTypeSelectionItem = (function (_super) {
                __extends(RequestTypeSelectionItem, _super);
                function RequestTypeSelectionItem(item) {
                    return _super.call(this, item) || this;
                }
                Object.defineProperty(RequestTypeSelectionItem.prototype, "DisplayName", {
                    get: function () {
                        return this.Project() + " / " + this.RequestType();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "HasValidRoutes", {
                    get: function () {
                        return (this.ExternalRoutes().length > 0 || this.LocalRoutes().length > 0);
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "CssClass", {
                    get: function () {
                        if (this.LocalRoutes().length == 0 && this.ExternalRoutes().length == 0)
                            return "disabled";
                        var css = "";
                        if (this.InvalidRoutes().length != 0 && this.HasValidRoutes) {
                            css = "list-group-item-warning";
                        }
                        else if (this.InvalidRoutes().length == 0 && this.HasValidRoutes) {
                            css = "list-group-item-success";
                        }
                        return css.trim();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(RequestTypeSelectionItem.prototype, "AnchorTitle", {
                    get: function () {
                        if (this.InvalidRoutes().length > 0 && this.HasValidRoutes)
                            return "Partially supported, not all selected DataSources support this request type.";
                        return "";
                    },
                    enumerable: true,
                    configurable: true
                });
                return RequestTypeSelectionItem;
            }(Dns.ViewModels.CNDSSourceRequestTypeViewModel));
            SelectRequestType.RequestTypeSelectionItem = RequestTypeSelectionItem;
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