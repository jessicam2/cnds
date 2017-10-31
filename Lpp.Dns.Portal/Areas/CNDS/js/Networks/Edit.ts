/// <reference path="../../../../js/_layout.ts" />
module CNDS.Networks.EditNetwork {
    var vm: ViewModel = null;

    export class ViewModel extends Global.DialogViewModel {

        public Network: Dns.ViewModels.CNDSNetworkViewModel;

        public onSubmit: () => void;

        constructor(bindingControl: JQuery) {
            super(bindingControl);

            var self = this;

            var network = <Dns.Interfaces.ICNDSNetworkDTO> this.Parameters.Network;
            self.Network = new Dns.ViewModels.CNDSNetworkViewModel(network);
            
            self.onSubmit = () => {
                if (self.Validate()) {
                    self.Close(self.Network.toData());
                }
            };
        }

        public onCancel() {
            this.Close(null);
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