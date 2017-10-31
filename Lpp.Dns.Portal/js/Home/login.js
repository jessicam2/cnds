/// <reference path="../_rootlayout.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Home;
(function (Home) {
    var Login;
    (function (Login) {
        var vm;
        var ViewModel = (function (_super) {
            __extends(ViewModel, _super);
            function ViewModel(bindingControl, themeResult) {
                var _this = _super.call(this, bindingControl) || this;
                _this.SystemUserConfirmationTitle = themeResult.SystemUserConfirmationTitle;
                _this.SystemUserConfirmationContent = themeResult.SystemUserConfirmationContent;
                return _this;
            }
            ViewModel.prototype.ShowTerms = function () {
                Global.ShowTerms(Layout.vmFooter.Theme.Terms());
            };
            ViewModel.prototype.ShowInfo = function () {
                Global.ShowInfo(Layout.vmFooter.Theme.Info());
            };
            ViewModel.prototype.ForgotPassword = function () {
                Global.Helpers.ShowDialog("Forgot Password", "/home/forgotpassword", ["close"], 800, 350, { Username: $('#txtUserName').val() });
            };
            ViewModel.prototype.Registration = function () {
                Global.Helpers.ShowDialog("User Registration", "/home/userregistration", ["close"], 900, 550);
            };
            ViewModel.prototype.Submit = function () {
                var formElement = $("#fLogin");
                if (!this.Validate())
                    return;
                Global.Helpers.ShowConfirm(this.SystemUserConfirmationTitle, this.SystemUserConfirmationContent)
                    .done(function () { formElement.submit(); });
            };
            return ViewModel;
        }(Global.PageViewModel));
        Login.ViewModel = ViewModel;
        function init() {
            Dns.WebApi.Theme.GetText(theme, ["SystemUserConfirmationTitle", "SystemUserConfirmationContent"]).done(function (themeResult) {
                $(function () {
                    var bindingControl = $("#fLogin");
                    vm = new ViewModel(bindingControl, themeResult[0]);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            });
        }
        init();
    })(Login = Home.Login || (Home.Login = {}));
})(Home || (Home = {}));
//# sourceMappingURL=login.js.map