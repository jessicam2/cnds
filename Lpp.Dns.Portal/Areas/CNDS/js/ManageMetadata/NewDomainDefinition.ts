/// <reference path="../../../../js/_layout.ts" />
module CNDS.ManageMetadata.NewDomainDefinition {

    var vm: ViewModel = null;

    export class ViewModel extends Global.DialogViewModel {
    
        public DomainTitle: KnockoutObservable<string>;
        public DomainDataType: KnockoutObservable<string>;
        public DataTypes: Dns.Structures.KeyValuePair[];

        constructor(bindingControl: JQuery) {
            super(bindingControl);

            this.DomainTitle = ko.observable<string>();
            this.DomainDataType = ko.observable<string>();

            this.DataTypes = [
                { text: 'Container', value: 'container' },
                { text: 'Text', value: 'string' },
                { text: 'Whole Number', value: 'int' },
                { text: 'True|False', value: 'boolean' },
                { text: 'Reference', value: 'reference' },
                { text: 'Boolean Group', value: 'booleanGroup' }
            ];
        }

        public onCancel() {
            this.Close(null);
        }

        public onSubmit(viewModel: ViewModel) {
            if (this.DomainDataType() == undefined || this.DomainDataType() == null || this.DomainDataType() == '-1')
                return;
            if (this.Validate()) {
                var newMetadata: Dns.Interfaces.IMetadataDTO = { ID: Constants.Guid.newGuid(), Title: this.DomainTitle(), DataType: this.DomainDataType(), Description: null, IsMultiValue: false, EntityType: null, DomainUseID: null, Value: null, ChildMetadata: null, References: null, Visibility: 0 };
                this.Close(newMetadata);
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