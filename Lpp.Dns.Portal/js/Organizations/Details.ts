 /// <reference path="../_rootlayout.ts" />

module Organization.Details {
    var vm: ViewModel;

    export class ViewModel extends Global.PageViewModel {
        //Details
        public Organization: Dns.ViewModels.OrganizationViewModel;
        public Metadata: Controls.MetadataViewer.Index.ViewModel;
        public MetadataViewer: CNDS.ManageVisibility.Index.ViewModel;

        //List Items
        public Organizations: KnockoutObservableArray<Dns.ViewModels.OrganizationViewModel>;
        public Registries: KnockoutObservableArray<Dns.ViewModels.OrganizationRegistryViewModel>;
        public Users: KnockoutObservableArray<Dns.Interfaces.IUserDTO>;
        public DataMarts: KnockoutObservableArray<Dns.Interfaces.IDataMartDTO>;
        public SecurityGroups: KnockoutObservableArray<Dns.ViewModels.SecurityGroupViewModel>;
        public RegistryList: Dns.Interfaces.IRegistryDTO[];
        
        //Addable List Items
        public AddableRegistryList: KnockoutComputed<Dns.Interfaces.IRegistryDTO[]>;
        
        
        //Security
        public Security: Security.Acl.AclEditViewModel<Dns.ViewModels.AclOrganizationViewModel>;
        public OrgAcls: KnockoutObservableArray<Dns.ViewModels.AclOrganizationViewModel>;

        //Events
        public Events: Events.Acl.EventAclEditViewModel<Dns.ViewModels.OrganizationEventViewModel>;
        public OrgEvents: KnockoutObservableArray<Dns.ViewModels.OrganizationEventViewModel>;

        constructor(
            screenPermissions: any[],
            organization: Dns.Interfaces.IOrganizationDTO,
            orgAcls: Dns.Interfaces.IAclOrganizationDTO[],
            orgEvents: Dns.Interfaces.IOrganizationEventDTO[],
            organizations: Dns.Interfaces.IOrganizationDTO[],
            securityGroupTree: Dns.Interfaces.ITreeItemDTO[],
            permissionList: Dns.Interfaces.IPermissionDTO[],
            eventList: Dns.Interfaces.IEventDTO[],
            registries: Dns.Interfaces.IOrganizationRegistryDTO[],
            registryList: Dns.Interfaces.IRegistryDTO[],
            users: Dns.Interfaces.IUserDTO[],
            datamarts: Dns.Interfaces.IDataMartDTO[],
            securityGroups: Dns.Interfaces.ISecurityGroupDTO[],
            metadataViewer: Controls.MetadataViewer.Index.ViewModel,
            domainViewer: CNDS.ManageVisibility.Index.ViewModel,
            bindingControl: JQuery) {
            super(bindingControl, screenPermissions);
            var self = this;
            this.Organization = new Dns.ViewModels.OrganizationViewModel(organization);
            this.Metadata = metadataViewer;
            this.MetadataViewer = domainViewer;
            this.Organizations = ko.observableArray(ko.utils.arrayMap(organizations, (item) => { return new Dns.ViewModels.OrganizationViewModel(item) }));
            this.Registries = ko.observableArray(ko.utils.arrayMap(registries, (item) => { return new Dns.ViewModels.OrganizationRegistryViewModel(item) }));
            this.Users = ko.observableArray(users);
            this.DataMarts = ko.observableArray(datamarts);
            this.SecurityGroups = ko.observableArray(securityGroups.map((sg) => {
                return new Dns.ViewModels.SecurityGroupViewModel(sg);
            }));    
            //Permissions
            this.OrgAcls = ko.observableArray(orgAcls.map((a) => {
                return new Dns.ViewModels.AclOrganizationViewModel(a);
            }));

            this.Security = new Security.Acl.AclEditViewModel<Dns.ViewModels.AclOrganizationViewModel>(permissionList, securityGroupTree, this.OrgAcls, [{ Field: "OrganizationID", Value: this.Organization.ID() }], Dns.ViewModels.AclOrganizationViewModel);

            //Events
            this.OrgEvents = ko.observableArray(orgEvents.map((e) => {
                return new Dns.ViewModels.OrganizationEventViewModel(e);
            }));

            this.Events = new Events.Acl.EventAclEditViewModel<Dns.ViewModels.OrganizationEventViewModel>(eventList, securityGroupTree, this.OrgEvents, [{ Field: "OrganizationID", Value: this.Organization.ID() }], Dns.ViewModels.OrganizationEventViewModel);
            this.RegistryList = registryList;
            this.AddableRegistryList = ko.computed<Dns.Interfaces.IRegistryDTO[]>(() => {
                return self.RegistryList.filter((reg) => {
                    var exists = false;
                    self.Registries().forEach((oreg) => {
                        if (oreg.RegistryID() == reg.ID) {
                            exists = true;
                            return;
                        }
                    });

                    return !exists;
                });
            });
            
            this.WatchTitle(this.Organization.Name, "Organization: ");
        }
        public AddSecurityGroup() {
            vm.Save(null, null, false).done(() => {
                window.location.href = "/securitygroups/details?OwnerID=" + this.Organization.ID();
            });
        }
        public AddUser() {
            if (this.HasPermission(Permissions.Organization.Edit)) {
                vm.Save(null, null, false).done(() => {
                    window.location.href = "/users/details?OrganizationID=" + this.Organization.ID();
                });
            } else {
                window.location.href = "/users/details?OrganizationID=" + this.Organization.ID();
            }
        }
        public AddDataMart() {
            if (this.HasPermission(Permissions.Organization.Edit)) {
                vm.Save(null, null, false).done(() => {
                    window.location.href = "/datamarts/details?OrganizationID=" + this.Organization.ID();
                });
            } else {
                window.location.href = "/datamarts/details?OrganizationID=" + this.Organization.ID();
            }
        }
        public NewRegistry() {
            if (this.HasPermission(Permissions.Organization.Edit)) {
                vm.Save(null, null, false).done(() => {
                    window.location.href = "/registries/details?OrganizationID=" + this.Organization.ID();
                });
            } else {
                window.location.href = "/registries/details?OrganizationID=" + this.Organization.ID();
            }
        }
        public AddRegistry = (reg: Dns.Interfaces.IRegistryDTO) => {
            var ro: Dns.Interfaces.IOrganizationRegistryDTO = {
                RegistryID: reg.ID,
                Registry: reg.Name,
                OrganizationID: this.Organization.ID(),
                Organization: this.Organization.Name(),
                Type: reg.Type,
                Description: "",
                Acronym: this.Organization.Acronym(),
                OrganizationParent: this.Organization.ParentOrganization()
            };            
            vm.Registries.push(new Dns.ViewModels.OrganizationRegistryViewModel(ro));
        }

