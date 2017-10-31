var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/// <reference path="../../../../js/_layout.ts" />
var Controls;
(function (Controls) {
    var MetadataViewer;
    (function (MetadataViewer) {
        var Index;
        (function (Index) {
            var ReferencesViewModel = (function () {
                function ReferencesViewModel(ref, parentDomainuseID) {
                    var self = this;
                    self.ID = ref.ID;
                    self.HTMLID = ref.ID.replace("-", "") + parentDomainuseID.replace("-", "");
                    self.Title = ref.Title;
                    self.Description = ref.Description;
                    self.DomainID = ref.DomainID;
                    self.ParentDomainReferenceID = ref.ParentDomainReferenceID;
                    self.Value = ko.observable(ref.Value);
                    self.OtherValue = ko.observable(null);
                    self.CheckValue = ko.observable(false);
                    self.ShowOtherBox = ko.observable(false);
                    if (self.Value() === "true" && self.Title != 'Other') {
                        self.CheckValue(true);
                        self.ShowOtherBox(false);
                    }
                    if (self.Title === 'Other') {
                        if (self.Value() != null && self.Value() != "" && self.Value() != "false") {
                            self.ShowOtherBox(true);
                            self.CheckValue(true);
                            self.OtherValue(ref.Value);
                        }
                    }
                    self.CheckValue.subscribe(function (val) {
                        if (self.Title != 'Other') {
                            if (val)
                                self.Value("true");
                            else
                                self.Value("false");
                        }
                        if (self.Title === 'Other') {
                            if (val) {
                                self.ShowOtherBox(true);
                                self.Value(self.OtherValue());
                            }
                            else {
                                self.Value("false");
                                self.ShowOtherBox(false);
                            }
                        }
                    });
                }
                ReferencesViewModel.prototype.ToData = function () {
                    if (this.Title === 'Other' && this.CheckValue()) {
                        this.Value(this.OtherValue());
                    }
                    else if (this.Title === 'Other' && !this.CheckValue()) {
                        this.Value("false");
                    }
                    return {
                        ID: this.ID,
                        Title: this.Title,
                        Description: this.Description,
                        DomainID: this.DomainID,
                        ParentDomainReferenceID: this.ParentDomainReferenceID,
                        Value: this.Value(),
                    };
                };
                ;
                return ReferencesViewModel;
            }());
            Index.ReferencesViewModel = ReferencesViewModel;
            var MetadataViewModel = (function () {
                function MetadataViewModel(metadata) {
                    var self = this;
                    this.ID = metadata.ID;
                    this.HTMLID = metadata.ID.replace("-", "") + metadata.DomainUseID.replace("-", "");
                    this.DomainReferenceID = metadata.DomainReferenceID;
                    this.Title = metadata.Title;
                    this.Description = metadata.Description;
                    this.IsMultiValue = metadata.IsMultiValue;
                    this.EnumValue = metadata.EnumValue;
                    this.DataType = metadata.DataType;
                    this.EntityType = metadata.EntityType;
                    this.DomainUseID = metadata.DomainUseID;
                    this.ParentDomainReferenceID = metadata.ParentDomainReferenceID;
                    this.ChildMetadata = metadata.ChildMetadata == null ? [] : metadata.ChildMetadata.map(function (item) { return new MetadataViewModel(item); });
                    this.ChildMetadata.sort(function (l, r) { return l.DataType > r.DataType ? 1 : -1; });
                    this.References = metadata.References == null ? [] : metadata.References.map(function (item) { return new ReferencesViewModel(item, metadata.DomainUseID); });
                    self.Value = ko.observable(metadata.Value);
                    self.CheckValue = ko.observable(false);
                    self.CheckValue = ko.observable(false);
                    if (self.Value() === "true") {
                        self.CheckValue(true);
                    }
                    self.CheckValue.subscribe(function (val) {
                        if (val)
                            self.Value("true");
                        else
                            self.Value("false");
                    });
                }
                MetadataViewModel.prototype.ToData = function () {
                    return {
                        ID: this.ID,
                        DomainReferenceID: this.DomainReferenceID,
                        Title: this.Title,
                        Description: this.Description,
                        IsMultiValue: this.IsMultiValue,
                        EnumValue: this.EnumValue,
                        DataType: this.DataType,
                        EntityType: this.EntityType,
                        DomainUseID: this.DomainUseID,
                        ParentDomainReferenceID: this.ParentDomainReferenceID,
                        Value: this.Value(),
                        ChildMetadata: this.ChildMetadata == null ? [] : this.ChildMetadata.map(function (item) { return item.ToData(); }),
                        References: this.References == null ? [] : this.References.map(function (item) { return item.ToData(); }),
                        Visibility: 0
                    };
                };
                ;
                return MetadataViewModel;
            }());
            Index.MetadataViewModel = MetadataViewModel;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, metadata) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    var nonGroups = ko.utils.arrayFilter((metadata), function (item) {
                        return item.DataType.toLowerCase() != "container" && item.DataType.toLowerCase() != "booleangroup";
                    });
                    var groupedMetadata = ko.utils.arrayFilter((metadata), function (item) {
                        return item.DataType.toLowerCase() == "container" || item.DataType.toLowerCase() == "booleangroup";
                    });
                    //debugger;
                    self.NonGroupedMetadata = nonGroups.map(function (item) { return new MetadataViewModel(item); });
                    self.GroupedMetadata = groupedMetadata.map(function (item) { return new MetadataViewModel(item); });
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
        })(Index = MetadataViewer.Index || (MetadataViewer.Index = {}));
    })(MetadataViewer = Controls.MetadataViewer || (Controls.MetadataViewer = {}));
})(Controls || (Controls = {}));
//# sourceMappingURL=Index.js.map