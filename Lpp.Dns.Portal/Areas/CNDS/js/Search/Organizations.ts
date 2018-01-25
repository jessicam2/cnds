/// <reference path="../../../../js/_rootlayout.ts" />

module CNDS.Search.Organizations_NoQlik {
    var vm: ViewModel;

    class ReferencesViewModel {
        public ID: any;
        public DomainID: any;
        public Title: string;
        public CheckedValue: KnockoutObservable<any>;
        constructor(reference: Dns.Interfaces.IDomainReferenceDTO) {
            let self = this;
            self.ID = reference.ID;
            self.DomainID = reference.DomainID;
            self.Title = reference.Title;
            self.CheckedValue = ko.observable(false);

            self.CheckedValue.subscribe((val) => {
                vm.SelectReference(self);
                vm.TriggerOrganizationSearch();
            });
        }
    }

    class DomainsViewModel {
        public ID: any;
        public Title: string;
        public ParentDomainID: any;
        public CheckedValue: KnockoutObservable<any>;
        public ChildMetadata: KnockoutObservableArray<DomainsViewModel>;
        public References: KnockoutObservableArray<ReferencesViewModel>;
        constructor(domain: Dns.Interfaces.ICNDSSearchMetaDataDTO) {
            let self = this;
            self.ID = domain.ID;
            self.Title = domain.Title;
            self.ParentDomainID = domain.ParentDomainID;
            self.CheckedValue = ko.observable(false);
            if (domain.ChildMetadata != null)
                self.ChildMetadata = ko.observableArray(domain.ChildMetadata.map((item) => { return new DomainsViewModel(item) }));
            else
                self.ChildMetadata = ko.observableArray([]);

            if (domain.References != null)
                self.References = ko.observableArray(domain.References.map((item) => { return new ReferencesViewModel(item) }));
            else
                self.References = ko.observableArray([]);

            self.CheckedValue.subscribe((val) => {
                vm.SelectDomain(self);
                vm.TriggerOrganizationSearch();
            });
        }
    }

    export class ViewModel extends Global.PageViewModel {
        public Domains: KnockoutObservableArray<DomainsViewModel>;
        public dsResults: kendo.data.DataSource;

        constructor(domains: Dns.Interfaces.ICNDSSearchMetaDataDTO[], bindingControl: JQuery) {
            super(bindingControl);
            let self = this;
            self.Domains = ko.observableArray(domains.map((item) => { return new DomainsViewModel(item) }));

            ViewModel.RecursivlySortDS(self.Domains);

            self.dsResults = new kendo.data.DataSource({
                data: []
            });
        }

        static FindRecursiveDomain(data: KnockoutObservableArray<DomainsViewModel>, ID: any, val: boolean) {
            var domain = ko.utils.arrayFirst(data(), (item) => {
                return item.ID == ID
            });
            if (domain == null) {
                ko.utils.arrayForEach(data(), (item) => {
                    ViewModel.FindRecursiveDomain(item.ChildMetadata, ID, val)
                })
            }
            else {
                if (val == true && domain.CheckedValue() == false) {
                    domain.CheckedValue(true);
                }
                else if (val == false && domain.ChildMetadata().length > 0) {

                    var childVals = ko.utils.arrayFilter(domain.ChildMetadata(), (child) => {
                        return child.CheckedValue() == true
                    });
                    if (childVals.length > 0)
                        return
                    else
                        domain.CheckedValue(false);
                }
                else if (val == false && domain.References().length > 0) {
                    var childRefVals = ko.utils.arrayFilter(domain.References(), (child) => {
                        return child.CheckedValue() == true
                    });
                    if (childRefVals.length > 0)
                        return
                    else
                        domain.CheckedValue(false);
                }
            }
        }

        static FindRecursiveReferences(data: DomainsViewModel) {
            ko.utils.arrayForEach(data.References(), (item) => {
                if (item.CheckedValue())
                    item.CheckedValue(false);
            })
        }

