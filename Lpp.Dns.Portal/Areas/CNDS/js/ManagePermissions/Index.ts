/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManagePermissions.Index {
    var vm = null;
    export class ViewModel extends Global.PageViewModel {
        //Secuirty Groups
        public SecurityGroups: KnockoutObservableArray<Dns.Interfaces.ICNDSSecurityGroupDTO>;
        public newSecurityGroup: () => void;
        public editSecurityGroup: (edit: Dns.Interfaces.ICNDSSecurityGroupDTO) => void;
        public removeSecurityGroup: (remove: Dns.Interfaces.ICNDSSecurityGroupDTO) => void;
        public SecurityGroupsToRemove: KnockoutObservableArray<any> = ko.observableArray([]);
        public canSecurityGroupEdit: KnockoutObservable<boolean> = ko.observable(false);
        public canSecurityGroupRemove: KnockoutObservable<boolean> = ko.observable(false);
        public canSecurityGroupCreate: KnockoutObservable<boolean> = ko.observable(false);
        public isSecurityGroupsEdited: KnockoutObservable<boolean> = ko.observable(false);
        //Permissions
        public Acls: KnockoutObservableArray<Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel>;
        public Security: AclEditViewModel;
        public canViewPermissions: boolean;
        //Overall
        public onCancel: () => void;
        public onSave: () => void;
        public onTabSelect: (e) => void;
        constructor(securityGroups: Dns.Interfaces.ICNDSSecurityGroupDTO[], userPermissions: Dns.Interfaces.ICNDSAssignedPermissionDTO[], permissions: Dns.Interfaces.ICNDSPermissionsDTO[], securityGroupPermissions: Dns.Interfaces.ICNDSAssignedPermissionDTO[], bindingControl: JQuery) {
            super(bindingControl, null);
            var self = this;
            //Security Groups
            self.SecurityGroups = ko.observableArray(securityGroups);
            self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1 });
            self.newSecurityGroup = () => {
                Global.Helpers.ShowDialog('New Security Group', '/cnds/ManagePermissions/AddSecurityGroupDialog', ['Close'], 300, 250, null)
                    .done((results) => {
                        if (results == null)
                            return;
                        self.SecurityGroups.push(results);
                        self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1 });
                        self.isSecurityGroupsEdited(true);
                    });

            };
            self.editSecurityGroup = (edit: Dns.Interfaces.ICNDSSecurityGroupDTO) => {
                Global.Helpers.ShowDialog('Edit Security Group', '/cnds/ManagePermissions/AddSecurityGroupDialog', ['Close'], 300, 250, { isNew: false, Name: edit.Name, ID: edit.ID })
                    .done((results) => {
                        if (results == null)
                            return;
                        self.SecurityGroups.remove(edit)
                        self.SecurityGroups.push(results);
                        self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1 });
                        self.isSecurityGroupsEdited(true);
                    });

            };
            self.removeSecurityGroup = (remove: Dns.Interfaces.ICNDSSecurityGroupDTO) => {
                self.SecurityGroupsToRemove.push(remove.ID)
                self.SecurityGroups.remove(remove);
                self.isSecurityGroupsEdited(true);
            };
            //Permissions

            self.Acls = ko.observableArray(ko.utils.arrayMap(securityGroupPermissions, (item) => {
                return new Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel({
                    Allowed: item.Allowed,
                    SecurityGroupID: item.SecurityGroupID,
                    PermissionID: item.PermissionID,
                    Delete: false
                });
            }));

            self.Security = new AclEditViewModel(permissions, securityGroups, self.Acls, [], Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel);

            //Overall

            var sgEdit = ko.utils.arrayFilter(userPermissions, (item) => { return item.PermissionID.toLowerCase() == "10CF0001-451E-44ED-8388-A6BF012ED2D6".toLowerCase() });
            if (sgEdit.length > 0)
                self.canSecurityGroupEdit(sgEdit.every(per => per.Allowed == true))

            var sgCreate = ko.utils.arrayFilter(userPermissions, (item) => { return item.PermissionID.toLowerCase() == "E2A20001-1B7F-463E-8990-A6BF012ECC72".toLowerCase() });
            if (sgCreate.length > 0)
                self.canSecurityGroupCreate(sgCreate.every(per => per.Allowed == true))

            var sgRemove = ko.utils.arrayFilter(userPermissions, (item) => { return item.PermissionID.toLowerCase() == "25D50001-03BD-4EDE-9E6F-A6BF012ED91E".toLowerCase() });
            if (sgRemove.length > 0)
                self.canSecurityGroupRemove(sgRemove.every(per => per.Allowed == true))

            self.onCancel = () => {
                window.history.back();
            };
            self.onSave = () => {
                var perms: Dns.Interfaces.ICNDSUpdateAssignedPermissionDTO[] = []
                ko.utils.arrayForEach(self.Acls(), (a) => {
                    perms.push(<Dns.Interfaces.ICNDSUpdateAssignedPermissionDTO>{
                        SecurityGroupID: a.SecurityGroupID(),
                        PermissionID: a.PermissionID(),
                        Delete: a.Delete(),
                        Allowed: a.Allowed()
                    });
                });
                Dns.WebApi.CNDSSecurity.SetPermissions(perms).done(() => {
                    Dns.WebApi.CNDSSecurity.InsertOrUpdateSecurityGroups(self.SecurityGroups()).done(() => {
                        if (self.SecurityGroupsToRemove().length > 0) {
                            Dns.WebApi.CNDSSecurity.SecurityGroupDelete(self.SecurityGroupsToRemove()).done(() => {
                                window.location.reload();
                            });
                        }
                        else {
                            window.location.reload();
                        }
                    });
                });

            };
            self.onTabSelect = (e) => {
                if (self.isSecurityGroupsEdited()) {
                    e.preventDefault();
                    Global.Helpers.ShowAlert("Security Groups have been Changed", "<p>You have added, deleted, or edited a Security Group.  Please Save your changes before Continuing onto another screen</p>");
                }
                else
                    return
            }
        }



    }
    function init() {
        Dns.WebApi.CNDSSecurity.GetCurrentUserPermissions().done((userPermissions) => {
            Dns.WebApi.CNDSSecurity.SecurityGroupList().done((securityGroups) => {
                Dns.WebApi.CNDSSecurity.ListPermissions().done((permissions) => {
                    Dns.WebApi.CNDSSecurity.GetSecurityGroupPermissions().done((securityGroupPermissions) => {
                        $(() => {
                            var bindingControl = $('#Content');
                            vm = new ViewModel(securityGroups, userPermissions, permissions, securityGroupPermissions, bindingControl);
                            ko.applyBindings(vm, bindingControl[0]);
                        });
                    });
                });
            });
        });
    }
    init();


    export class AclEditViewModel
    {
        public Allowed: KnockoutObservable<boolean>;
        public Targets: AclTargets[];
        public PermissionID: KnockoutObservable<any>;
        public Permission: KnockoutObservable<string>;

        public SecurityGroupID: KnockoutObservable<any>;
        public SecurityGroup: KnockoutObservable<string>;
        public Overridden: KnockoutObservable<boolean>;
        public AclType;
        public Identifier: KnockoutObservable<string>;

        public SecurityGroupTree: Dns.Interfaces.ICNDSSecurityGroupDTO[];
        public AllAcls: KnockoutObservableArray<Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel>;
        public Acls: KnockoutComputed<Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel[]>;
        public Permissions: KnockoutObservableArray<PermissionListViewModel>;
        public SecurityGroups: Dns.Interfaces.ICNDSSecurityGroupDTO[];

        public SelectedSecurityGroup: KnockoutObservable<any>;

        public InheritSelectAll: () => void;
        public AllowSelectAll: () => void;
        public DenySelectAll: () => void;
        public SecurityGroupSelected: (e: kendo.ui.TreeViewSelectEvent) => boolean;
        public RemoveSecurityGroup: (data: AclEditViewModel) => void;
        public dsSecurityGroups: kendo.data.DataSource;

        constructor(permissions: Dns.Interfaces.ICNDSPermissionsDTO[], securityGroupTree: Dns.Interfaces.ICNDSSecurityGroupDTO[], acls: KnockoutObservableArray<Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel>, targets: AclTargets[], aclType, identifier: string = null) {
            var self = this;
            self.SecurityGroupTree = securityGroupTree;
            self.AclType = aclType;
            self.AllAcls = acls;
            self.Targets = targets;
            if (identifier == null)
                identifier = "";

            targets.forEach((t) => {
                identifier += t.Value;
            });
            self.Identifier = ko.observable(identifier);
            self.SecurityGroups = [];

            self.Acls = ko.computed(() => {
                return self.AllAcls().filter((a) => {
                    var isValid = true;
                    targets.forEach((t) => {
                        if (a[t.Field]() != t.Value) {
                            isValid = false;
                            return;
                        }
                    });
                    return isValid;
                });
            });
            var distinctIDs = ko.utils.arrayGetDistinctValues(ko.utils.arrayMap(acls(), (item) => { return item.SecurityGroupID() }))
            ko.utils.arrayForEach(distinctIDs, (item) => {
                self.SecurityGroups.push(ko.utils.arrayFirst(securityGroupTree, (sg) => { return sg.ID == item }))
            });
            self.SelectedSecurityGroup = ko.observable(null);
            self.Permissions = ko.observableArray(permissions.map((p) => {
                return new PermissionListViewModel(self, p);
            }));

            self.SelectedSecurityGroup(self.SecurityGroups.length == 0 ? null : self.SecurityGroups[0].ID);

            self.InheritSelectAll = () => {
                self.Permissions().forEach((p) => {
                    p.Allowed("Inherit");
                });
            };

            self.AllowSelectAll = () => {
                self.Permissions().forEach((p) => {
                    p.Allowed("Allow");
                });
            };

            self.DenySelectAll = () => {
                self.Permissions().forEach((p) => {
                    p.Allowed("Deny");
                });
            };


            self.dsSecurityGroups = new kendo.data.DataSource({
                data: self.SecurityGroups.sort((a, b) => a.Name < b.Name ? -1 : 1)
            });

            self.RemoveSecurityGroup = (data: AclEditViewModel) => {
                Global.Helpers.ShowConfirm("Confirmation", "<p>Are you sure that you want to remove the selected security group?</p>").done(() => {
                    //Remove all of the acls by setting the allowed to null
                    self.Acls().forEach((a) => {
                        if (a.SecurityGroupID() == self.SelectedSecurityGroup()) {
                            a.Allowed(false);
                            a.Delete(true);
                        }
                    });

                    //Remove the security group by id.
                    self.SecurityGroups.forEach((sg, index) => {

                        if (sg.ID == self.SelectedSecurityGroup()) {
                            self.SecurityGroups.splice(index, 1);

                            //Now refresh the combo etc.
                            var cboSecurityGroups: kendo.ui.DropDownList = $('#cboSecurityGroups').data("kendoDropDownList");

                            self.dsSecurityGroups.data(self.SecurityGroups);
                            cboSecurityGroups.setDataSource(self.dsSecurityGroups);
                            if (index > 0)
                                index--;

                            if (self.SecurityGroups.length > index) {
                                self.SelectedSecurityGroup(self.SecurityGroups[index].ID);
                                cboSecurityGroups.value(self.SelectedSecurityGroup());
                            } else {
                                self.SelectedSecurityGroup(null);
                            }
                            return;
                        }
                    });

                });
            }
        }

        public SelectSecurityGroup() {
            var self = this;
            Global.Helpers.ShowDialog("Add Security Group", "/CNDS/ManagePermissions/AddSecurityGroupPermissionDialog", ["close"], 550, 350).done((result: Dns.Interfaces.ICNDSSecurityGroupDTO) => {
                if (!result)
                    return;
                self.SecurityGroups.push(<any>{
                    ID: result.ID,
                    Name: result.Name
                });
                var cboSecurityGroups = $('#cboSecurityGroups').data("kendoDropDownList");

                self.dsSecurityGroups.data(self.SecurityGroups);
                cboSecurityGroups.setDataSource(self.dsSecurityGroups);
                cboSecurityGroups.value(result.ID);
                self.SelectedSecurityGroup(result.ID);
            });

        }

        public SecurityGroupMenu_Click(data, e: JQueryEventObject): boolean {
            e.stopPropagation();
            return false;
        }

        private SortItems(items: Dns.Interfaces.ITreeItemDTO[]) {
            items.sort((a, b) => {
                return a.Name < b.Name ? -1 : 1;
            });

            items.forEach((subitems: Dns.Interfaces.ITreeItemDTO) => {
                if (subitems.HasChildren)
                    this.SortItems(subitems.SubItems);
            });
        }

    }

    export interface AclTargets {
        Field: string;
        Value: any;
    }

    export class PermissionListViewModel{
        public ID: KnockoutObservable<any>;
        public Name: KnockoutObservable<string>;
        public Calculated: KnockoutObservable<string>;
        public Allowed: KnockoutObservable<string>;
        public Permission: KnockoutObservable<boolean> = ko.observable(false);
        public Delete: KnockoutObservable<boolean> = ko.observable(false);
        private VM: AclEditViewModel;
        public Description: KnockoutObservable<string>;

        constructor(vm: AclEditViewModel, permission: Dns.Interfaces.ICNDSPermissionsDTO) {
            var self = this;
            this.VM = vm;
            this.ID = ko.observable(permission.ID);
            this.Name = ko.observable(permission.Name);
            this.Calculated = ko.observable("");
            this.Allowed = ko.observable<string>("");
            this.Description = ko.observable(permission.Description);
            vm.SelectedSecurityGroup.subscribe((value) => {
                var acls = self.VM.Acls().filter((a) => {
                    return a.PermissionID() == self.ID() && a.SecurityGroupID() == value;
                });
                self.Permission(!acls || acls.length == 0 || acls[0].Allowed() == null ? false : acls[0].Allowed() ? true : false);
                self.Allowed(!acls || acls.length == 0 || acls[0].Allowed() == null ? "Inherit" : acls[0].Allowed() ? "Allow" : "Deny");
            });

            this.Allowed.subscribe((value) => {
                //This is hackery because of a bug in computeds in knockout where they're not firing updates unless the observable is inside this class.               
                var acls = self.VM.Acls().filter((a) => {
                    return a.PermissionID() == self.ID() && a.SecurityGroupID() == self.VM.SelectedSecurityGroup();
                });

                var acl: Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel = null;

                if (acls.length > 0)
                    acl = acls[0];

                switch (value) {
                    case "Inherit":
                        if (acl != null) {
                            acl.Delete(true);
                            acl.Allowed(false);
                        }
                        break;
                    case "Allow":
                        if (acl == null)
                            acl = this.CreateAcl(self.VM);
                        acl.Allowed(true);
                        acl.Delete(false);
                        break;
                    case "Deny":
                        if (acl == null)
                            acl = this.CreateAcl(self.VM);
                        acl.Delete(false);
                        acl.Allowed(false);
                        break;
                }
            });
        }

        public CreateAcl(vm: AclEditViewModel, securityGroupID: any = null) {
            var acl = new vm.AclType();
            acl.Allowed(false);
            acl.Delete(true);
            acl.PermissionID(this.ID());
            acl.SecurityGroupID(securityGroupID || vm.SelectedSecurityGroup());

            vm.AllAcls.push(acl);

            return acl;
        }

    }
    
}