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
/// <reference path="../../../../../js/requests/details.ts" />
var Workflow;
(function (Workflow) {
    var DistributedRegression;
    (function (DistributedRegression) {
        var ConductAnalysis;
        (function (ConductAnalysis) {
            var vm;
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(bindingControl) {
                    var _this = _super.call(this, bindingControl) || this;
                    var self = _this;
                    return _this;
                }
                return ViewModel;
            }(Global.WorkflowActivityViewModel));
            ConductAnalysis.ViewModel = ViewModel;
            function init() {
                $(function () {
                    var bindingControl = $("#DRConductAnalysis");
                    vm = new ViewModel(bindingControl);
                    ko.applyBindings(vm, bindingControl[0]);
                });
            }
            init();
        })(ConductAnalysis = DistributedRegression.ConductAnalysis || (DistributedRegression.ConductAnalysis = {}));
    })(DistributedRegression = Workflow.DistributedRegression || (Workflow.DistributedRegression = {}));
})(Workflow || (Workflow = {}));
//# sourceMappingURL=ConductAnalysis.js.map