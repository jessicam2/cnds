/// <reference path="../../../../../Lpp.Pmn.Resources/scripts/typings/requirejs/require.d.ts" />
/// <reference path="../../../../js/_layout.ts" />
/// <reference path="common.ts" />
var CNDS;
(function (CNDS) {
    var Search;
    (function (Search) {
        var DataSources;
        (function (DataSources) {
            var vm = null;
            var ViewModel = (function () {
                function ViewModel(q, config) {
                    var self = this;
                    self.DisableNewRequestButton = ko.observable(false);
                    self.NumberOfSelectedDataSources = ko.observable(0);
                    self.SelectedDataSources = [];
                    self.SelectedRequestTypeDetails = ko.observable(null);
                    self.SelectedRequestTypeName = ko.pureComputed(function () {
                        if (self.SelectedRequestTypeDetails() != null) {
                            return self.SelectedRequestTypeDetails().RequestType;
                        }
                        return "";
                    });
                    self.SelectedRequestTypeProjectName = ko.pureComputed(function () {
                        if (self.SelectedRequestTypeDetails() != null) {
                            return self.SelectedRequestTypeDetails().Project;
                        }
                        return "";
                    });
                    self.SelectedRequestTypeRoutes = ko.pureComputed(function () {
                        if (self.SelectedRequestTypeDetails() != null) {
                            return self.SelectedRequestTypeDetails().Routes;
                        }
                        return [];
                    });
                    self.qlik = q;
                    self.app = self.qlik.openApp(config.dataSourceSearchAppID, config);
                    var table = self.app.getObject('QV01', config.dataSourceSearchObjectID);
                    var datasourcesTable = self.app.createTable(['NetworkID', 'datasource.Network', 'OrganizationID', 'datasource.Organization', 'DataSourceID', 'datasource.Name', 'datasource.DataSourceAdapterSupportedID'], []);
                    datasourcesTable.OnData.bind(function () {
                        if (datasourcesTable.rowCount > 0) {
                            var validSelections = ko.utils.arrayMap(ko.utils.arrayFilter(datasourcesTable.rows, function (row) {
                                var adapterID = row.cells[6].qText;
                                return adapterID != null && adapterID.length > 8;
                            }), function (row) {
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
                        }
                        else {
                            //no rows available
                            self.NumberOfSelectedDataSources(0);
                            self.DisableNewRequestButton(true);
                        }
                    });
                    self.onSelectRequestType = function () {
                        self.DisableNewRequestButton(true);
                        $.noConflict();
                        Global.Helpers.ShowDialog('Select the Request Type', '/CNDS/Search/SelectRequestType', [], 800, 700, { DataSources: self.SelectedDataSources }).done(function (result) {
                            if (result == null) {
                                self.DisableNewRequestButton(false);
                                self.SelectedRequestTypeDetails(null);
                                return;
                            }
                            self.SelectedRequestTypeDetails(result);
                            Global.Helpers.ShowDialog('Request Form', '/cnds/search/createrequest', [], 800, 700, result).done(function (request) {
                                if (!request) {
                                    self.DisableNewRequestButton(false);
                                    return;
                                }
                                Global.Helpers.ShowExecuting();
                                //save the request, navigate to the request details page                        
                                Dns.WebApi.Requests.CreateRequest({
                                    Comment: null,
                                    Data: null,
                                    Routes: self.SelectedRequestTypeDetails().Routes,
                                    DemandActivityResultID: null,
                                    Dto: request
                                }).done(function (res) {
                                    Global.Helpers.RedirectTo(res[0].Uri);
                                });
                            });
                        });
                    };
                }
                return ViewModel;
            }());
            DataSources.ViewModel = ViewModel;
            function init() {
                $.getJSON('/cnds/search/qlikconfiguration').done(function (config) {
                    require.config({
                        baseUrl: (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources",
                        paths: {
                            "qlik": (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources/js/qlik"
                        }
                    });
                    $(function () {
                        require(["js/qlik"], function (qlik) {
                            vm = new ViewModel(qlik, config);
                            var bindingControl = $('#Content');
                            ko.applyBindings(vm, bindingControl[0]);
                        });
                    });
                });
            }
            init();
        })(DataSources = Search.DataSources || (Search.DataSources = {}));
    })(Search = CNDS.Search || (CNDS.Search = {}));
})(CNDS || (CNDS = {}));
//# sourceMappingURL=DataSources-Qlik.js.map