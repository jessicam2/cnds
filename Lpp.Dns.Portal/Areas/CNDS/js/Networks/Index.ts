module CNDS.Networks.Index {
    var vm = null;
    export class ViewModel extends Global.PageViewModel {

        public dsNetworks: kendo.data.DataSource;

        public SelectedNetworks: KnockoutObservableArray<Dns.Interfaces.ICNDSNetworkDTO>;
        public onNetworkRowSelectionChange: (e) => void;
        public onEditNetwork: () => void;
        public onNewNetwork: () => void;
        public onDeleteNetwork: () => void;

        constructor(bindingControl: JQuery, networks: Dns.Interfaces.ICNDSNetworkDTO[]) {
            super(bindingControl, null);
            var self = this;

            self.SelectedNetworks = ko.observableArray([]);
            self.dsNetworks = kendo.data.DataSource.create({ data: networks });

            self.onNetworkRowSelectionChange = (e) => {
                let networks: Dns.Interfaces.ICNDSNetworkDTO[] = [];

                let grid = $(e.sender.wrapper).data('kendoGrid');
                let rows = grid.select();

                if (rows.length > 0) {
                    for (var i = 0; i < rows.length; i++) {
                        let request: any = grid.dataItem(rows[i]);
                        networks.push(request);
                    }
                }
                self.SelectedNetworks(networks);
            };


            self.onEditNetwork = () => {
                if (self.SelectedNetworks().length == 0)
                    return;

                Global.Helpers.ShowDialog('Edit Network', '/cnds/networks/edit', [], 500, 550, { Network: self.SelectedNetworks()[0] }).done((n) => {
                    let updatedNetwork: Dns.Interfaces.ICNDSNetworkDTO = n;
                    if (updatedNetwork != null) {
                        Dns.WebApi.CNDSNetworks.Update(updatedNetwork).done(() => {
                            Dns.WebApi.CNDSNetworks.List().done((result: Dns.Interfaces.ICNDSNetworkDTO[]) => {
                                self.dsNetworks.data(result);
                            });
                        });
                    }

                });

            };

            self.onNewNetwork = () => {
                Global.Helpers.ShowDialog('New Network', '/cnds/networks/edit', [], 500, 550, { Network: new Dns.ViewModels.CNDSNetworkViewModel().toData() }).done((n) => {
                    let newNetwork: Dns.Interfaces.ICNDSNetworkDTO = n;
                    if (newNetwork != null) {
                        Dns.WebApi.CNDSNetworks.Register(newNetwork).done(() => {
                            Dns.WebApi.CNDSNetworks.List().done((result: Dns.Interfaces.ICNDSNetworkDTO[]) => {
                                self.dsNetworks.data(result);
                            });
                        });
                    }

                });
            };

            self.onDeleteNetwork = () => {
                if (self.SelectedNetworks().length == 0)
                    return;

                let selectedNetwork = self.SelectedNetworks()[0];
                let msg = '<p>Please confirm you wish to delete the network: ' + selectedNetwork.Name + '.</p>';

                Global.Helpers.ShowConfirm('Delete Network?', msg).done(() => {
                    
                    Dns.WebApi.CNDSNetworks.Delete(selectedNetwork.ID).done(() => {
                        Dns.WebApi.CNDSNetworks.List().done((result: Dns.Interfaces.ICNDSNetworkDTO[]) => {
                            self.dsNetworks.data(result);
                        });
                    });
                });


            }
        }
    }

    function init() {
        Dns.WebApi.CNDSNetworks.List().done((result: Dns.Interfaces.ICNDSNetworkDTO[]) => {            
            $(() => {
                
                var bindingControl = $('#Content');
                vm = new ViewModel(bindingControl, result);
                ko.applyBindings(vm, bindingControl[0]);
            });
        });        
    }
    init();
}