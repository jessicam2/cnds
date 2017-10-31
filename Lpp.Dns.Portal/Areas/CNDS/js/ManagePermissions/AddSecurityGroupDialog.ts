/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManagePermissions.AddSecurityGroup {
    var vm: ViewModel = null;
    export class ViewModel extends Global.DialogViewModel {
        public ID: KnockoutObservable<any> = ko.observable(Constants.GuidEmpty);
        public Name: KnockoutObservable<string> = ko.observable("");
        constructor(bindingControl: JQuery) {
            super(bindingControl);
            if (this.Parameters != null && this.Parameters.isNew != undefined && this.Parameters.isNew == false) {
                this.ID(this.Parameters.ID);
                this.Name(this.Parameters.Name);
            }
        }

        public onCancel() {
            this.Close(null);
        }

        public onSubmit(viewModel: ViewModel) {
            if (this.Name() == undefined || this.Name() == null || this.Name() == '')
                return;
            if (this.Validate()) {
                var newSecurityGroup: Dns.Interfaces.ICNDSSecurityGroupDTO = { ID: this.ID(), Name: this.Name()};
                this.Close(newSecurityGroup);
            }
        }

    }


    export function init() {

        $(() => {
            var bindingControl = $('#Content');
            vm = new ViewModel(bindingControl);
            ko.applyBindings(vm, bindingControl[0]);
        });
    }

    init();


}