        public RemoveRegistry = (reg: Dns.ViewModels.OrganizationRegistryViewModel) => {
            Global.Helpers.ShowConfirm("Delete Confirmation", "<p>Are you sure that you wish to delete this registry?</p>").done(() => {
                
                Dns.WebApi.OrganizationRegistries.Remove([reg.toData()]).done(() => {
                    vm.Registries.remove(reg);
                });
            });
        }
        public Save(data, e, showPrompt: boolean = true): JQueryDeferred<void> {
            var self = this;
            var deferred = $.Deferred<void>();
            var org = vm.Organization.toData();
            var meta = []
            ko.utils.arrayForEach(self.Metadata.NonGroupedMetadata, (item) => {
                meta.push(item.ToData())
            });
            ko.utils.arrayForEach(self.Metadata.GroupedMetadata, (item) => {
                meta.push(item.ToData())
            });
            org.Metadata = meta;
            Dns.WebApi.Organizations.InsertOrUpdate([org]).done((orgs) => {
                vm.Organization.ID(orgs[0].ID);
                vm.Organization.Timestamp(orgs[0].Timestamp);
                var visibilities: Dns.Interfaces.IMetadataDTO[] = [];
                ko.utils.arrayForEach(self.MetadataViewer.Metadata, (item) => {
                    visibilities.push(item.toData());
                });
                $.when<any>(
                    Dns.WebApi.Organizations.UpdateOrganizationVisibility(visibilities)
                ).done(() => {
                    var orgAcls = null;
                    var orgEvents = null;
                    if (this.HasPermission(Permissions.Organization.ManageSecurity)) {
                        orgAcls = this.OrgAcls().map((a) => {
                            a.OrganizationID(this.Organization.ID());
                            return a.toData();
                        });

                        orgEvents = this.OrgEvents().map((e) => {
                            e.OrganizationID(this.Organization.ID());
                            return e.toData();
                        });
                    }

                    var orgRegistries = this.Registries().map((r) => {
                        r.OrganizationID(this.Organization.ID());
                        return r.toData();
                    });

                    $.when<any>(
                        orgAcls != null ? Dns.WebApi.Security.UpdateOrganizationPermissions(orgAcls) : null,
                        orgEvents != null ? Dns.WebApi.Events.UpdateOrganizationEventPermissions(orgEvents) : null,
                        this.HasPermission(Permissions.Organization.Edit) ? Dns.WebApi.OrganizationRegistries.InsertOrUpdate(orgRegistries) : null
                    ).done(() => {
                        if (showPrompt) {
                            Global.Helpers.ShowAlert("Save", "<p>Save completed successfully!</p>").done(() => {
                                deferred.resolve();
                            });
                        } else {
                            deferred.resolve();
                        }
                    });
                });
            }).fail(() => {
                    deferred.reject();
                });

            return deferred;
        }
        public Delete() {
            Global.Helpers.ShowConfirm("Delete Confirmation", "<p>Are you sure you wish to delete this Organization?</p>").done(() => {
                Dns.WebApi.Organizations.Delete([vm.Organization.ID()]).done(() => {
                    window.location.href = "/organizations";
                });
            });
        }

