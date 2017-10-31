/// <reference path="../../../Lpp.Pmn.Resources/Scripts/page/5.1.0/Page.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Users;
(function (Users) {
    var SecurityGroupWindow;
    (function (SecurityGroupWindow) {
        var vm;
        var ViewModel = (function (_super) {
            __extends(ViewModel, _super);
            function ViewModel(bindingControl) {
                var _this = _super.call(this, bindingControl) || this;
                var self = _this;
                self.dsOrgResults = new kendo.data.DataSource({
                    type: "webapi",
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                    transport: {
                        read: {
                            url: Global.Helpers.GetServiceUrl("/organizations/list"),
                        }
                    },
                    schema: {
                        model: kendo.data.Model.define(Dns.Interfaces.KendoModelDataMartDTO)
                    },
                    sort: { field: "Name", dir: "asc" },
                });
                self.dsOrgSecResults = new kendo.data.DataSource({
                    data: []
                });
                self.dsProjResults = new kendo.data.DataSource({
                    type: "webapi",
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                    transport: {
                        read: {
                            url: Global.Helpers.GetServiceUrl("/projects/list?$select=ID,Name,Group, GroupID,Description"),
                        }
                    },
                    schema: {
                        model: kendo.data.Model.define(Dns.Interfaces.KendoModelDataMartDTO)
                    },
                    sort: { field: "Name", dir: "asc" },
                });
                self.dsProjSecResults = new kendo.data.DataSource({
                    data: []
                });
                self.dsProjSecResults = new kendo.data.DataSource({
                    data: []
                });
                self.dsCNDSSecResults = new kendo.data.DataSource({
                    data: []
                });
                Dns.WebApi.CNDSSecurity.SecurityGroupList().done(function (results) {
                    $("#gCNDSResults").data("kendoGrid").dataSource.data(results.map(function (r) {
                        return {
                            ID: r.ID,
                            Name: r.Name,
                            ParentSecurityGroup: null,
                            ParentSecurityGroupID: null,
                            Kind: CNDSSecurityGroupKinds.CNDS,
                            Path: 'CNDS\\' + r.Name,
                            Owner: "CNDS",
                            Type: CNDSSecurityGroupTypes.CNDS,
                            OwnerID: null,
                            Timestamp: null
                        };
                    }));
                    $("#gCNDSResults").data("kendoGrid").refresh();
                });
                return _this;
            }
            ViewModel.prototype.AddOrganization = function (arg) {
                $.each(arg.sender.select(), function (count, item) {
                    var dataItem = arg.sender.dataItem(item);
                    Dns.WebApi.SecurityGroups.List("OwnerID eq " + dataItem.ID).done(function (results) {
                        $("#gOrgSecResults").data("kendoGrid").dataSource.data(results);
                        $("#gOrgSecResults").data("kendoGrid").refresh();
                    });
                });
            };
            ViewModel.prototype.AddProject = function (arg) {
                $.each(arg.sender.select(), function (count, item) {
                    var dataItem = arg.sender.dataItem(item);
                    Dns.WebApi.SecurityGroups.List("OwnerID eq " + dataItem.ID).done(function (results) {
                        $("#gProjSecResults").data("kendoGrid").dataSource.data(results);
                        $("#gProjSecResults").data("kendoGrid").refresh();
                    });
                });
            };
            ViewModel.prototype.AddOrgGrp = function (arg) {
                $.each(arg.sender.select(), function (count, item) {
                    var dataItem = arg.sender.dataItem(item);
                    vm.Close({
                        ID: dataItem.ID,
                        Name: dataItem.Name,
                        ParentSecurityGroup: dataItem.ParentSecurityGroup,
                        ParentSecurityGroupID: dataItem.ParentSecurityGroupID,
                        Kind: dataItem.Kind,
                        Path: dataItem.Path,
                        Owner: dataItem.Owner,
                        Type: vm.ConvertToSecurityGroupEnum(dataItem.Type),
                        OwnerID: dataItem.OwnerID,
                        Timestamp: dataItem.Timestamp
                    });
                });
            };
            ViewModel.prototype.AddProjectGrp = function (arg) {
                $.each(arg.sender.select(), function (count, item) {
                    var dataItem = arg.sender.dataItem(item);
                    vm.Close({
                        ID: dataItem.ID,
                        Name: dataItem.Name,
                        ParentSecurityGroup: dataItem.ParentSecurityGroup,
                        ParentSecurityGroupID: dataItem.ParentSecurityGroupID,
                        Kind: dataItem.Kind,
                        Path: dataItem.Path,
                        Owner: dataItem.Owner,
                        Type: vm.ConvertToSecurityGroupEnum(dataItem.Type),
                        OwnerID: dataItem.OwnerID,
                        Timestamp: dataItem.Timestamp
                    });
                });
            };
            ViewModel.prototype.AddCNDSGrp = function (arg) {
                $.each(arg.sender.select(), function (count, item) {
                    var dataItem = arg.sender.dataItem(item);
                    vm.Close(dataItem);
                });
            };
            ViewModel.prototype.ConvertToSecurityGroupEnum = function (old) {
                switch (old) {
                    case Dns.Enums.SecurityGroupTypes.Project:
                        return CNDSSecurityGroupTypes.Project;
                    case Dns.Enums.SecurityGroupTypes.Organization:
                        return CNDSSecurityGroupTypes.Organization;
                }
            };
            ViewModel.prototype.ConvertToSecurityKindEnum = function (old) {
                switch (old) {
                    case Dns.Enums.SecurityGroupKinds.Administrators:
                        return CNDSSecurityGroupKinds.Administrators;
                    case Dns.Enums.SecurityGroupKinds.Custom:
                        return CNDSSecurityGroupKinds.Custom;
                    case Dns.Enums.SecurityGroupKinds.Everyone:
                        return CNDSSecurityGroupKinds.Everyone;
                    case Dns.Enums.SecurityGroupKinds.Investigators:
                        return CNDSSecurityGroupKinds.Investigators;
                    case Dns.Enums.SecurityGroupKinds.EnhancedInvestigators:
                        return CNDSSecurityGroupKinds.EnhancedInvestigators;
                    case Dns.Enums.SecurityGroupKinds.QueryAdministrators:
                        return CNDSSecurityGroupKinds.QueryAdministrators;
                    case Dns.Enums.SecurityGroupKinds.DataMartAdministrators:
                        return CNDSSecurityGroupKinds.DataMartAdministrators;
                    case Dns.Enums.SecurityGroupKinds.Observers:
                        return CNDSSecurityGroupKinds.Observers;
                    case Dns.Enums.SecurityGroupKinds.Users:
                        return CNDSSecurityGroupKinds.Users;
                    case Dns.Enums.SecurityGroupKinds.GroupDataMartAdministrator:
                        return CNDSSecurityGroupKinds.GroupDataMartAdministrator;
                }
            };
            return ViewModel;
        }(Global.DialogViewModel));
        SecurityGroupWindow.ViewModel = ViewModel;
        function init() {
            $(function () {
                var bindingControl = $("body");
                vm = new ViewModel(bindingControl);
                ko.applyBindings(vm, bindingControl[0]);
            });
        }
        init();
        var CNDSSecurityGroupTypes;
        (function (CNDSSecurityGroupTypes) {
            CNDSSecurityGroupTypes[CNDSSecurityGroupTypes["Organization"] = 1] = "Organization";
            CNDSSecurityGroupTypes[CNDSSecurityGroupTypes["Project"] = 2] = "Project";
            CNDSSecurityGroupTypes[CNDSSecurityGroupTypes["CNDS"] = 3] = "CNDS";
        })(CNDSSecurityGroupTypes = SecurityGroupWindow.CNDSSecurityGroupTypes || (SecurityGroupWindow.CNDSSecurityGroupTypes = {}));
        var CNDSSecurityGroupKinds;
        (function (CNDSSecurityGroupKinds) {
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Custom"] = 0] = "Custom";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Everyone"] = 1] = "Everyone";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Administrators"] = 2] = "Administrators";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Investigators"] = 3] = "Investigators";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["EnhancedInvestigators"] = 4] = "EnhancedInvestigators";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["QueryAdministrators"] = 5] = "QueryAdministrators";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["DataMartAdministrators"] = 6] = "DataMartAdministrators";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Observers"] = 7] = "Observers";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["Users"] = 8] = "Users";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["GroupDataMartAdministrator"] = 9] = "GroupDataMartAdministrator";
            CNDSSecurityGroupKinds[CNDSSecurityGroupKinds["CNDS"] = 10] = "CNDS";
        })(CNDSSecurityGroupKinds = SecurityGroupWindow.CNDSSecurityGroupKinds || (SecurityGroupWindow.CNDSSecurityGroupKinds = {}));
    })(SecurityGroupWindow = Users.SecurityGroupWindow || (Users.SecurityGroupWindow = {}));
})(Users || (Users = {}));
//# sourceMappingURL=SecurityGroups.js.map