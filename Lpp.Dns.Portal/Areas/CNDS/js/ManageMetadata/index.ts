/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManageMetadata.Index {
    var vm = null;
    export class ViewModel extends Global.PageViewModel {
        public RootDomains: KnockoutObservableArray<DomainViewModel>;
        public ChildDomains: KnockoutObservableArray<DomainViewModel>;
        public onNewDomain: () => void;
        public onNewChildDomain: (parentDomain: DomainViewModel) => void;
        public onRemove: (domain: DomainViewModel) => void;
        public UserDomainUseDataSource: KnockoutObservableArray<DomainUseViewModel>;
        public OrganizationDomainUseDataSource: KnockoutObservableArray<DomainUseViewModel>;
        public DataSourceDomainUseDataSource: KnockoutObservableArray<DomainUseViewModel>;

        public DomainChanged: KnockoutObservable<boolean> = ko.observable(false);
        public UsersChanged: KnockoutObservable<boolean> = ko.observable(false);
        public OrganizationsChanged: KnockoutObservable<boolean> = ko.observable(false);
        public DataSourceChanged: KnockoutObservable<boolean> = ko.observable(false);
        public onTabSelect: (e) => void;
        public onSave: () => void;

        constructor(allMetaData: Dns.Interfaces.IMetadataDTO[], bindingControl: JQuery) {
            super(bindingControl, null);
            var self = this;

            self.onNewDomain = () => {
                Global.Helpers.ShowDialog('New Domain Definition', '/cnds/managemetadata/NewDomainDefinitionDialog', [], 400,350, null)
                    .done((results) => {
                        if (results == null)
                            return;

                        var item = new DomainViewModel(results);
                        item.ViewExpanded(true);
                        self.RootDomains.push(item);
                        self.DomainChanged(true);
                });
            };
            self.onNewChildDomain = (parentDomain: DomainViewModel) => {
                Global.Helpers.ShowDialog('New Domain Definition', '/cnds/managemetadata/NewDomainDefinitionDialog', [], 400, 350, null)
                    .done((results) => {
                        if (results == null)
                            return;

                        var item = new DomainViewModel(results);
                        item.ViewExpanded(true);
                        parentDomain.ChildDomains.push(item);
                        self.DomainChanged(true);
                    });
            };
            self.RootDomains = ko.observableArray([]);
            self.ChildDomains = ko.observableArray([]);
            allMetaData.forEach((item) => {
                self.RootDomains.push(new DomainViewModel(item));
                self.ChildDomains.push(new DomainViewModel(item));
            });
            self.OrganizationDomainUseDataSource = ko.observableArray([]);
            self.UserDomainUseDataSource = ko.observableArray([]);
            self.DataSourceDomainUseDataSource = ko.observableArray([]);

            self.onSave = () => {
                var domains: Dns.Interfaces.IMetadataDTO[] = [];
                ko.utils.arrayForEach(self.RootDomains(), (item: DomainViewModel) => {
                    domains.push(item.toData());
                                   
                });
                Dns.WebApi.CNDSMetadata.InsertOrUpdateDomains(domains).done(() => {
                    var domainUses: Dns.Interfaces.IDomainUseReturnDTO[] = [];
                    ko.utils.arrayForEach(self.OrganizationDomainUseDataSource(), (org) => {
                        var ds = org.toData();
                        ko.utils.arrayForEach(ds, (item) => {
                            domainUses.push(item);
                        });
                       
                    });
                    ko.utils.arrayForEach(self.DataSourceDomainUseDataSource(), (org) => {
                        var ds = org.toData();
                        ko.utils.arrayForEach(ds, (item) => {
                            domainUses.push(item);
                        });
                    });
                    ko.utils.arrayForEach(self.UserDomainUseDataSource(), (org) => {
                        var ds = org.toData();
                        ko.utils.arrayForEach(ds, (item) => {
                            domainUses.push(item);
                        });
                    });
                    Dns.WebApi.CNDSMetadata.InsertorUpdateDataDomains(domainUses).done(() => {
                        window.location.reload();
                    });
                   
                });
            };

            self.onRemove = (remove: DomainViewModel) => {
                if (self.RootDomains.indexOf(remove) > -1)
                    self.RootDomains.remove(remove);
                else
                {
                    ViewModel.RecursivelyRemoveChild(remove, self.RootDomains);
                }  
                self.DomainChanged(true);             
            }

            $.when<any>(
                Dns.WebApi.CNDSMetadata.GetForOrganization(),
                Dns.WebApi.CNDSMetadata.GetForDataMarts(),
                Dns.WebApi.CNDSMetadata.GetForUsers()
            ).done((org: Dns.Interfaces.IDomainDTO[], dms: Dns.Interfaces.IDomainDTO[], users: Dns.Interfaces.IDomainDTO[]) => {
                ko.utils.arrayForEach(self.ChildDomains(), (d: DomainViewModel) => {
                    var orgs = ViewModel.RecusivelyLoadDS(d, org, 0)
                    self.OrganizationDomainUseDataSource.push(ViewModel.MapToVieModel(orgs, null));

                    var user = ViewModel.RecusivelyLoadDS(d, users, 1);
                    self.UserDomainUseDataSource.push(ViewModel.MapToVieModel(user, null));

                    var ds = ViewModel.RecusivelyLoadDS(d, dms, 2);
                    self.DataSourceDomainUseDataSource.push(ViewModel.MapToVieModel(ds, null));
                });

            }) 

            self.onTabSelect = (e) => {
                if (self.DomainChanged()) {
                    e.preventDefault();
                    Global.Helpers.ShowAlert("Domains have been Changed", "<p>You have added, deleted, or edited a domain.  Please Save your changes before Continuing onto another screen</p>");
                }
                else
                    return
            }

        }

        public onCancel() {
            window.location.reload();
        }

        public treeNodeChecked(e) {
            var treeview = $('#trvUserDomainUse').data('kendoTreeView');
            var dataItem: any = <any>treeview.dataItem(e.node);

            //dataItem.Enabled(dataItem.checked);
            //does not fire for recursive children, will need to handle the recursive explicitly
        }

        static RecursivelyRemoveChild(remove: DomainViewModel, RootDomain: KnockoutObservableArray<DomainViewModel>) {

            ko.utils.arrayForEach(RootDomain(), (item) => {
                if (item.ChildDomains.indexOf(remove) > -1)
                    item.ChildDomains.remove(remove);
                else {
                    ViewModel.RecursivelyRemoveChild(remove, item.ChildDomains);
                }
            });

        }

        public OpenChildDetail(DomainID: string, EntityType: any) {
            var img = $('#img-' + DomainID + EntityType);
            var child = $('#children-' + DomainID + EntityType);
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

        static MapToVieModel(d: Dns.Interfaces.IDomainDTO, parent?:DomainUseViewModel): DomainUseViewModel {
            var domainUse: DomainUseViewModel = new DomainUseViewModel(d, parent);

            if (d.Children != null && d.Children.length > 0) {
                ko.utils.arrayForEach(d.Children, (item) => {
                    domainUse.SubDomainUses.push(ViewModel.MapToVieModel(item, domainUse));
                });
            }
            return domainUse;
        }

        static RecusivelyLoadDS(d: DomainViewModel, savedEntity: Dns.Interfaces.IDomainDTO[], entityType: number): Dns.Interfaces.IDomainDTO {
            var saved = ko.utils.arrayFirst(savedEntity, (item) => {
                return item.ID == d.ID;
            });


            var returnDTO = <Dns.Interfaces.IDomainDTO>{
                ID: d.ID,
                DomainUseID: null,
                Title: d.Title(),
                DataType: d.DataType(),
                EnumValue: null,
                EntityType: entityType,
                IsMultiValue: d.IsMultiValue(),
                ParentDomainID: null,
                Children: [],
                DomainReferences: null
            };
            if (saved != null)
            {
                returnDTO.DomainUseID = saved.DomainUseID;
            }

            if (d.ChildDomains().length > 0) {
                ko.utils.arrayForEach(d.ChildDomains(), (item) => {
                    var sub = saved != null ? ko.utils.arrayFilter(saved.Children, (subchild) => {
                        return item.ID == subchild.ID
                    }) : [];
                    returnDTO.Children.push(ViewModel.RecusivelyLoadDS(item, sub, entityType));
                });
            }
            return returnDTO;
        }

    }
    function init() {
        $.when<any>(
            Dns.WebApi.CNDSMetadata.ListDomains()
        ).done((
            allMetaData: Dns.Interfaces.IMetadataDTO[]
        ) => {
            $(() => {
                var bindingControl = $('#Content');
                vm = new ViewModel(allMetaData, bindingControl);
                ko.applyBindings(vm, bindingControl[0]);

            });
        });
    }

    init();

    export function domainReferenceEditCommandTemplate(): string {
        return "<a class='k-grid-edit' href='' style='min-width:16px;margin-right:16px;' title='Click to edit row'><span class='glyphicon glyphicon-edit'></span></a>";
    }

    export function domainReferenceDeleteCommandTemplate(): string {
        return "<a class='k-grid-delete' href='' style='min-width:16px;' title='Click to delete row'><span class='glyphicon glyphicon-remove-circle'></span></a>";
    }

    export class DomainViewModel {
        public ID: any;
        public Title: KnockoutObservable<string>;
		public DataTypeDisplay: KnockoutComputed<string>;
        public DataType: KnockoutObservable<string>;
        public IsMultiValue: KnockoutObservable<boolean>;
        public ChildDomains: KnockoutObservableArray<DomainViewModel>;
        public DomainReferences: KnockoutObservableArray<DomainReferenceViewModel>;
        public DomainReferencesDataSource: kendo.data.DataSource;
        public EntityType: any;
        public DomainUseID: any;
        public ViewTemplate: KnockoutComputed<string>;
        public ViewExpanded: KnockoutObservable<boolean>;
        public ViewToggleCss: KnockoutComputed<string>;
        public ToggleView: () => void;
        public AvailableDataTypes: KnockoutComputed<any[]>;

        constructor(domain: Dns.Interfaces.IMetadataDTO) {
            var self = this;
            this.ID = domain.ID || Constants.Guid.newGuid();
            this.Title = ko.observable(domain.Title || '');
            this.DataType = ko.observable(domain.DataType || '');
            this.IsMultiValue = ko.observable(domain.IsMultiValue || false);
            this.ChildDomains = ko.observableArray([]);
            this.DomainUseID = domain.DomainUseID;
            this.EntityType = domain.EntityType;
            if (domain.ChildMetadata != null && domain.ChildMetadata.length > 0) {
                domain.ChildMetadata.forEach((item) => {
                    self.ChildDomains.push(new DomainViewModel(item));
                });
            }
            this.DomainReferences = ko.observableArray([]);
            if (domain.References != null &&  domain.References.length > 0) {
                domain.References.forEach((item) => {
                    self.DomainReferences.push(new DomainReferenceViewModel(item));
                });
            }

            this.DomainReferencesDataSource = kendo.data.DataSource.create({
                data: self.DomainReferences(),
                schema: {
                    model: {
                        id: 'ID',
                        fields: {
                            Title: {
                                editable: true, nullable: false, validation: { required: true }
                            },
                            Description: { editable: true, nullable: true },
                            Value: { editable: true, nullable: true }
                        }
                    }
                }
            });

            this.ViewTemplate = ko.pureComputed(() => self.DataType() + '-template');
            this.ViewExpanded = ko.observable(false);
            self.ToggleView = () => { self.ViewExpanded(!self.ViewExpanded()); };
            self.ViewToggleCss = ko.pureComputed<string>(() => { return self.ViewExpanded() ? 'glyphicon-triangle-bottom' : 'glyphicon-triangle-right'; });

            self.AvailableDataTypes = ko.pureComputed<any[]>(() => {
                var dataTypes = [];
                dataTypes.push({ text: 'Group', value: 'group' });
                dataTypes.push({ text: 'String', value: 'string' });
                dataTypes.push({ text: 'Number', value: 'int' });
                dataTypes.push({ text: 'Yes/No | True/False', value: 'boolean' });
                dataTypes.push({ text: 'Reference', value: 'reference' });
                dataTypes.push({ text: 'Boolean Group', value: 'booleanGroup' });
                return dataTypes;
            });
            this.DataTypeDisplay = ko.pureComputed<string>(() => {
				if(self.DataType().toLowerCase() == "boolean")
					return "True | False";
				else if (self.DataType().toLowerCase() == "int")
					return "Whole Number";
				else if(self.DataType().toLowerCase() == "string")
					return "Text";
                else if (self.DataType().toLowerCase() == "container")
                    return "Container";
				else if (self.DataType().toLowerCase() == "reference")
                    return "Reference";
                else if (self.DataType().toLowerCase() == "booleangroup")
                    return "Boolean Group";
				else
					return self.DataType();
			});
        }

        public toData(): Dns.Interfaces.IMetadataDTO {
            var preReferences = this.DomainReferencesDataSource.data().toJSON();
            var refs: Dns.Interfaces.IDomainReferenceDTO[] = [];
            preReferences.forEach((item) => {
                refs.push(<Dns.Interfaces.IDomainReferenceDTO>{
                    ID : item.ID,
                    Title : item.Title,
                    Value : item.Value,
                    Description : item.Description,
                    DomainID : null,
                    ParentDomainReferenceID : null
                })
            })
            this.DomainReferences.removeAll();
            this.DomainReferences(refs.map((item) => { return new DomainReferenceViewModel(item) }));
            return {
                ID: this.ID,
                DomainReferenceID: null,
                Title: this.Title(),
                Description: null,
                IsMultiValue: this.IsMultiValue(),
                EnumValue: null,
                DataType: this.DataType(),
                EntityType: null,
                //DomainUseID: null,
                ParentDomainReferenceID: null,
                Value: null,
                ChildMetadata: this.ChildDomains() == null ? [] : this.ChildDomains().map((item) => { return item.toData() }),
                References: this.DomainReferences == null ? [] : this.DomainReferences().map((item) => { return item.ToData() }),
                Visibility: 0
            }

        };

    }

    export class DomainReferenceViewModel {
        public ID: any;
        public Title: string;
        public Description: string;
        public Value: string;

        constructor(reference: Dns.Interfaces.IDomainReferenceDTO) {
            this.ID = reference.ID;
            this.Title = (reference.Title || '');
            this.Description = (reference.Description || '');
            this.Value = (reference.Value || '');
        }

        public ToData(): Dns.Interfaces.IDomainReferenceDTO {
            return {
                ID: this.ID == null || this.ID == "" ? Constants.Guid.newGuid() : this.ID,
                Title: this.Title,
                Description: this.Description,
                DomainID: null,
                ParentDomainReferenceID: null,
                Value: null
            }

        };

    }

    export class DomainUseViewModel {        
        public DomainID: any;
        public Title: string;
        public EntityType: any;
        public DomainUseID: any;
        public Enabled: KnockoutObservable<boolean>;
        public SubDomainUses: DomainUseViewModel[];
        public CheckedForUse: KnockoutObservable<boolean> = ko.observable(false);

        public ParentDomainUse: DomainUseViewModel;

        private enableCascadeToChild: boolean = true;

        constructor(domain: Dns.Interfaces.IDomainDTO, parentDomain: DomainUseViewModel) {
            var self = this;
            self.DomainID = domain.ID;
            self.Title = domain.Title;
            self.EntityType = domain.EntityType;
            self.DomainUseID = domain.DomainUseID;
            self.Enabled = ko.observable<boolean>(domain.DomainUseID != null);
            if (domain.DomainUseID != null)
                self.CheckedForUse(true);
            self.SubDomainUses = [];
            self.ParentDomainUse = parentDomain || null;

            self.CheckedForUse.subscribe((val) => {
                if (self.enableCascadeToChild){
                    if (val) {
                        if (self.ParentDomainUse != null) {
                            self.ParentDomainUse.SetToTrue();
                        }
                        ko.utils.arrayForEach(self.SubDomainUses, (child) => {
                            child.CheckedForUse(true);
                        });
                    }
                    else {
                        ko.utils.arrayForEach(self.SubDomainUses, (child) => {
                            child.CheckedForUse(false);
                        });
                    }
                }
            });
        }
        public SetToTrue() {
            var self = this;
            self.enableCascadeToChild = false

            self.CheckedForUse(true);
            self.enableCascadeToChild = true;

            if (self.ParentDomainUse != null)
                self.ParentDomainUse.SetToTrue();
        }

        public toData(): Dns.Interfaces.IDomainUseReturnDTO[] {
            var self = this;
            var returnDTO: Dns.Interfaces.IDomainUseReturnDTO[] = [];

            returnDTO.push(<Dns.Interfaces.IDomainUseReturnDTO> {
                ID: self.DomainID,
                DomainUseID: self.DomainUseID != null ? self.DomainUseID : Constants.GuidEmpty,
                EntityType: self.EntityType,
                Checked: self.CheckedForUse()

            });
            if (self.SubDomainUses.length > 0) {
                ko.utils.arrayForEach(self.SubDomainUses, (item) => {
                    var childReturn = item.toData();
                    ko.utils.arrayForEach(childReturn, (child) => {
                        returnDTO.push(child);
                    });
                });
            }
            return returnDTO;

        };

    }

}