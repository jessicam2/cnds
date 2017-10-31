/// <reference path="../../../Lpp.Pmn.Resources/scripts/typings/requirejs/require.d.ts" />
var QlikItegration;
(function (QlikItegration) {
    var SelectionTest;
    (function (SelectionTest) {
        var vm;
        var ViewModel = (function () {
            function ViewModel(q) {
                this.CurrentData = null;
                var self = this;
                self.Log = ko.observableArray();
                self.qlik = q;
                self.app = self.qlik.openApp('24851d74-3ad1-4d11-9927-775bc97eb31e', config);
                //let getCubeData = (model) => {
                //    model.getHyperCubeData('/qHyperCubeDef', [{
                //        qTop: 0,
                //        qLeft: 0,
                //        qWidth: 10,
                //        qHeight: 1000
                //    }]).then((data) => {
                //        if (data && data.qDataPages.length > 0) {
                //            self.CurrentData = data.qDataPages[0].qMatrix;
                //        } else {
                //            self.CurrentData = [];
                //        }
                //    });
                //};
                var table = self.app.getObject('QV01', 'XLbKc');
                //table.then((model) => {
                //    model.Validated.bind(() => {
                //        getCubeData(model);                    
                //    });
                //    //initial data
                //    getCubeData(model);
                //});
                self.CurrentData = self.app.createTable(['NetworkID', 'OrganizationID'], []);
                //self.CurrentData.OnData.bind(() => {
                //    debugger;
                //});
                //self.CurrentData.OnData.bind(self.onQlikData);
            }
            ViewModel.prototype.onQlikData = function () {
                //table will be the qlik object containing the filtered rows of the created table based on the selections in the display table
                var table = this;
                //debugger;
            };
            ViewModel.prototype.getSelections = function () {
                var self = this;
                //if (self.CurrentData == null || self.CurrentData.length == 0) {
                //    self.DebugLog('Currently there are no rows available.');
                //} else {
                //    self.DebugLog('Current selection has ' + self.CurrentData.length + ' rows of data.');
                //}   
                if (self.CurrentData == null || self.CurrentData.rowCount == 0) {
                    self.DebugLog('Currently there are no rows available.');
                }
                else {
                    var msg_1 = 'Current selection has ' + self.CurrentData.rowCount + ' rows of data.<br/>';
                    self.CurrentData.rows.forEach(function (row) {
                        msg_1 += 'NetworkID: ' + row.cells[0].qText + '    OrganizationID:' + row.cells[1].qText + '<br/>';
                    });
                    self.DebugLog(msg_1);
                }
            };
            ViewModel.prototype.DebugLog = function (msg) {
                this.Log.splice(0, 0, msg);
            };
            ViewModel.prototype.clearLog = function () {
                this.Log.removeAll();
            };
            return ViewModel;
        }());
        SelectionTest.ViewModel = ViewModel;
        function init(qlik) {
            vm = new ViewModel(qlik);
            $(function () {
                var bindingControl = $("#actionsContainer");
                ko.applyBindings(vm, bindingControl[0]);
            });
        }
        SelectionTest.init = init;
        var config = {
            host: 'harriette',
            prefix: '/',
            port: 443,
            isSecure: true
        };
        require.config({
            baseUrl: (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources",
            paths: {
                "qlik": (config.isSecure ? "https://" : "http://") + config.host + (config.port ? ":" + config.port : "") + "/resources/js/qlik"
            }
        });
        require(["js/qlik"], function (qlik) {
            QlikItegration.SelectionTest.init(qlik);
        });
    })(SelectionTest = QlikItegration.SelectionTest || (QlikItegration.SelectionTest = {}));
})(QlikItegration || (QlikItegration = {}));
//# sourceMappingURL=SelectionTest.js.map