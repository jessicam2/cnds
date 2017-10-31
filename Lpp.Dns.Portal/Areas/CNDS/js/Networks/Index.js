var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CNDS;
(function (CNDS) {
    var Networks;
    (function (Networks) {
        var Index;
        (function (Index) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl, networks) {
                    var _this = _super.call(this, bindingControl, null) || this;
                    var self = _this;
                    self.SelectedNetworks = ko.observableArray([]);
                    self.dsNetworks = kendo.data.DataSource.create({ data: networks });
                    self.onNetworkRowSelectionChange = function (e) {
                        var networks = [];
                        var grid = $(e.sender.wrapper).data('kendoGrid');
                        var rows = grid.select();
                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++) {
                                var request = grid.dataItem(rows[i]);
                                networks.push(request);
                            }
                        }
                        self.SelectedNetworks(networks);
                    };
                    self.onEditNetwork = function () {
                        if (self.SelectedNetworks().length == 0)
                            return;
                        Global.Helpers.ShowDialog('Edit Network', '/cnds/networks/edit', [], 500, 550, { Network: self.SelectedNetworks()[0] }).done(function (n) {
                            var updatedNetwork = n;
                            if (updatedNetwork != null) {
                                Dns.WebApi.CNDSNetworks.Update(updatedNetwork).done(function () {
                                    Dns.WebApi.CNDSNetworks.List().done(function (result) {
                                        self.dsNetworks.data(result);
                                    });
                                });
                            }
                        });
                    };
                    self.onNewNetwork = function () {
                        Global.Helpers.ShowDialog('New Network', '/cnds/networks/edit', [], 500, 550, { Network: new Dns.ViewModels.CNDSNetworkViewModel().toData() }).done(function (n) {
                            var newNetwork = n;
                            if (newNetwork != null) {
                                Dns.WebApi.CNDSNetworks.Register(newNetwork).done(function () {
                                    Dns.WebApi.CNDSNetworks.List().done(function (result) {
                                        self.dsNetworks.data(result);
                                    });
                                });
                            }
                        });
                    };
                    self.onDeleteNetwork = function () {
                        if (self.SelectedNetworks().length == 0)
                            return;
                        var selectedNetwork = self.SelectedNetworks()[0];
                        var msg = '<p>Please confirm you wish to delete the network: ' + selectedNetwork.Name + '.</p>';
                        Global.Helpers.ShowConfirm('Delete Network?', msg).done(function () {
                            Dns.WebApi.CNDSNetworks.Delete(selectedNetwork.ID).done(function () {
                                Dns.WebApi.CNDSNetworks.List().done(function (result) {
                                    self.dsNetworks.data(result);
                                });
                            });
                        });
                    };
                    return _this;
                }
                return ViewModel;
            }(Global.PageViewModel));
            Index.ViewModel = ViewModel;
            function init() {
                Dns.WebApi.CNDSNetworks.List().done(function (result) {
                    $(function () {
                        var bindingControl = $('#Content');
                        vm = new ViewModel(bindingControl, result);
                        ko.applyBindings(vm, bindingControl[0]);
                    });
                });
            }
            init();
        })(Index = Networks.Index || (Networks.Index = {}));
    })(Networks = CNDS.Networks || (CNDS.Networks = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=Index.js.map