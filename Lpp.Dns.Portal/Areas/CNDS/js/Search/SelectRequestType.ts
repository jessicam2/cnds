/// <reference path="../../../../js/_layout.ts" />

module CNDS.Search.SelectRequestType {

    let vm: ViewModel;

    export class ViewModel extends Global.DialogViewModel {
        
        public onSelectRequestType: (item: RequestTypeSelectionItem, evt: JQueryEventObject) => void;

        public DataSources: NetworkDataSource[];
        public NetworkRequestTypes: KnockoutObservableArray<RequestTypeSelectionItem>;
        public DataSourceNames: KnockoutComputed<string>;
        public ShowOnlyAvailable: KnockoutObservable<boolean>;
        public AvailableNetworkRequestTypes: KnockoutComputed<RequestTypeSelectionItem[]>;

        constructor(bindingControl: JQuery) {
            super(bindingControl);
            
            let self = this;
            
            self.ShowOnlyAvailable = ko.observable(false);
            self.DataSources = ko.utils.arrayMap(this.Parameters.DataSources, (ds: INetworkDataSource) => new NetworkDataSource(ds));

            self.NetworkRequestTypes = ko.observableArray<RequestTypeSelectionItem>([]);

            Dns.WebApi.CNDSRequestTypes.AvailableRequestTypesForNewRequest().done((results) => {
                self.NetworkRequestTypes(ko.utils.arrayMap(results || [], (rt) => new RequestTypeSelectionItem(rt, self.DataSources)));
                if (self.NetworkRequestTypes().length == 0) {
                    $('#txtLoadingMessage').text("No request types available.");
                    $('#loadingMessageContainer').removeClass('alert-success').addClass('alert-danger');
                }
            });

            self.AvailableNetworkRequestTypes = ko.pureComputed(() => {
                return ko.utils.arrayFilter(self.NetworkRequestTypes(), (rt) => rt.Disabled != self.ShowOnlyAvailable() || rt.Disabled == false);
            });

            self.DataSourceNames = ko.pureComputed(() => {
                return ko.utils.arrayMap(self.DataSources, (ds) => ds.Name).join(", ");
            });
            
            self.onSelectRequestType = (item: RequestTypeSelectionItem, evt: JQueryEventObject) => {
                if (item.Disabled) {
                    evt.preventDefault();
                    evt.stopPropagation();
                    return;
                }

                let selection = {
                    RequestType: item.RequestType,
                    RequestTypeID: item.RequestTypeID,
                    Project: item.Project,
                    ProjectID: item.RequestTypeProjectID,
                    Routes: item.ValidRoutes
                };

                let promise: JQueryDeferred<any>;
                if (item.NotSupportedDataSources.length > 0) {
                    promise = Global.Helpers.ShowConfirm("Please Confirm:", '<p class="alert alert-warning" style="width:600px">One or more selected DataSources is not compatible with the selected RequestType. Incompatible DataSources will not be included as routes for the new Request.<br/><br />Do you wish to continue?</p>')
                } else {
                    promise = $.Deferred<any>();
                    promise.resolve();
                }
                promise.done(() => {
                    self.Close(selection);
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

    export interface IExternalDataSource {
        Name: string
    }

    export interface INetworkRequestTypeItem {
        Project: string,
        RequestType: string,
        DataSources: IExternalDataSource[]
        Supported: number;
    }

    export class RequestTypeSelectionItem {
        private RequestTypeItem: Dns.Interfaces.ICNDSExternalRequestTypeSelectionItemDTO;
        private _invalidRoutes: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO[];
        private _validRoutes: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO[];
        private _notSupportedDataSources: NetworkDataSource[];

        get Project(): string {
            return this.RequestTypeItem.Project;
        }

        get RequestType(): string {
            return this.RequestTypeItem.RequestType;
        }

        get RequestTypeID(): any {
            return this.RequestTypeItem.RequestTypeID;
        }

        get RequestTypeProjectID(): any {
            return this.RequestTypeItem.ProjectID;
        }

        get DisplayName(): string {
            return this.RequestTypeItem.Project + " / " + this.RequestTypeItem.RequestType;
        }

        get DisplayDataSources(): string {
            if (this._validRoutes.length == 0) {
                return "* No Supported DataSources *";
            }
            
            return $.map(this._validRoutes, (definition) => definition.Network + " // " + definition.Project + " // " + definition.DataMart).join(" &bull; ");
        }

        get ValidRoutes(): Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO[] {
            return this._validRoutes;
        }

        get NotSupportedDataSources(): NetworkDataSource[] {
            if (this._validRoutes.length == 0)
                return [];

            return this._notSupportedDataSources;
        }

        get Disabled(): boolean {
            return (this._validRoutes.length == 0);
        }

        get CssClass(): string {
            if (this._validRoutes.length == 0)
                return "disabled";

            let css = "";
            

            if (this._notSupportedDataSources.length > 0 && this._validRoutes.length > 0) {
                css = "list-group-item-warning";
            } else if (this._notSupportedDataSources.length == 0 && this._validRoutes.length > 0) {
                css = "list-group-item-success";
            }

            //if (vm.SelectedRequestType() != null && vm.SelectedRequestType() == this) {
            //    css += " SelectedRequestType";
            //}

            return css.trim();
        }

        get AnchorTitle(): string {
            if (this._notSupportedDataSources.length > 0 && this._validRoutes.length > 0)
                return "Partially supported, not all selected DataSources support this request type.";

            return "";
        }

        constructor(requestTypeItem: Dns.Interfaces.ICNDSExternalRequestTypeSelectionItemDTO, selectedDataSources: NetworkDataSource[]) {
            this.RequestTypeItem = requestTypeItem;

            this._invalidRoutes = [];
            this._validRoutes = [];
            this._notSupportedDataSources = [];

            for (let i = 0; i < requestTypeItem.MappingDefinitions.length; i++) {
                let definition = requestTypeItem.MappingDefinitions[i];
                
                let datasource = ko.utils.arrayFirst(selectedDataSources, (ds: NetworkDataSource) => { return ds.ID == definition.DataMartID; });
                if (datasource != null && ko.utils.arrayFirst(this._validRoutes, (ds: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO) => { return ds.DefinitionID == definition.DefinitionID; }) == null) {
                    //the mapping has a matching route for the selected datasource.
                    this._validRoutes.push(definition);
                } else {
                    //the mapping does not have a matching route for the selected datasource.
                    this._invalidRoutes.push(definition);
                }
            }

            if (requestTypeItem.MappingDefinitions != null && requestTypeItem.MappingDefinitions.length > 0) {
            //build the list of selected datasources that are not supported by this mapping definition
                for (let j = 0; j < selectedDataSources.length; j++) {
                    if (ko.utils.arrayFirst(requestTypeItem.MappingDefinitions, (md: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO) => { return md.DataMartID == selectedDataSources[j].ID; }) == null) {
                        this._notSupportedDataSources.push(selectedDataSources[j]);
                    }
                }
            } else {
            //none of the selected datasource are valid since there are no routes for the mapping
                this._notSupportedDataSources = selectedDataSources;
            }


        }

        
    }

    export function formatRouteName(item: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO): string {
        return item.Network + " | " + item.Project + " | " + item.DataMart + " - " + item.RequestType;
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