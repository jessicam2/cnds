/// <reference path="../../../../js/_layout.ts" />

module CNDS.Search.SelectRequestType {

    let vm: ViewModel;

    export class ViewModel extends Global.DialogViewModel {
        
        public onSelectRequestType: (item: RequestTypeSelectionItem, evt: JQueryEventObject) => void;

        public DataSources: NetworkDataSource[];

        public AvailableNetworkRequestTypes: KnockoutObservableArray<RequestTypeSelectionItem>;

        constructor(bindingControl: JQuery) {
            super(bindingControl);
            
            let self = this;
            self.DataSources = ko.utils.arrayMap(this.Parameters.DataSources, (ds: INetworkDataSource) => new NetworkDataSource(ds));

            self.AvailableNetworkRequestTypes = ko.observableArray([]);

            Dns.WebApi.CNDSRequestTypes.AvailableRequestTypesForNewRequest(self.DataSources.map(ds => ds.ID)).done((requestTypes) => {
                
                self.AvailableNetworkRequestTypes((requestTypes || []).map(rt => new RequestTypeSelectionItem(rt)));

                if (self.AvailableNetworkRequestTypes().length == 0) {
                    $('#txtLoadingMessage').text("No Request Types available for the selected Data Sources.");
                    $('#txtLoadingMessage').removeClass('alert-success').addClass('alert-danger');
                } else {
                    $('#LoadingBox').hide();
                }

            });

            
            
            self.onSelectRequestType = (item: RequestTypeSelectionItem, evt: JQueryEventObject) => {

                if (!item.HasValidRoutes) {
                    evt.preventDefault();
                    evt.stopPropagation();
                    return;
                }

                let promise: JQueryDeferred<any>;
                if (item.InvalidRoutes().length > 0) {
                    promise = Global.Helpers.ShowConfirm("Please Confirm:", '<p class="alert alert-warning" style="width:600px">One or more selected DataSources is not compatible with the selected RequestType. Incompatible DataSources will not be included as routes for the new Request.<br/><br />Do you wish to continue?</p>')
                } else {
                    promise = $.Deferred<any>();
                    promise.resolve();
                }
                promise.done(() => {
                    self.Close(item.toData());
                });
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

    export class RequestTypeSelectionItem extends Dns.ViewModels.CNDSSourceRequestTypeViewModel {
        constructor(item: Dns.Interfaces.ICNDSSourceRequestTypeDTO) {
            super(item);
        }

        get DisplayName(): string {
            return this.Project() + " / " + this.RequestType();
        }

        get HasValidRoutes(): boolean {
            return (this.ExternalRoutes().length > 0 || this.LocalRoutes().length > 0);
        }

        get CssClass(): string {
            if (this.LocalRoutes().length == 0 && this.ExternalRoutes().length == 0)
                return "disabled";

            let css = "";


            if (this.InvalidRoutes().length != 0 && this.HasValidRoutes) {
                css = "list-group-item-warning";
            } else if (this.InvalidRoutes().length == 0 && this.HasValidRoutes) {
                css = "list-group-item-success";
            }

            return css.trim();
        }

        get AnchorTitle(): string {
            if (this.InvalidRoutes().length > 0 && this.HasValidRoutes)
                return "Partially supported, not all selected DataSources support this request type.";

            return "";
        }
    }

    export interface INetworkDataSource {
        ID: any;
        Name: string;
        NetworkID: any;
        Network: string;
        OrganizationID: any;
        Organization: string;
    }

    export class NetworkDataSource {
        private _ds: INetworkDataSource;

        get ID(): any {
            return this._ds.ID;
        }

        get Name(): string {
            return this._ds.Name;
        }

        get NetworkID(): any {
            return this._ds.NetworkID;
        }

        get Network(): string {
            return this._ds.Network;
        }

        get OrganizationID(): any {
            return this._ds.OrganizationID;
        }

        get Organization(): string {
            return this._ds.Organization;
        }

        get NetworkName(): string {
            return "Network: " + this._ds.Network + " // Organization: " + this._ds.Organization;
        }

        constructor(ds: INetworkDataSource) {
            this._ds = ds;
        }
    }

}