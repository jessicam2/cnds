/// <reference path="../../../../../js/requests/details.ts" />
module Workflow.DistributedRegression.ConductAnalysis {
    var vm: ViewModel;

    export class ViewModel extends Global.WorkflowActivityViewModel {
       
        constructor(bindingControl: JQuery) {
            super(bindingControl);
            var self = this;

        }
    }

    function init() {
            $(() => {
                var bindingControl = $("#DRConductAnalysis");
                vm = new ViewModel(bindingControl);
                ko.applyBindings(vm, bindingControl[0]);

            });
    }

    init();
}