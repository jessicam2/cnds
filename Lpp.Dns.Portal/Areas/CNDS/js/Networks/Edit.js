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
    var Networks;
    (function (Networks) {
        var EditNetwork;
        (function (EditNetwork) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    var network = _this.Parameters.Network;
                    self.Network = new Dns.ViewModels.CNDSNetworkViewModel(network);
                    self.onSubmit = function () {
                        if (self.Validate()) {
                            self.Close(self.Network.toData());
                        }
                    };
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    this.Close(null);
                };
                return ViewModel;
            }(Global.DialogViewModel));
            EditNetwork.ViewModel = ViewModel;
            function init() {
                $(function () {
                    var bindingControl = $('#Content');
                    vm = new ViewModel(bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            }
            EditNetwork.init = init;
            init();
        })(EditNetwork = Networks.EditNetwork || (Networks.EditNetwork = {}));
    })(Networks = CNDS.Networks || (CNDS.Networks = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=Edit.js.map