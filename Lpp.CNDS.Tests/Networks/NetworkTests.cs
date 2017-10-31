using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Lpp.Utilities;
using Lpp.CNDS.DTO;
using Lpp.CNDS.Api.Networks;
using System.Threading.Tasks;

namespace Lpp.CNDS.Tests.Networks
{
    [TestClass]
    public class NetworkTests
    {
        static readonly log4net.ILog Logger;
        static NetworkTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(NetworkTests));
        }
        public static Guid NewNetworkID;
        const string ResourceFolder = "../Resources";
        [TestMethod]
        public async Task RunAllNetworkTests()
        {
            Logger.Debug("Beginning Testing of Network Inserts");
            Logger.Debug("----------------------------------------------");
            await InsertNewNetworkTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Network Inserts");
            Logger.Debug("Beginning Testing of Network Lists");
            Logger.Debug("----------------------------------------------");
            GetAllNetworkTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Network Lists");
            Logger.Debug("Beginning Testing of Network Updates");
            Logger.Debug("----------------------------------------------");
            UpdateNewNetworkTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Network Updates");
            Logger.Debug("Beginning Testing of Network Deletes");
            Logger.Debug("----------------------------------------------");
            DeleteNewNetworkTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Network Deletes");
        }
        public void GetAllNetworkTest()
        {
            var controller = new NetworksController();
            var networks = controller.List();
            Assert.IsTrue(networks.Count() > 0);
            var network = controller.Get(NewNetworkID);
            Assert.IsTrue(!network.IsNull());
        }
        public static async Task InsertNewNetworkTest()
        {
            string filepath = System.IO.Path.Combine(ResourceFolder, "NetworkImport.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            NetworkTransferDTO network = Newtonsoft.Json.JsonConvert.DeserializeObject<NetworkTransferDTO>(json, jsonSettings);
            var controller = new NetworksController();
            await controller.Register(network);
            using (var db = new Data.DataContext())
            {
                Assert.IsNotNull(db.Networks.FindAsync(network.ID));
                NewNetworkID = network.ID;
            }
        }
        public static void UpdateNewNetworkTest()
        {
            using (var db = new Data.DataContext())
            {
                var network = db.Networks.Find(NewNetworkID);
                network.Name = network.Name + " Test Update";
                db.SaveChanges();
                var updated = db.Networks.Find(NewNetworkID);
                Assert.AreEqual(updated, network);
                Logger.Debug(String.Format("Network {0} was successfully Updated", network.Name));
            }
        }
        public static void DeleteNewNetworkTest()
        {
            using (var db = new Data.DataContext())
            {
                var removeNetwork = db.Networks.Find(NewNetworkID);
                db.Networks.Remove(removeNetwork);
                db.SaveChanges();
                var deletedOrg = db.Networks.Find(NewNetworkID);
                Assert.IsTrue(deletedOrg.IsEmpty());
                Logger.Debug(String.Format("Network {0} was successfully Deleted", removeNetwork.Name));
            }
        }
    }
}
