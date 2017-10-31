/// <reference path="../../../../../Lpp.Pmn.Resources/scripts/typings/requirejs/require.d.ts" />
/// <reference path="../../../../js/_layout.ts" />
/// <reference path="common.ts" />

module CNDS.Search.DataSources {
    var vm = null;

    export class ViewModel {
        private qlik: any;
        private app: any;
        
        public onSelectRequestType: () => void;
        public NumberOfSelectedDataSources: KnockoutObservable<number>;
        public DisableNewRequestButton: KnockoutObservable<boolean>;
        private SelectedDataSources: any;
        public SelectedRequestTypeDetails: KnockoutObservable<CNDS.Search.Interfaces.IRequestTypeSelection>;
        public SelectedRequestTypeName: KnockoutComputed<string>;
        public SelectedRequestTypeProjectName: KnockoutComputed<string>;
        public SelectedRequestTypeRoutes: KnockoutComputed<Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO[]>;

        constructor(q: any, config: any) {
            var self = this;

            self.DisableNewRequestButton = ko.observable(false);
            self.NumberOfSelectedDataSources = ko.observable(0);
            self.SelectedDataSources = [];
            self.SelectedRequestTypeDetails = ko.observable(null);
            self.SelectedRequestTypeName = ko.pureComputed(() => {
                if (self.SelectedRequestTypeDetails() != null) {
                    return self.SelectedRequestTypeDetails().RequestType;
                }
                return "";
            });

            self.SelectedRequestTypeProjectName = ko.pureComputed(() => {
                if (self.SelectedRequestTypeDetails() != null) {
                    return self.SelectedRequestTypeDetails().Project;
                }
                return "";
            });

            self.SelectedRequestTypeRoutes = ko.pureComputed(() => {
                if (self.SelectedRequestTypeDetails() != null) {
                    return self.SelectedRequestTypeDetails().Routes;
                }
                return [];
            });

            self.qlik = q;
            
            self.app = self.qlik.openApp(config.dataSourceSearchAppID, config);
            let table = self.app.getObject('QV01', config.dataSourceSearchObjectID);
            

            let datasourcesTable = self.app.createTable(['NetworkID', 'datasource.Network', 'OrganizationID', 'datasource.Organization', 'DataSourceID', 'datasource.Name', 'datasource.DataSourceAdapterSupportedID'], []);
            datasourcesTable.OnData.bind(() => {
                if (datasourcesTable.rowCount > 0) {

                    var validSelections = ko.utils.arrayMap(ko.utils.arrayFilter(datasourcesTable.rows, (row: any) => {
                        let adapterID = row.cells[6].qText;
                        return adapterID != null && adapterID.length > 8;
                    }), (row) => {
                        return {
                            NetworkID: row.cells[0].qText,
                            Network: row.cells[1].qText,
                            OrganizationID: row.cells[2].qText,
                            Organization: row.cells[3].qText,
                            ID: row.cells[4].qText,
                            Name: row.cells[5].qText
                        };
                    });

                    self.NumberOfSelectedDataSources(validSelections.length);
                    self.DisableNewRequestButton(validSelections.length == 0);
                    self.SelectedDataSources = validSelections;

                } else {
                    //no rows available
                    self.NumberOfSelectedDataSources(0);
                    self.DisableNewRequestButton(true);
                }
            });

            self.onSelectRequestType = () => {
                self.DisableNewRequestButton(true);

                $.noConflict();

                Global.Helpers.ShowDialog('Select the Request Type', '/CNDS/Search/SelectRequestType', [], 800, 700, { DataSources: self.SelectedDataSources }).done((result: CNDS.Search.Interfaces.IRequestTypeSelection) => {
                    if (result == null) {
                        self.DisableNewRequestButton(false);
                        self.SelectedRequestTypeDetails(null);
                        return;
                    }

                    self.SelectedRequestTypeDetails(result);

                    Global.Helpers.ShowDialog('Request Form', '/cnds/search/createrequest', [], 800, 700, result).done((request: Dns.Interfaces.IRequestDTO) => {
                        if (!request) {
                            self.DisableNewRequestButton(false);
                            return;
                        }

                        Global.Helpers.ShowExecuting();

                        //save the request, navigate to the request details page                        
                        Dns.WebApi.Requests.CreateRequest(<Dns.Interfaces.ICreateCNDSRequestDetailsDTO>{
                            Comment: null,
                            Data: null,
                            Routes: self.SelectedRequestTypeDetails().Routes,
                            DemandActivityResultID: null,
                            Dto: request
                        }).done((res: Dns.Interfaces.IRequestCompletionResponseDTO[]) => {
                            Global.Helpers.RedirectTo(res[0].Uri);
                        });

                    });
                });
            };

        }
    }

    function init() {

        $.getJSON('/cnds/search/qlikconfiguration').done((config: any) => {
            
            require.config({
                baseUrl: (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources",
                paths: {
                    "qlik": (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources/js/qlik"
                }
            });
            
            $(() => {
                require(["js/qlik"], function (qlik) {
                    vm = new ViewModel(qlik, config);
                    var bindingControl = $('#Content');
                    ko.applyBindings(vm, bindingControl[0]);
                });

            });
        });

        
    }

    init();
    
}