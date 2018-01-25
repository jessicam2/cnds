/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManageVisibility.Index {
    export class ManageVisibilityViewModel {
        public ID: any;
        public DomainUseID: any;
        public EntitiyID:any;
        public Title: string;
        public Visibility: KnockoutObservable<Dns.Enums.AccessType>;
        public Level: string;
        public Children: ManageVisibilityViewModel[];
        public Parent: ManageVisibilityViewModel;
        public isAnyoneVisible: KnockoutComputed<boolean>;
        public isAllNetworksVisible: KnockoutComputed<boolean>;
        public isAllPMNNetworksVisible: KnockoutComputed<boolean>;
        public isMyNetworkVisible: KnockoutComputed<boolean>;
        public isNoOneVisible: KnockoutComputed<boolean>;
        public isNoOneDisabled: KnockoutComputed<boolean>;

        constructor(metadata: Dns.Interfaces.IMetadataDTO, level: number, parent?: ManageVisibilityViewModel) {
            var self = this;
            self.Title = metadata.Title;
            self.ID = metadata.ID;
            self.EntitiyID = metadata.EntityID;
            self.DomainUseID = metadata.DomainUseID;

            if (metadata.Visibility > Dns.Enums.AccessType.AllNetworks)
                metadata.Visibility = Dns.Enums.AccessType.AllNetworks;

            self.Visibility = ko.observable(metadata.Visibility);
            self.Level = "Level" + level;
            self.Children = metadata.ChildMetadata == null ? [] : metadata.ChildMetadata.map((item) => { return new ManageVisibilityViewModel(item, level + 1, self) });
            self.Parent = parent;
            
            self.Visibility.subscribe((val) => {
                ko.utils.arrayForEach(self.Children, (item) => {
                    item.Visibility(val);
                });
            });

            self.isAnyoneVisible = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() == Dns.Enums.AccessType.Anyone;
            }, self, { deferred: true });
            

            self.isAllNetworksVisible = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.AllNetworks;
            }, self, { deferred: true });
            

            self.isAllPMNNetworksVisible = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.AllPMNNetworks;
            }, self, { deferred: true });
            

            self.isMyNetworkVisible = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.MyNetwork;
            }, self, { deferred: true });
            

            self.isNoOneVisible = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() >= Dns.Enums.AccessType.NoOne;
            }, self, { deferred: true });
            

            self.isNoOneDisabled = ko.computed(() => {
                return self.Parent == null || self.Parent.Visibility() == Dns.Enums.AccessType.NoOne;
            }, this, { deferred: true });



            
        }

        public toData(): Dns.Interfaces.IMetadataDTO{
            var self = this;
            return {
                ID: self.ID,
                Title: self.Title,
                DataType: null,
                Description: null,
                IsMultiValue: false,
                Value: null,
                ChildMetadata: self.Children == null ? null : self.Children.map((item) => { return item.toData() }),
                DomainUseID: self.DomainUseID,
                DomainReferenceID: null,
                EntityID: self.EntitiyID,
                EntityType: null,
                EnumValue: null,
                ParentDomainReferenceID: null,
                References: null,
                Visibility: self.Visibility()
            };
        }
    }

    export class ViewModel extends Global.PageViewModel {
        public Metadata: ManageVisibilityViewModel[];
        public NoOneSelectAll: () => void;
        public MyNetworkSelectAll: () => void;
        public AllPMNNetworksSelectAll: () => void;
        public AllNetworksSelectAll: () => void;
        public AnyoneSelectAll: () => void;

        public toData: () => Dns.Interfaces.IMetadataDTO[];

        constructor(bindingControl: JQuery, metadata: Dns.Interfaces.IMetadataDTO[]) {
            super(bindingControl);
            var self = this;
            var sortedMetadata: Dns.Interfaces.IMetadataDTO[] = [];

            ko.utils.arrayForEach(metadata, (item) => {
                if (item.DataType.toLowerCase() != "container" && item.DataType.toLowerCase() != "booleangroup")
                    sortedMetadata.push(item);
            });

            ko.utils.arrayForEach(metadata, (item) => {
                if (item.DataType.toLowerCase() == "container" || item.DataType.toLowerCase() == "booleangroup")
                    sortedMetadata.push(item);
            });

            self.Metadata = sortedMetadata.map((item) => { return new ManageVisibilityViewModel(item, 1, null) });
            
            self.NoOneSelectAll = () => {
                ko.utils.arrayForEach(self.Metadata, (item) => {
                    item.Visibility(Dns.Enums.AccessType.NoOne);
                });
            };
            self.MyNetworkSelectAll = () => {
                ko.utils.arrayForEach(self.Metadata, (item) => {
                    item.Visibility(Dns.Enums.AccessType.MyNetwork);
                });
            };
            self.AllPMNNetworksSelectAll = () => {
                ko.utils.arrayForEach(self.Metadata, (item) => {
                    item.Visibility(Dns.Enums.AccessType.AllPMNNetworks);
                });
            };
            self.AllNetworksSelectAll = () => {
                ko.utils.arrayForEach(self.Metadata, (item) => {
                    item.Visibility(Dns.Enums.AccessType.AllNetworks);
                });
            };
            self.AnyoneSelectAll = () => {
                ko.utils.arrayForEach(self.Metadata, (item) => {
                    item.Visibility(Dns.Enums.AccessType.Anyone);
                });
            };

            self.toData = () => {
                let output = ko.utils.arrayMap(self.Metadata, (m) => m.toData());
                return output;
            };
        }
    }

    export function init(bindingControl: JQuery, metadata: Dns.Interfaces.IMetadataDTO[]) {
        
        var vm = new ViewModel(bindingControl, metadata);
        $(() => {
            ko.applyBindings(vm, bindingControl[0]);
        });
        return vm;
    }
}