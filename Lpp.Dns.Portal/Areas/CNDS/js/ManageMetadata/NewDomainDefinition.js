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
    var ManageMetadata;
    (function (ManageMetadata) {
        var NewDomainDefinition;
        (function (NewDomainDefinition) {
            var vm = null;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    _this.DomainTitle = ko.observable();
                    _this.DomainDataType = ko.observable();
                    _this.DataTypes = [
                        { text: 'Container', value: 'container' },
                        { text: 'Text', value: 'string' },
                        { text: 'Whole Number', value: 'int' },
                        { text: 'True|False', value: 'boolean' },
                        { text: 'Reference', value: 'reference' },
                        { text: 'Boolean Group', value: 'booleanGroup' }
                    ];
                    return _this;
                }
                ViewModel.prototype.onCancel = function () {
                    this.Close(null);
                };
                ViewModel.prototype.onSubmit = function (viewModel) {
                    if (this.DomainDataType() == undefined || this.DomainDataType() == null || this.DomainDataType() == '-1')
                        return;
                    if (this.Validate()) {
                        var newMetadata = { ID: Constants.Guid.newGuid(), Title: this.DomainTitle(), DataType: this.DomainDataType(), Description: null, IsMultiValue: false, EntityType: null, DomainUseID: null, Value: null, ChildMetadata: null, References: null, Visibility: 0 };
                        this.Close(newMetadata);
                    }
                };
                return ViewModel;
            }(Global.DialogViewModel));
            NewDomainDefinition.ViewModel = ViewModel;
            function init() {
                $(function () {
                    var bindingControl = $('#Content');
                    vm = new ViewModel(bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            }
            NewDomainDefinition.init = init;
            init();
        })(NewDomainDefinition = ManageMetadata.NewDomainDefinition || (ManageMetadata.NewDomainDefinition = {}));
    })(ManageMetadata = CNDS.ManageMetadata || (CNDS.ManageMetadata = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=NewDomainDefinition.js.map