/// <reference path="../../../Lpp.Pmn.Resources/Scripts/page/5.1.0/Page.ts" />

module Users.SecurityGroupWindow {
    var vm: ViewModel;

    export class ViewModel extends Global.DialogViewModel {
        public dsOrgResults: kendo.data.DataSource;
        public dsOrgSecResults: kendo.data.DataSource;
        public dsProjResults: kendo.data.DataSource;
        public dsProjSecResults: kendo.data.DataSource;
        public dsCNDSResults: kendo.data.DataSource;
        public dsCNDSSecResults: kendo.data.DataSource;

        constructor(bindingControl: JQuery) {
            super(bindingControl);
            var self = this;
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

            Dns.WebApi.CNDSSecurity.SecurityGroupList().done((results: Dns.Interfaces.ICNDSSecurityGroupDTO[]) => {
                $("#gCNDSResults").data("kendoGrid").dataSource.data(results.map((r) => {
                    return <ICNDSSecurityGroupDTO>{
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
                    }
                }));
                $("#gCNDSResults").data("kendoGrid").refresh();
            });
        }

        public AddOrganization(arg: kendo.ui.GridChangeEvent) {

            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                Dns.WebApi.SecurityGroups.List("OwnerID eq " + dataItem.ID).done((results) => {
                    $("#gOrgSecResults").data("kendoGrid").dataSource.data(results);
                    $("#gOrgSecResults").data("kendoGrid").refresh();
                })
            });
        }

        public AddProject(arg: kendo.ui.GridChangeEvent) {

            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                Dns.WebApi.SecurityGroups.List("OwnerID eq " + dataItem.ID).done((results) => {
                    $("#gProjSecResults").data("kendoGrid").dataSource.data(results);
                    $("#gProjSecResults").data("kendoGrid").refresh();
                })
            });
        }

        public AddOrgGrp(arg: kendo.ui.GridChangeEvent) {
      
            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                vm.Close(<ICNDSSecurityGroupDTO>{
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
        }

        public AddProjectGrp(arg: kendo.ui.GridChangeEvent) {
            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                vm.Close(<ICNDSSecurityGroupDTO>{
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
        }

        public AddCNDSGrp(arg: kendo.ui.GridChangeEvent) {
            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                vm.Close(dataItem);
            });
        }
        public ConvertToSecurityGroupEnum(old: Dns.Enums.SecurityGroupTypes): CNDSSecurityGroupTypes {
            switch (old) {
                case Dns.Enums.SecurityGroupTypes.Project:
                    return CNDSSecurityGroupTypes.Project;
                case Dns.Enums.SecurityGroupTypes.Organization:
                    return CNDSSecurityGroupTypes.Organization;
            }
        }

        public ConvertToSecurityKindEnum(old: Dns.Enums.SecurityGroupKinds): CNDSSecurityGroupKinds {
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
        }
    }

    function init() {
        $(() => {
            var bindingControl = $("body");
            vm = new ViewModel(bindingControl);
            ko.applyBindings(vm, bindingControl[0]);
        });
    }

    init();

    export enum CNDSSecurityGroupTypes {
        Organization = 1,
        Project = 2,
        CNDS = 3
    }

    export enum CNDSSecurityGroupKinds {
        Custom = 0,
        Everyone = 1,
        Administrators = 2,
        Investigators = 3,
        EnhancedInvestigators = 4,
        QueryAdministrators = 5,
        DataMartAdministrators = 6,
        Observers = 7,
        Users = 8,
        GroupDataMartAdministrator = 9,
        CNDS = 10
    }

    export interface ICNDSSecurityGroupDTO extends Dns.Interfaces.IEntityDtoWithID {
        Name: string;
        Path: string;
        OwnerID: any;
        Owner: string;
        ParentSecurityGroupID?: any;
        ParentSecurityGroup: string;
        Kind: CNDSSecurityGroupKinds;
        Type: CNDSSecurityGroupTypes;
    }
}