        public Cancel() {
            window.history.back();
        }
        public Copy() {
            Dns.WebApi.Organizations.Copy(vm.Organization.ID()).done((results) => {
                var newOrgID = results[0];

                window.location.href = "/organizations/details?ID=" + newOrgID;
            });
        }
    }

    function init() {
        var id: any = $.url().param("ID");

        var defaultPermissions = [
            Permissions.Organization.CreateUsers,
            Permissions.Organization.Delete,
            Permissions.Organization.Edit,
            Permissions.Organization.ManageSecurity,
            Permissions.Organization.View,
            Permissions.Organization.CreateDataMarts,
            Permissions.Organization.CreateRegistries,
            Permissions.Organization.Copy,
            Permissions.Organization.ManageCNDSVisibility
        ];
        $.when<any>(
            id == null ? null : Dns.WebApi.Organizations.GetPermissions([id], defaultPermissions),
            id == null ? null : Dns.WebApi.Organizations.Get(id),
            id == null ? Dns.WebApi.Organizations.GetAvailableOrganizationMetadata() : null,
            id == null ? null : Dns.WebApi.Security.GetOrganizationPermissions(id),
            id == null ? null : Dns.WebApi.Events.GetOrganizationEventPermissions(id),
            Dns.WebApi.Organizations.List(null, null, "Name", null, null,null),
            Dns.WebApi.Security.GetAvailableSecurityGroupTree(),
            Dns.WebApi.Security.GetPermissionsByLocation([Dns.Enums.PermissionAclTypes.Organizations]),
            Dns.WebApi.Events.GetEventsByLocation([Dns.Enums.PermissionAclTypes.Organizations]),
            id == null ? null : Dns.WebApi.OrganizationRegistries.List("OrganizationID eq " + id ),
            Dns.WebApi.Registries.List(),
            id == null ? null : Dns.WebApi.Users.List("OrganizationID eq " + id ),
            id == null ? null : Dns.WebApi.DataMarts.List("OrganizationID eq " + id),
            id == null ? null : Dns.WebApi.SecurityGroups.List("OwnerID eq " + id)
            ).done((
                screenPermissions: any[],
                organizationGet: Dns.Interfaces.IOrganizationDTO,
                availMetadata: Dns.Interfaces.IMetadataDTO[],
                orgAcls,
                orgEvents,
                organizationList,
                securityGroupTree,
                permissionList,
                eventList,
                registries: Dns.Interfaces.IOrganizationRegistryDTO[],
                registryList,
                users: Dns.Interfaces.IUserDTO[],
                datamarts: Dns.Interfaces.IDataMartDTO[],
                securityGroups
                ) => {
                var organization: Dns.Interfaces.IOrganizationDTO = organizationGet == null ? null : organizationGet[0];
                screenPermissions = screenPermissions || defaultPermissions;
                $(() => {
                    var bindingControl = $("#Content");
                    var metadataViewer = Controls.MetadataViewer.Index.init($('#orgMetadata'), organization == null ? availMetadata : organization.Metadata);
                    var domainVisViewer = CNDS.ManageVisibility.Index.init($('#orgVisibility'), organization == null ? availMetadata : organization.Metadata);
                    vm = new ViewModel(
                        screenPermissions,
                        organization,
                        orgAcls || [],
                        orgEvents || [],
                        organizationList,
                        securityGroupTree,
                        permissionList || [],
                        eventList || [],
                        registries,
                        registryList,
                        users,
                        datamarts,
                        securityGroups || [],
                        metadataViewer,
                        domainVisViewer,
                        bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            });
    }

    init();
}