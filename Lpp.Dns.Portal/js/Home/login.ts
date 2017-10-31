/// <reference path="../_rootlayout.ts" />

module Home.Login {
    var vm: ViewModel;

    export class ViewModel extends Global.PageViewModel {
        private SystemUserConfirmationTitle: string;
        private SystemUserConfirmationContent: string;

        constructor(bindingControl: JQuery, themeResult: Dns.Interfaces.IThemeDTO) {
            super(bindingControl);

            this.SystemUserConfirmationTitle = themeResult.SystemUserConfirmationTitle;
            this.SystemUserConfirmationContent = themeResult.SystemUserConfirmationContent;
           
        }

        public ShowTerms() {
            Global.ShowTerms(Layout.vmFooter.Theme.Terms());
        }

        public ShowInfo() {
            Global.ShowInfo(Layout.vmFooter.Theme.Info());
        }

        public ForgotPassword() {
            Global.Helpers.ShowDialog("Forgot Password", "/home/forgotpassword", ["close"], 800, 350, { Username: $('#txtUserName').val() });
        }

        public Registration() {
            Global.Helpers.ShowDialog("User Registration", "/home/userregistration", ["close"], 900, 550);
        }

        public Submit() {
            var formElement: JQuery = $("#fLogin");
            if (!this.Validate())
                return;
            Global.Helpers.ShowConfirm(this.SystemUserConfirmationTitle, this.SystemUserConfirmationContent)
                .done(() => { formElement.submit(); });
        }
    }

    function init() {
        Dns.WebApi.Theme.GetText(theme, ["SystemUserConfirmationTitle", "SystemUserConfirmationContent"]).done(
            (themeResult) => {
                $(() => {
                    var bindingControl = $("#fLogin");
                    vm = new ViewModel(bindingControl, themeResult[0]);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            });
    }

    init();
}