/// <reference path="../../../Lpp.Pmn.Resources/scripts/typings/requirejs/require.d.ts" />

module QlikItegration.SelectionTest {

    var vm: ViewModel;

    export class ViewModel {
        private qlik: any;
        private app: any;

        private Log: KnockoutObservableArray<string>;
        private CurrentData: any = null;

        constructor(q: any) {
            var self = this;

            self.Log = ko.observableArray<string>();
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
            
            let table = self.app.getObject('QV01', 'XLbKc');
            
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

        private onQlikData() {
            //table will be the qlik object containing the filtered rows of the created table based on the selections in the display table
            let table = this;
            //debugger;
        }

        public getSelections() {
            var self = this;
            
            //if (self.CurrentData == null || self.CurrentData.length == 0) {
            //    self.DebugLog('Currently there are no rows available.');
            //} else {
            //    self.DebugLog('Current selection has ' + self.CurrentData.length + ' rows of data.');
            //}   

            if (self.CurrentData == null || self.CurrentData.rowCount == 0) {
                self.DebugLog('Currently there are no rows available.');
            } else {
                let msg = 'Current selection has ' + self.CurrentData.rowCount + ' rows of data.<br/>';
                self.CurrentData.rows.forEach((row) => {
                    msg += 'NetworkID: ' + row.cells[0].qText + '    OrganizationID:' + row.cells[1].qText + '<br/>';
                });

                self.DebugLog(msg);
            }   


        }

        private DebugLog(msg: string) {

            this.Log.splice(0, 0, msg);
        }

        public clearLog() {
            this.Log.removeAll();
        }

    }

    export function init(qlik: any) {
        vm = new ViewModel(qlik);

        $(() => {
            var bindingControl = $("#actionsContainer");
            ko.applyBindings(vm, bindingControl[0]);
        });
    }

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

}

