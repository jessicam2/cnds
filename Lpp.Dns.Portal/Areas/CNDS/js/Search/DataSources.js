/// <reference path="../../../../js/_rootlayout.ts" />
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
        var DataSources_NoQlik;
        (function (DataSources_NoQlik) {
            var vm;
            var ReferencesViewModel = (function () {
                function ReferencesViewModel(reference) {
                    var self = this;
                    self.ID = reference.ID;
                    self.DomainID = reference.DomainID;
                    self.Title = reference.Title;
                    self.CheckedValue = ko.observable(false);
                    self.CheckedValue.subscribe(function (val) {
                        vm.SelectReference(self);
                        vm.TriggerDataSourceSearch();
                    });
                }
                return ReferencesViewModel;
            }());
            var DomainsViewModel = (function () {
                function DomainsViewModel(domain) {
                    var self = this;
                    self.ID = domain.ID;
                    self.Title = domain.Title;
                    self.ParentDomainID = domain.ParentDomainID;
                    self.CheckedValue = ko.observable(false);
                    if (domain.ChildMetadata != null)
                        self.ChildMetadata = ko.observableArray(domain.ChildMetadata.map(function (item) { return new DomainsViewModel(item); }));
                    else
                        self.ChildMetadata = ko.observableArray([]);
                    if (domain.References != null)
                        self.References = ko.observableArray(domain.References.map(function (item) { return new ReferencesViewModel(item); }));
                    else
                        self.References = ko.observableArray([]);
                    self.CheckedValue.subscribe(function (val) {
                        vm.SelectDomain(self);
                        vm.TriggerDataSourceSearch();
                    });
                }
                return DomainsViewModel;
            }());
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(domains, bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    self.Domains = ko.observableArray(domains.map(function (item) { return new DomainsViewModel(item); }));
                    ViewModel.RecursivlySortDS(self.Domains);
                    self.dsResults = new kendo.data.DataSource({
                        data: []
                    });
                    self.EnableContinue = ko.observable(false);
                    return _this;
                }
                ViewModel.FindRecursiveDomain = function (data, ID, val) {
                    var domain = ko.utils.arrayFirst(data(), function (item) {
                        return item.ID == ID;
                    });
                    if (domain == null) {
                        ko.utils.arrayForEach(data(), function (item) {
                            ViewModel.FindRecursiveDomain(item.ChildMetadata, ID, val);
                        });
                    }
                    else {
                        if (val == true && domain.CheckedValue() == false) {
                            domain.CheckedValue(true);
                        }
                        else if (val == false && domain.ChildMetadata().length > 0) {
                            var childVals = ko.utils.arrayFilter(domain.ChildMetadata(), function (child) {
                                return child.CheckedValue() == true;
                            });
                            if (childVals.length > 0)
                                return;
                            else
                                domain.CheckedValue(false);
                        }
                        else if (val == false && domain.References().length > 0) {
                            var childRefVals = ko.utils.arrayFilter(domain.References(), function (child) {
                                return child.CheckedValue() == true;
                            });
                            if (childRefVals.length > 0)
                                return;
                            else
                                domain.CheckedValue(false);
                        }
                    }
                };
                ViewModel.FindRecursiveReferences = function (data) {
                    ko.utils.arrayForEach(data.References(), function (item) {
                        if (item.CheckedValue())
                            item.CheckedValue(false);
                    });
                };
                ViewModel.GetDomainIDs = function (data) {
                    var ids = { DomainIDs: [], DomainReferences: [] };
                    if (data.CheckedValue())
                        ids.DomainIDs.push(data.ID);
                    if (data.ChildMetadata().length > 0) {
                        ko.utils.arrayForEach(data.ChildMetadata(), function (item) {
                            var returnIDs = ViewModel.GetDomainIDs(item);
                            if (returnIDs.DomainIDs.length > 0)
                                ko.utils.arrayForEach(returnIDs.DomainIDs, function (id) { ids.DomainIDs.push(id); });
                            if (returnIDs.DomainReferences.length > 0)
                                ko.utils.arrayForEach(returnIDs.DomainReferences, function (id) { ids.DomainReferences.push(id); });
                        });
                    }
                    if (data.References().length > 0) {
                        ko.utils.arrayForEach(data.References(), function (item) {
                            if (item.CheckedValue()) {
                                ids.DomainReferences.push(item.ID);
                            }
                        });
                    }
                    return ids;
                };
                ViewModel.RecursivlySortDS = function (ds) {
                    ds.sort(function (left, right) { return left.Title.toLowerCase() == right.Title.toLowerCase() ? 0 : (left.Title.toLowerCase() < right.Title.toLowerCase() ? -1 : 1); });
                    ko.utils.arrayForEach(ds(), function (item) {
                        if (item.ChildMetadata().length > 0)
                            ViewModel.RecursivlySortDS(item.ChildMetadata);
                    });
                };
                ViewModel.prototype.SelectDomain = function (data) {
                    var self = this;
                    if (data.CheckedValue() == false) {
                        ko.utils.arrayForEach(data.ChildMetadata(), function (child) {
                            child.CheckedValue(false);
                        });
                        if (data.References().length > 0)
                            ViewModel.FindRecursiveReferences(data);
                    }
                    if (data.ParentDomainID == null)
                        return;
                    else
                        ViewModel.FindRecursiveDomain(self.Domains, data.ParentDomainID, data.CheckedValue());
                };
                ViewModel.prototype.SelectReference = function (data) {
                    var self = this;
                    ViewModel.FindRecursiveDomain(self.Domains, data.DomainID, data.CheckedValue());
                };
                ViewModel.prototype.TriggerDataSourceSearch = function () {
                    var self = this;
                    var ids = { DomainIDs: [], DomainReferences: [] };
                    ko.utils.arrayForEach(self.Domains(), function (item) {
                        var returnedIDs = ViewModel.GetDomainIDs(item);
                        if (returnedIDs.DomainIDs.length > 0)
                            ko.utils.arrayForEach(returnedIDs.DomainIDs, function (id) { ids.DomainIDs.push(id); });
                        if (returnedIDs.DomainReferences.length > 0)
                            ko.utils.arrayForEach(returnedIDs.DomainReferences, function (id) { ids.DomainReferences.push(id); });
                    });
                    Dns.WebApi.CNDSSearch.DataSourcesSearch(ids).done(function (results) {
                        self.dsResults.data().empty();
                        ko.utils.arrayForEach(results, function (item) {
                            self.dsResults.data().push(item);
                        });
                        self.EnableContinue(results != null && results.length > 0);
                    });
                };
                ViewModel.prototype.NewRequest = function () {
                    var self = this;
                    var ids = '';
                    $.each(vm.dsResults.data(), function (count, data) {
                        if (data == null)
                            return;
                        ids += "&id=" + data.ID;
                    });
                    if (ids.length < 4) {
                        return;
                    }
                    window.location.href = '/cnds/search/NewRequest?' + ids.slice(1);
                };
                ViewModel.prototype.ClearSearch = function () {
                    var self = this;
                    ko.utils.arrayForEach(self.Domains(), function (item) {
                        item.CheckedValue(false);
                    });
                };
                ViewModel.prototype.FormatExcelExport = function (e) {
                    var sheet = e.workbook.sheets[0];
                    for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                        var row = sheet.rows[rowIndex];
                        row.height = 60;
                        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                            row.cells[cellIndex].value = row.cells[cellIndex].value.replace(/<p[^>]*>/g, '\n');
                            row.cells[cellIndex].value = row.cells[cellIndex].value.replace(/<[^>]+>/g, '');
                            row.cells[cellIndex].wrap = true;
                        }
                    }
                };
                ViewModel.prototype.OpenChildDetail = function (DomainID) {
                    var img = $('#img-' + DomainID);
                    var child = $('#children-' + DomainID);
                    if (img.hasClass('k-plus')) {
                        img.removeClass('k-plus');
                        img.addClass('k-minus');
                        child.show();
                    }
                    else {
                        img.addClass('k-plus');
                        img.removeClass('k-minus');
                        child.hide();
                    }
                };
                return ViewModel;
            }(Global.PageViewModel));
            DataSources_NoQlik.ViewModel = ViewModel;
            function init() {
                Dns.WebApi.CNDSSearch.DataSourcesDomains().done(function (domains) {
                    $(function () {
                        var bindingControl = $("#Content");
                        vm = new ViewModel(domains, bindingControl);
                        ko.applyBindings(vm, bindingControl[0]);
                    });
                });
            }
            init();
        })(DataSources_NoQlik = Search.DataSources_NoQlik || (Search.DataSources_NoQlik = {}));
    })(Search = CNDS.Search || (CNDS.Search = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=DataSources.js.map