        static GetDomainIDs(data: DomainsViewModel): Dns.Interfaces.ICNDSDomainSearchDTO {
            var ids: Dns.Interfaces.ICNDSDomainSearchDTO = { DomainIDs: [], DomainReferences: [] };
            if (data.CheckedValue())
                ids.DomainIDs.push(data.ID)
            if (data.ChildMetadata().length > 0) {
                ko.utils.arrayForEach(data.ChildMetadata(), (item) => {
                    var returnIDs = ViewModel.GetDomainIDs(item);
                    if (returnIDs.DomainIDs.length > 0)
                        ko.utils.arrayForEach(returnIDs.DomainIDs, (id) => { ids.DomainIDs.push(id) });
                    if (returnIDs.DomainReferences.length > 0)
                        ko.utils.arrayForEach(returnIDs.DomainReferences, (id) => { ids.DomainReferences.push(id) });
                });
            }
            if (data.References().length > 0) {
                ko.utils.arrayForEach(data.References(), (item) => {
                    if (item.CheckedValue()) {
                        ids.DomainReferences.push(item.ID);
                    }
                });
            }
            return ids;
        }

        static RecursivlySortDS(ds: KnockoutObservableArray<DomainsViewModel>) {
            ds.sort(function (left, right) { return left.Title.toLowerCase() == right.Title.toLowerCase() ? 0 : (left.Title.toLowerCase() < right.Title.toLowerCase() ? -1 : 1) });
            ko.utils.arrayForEach(ds(), (item) => {
                if (item.ChildMetadata().length > 0)
                    ViewModel.RecursivlySortDS(item.ChildMetadata);
            });
        }

        public SelectDomain(data: DomainsViewModel) {
            let self = this;
            if (data.CheckedValue() == false) {
                ko.utils.arrayForEach(data.ChildMetadata(), (child) => {
                    child.CheckedValue(false);
                });
                if (data.References().length > 0)
                    ViewModel.FindRecursiveReferences(data);
            }

            if (data.ParentDomainID == null)
                return
            else
                ViewModel.FindRecursiveDomain(self.Domains, data.ParentDomainID, data.CheckedValue())
        }

        public SelectReference(data: ReferencesViewModel) {
            let self = this;
            ViewModel.FindRecursiveDomain(self.Domains, data.DomainID, data.CheckedValue())
        }

        public TriggerOrganizationSearch() {
            let self = this;
            var ids: Dns.Interfaces.ICNDSDomainSearchDTO = { DomainIDs: [], DomainReferences: [] };
            ko.utils.arrayForEach(self.Domains(), (item) => {
                var returnedIDs = ViewModel.GetDomainIDs(item);
                if (returnedIDs.DomainIDs.length > 0)
                    ko.utils.arrayForEach(returnedIDs.DomainIDs, (id) => { ids.DomainIDs.push(id) });
                if (returnedIDs.DomainReferences.length > 0)
                    ko.utils.arrayForEach(returnedIDs.DomainReferences, (id) => { ids.DomainReferences.push(id) });
            });
            Dns.WebApi.CNDSSearch.OrganizationsSearch(ids).done((results) => {
                self.dsResults.data().empty();
                ko.utils.arrayForEach(results, (item) => {
                    self.dsResults.data().push(item);
                })
            })
        }

        public ClearSearch() {
            let self = this;
            ko.utils.arrayForEach(self.Domains(), (item) => {
                item.CheckedValue(false);
            });
        }

        public FormatExcelExport(e) {
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
        }

        public OpenChildDetail(DomainID: string) {
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
        }
    }

    function init() {
        Dns.WebApi.CNDSSearch.OrganizationsDomains().done((domains) => {
            $(() => {
                var bindingControl = $("#Content");
                vm = new ViewModel(domains, bindingControl);
                ko.applyBindings(vm, bindingControl[0]);
            });
        });
    }

    init();
}