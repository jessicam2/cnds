var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/// <reference path="../../../../js/_layout.ts" />
var CNDS;
(function (CNDS) {
    var ManagePermissions;
    (function (ManagePermissions) {
        var SecurityGroupWindow;
        (function (SecurityGroupWindow) {
            var vm;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    _this.dsSecurityGroup = new kendo.data.DataSource({
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
                    return _this;
                }
                ViewModel.prototype.AddSecurityGroup = function (arg) {
                    $.each(arg.sender.select(), function (count, item) {
                        var dataItem = arg.sender.dataItem(item);
                        vm.Close(dataItem);
                    });
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
        })(SecurityGroupWindow = ManagePermissions.SecurityGroupWindow || (ManagePermissions.SecurityGroupWindow = {}));
    })(ManagePermissions = CNDS.ManagePermissions || (CNDS.ManagePermissions = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=AddCNDSSecurityGroup.js.map