var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
                        if (parent == null)
                            return true;
                        else if (parent.Visibility() == Dns.Enums.AccessType.Anyone)
                            return true;
                        else
                            return false;
                    });
                    self.isAllNetworksVisible = ko.computed(function () {
                        if (parent == null)
                            return true;
                        else if (parent.Visibility() == Dns.Enums.AccessType.Anyone || parent.Visibility() == Dns.Enums.AccessType.AllNetworks)
                            return true;
                        else
                            return false;
                    });
                    self.isAllPMNNetworksVisible = ko.computed(function () {
                        if (parent == null)
                            return true;
                        else if (parent.Visibility() == Dns.Enums.AccessType.Anyone || parent.Visibility() == Dns.Enums.AccessType.AllNetworks || parent.Visibility() == Dns.Enums.AccessType.AllPMNNetworks)
                            return true;
                        else
                            return false;
                    });
                    self.isMyNetworkVisible = ko.computed(function () {
                        if (parent == null)
                            return true;
                        else if (parent.Visibility() == Dns.Enums.AccessType.Anyone || parent.Visibility() == Dns.Enums.AccessType.AllNetworks || parent.Visibility() == Dns.Enums.AccessType.AllPMNNetworks || parent.Visibility() == Dns.Enums.AccessType.MyNetwork)
                            return true;
                        else
                            return false;
                    });
                    self.isNoOneVisible = ko.computed(function () {
                        if (parent == null)
                            return true;
                        else if (parent.Visibility() == Dns.Enums.AccessType.Anyone || parent.Visibility() == Dns.Enums.AccessType.AllNetworks || parent.Visibility() == Dns.Enums.AccessType.AllPMNNetworks || parent.Visibility() == Dns.Enums.AccessType.MyNetwork || parent.Visibility() == Dns.Enums.AccessType.NoOne)
                            return true;
                        else
                            return false;
                    });
                    self.isNoOneDisabled = ko.computed(function () {
                        if (parent == null)
                            return false;
                        else if (parent.Visibility() == Dns.Enums.AccessType.NoOne)
                            return true;
                        else
                            return false;
                    });
                }
                ManageVisibilityViewModel.prototype.toData = function () {
                    var self = this;
                    return {
                        ID: self.ID,
                        Title: "asd",
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
                    self.Metadata = metadata.map(function (item) { return new ManageVisibilityViewModel(item, 1, null); });
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