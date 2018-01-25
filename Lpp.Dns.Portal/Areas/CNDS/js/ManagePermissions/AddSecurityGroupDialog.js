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
        var AddSecurityGroup;
        (function (AddSecurityGroup) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    _this.ID = ko.observable(Constants.GuidEmpty);
                    _this.Name = ko.observable("");
                    if (_this.Parameters != null && _this.Parameters.isNew != undefined && _this.Parameters.isNew == false) {
                        _this.ID(_this.Parameters.ID);
                        _this.Name(_this.Parameters.Name);
                    }
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    this.Close(null);
                };
                ViewModel.prototype.onSubmit = function (viewModel) {
                    if (this.Name() == undefined || this.Name() == null || this.Name() == '')
                        return;
                    if (this.Validate()) {
                        var newSecurityGroup = { ID: this.ID(), Name: this.Name() };
                        this.Close(newSecurityGroup);
                    }
                };
                return ViewModel;
            }(Global.DialogViewModel));
            AddSecurityGroup.ViewModel = ViewModel;
            function init() {
                $(function () {
                    var bindingControl = $('#Content');
                    vm = new ViewModel(bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            }
            AddSecurityGroup.init = init;
            init();
        })(AddSecurityGroup = ManagePermissions.AddSecurityGroup || (ManagePermissions.AddSecurityGroup = {}));
    })(ManagePermissions = CNDS.ManagePermissions || (CNDS.ManagePermissions = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=AddSecurityGroupDialog.js.map