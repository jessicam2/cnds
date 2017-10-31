/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManagePermissions.SecurityGroupWindow {
    var vm: ViewModel;

    export class ViewModel extends Global.DialogViewModel {
        public dsSecurityGroup: kendo.data.DataSource;

        constructor(bindingControl: JQuery) {
            super(bindingControl);
            this.dsSecurityGroup = new kendo.data.DataSource({
                type: "webapi",
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                transport: {
                    read: {
                        url: Global.Helpers.GetServiceUrl("/CNDSSecurity/SecurityGroupList"),
                    }
                },
                sort: { field: "Name", dir: "asc" },
            });
        }

        public AddSecurityGroup(arg: kendo.ui.GridChangeEvent) {
            $.each(arg.sender.select(), (count: number, item: JQuery) => {
                var dataItem: any = arg.sender.dataItem(item);
                vm.Close(dataItem);
            });
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
}