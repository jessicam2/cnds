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
    var ManagePermissions;
    (function (ManagePermissions) {
        var Index;
        (function (Index) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(securityGroups, userPermissions, permissions, securityGroupPermissions, bindingControl) {
                    var _this = _super.call(this, bindingControl, null) || this;
                    _this.SecurityGroupsToRemove = ko.observableArray([]);
                    _this.canSecurityGroupEdit = ko.observable(false);
                    _this.canSecurityGroupRemove = ko.observable(false);
                    _this.canSecurityGroupCreate = ko.observable(false);
                    _this.isSecurityGroupsEdited = ko.observable(false);
                    var self = _this;
                    //Security Groups
                    self.SecurityGroups = ko.observableArray(securityGroups);
                    self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1; });
                    self.newSecurityGroup = function () {
                        Global.Helpers.ShowDialog('New Security Group', '/cnds/ManagePermissions/AddSecurityGroupDialog', ['Close'], 300, 250, null)
                            .done(function (results) {
                            if (results == null)
                                return;
                            self.SecurityGroups.push(results);
                            self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1; });
                            self.isSecurityGroupsEdited(true);
                        });
                    };
                    self.editSecurityGroup = function (edit) {
                        Global.Helpers.ShowDialog('Edit Security Group', '/cnds/ManagePermissions/AddSecurityGroupDialog', ['Close'], 300, 250, { isNew: false, Name: edit.Name, ID: edit.ID })
                            .done(function (results) {
                            if (results == null)
                                return;
                            self.SecurityGroups.remove(edit);
                            self.SecurityGroups.push(results);
                            self.SecurityGroups.sort(function (a, b) { return a.Name > b.Name ? 1 : -1; });
                            self.isSecurityGroupsEdited(true);
                        });
                    };
                    self.removeSecurityGroup = function (remove) {
                        self.SecurityGroupsToRemove.push(remove.ID);
                        self.SecurityGroups.remove(remove);
                        self.isSecurityGroupsEdited(true);
                    };
                    //Permissions
                    self.Acls = ko.observableArray(ko.utils.arrayMap(securityGroupPermissions, function (item) {
                        return new Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel({
                            Allowed: item.Allowed,
                            SecurityGroupID: item.SecurityGroupID,
                            PermissionID: item.PermissionID,
                            Delete: false
                        });
                    }));
                    self.Security = new AclEditViewModel(permissions, securityGroups, self.Acls, [], Dns.ViewModels.CNDSUpdateAssignedPermissionViewModel);
                    //Overall
                    var sgEdit = ko.utils.arrayFilter(userPermissions, function (item) { return item.PermissionID.toLowerCase() == "10CF0001-451E-44ED-8388-A6BF012ED2D6".toLowerCase(); });
                    if (sgEdit.length > 0)
                        self.canSecurityGroupEdit(sgEdit.every(function (per) { return per.Allowed == true; }));
                    var sgCreate = ko.utils.arrayFilter(userPermissions, function (item) { return item.PermissionID.toLowerCase() == "E2A20001-1B7F-463E-8990-A6BF012ECC72".toLowerCase(); });
                    if (sgCreate.length > 0)
                        self.canSecurityGroupCreate(sgCreate.every(function (per) { return per.Allowed == true; }));
                    var sgRemove = ko.utils.arrayFilter(userPermissions, function (item) { return item.PermissionID.toLowerCase() == "25D50001-03BD-4EDE-9E6F-A6BF012ED91E".toLowerCase(); });
                    if (sgRemove.length > 0)
                        self.canSecurityGroupRemove(sgRemove.every(function (per) { return per.Allowed == true; }));
                    self.onCancel = function () {
                        window.history.back();
                    };
                    self.onSave = function () {
                        var perms = [];
                        ko.utils.arrayForEach(self.Acls(), function (a) {
                            perms.push({
                                SecurityGroupID: a.SecurityGroupID(),
                                PermissionID: a.PermissionID(),
                                Delete: a.Delete(),
                                Allowed: a.Allowed()
                            });
                        });
                        Dns.WebApi.CNDSSecurity.SetPermissions(perms).done(function () {
                            Dns.WebApi.CNDSSecurity.InsertOrUpdateSecurityGroups(self.SecurityGroups()).done(function () {
                                if (self.SecurityGroupsToRemove().length > 0) {
                                    Dns.WebApi.CNDSSecurity.SecurityGroupDelete(self.SecurityGroupsToRemove()).done(function () {
                                        window.location.reload();
                                    });
                                }
                                else {
                                    window.location.reload();
                                }
                            });
                        });
                    };
                    self.onTabSelect = function (e) {
                        if (self.isSecurityGroupsEdited()) {
                            e.preventDefault();
                            Global.Helpers.ShowAlert("Security Groups have been Changed", "<p>You have added, deleted, or edited a Security Group.  Please Save your changes before Continuing onto another screen</p>");
                        }
                        else
                            return;
                    };
                    return _this;
                }
                return ViewModel;
            }(Global.PageViewModel));
            Index.ViewModel = ViewModel;
            function init() {
                Dns.WebApi.CNDSSecurity.GetCurrentUserPermissions().done(function (userPermissions) {
                    Dns.WebApi.CNDSSecurity.SecurityGroupList().done(function (securityGroups) {
                        Dns.WebApi.CNDSSecurity.ListPermissions().done(function (permissions) {
                            Dns.WebApi.CNDSSecurity.GetSecurityGroupPermissions().done(function (securityGroupPermissions) {
                                $(function () {
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
            var AclEditViewModel = (function () {
                function AclEditViewModel(permissions, securityGroupTree, acls, targets, aclType, identifier) {
                    if (identifier === void 0) { identifier = null; }
                    var self = this;
                    self.SecurityGroupTree = securityGroupTree;
                    self.AclType = aclType;
                    self.AllAcls = acls;
                    self.Targets = targets;
                    if (identifier == null)
                        identifier = "";
                    targets.forEach(function (t) {
                        identifier += t.Value;
                    });
                    self.Identifier = ko.observable(identifier);
                    self.SecurityGroups = [];
                    self.Acls = ko.computed(function () {
                        return self.AllAcls().filter(function (a) {
                            var isValid = true;
                            targets.forEach(function (t) {
                                if (a[t.Field]() != t.Value) {
                                    isValid = false;
                                    return;
                                }
                            });
                            return isValid;
                        });
                    });
                    var distinctIDs = ko.utils.arrayGetDistinctValues(ko.utils.arrayMap(acls(), function (item) { return item.SecurityGroupID(); }));
                    ko.utils.arrayForEach(distinctIDs, function (item) {
                        self.SecurityGroups.push(ko.utils.arrayFirst(securityGroupTree, function (sg) { return sg.ID == item; }));
                    });
                    self.SelectedSecurityGroup = ko.observable(null);
                    self.Permissions = ko.observableArray(permissions.map(function (p) {
                        return new PermissionListViewModel(self, p);
                    }));
                    self.SelectedSecurityGroup(self.SecurityGroups.length == 0 ? null : self.SecurityGroups[0].ID);
                    self.InheritSelectAll = function () {
                        self.Permissions().forEach(function (p) {
                            p.Allowed("Inherit");
                        });
                    };
                    self.AllowSelectAll = function () {
                        self.Permissions().forEach(function (p) {
                            p.Allowed("Allow");
                        });
                    };
                    self.DenySelectAll = function () {
                        self.Permissions().forEach(function (p) {
                            p.Allowed("Deny");
                        });
                    };
                    self.dsSecurityGroups = new kendo.data.DataSource({
                        data: self.SecurityGroups.sort(function (a, b) { return a.Name < b.Name ? -1 : 1; })
                    });
                    self.RemoveSecurityGroup = function (data) {
                        Global.Helpers.ShowConfirm("Confirmation", "<p>Are you sure that you want to remove the selected security group?</p>").done(function () {
                            //Remove all of the acls by setting the allowed to null
                            self.Acls().forEach(function (a) {
                                if (a.SecurityGroupID() == self.SelectedSecurityGroup()) {
                                    a.Allowed(false);
                                    a.Delete(true);
                                }
                            });
                            //Remove the security group by id.
                            self.SecurityGroups.forEach(function (sg, index) {
                                if (sg.ID == self.SelectedSecurityGroup()) {
                                    self.SecurityGroups.splice(index, 1);
                                    //Now refresh the combo etc.
                                    var cboSecurityGroups = $('#cboSecurityGroups').data("kendoDropDownList");
                                    self.dsSecurityGroups.data(self.SecurityGroups);
                                    cboSecurityGroups.setDataSource(self.dsSecurityGroups);
                                    if (index > 0)
                                        index--;
                                    if (self.SecurityGroups.length > index) {
                                        self.SelectedSecurityGroup(self.SecurityGroups[index].ID);
                                        cboSecurityGroups.value(self.SelectedSecurityGroup());
                                    }
                                    else {
                                        self.SelectedSecurityGroup(null);
                                    }
                                    return;
                                }
                            });
                        });
                    };
                }
                AclEditViewModel.prototype.SelectSecurityGroup = function () {
                    var self = this;
                    Global.Helpers.ShowDialog("Add Security Group", "/CNDS/ManagePermissions/AddSecurityGroupPermissionDialog", ["close"], 550, 350).done(function (result) {
                        if (!result)
                            return;
                        self.SecurityGroups.push({
                            ID: result.ID,
                            Name: result.Name
                        });
                        var cboSecurityGroups = $('#cboSecurityGroups').data("kendoDropDownList");
                        self.dsSecurityGroups.data(self.SecurityGroups);
                        cboSecurityGroups.setDataSource(self.dsSecurityGroups);
                        cboSecurityGroups.value(result.ID);
                        self.SelectedSecurityGroup(result.ID);
                    });
                };
                AclEditViewModel.prototype.SecurityGroupMenu_Click = function (data, e) {
                    e.stopPropagation();
                    return false;
                };
                AclEditViewModel.prototype.SortItems = function (items) {
                    var _this = this;
                    items.sort(function (a, b) {
                        return a.Name < b.Name ? -1 : 1;
                    });
                    items.forEach(function (subitems) {
                        if (subitems.HasChildren)
                            _this.SortItems(subitems.SubItems);
                    });
                };
                return AclEditViewModel;
            }());
            Index.AclEditViewModel = AclEditViewModel;
            var PermissionListViewModel = (function () {
                function PermissionListViewModel(vm, permission) {
                    var _this = this;
                    this.Permission = ko.observable(false);
                    this.Delete = ko.observable(false);
                    var self = this;
                    this.VM = vm;
                    this.ID = ko.observable(permission.ID);
                    this.Name = ko.observable(permission.Name);
                    this.Calculated = ko.observable("");
                    this.Allowed = ko.observable("");
                    this.Description = ko.observable(permission.Description);
                    vm.SelectedSecurityGroup.subscribe(function (value) {
                        var acls = self.VM.Acls().filter(function (a) {
                            return a.PermissionID() == self.ID() && a.SecurityGroupID() == value;
                        });
                        self.Permission(!acls || acls.length == 0 || acls[0].Allowed() == null ? false : acls[0].Allowed() ? true : false);
                        self.Allowed(!acls || acls.length == 0 || acls[0].Allowed() == null ? "Inherit" : acls[0].Allowed() ? "Allow" : "Deny");
                    });
                    this.Allowed.subscribe(function (value) {
                        //This is hackery because of a bug in computeds in knockout where they're not firing updates unless the observable is inside this class.               
                        var acls = self.VM.Acls().filter(function (a) {
                            return a.PermissionID() == self.ID() && a.SecurityGroupID() == self.VM.SelectedSecurityGroup();
                        });
                        var acl = null;
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
                                    acl = _this.CreateAcl(self.VM);
                                acl.Allowed(true);
                                acl.Delete(false);
                                break;
                            case "Deny":
                                if (acl == null)
                                    acl = _this.CreateAcl(self.VM);
                                acl.Delete(false);
                                acl.Allowed(false);
                                break;
                        }
                    });
                }
                PermissionListViewModel.prototype.CreateAcl = function (vm, securityGroupID) {
                    if (securityGroupID === void 0) { securityGroupID = null; }
                    var acl = new vm.AclType();
                    acl.Allowed(false);
                    acl.Delete(true);
                    acl.PermissionID(this.ID());
                    acl.SecurityGroupID(securityGroupID || vm.SelectedSecurityGroup());
                    vm.AllAcls.push(acl);
                    return acl;
                };
                return PermissionListViewModel;
            }());
            Index.PermissionListViewModel = PermissionListViewModel;
        })(Index = ManagePermissions.Index || (ManagePermissions.Index = {}));
    })(ManagePermissions = CNDS.ManagePermissions || (CNDS.ManagePermissions = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=Index.js.map