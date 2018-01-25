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
/// <reference path="../../../../js/_layout.ts" />
var CNDS;
(function (CNDS) {
    var ManageVisibility;
    (function (ManageVisibility) {
        var Index;
        (function (Index) {
            var ManageVisibilityViewModel = (function () {
                function ManageVisibilityViewModel(metadata, level, parent) {
                    var self = this;
                    self.Title = metadata.Title;
                    self.ID = metadata.ID;
                    self.EntitiyID = metadata.EntityID;
                    self.DomainUseID = metadata.DomainUseID;
                    if (metadata.Visibility > Dns.Enums.AccessType.AllNetworks)
                        metadata.Visibility = Dns.Enums.AccessType.AllNetworks;
                    self.Visibility = ko.observable(metadata.Visibility);
                    self.Level = "Level" + level;
                    self.Children = metadata.ChildMetadata == null ? [] : metadata.ChildMetadata.map(function (item) { return new ManageVisibilityViewModel(item, level + 1, self); });
                    self.Parent = parent;
                    self.Visibility.subscribe(function (val) {
                        ko.utils.arrayForEach(self.Children, function (item) {
                            item.Visibility(val);
                        });
                    });
                    self.isAnyoneVisible = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() == Dns.Enums.AccessType.Anyone;
                    }, self, { deferred: true });
                    self.isAllNetworksVisible = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.AllNetworks;
                    }, self, { deferred: true });
                    self.isAllPMNNetworksVisible = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.AllPMNNetworks;
                    }, self, { deferred: true });
                    self.isMyNetworkVisible = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.MyNetwork;
                    }, self, { deferred: true });
                    self.isNoOneVisible = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.NoOne;
                    }, self, { deferred: true });
                    self.isNoOneDisabled = ko.computed(function () {
                        return self.Parent == null || self.Parent.Visibility() == Dns.Enums.AccessType.NoOne;
                    }, this, { deferred: true });
                }
                ManageVisibilityViewModel.prototype.toData = function () {
                    var self = this;
                    return {
                        ID: self.ID,
                        Title: self.Title,
                        DataType: null,
                        Description: null,
                        IsMultiValue: false,
                        Value: null,
                        ChildMetadata: self.Children == null ? null : self.Children.map(function (item) { return item.toData(); }),
                        DomainUseID: self.DomainUseID,
                        DomainReferenceID: null,
                        EntityID: self.EntitiyID,
                        EntityType: null,
                        EnumValue: null,
                        ParentDomainReferenceID: null,
                        References: null,
                        Visibility: self.Visibility()
                    };
                };
                return ManageVisibilityViewModel;
            }());
            Index.ManageVisibilityViewModel = ManageVisibilityViewModel;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, metadata) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    var sortedMetadata = [];
                    ko.utils.arrayForEach(metadata, function (item) {
                        if (item.DataType.toLowerCase() != "container" && item.DataType.toLowerCase() != "booleangroup")
                            sortedMetadata.push(item);
                    });
                    ko.utils.arrayForEach(metadata, function (item) {
                        if (item.DataType.toLowerCase() == "container" || item.DataType.toLowerCase() == "booleangroup")
                            sortedMetadata.push(item);
                    });
                    self.Metadata = sortedMetadata.map(function (item) { return new ManageVisibilityViewModel(item, 1, null); });
                    self.NoOneSelectAll = function () {
                        ko.utils.arrayForEach(self.Metadata, function (item) {
                            item.Visibility(Dns.Enums.AccessType.NoOne);
                        });
                    };
                    self.MyNetworkSelectAll = function () {
                        ko.utils.arrayForEach(self.Metadata, function (item) {
                            item.Visibility(Dns.Enums.AccessType.MyNetwork);
                        });
                    };
                    self.AllPMNNetworksSelectAll = function () {
                        ko.utils.arrayForEach(self.Metadata, function (item) {
                            item.Visibility(Dns.Enums.AccessType.AllPMNNetworks);
                        });
                    };
                    self.AllNetworksSelectAll = function () {
                        ko.utils.arrayForEach(self.Metadata, function (item) {
                            item.Visibility(Dns.Enums.AccessType.AllNetworks);
                        });
                    };
                    self.AnyoneSelectAll = function () {
                        ko.utils.arrayForEach(self.Metadata, function (item) {
                            item.Visibility(Dns.Enums.AccessType.Anyone);
                        });
                    };
                    self.toData = function () {
                        var output = ko.utils.arrayMap(self.Metadata, function (m) { return m.toData(); });
                        return output;
                    };
                    return _this;
                }
                return ViewModel;
            }(Global.PageViewModel));
            Index.ViewModel = ViewModel;
            function init(bindingControl, metadata) {
                var vm = new ViewModel(bindingControl, metadata);
                $(function () {
                    ko.applyBindings(vm, bindingControl[0]);
                });
                return vm;
            }
            Index.init = init;
        })(Index = ManageVisibility.Index || (ManageVisibility.Index = {}));
    })(ManageVisibility = CNDS.ManageVisibility || (CNDS.ManageVisibility = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=Index.js.map