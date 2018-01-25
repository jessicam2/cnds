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
    var ManageMetadata;
    (function (ManageMetadata) {
        var Index;
        (function (Index) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(allMetaData, bindingControl) {
                    var _this = _super.call(this, bindingControl, null) || this;
                    _this.DomainChanged = ko.observable(false);
                    _this.UsersChanged = ko.observable(false);
                    _this.OrganizationsChanged = ko.observable(false);
                    _this.DataSourceChanged = ko.observable(false);
                    var self = _this;
                    self.onNewDomain = function () {
                        Global.Helpers.ShowDialog('New Domain Definition', '/cnds/managemetadata/NewDomainDefinitionDialog', [], 400, 350, null)
                            .done(function (results) {
                            if (results == null)
                                return;
                            var item = new DomainViewModel(results);
                            item.ViewExpanded(true);
                            self.RootDomains.push(item);
                            self.DomainChanged(true);
                        });
                    };
                    self.onNewChildDomain = function (parentDomain) {
                        Global.Helpers.ShowDialog('New Domain Definition', '/cnds/managemetadata/NewDomainDefinitionDialog', [], 400, 350, null)
                            .done(function (results) {
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
                    allMetaData.forEach(function (item) {
                        self.RootDomains.push(new DomainViewModel(item));
                        self.ChildDomains.push(new DomainViewModel(item));
                    });
                    ViewModel.RecursivlySortDS(self.RootDomains);
                    self.OrganizationDomainUseDataSource = ko.observableArray([]);
                    self.UserDomainUseDataSource = ko.observableArray([]);
                    self.DataSourceDomainUseDataSource = ko.observableArray([]);
                    self.onSave = function () {
                        var domains = [];
                        ko.utils.arrayForEach(self.RootDomains(), function (item) {
                            domains.push(item.toData());
                        });
                        Dns.WebApi.CNDSMetadata.InsertOrUpdateDomains(domains).done(function () {
                            var domainUses = [];
                            ko.utils.arrayForEach(self.OrganizationDomainUseDataSource(), function (org) {
                                var ds = org.toData();
                                ko.utils.arrayForEach(ds, function (item) {
                                    domainUses.push(item);
                                });
                            });
                            ko.utils.arrayForEach(self.DataSourceDomainUseDataSource(), function (datasource) {
                                var ds = datasource.toData();
                                ko.utils.arrayForEach(ds, function (item) {
                                    domainUses.push(item);
                                });
                            });
                            ko.utils.arrayForEach(self.UserDomainUseDataSource(), function (user) {
                                var ds = user.toData();
                                ko.utils.arrayForEach(ds, function (item) {
                                    domainUses.push(item);
                                });
                            });
                            Dns.WebApi.CNDSMetadata.InsertorUpdateDataDomains(domainUses).done(function () {
                                window.location.reload();
                            });
                        });
                    };
                    self.onRemove = function (remove) {
                        if (self.RootDomains.indexOf(remove) > -1)
                            self.RootDomains.remove(remove);
                        else {
                            ViewModel.RecursivelyRemoveChild(remove, self.RootDomains);
                        }
                        self.DomainChanged(true);
                    };
                    $.when(Dns.WebApi.CNDSMetadata.GetForOrganization(), Dns.WebApi.CNDSMetadata.GetForDataMarts(), Dns.WebApi.CNDSMetadata.GetForUsers()).done(function (org, dms, users) {
                        ko.utils.arrayForEach(self.ChildDomains(), function (d) {
                            var orgs = ViewModel.RecusivelyLoadDS(d, org, 0);
                            self.OrganizationDomainUseDataSource.push(ViewModel.MapToVieModel(orgs, null));
                            var user = ViewModel.RecusivelyLoadDS(d, users, 1);
                            self.UserDomainUseDataSource.push(ViewModel.MapToVieModel(user, null));
                            var ds = ViewModel.RecusivelyLoadDS(d, dms, 2);
                            self.DataSourceDomainUseDataSource.push(ViewModel.MapToVieModel(ds, null));
                        });
                        ViewModel.RecursivlySortDU(self.OrganizationDomainUseDataSource);
                        ViewModel.RecursivlySortDU(self.UserDomainUseDataSource);
                        ViewModel.RecursivlySortDU(self.DataSourceDomainUseDataSource);
                    });
                    self.onTabSelect = function (e) {
                        if (self.DomainChanged()) {
                            e.preventDefault();
                            Global.Helpers.ShowAlert("Domains have been Changed", "<p>You have added, deleted, or edited a domain.  Please Save your changes before Continuing onto another screen</p>");
                        }
                    };
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    window.location.reload();
                };
                ViewModel.prototype.treeNodeChecked = function (e) {
                    var treeview = $('#trvUserDomainUse').data('kendoTreeView');
                    var dataItem = treeview.dataItem(e.node);
                    //dataItem.Enabled(dataItem.checked);
                    //does not fire for recursive children, will need to handle the recursive explicitly
                };
                ViewModel.RecursivelyRemoveChild = function (remove, RootDomain) {
                    ko.utils.arrayForEach(RootDomain(), function (item) {
                        if (item.ChildDomains.indexOf(remove) > -1)
                            item.ChildDomains.remove(remove);
                        else {
                            ViewModel.RecursivelyRemoveChild(remove, item.ChildDomains);
                        }
                    });
                };
                ViewModel.prototype.OpenChildDetail = function (DomainID, EntityType) {
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
                };
                ViewModel.MapToVieModel = function (d, parent) {
                    var domainUse = new DomainUseViewModel(d, parent);
                    if (d.Children != null && d.Children.length > 0) {
                        ko.utils.arrayForEach(d.Children, function (item) {
                            domainUse.SubDomainUses.push(ViewModel.MapToVieModel(item, domainUse));
                        });
                    }
                    return domainUse;
                };
                ViewModel.RecusivelyLoadDS = function (d, savedEntity, entityType) {
                    var saved = ko.utils.arrayFirst(savedEntity, function (item) {
                        return item.ID == d.ID;
                    });
                    var returnDTO = {
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
                    if (saved != null) {
                        returnDTO.DomainUseID = saved.DomainUseID;
                    }
                    if (d.ChildDomains().length > 0) {
                        ko.utils.arrayForEach(d.ChildDomains(), function (item) {
                            var sub = saved != null ? ko.utils.arrayFilter(saved.Children, function (subchild) {
                                return item.ID == subchild.ID;
                            }) : [];
                            returnDTO.Children.push(ViewModel.RecusivelyLoadDS(item, sub, entityType));
                        });
                    }
                    return returnDTO;
                };
                ViewModel.RecursivlySortDS = function (ds) {
                    ds.sort(function (left, right) {
                        return left.Title().toLowerCase() == right.Title().toLowerCase() ? 0 : (left.Title().toLowerCase() < right.Title().toLowerCase() ? -1 : 1);
                    });
                    ko.utils.arrayForEach(ds(), function (item) {
                        if (item.ChildDomains().length > 0)
                            ViewModel.RecursivlySortDS(item.ChildDomains);
                    });
                };
                ViewModel.RecursivlySortDU = function (ds) {
                    ds.sort(function (left, right) {
                        return left.Title.toLowerCase() == right.Title.toLowerCase() ? 0 : (left.Title.toLowerCase() < right.Title.toLowerCase() ? -1 : 1);
                    });
                    ko.utils.arrayForEach(ds(), function (item) {
                        if (item.SubDomainUses().length > 0)
                            ViewModel.RecursivlySortDU(item.SubDomainUses);
                    });
                };
                return ViewModel;
            }(Global.PageViewModel));
            Index.ViewModel = ViewModel;
            function init() {
                $.when(Dns.WebApi.CNDSMetadata.ListDomains()).done(function (allMetaData) {
                    $(function () {
                        var bindingControl = $('#Content');
                        vm = new ViewModel(allMetaData, bindingControl);
                        ko.applyBindings(vm, bindingControl[0]);
                    });
                });
            }
            init();
            function domainReferenceEditCommandTemplate() {
                return "<a class='k-grid-edit' href='' style='min-width:16px;margin-right:16px;' title='Click to edit row'><span class='glyphicon glyphicon-edit'></span></a>";
            }
            Index.domainReferenceEditCommandTemplate = domainReferenceEditCommandTemplate;
            function domainReferenceDeleteCommandTemplate() {
                return "<a class='k-grid-delete' href='' style='min-width:16px;' title='Click to delete row'><span class='glyphicon glyphicon-remove-circle'></span></a>";
            }
            Index.domainReferenceDeleteCommandTemplate = domainReferenceDeleteCommandTemplate;
            var DomainViewModel = (function () {
                function DomainViewModel(domain) {
                    var self = this;
                    self.ID = domain.ID || Constants.Guid.newGuid();
                    self.Title = ko.observable(domain.Title || '');
                    self.DataType = ko.observable(domain.DataType || '');
                    self.IsMultiValue = ko.observable(domain.IsMultiValue || false);
                    self.ChildDomains = ko.observableArray([]);
                    self.DomainUseID = domain.DomainUseID;
                    self.EntityType = domain.EntityType;
                    if (domain.ChildMetadata != null && domain.ChildMetadata.length > 0) {
                        domain.ChildMetadata.forEach(function (item) {
                            self.ChildDomains.push(new DomainViewModel(item));
                        });
                    }
                    this.DomainReferences = ko.observableArray([]);
                    if (domain.References != null && domain.References.length > 0) {
                        domain.References.forEach(function (item) {
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
                    self.ViewTemplate = ko.pureComputed(function () { return self.DataType() + '-template'; });
                    self.ViewExpanded = ko.observable(false);
                    self.ToggleView = function () { self.ViewExpanded(!self.ViewExpanded()); };
                    self.ViewToggleCss = ko.pureComputed(function () { return self.ViewExpanded() ? 'glyphicon-triangle-bottom' : 'glyphicon-triangle-right'; });
                    self.AvailableDataTypes = ko.pureComputed(function () {
                        var dataTypes = [
                            { text: 'Group', value: 'group' },
                            { text: 'String', value: 'string' },
                            { text: 'Number', value: 'int' },
                            { text: 'Yes/No | True/False', value: 'boolean' },
                            { text: 'Reference', value: 'reference' },
                            { text: 'Boolean Group', value: 'booleanGroup' }
                        ];
                        return dataTypes;
                    });
                    self.DataTypeDisplay = ko.pureComputed(function () {
                        if (self.DataType().toLowerCase() == "boolean")
                            return "True | False";
                        else if (self.DataType().toLowerCase() == "int")
                            return "Whole Number";
                        else if (self.DataType().toLowerCase() == "string")
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
                DomainViewModel.prototype.toData = function () {
                    var preReferences = this.DomainReferencesDataSource.data().toJSON();
                    var refs = [];
                    preReferences.forEach(function (item) {
                        refs.push({
                            ID: item.ID,
                            Title: item.Title,
                            Value: item.Value,
                            Description: item.Description,
                            DomainID: null,
                            ParentDomainReferenceID: null
                        });
                    });
                    this.DomainReferences.removeAll();
                    this.DomainReferences(refs.map(function (item) { return new DomainReferenceViewModel(item); }));
                    return {
                        ID: this.ID,
                        DomainReferenceID: null,
                        Title: this.Title(),
                        Description: null,
                        IsMultiValue: this.IsMultiValue(),
                        EnumValue: null,
                        DataType: this.DataType(),
                        EntityType: null,
                        ParentDomainReferenceID: null,
                        Value: null,
                        ChildMetadata: this.ChildDomains() == null ? [] : this.ChildDomains().map(function (item) { return item.toData(); }),
                        References: this.DomainReferences() == null ? [] : this.DomainReferences().map(function (item) { return item.ToData(); }),
                        Visibility: 0
                    };
                };
                ;
                return DomainViewModel;
            }());
            Index.DomainViewModel = DomainViewModel;
            var DomainReferenceViewModel = (function () {
                function DomainReferenceViewModel(reference) {
                    this.ID = reference.ID;
                    this.Title = (reference.Title || '');
                    this.Description = (reference.Description || '');
                    this.Value = (reference.Value || '');
                }
                DomainReferenceViewModel.prototype.ToData = function () {
                    return {
                        ID: (this.ID == null || this.ID == "") ? Constants.Guid.newGuid() : this.ID,
                        Title: this.Title,
                        Description: this.Description,
                        DomainID: null,
                        ParentDomainReferenceID: null,
                        Value: null
                    };
                };
                ;
                return DomainReferenceViewModel;
            }());
            Index.DomainReferenceViewModel = DomainReferenceViewModel;
            var DomainUseViewModel = (function () {
                function DomainUseViewModel(domain, parentDomain) {
                    this.CheckedForUse = ko.observable(false);
                    this.enableCascadeToChild = true;
                    var self = this;
                    self.DomainID = domain.ID;
                    self.Title = domain.Title;
                    self.EntityType = domain.EntityType;
                    self.DomainUseID = domain.DomainUseID;
                    self.Enabled = ko.observable(domain.DomainUseID != null);
                    if (domain.DomainUseID != null)
                        self.CheckedForUse(true);
                    self.SubDomainUses = ko.observableArray([]);
                    self.ParentDomainUse = parentDomain || null;
                    self.CheckedForUse.subscribe(function (val) {
                        if (self.enableCascadeToChild) {
                            if (val) {
                                if (self.ParentDomainUse != null) {
                                    self.ParentDomainUse.SetToTrue();
                                }
                                ko.utils.arrayForEach(self.SubDomainUses(), function (child) {
                                    child.CheckedForUse(true);
                                });
                            }
                            else {
                                ko.utils.arrayForEach(self.SubDomainUses(), function (child) {
                                    child.CheckedForUse(false);
                                });
                            }
                        }
                    });
                }
                DomainUseViewModel.prototype.SetToTrue = function () {
                    var self = this;
                    self.enableCascadeToChild = false;
                    self.CheckedForUse(true);
                    self.enableCascadeToChild = true;
                    if (self.ParentDomainUse != null)
                        self.ParentDomainUse.SetToTrue();
                };
                DomainUseViewModel.prototype.toData = function () {
                    var self = this;
                    var returnDTO = [];
                    returnDTO.push({
                        ID: self.DomainID,
                        DomainUseID: self.DomainUseID != null ? self.DomainUseID : Constants.GuidEmpty,
                        EntityType: self.EntityType,
                        Checked: self.CheckedForUse()
                    });
                    if (self.SubDomainUses().length > 0) {
                        ko.utils.arrayForEach(self.SubDomainUses(), function (item) {
                            var childReturn = item.toData();
                            ko.utils.arrayForEach(childReturn, function (child) {
                                returnDTO.push(child);
                            });
                        });
                    }
                    return returnDTO;
                };
                ;
                return DomainUseViewModel;
            }());
            Index.DomainUseViewModel = DomainUseViewModel;
        })(Index = ManageMetadata.Index || (ManageMetadata.Index = {}));
    })(ManageMetadata = CNDS.ManageMetadata || (CNDS.ManageMetadata = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=index.js.map