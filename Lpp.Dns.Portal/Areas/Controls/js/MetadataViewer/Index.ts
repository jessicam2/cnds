/// <reference path="../../../../js/_layout.ts" />
module Controls.MetadataViewer.Index {
    export class ReferencesViewModel {
        public ID: any;
        public HTMLID: string
        public DomainID: any;
        public ParentDomainReferenceID: any;
        public Title: string;
        public Description: string;
        public Value: KnockoutObservable<string>;
        public ShowOtherBox: KnockoutObservable<boolean>;
        public CheckValue: KnockoutObservable<boolean>;
        public OtherValue: KnockoutObservable<string>;


        constructor(ref: Dns.Interfaces.IDomainReferenceDTO, parentDomainuseID: any) {
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
            self.CheckValue.subscribe((val) => {
                if (self.Title != 'Other') {
                    if (val)
                        self.Value("true");
                    else
                        self.Value("false");
                }
                if (self.Title === 'Other')
                {
                    if (val)
                    {
                        self.ShowOtherBox(true);
                        self.Value(self.OtherValue());
                    }
                        
                    else
                    {
                        self.Value("false");
                        self.ShowOtherBox(false);
                    }
                       
                }
            });

        }
        public ToData(): Dns.Interfaces.IDomainReferenceDTO {
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
            }

        };
    }
    export class MetadataViewModel {
        public ID: any;
        public HTMLID: string;
        public DomainReferenceID: any;
        public Title: string;
        public Description: string;
        public IsMultiValue: boolean;
        public EnumValue: any;
        public DataType: string;
        public EntityType: any;
        public DomainUseID: any;
        public ParentDomainReferenceID: any;
        public Value: KnockoutObservable<string>;
        public CheckValue: KnockoutObservable<boolean>;
        public OtherValue: KnockoutComputed<boolean>;
        public ChildMetadata: MetadataViewModel[];
        public References: ReferencesViewModel[];

        constructor(metadata: Dns.Interfaces.IMetadataDTO) {
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
            this.ChildMetadata = metadata.ChildMetadata == null ? [] : metadata.ChildMetadata.map((item) => { return new MetadataViewModel(item); });
            this.ChildMetadata.sort(function (l, r) { return l.DataType > r.DataType ? 1 : -1 });
            this.References = metadata.References == null ? [] : metadata.References.map((item) => { return new ReferencesViewModel(item, metadata.DomainUseID); });
            self.Value = ko.observable(metadata.Value);
            self.CheckValue = ko.observable(false);
            self.CheckValue = ko.observable(false);
            if (self.Value() === "true") {
                self.CheckValue(true);
            }
            self.CheckValue.subscribe((val) => {
                if (val)
                    self.Value("true");
                else
                    self.Value("false");
               
            });
        }

        public ToData(): Dns.Interfaces.IMetadataDTO {
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
                ChildMetadata: this.ChildMetadata == null ? [] : this.ChildMetadata.map((item) => { return item.ToData() }),
                References: this.References == null ? [] : this.References.map((item) => { return item.ToData() }),
                Visibility: 0
            }

        };

    }
    export class ViewModel extends Global.PageViewModel {
        public GroupedMetadata: MetadataViewModel[];
        public NonGroupedMetadata: MetadataViewModel[];
        constructor(bindingControl: JQuery, metadata: Dns.Interfaces.IMetadataDTO[]) {
            super(bindingControl);
            var self = this;
            var nonGroups = ko.utils.arrayFilter((metadata), (item) => {
                return item.DataType.toLowerCase() != "container" && item.DataType.toLowerCase() != "booleangroup";
            });
            var groupedMetadata = ko.utils.arrayFilter((metadata), (item) => {
                return item.DataType.toLowerCase() == "container" || item.DataType.toLowerCase() == "booleangroup";
            });
            self.NonGroupedMetadata = nonGroups.map((item) => { return new MetadataViewModel(item) });
            self.GroupedMetadata = groupedMetadata.map((item) => { return new MetadataViewModel(item) });
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