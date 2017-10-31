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
            self.Visibility = ko.observable(metadata.Visibility);
            self.Level = "Level" + level;
            self.Children = metadata.ChildMetadata == null ? [] : metadata.ChildMetadata.map((item) => { return new ManageVisibilityViewModel(item, level + 1, self) });
            self.Parent = parent;
            self.Visibility.subscribe((val) => {
                ko.utils.arrayForEach(self.Children, (item) => {
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

        public toData(): Dns.Interfaces.IMetadataDTO{
            var self = this;
            return {
                ID: self.ID,
                Title: "asd",
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
        constructor(bindingControl: JQuery, metadata: Dns.Interfaces.IMetadataDTO[]) {
            super(bindingControl);
            var self = this;
            self.Metadata = metadata.map((item) => { return new ManageVisibilityViewModel(item, 1, null) });
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