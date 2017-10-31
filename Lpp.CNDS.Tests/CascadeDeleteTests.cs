using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Lpp.Utilities;


namespace Lpp.CNDS.Tests
{
    [TestClass]
    public class CascadeDeleteTests
    {
        static readonly log4net.ILog Logger;
        static CascadeDeleteTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(CascadeDeleteTests));
        }
        ////Broken
        //[TestMethod]
        //public void ListItemsCascadeDeletes()
        //{
        //    Logger.Debug("Starting Prep work for testing Cascade Deletes from List Items");
        //    MetaData.OrganizationMetaDataTests.InsertOrganizationMetadataTest();
        //    using (var db = new Data.DataContext())
        //    {
        //        List<Guid> guids = new List<Guid>();
        //        guids.Add(MetaData.OrganizationMetaDataTests.ListItem1);
        //        guids.Add(MetaData.OrganizationMetaDataTests.ListItem2);
        //        guids.Add(MetaData.OrganizationMetaDataTests.ListItem3);
        //        var listitems = db.ListItems.Where(x => guids.Contains(x.ID)).ToArray();
        //        Logger.Debug("Starting deleting List Items");
        //        db.ListItems.RemoveRange(listitems);
        //        db.SaveChanges();
        //        Logger.Debug(String.Format("Deletion Complete.  Searching Database for all Items with ItemID's with {0}, {1}, and  {2}", MetaData.OrganizationMetaDataTests.ListItem1, MetaData.OrganizationMetaDataTests.ListItem2, MetaData.OrganizationMetaDataTests.ListItem3));
        //        Assert.IsTrue(db.ListItems.Where(x => guids.Contains(x.ID)).Count() == 0);
        //        Assert.IsTrue(db.EntityMetadata.Where(x => guids.Contains(x.ItemID)).Count() == 0);
        //        Logger.Debug("Tests Complete.  Cascade Deletes for List Items are intact");
        //    }
        //}
        ////Broken
        //[TestMethod]
        //public void ListCascadeDeletes()
        //{
        //    Logger.Debug("Starting Prep work for testing Cascade Deletes from Lists");
        //    MetaData.OrganizationMetaDataTests.InsertOrganizationMetadataTest();
        //    using (var db = new Data.DataContext())
        //    {
        //        var list = db.Lists.Find(Lists.ListsTests.NewListID);
        //        Logger.Debug("Starting deleting List Items");
        //        db.Lists.Remove(list);
        //        Logger.Debug(String.Format("Deletion Complete.  Searching Database for List with ID of {0}", Lists.ListsTests.NewListID));
        //        db.SaveChanges();
        //        var testList = db.Lists.Find(Lists.ListsTests.NewListID);
        //        var testListItems = db.ListItems.Where(x => x.ListID == Lists.ListsTests.NewListID).ToArray();
        //        Assert.IsTrue(testList.IsNull());
        //        Assert.IsTrue(testListItems.Count() == 0);
        //        Logger.Debug("Tests Complete.  Cascade Deletes for Lists are intact");
        //    }
        //}
        //[TestMethod]
        //public void OrganizationWithDataMartsCascadeDeletes()
        //{
        //    Logger.Debug("Starting Prep work for testing Cascade Deletes from Organizations");
        //    DataMarts.DataMartTests.InsertNewDataMartTest();
        //    using (var db = new Data.DataContext())
        //    {
        //        var list = db.Organizations.Find(Organizations.OrganizationTests.NewOrgID);
        //        Logger.Debug("Starting deleting Organizations");
        //        db.Organizations.Remove(list);
        //        Logger.Debug(String.Format("Deletion Complete.  Searching Database for Organizations with ID of {0}", Organizations.OrganizationTests.NewOrgID));
        //        db.SaveChanges();
        //        var testOrg = db.Organizations.Find(Organizations.OrganizationTests.NewOrgID);
        //        var testDMS = db.DataSources.Where(x => x.OrganizationID == Organizations.OrganizationTests.NewOrgID).ToArray();
        //        Assert.IsTrue(testOrg.IsNull());
        //        Assert.IsTrue(testDMS.Count() == 0);
        //        Logger.Debug("Tests Complete.  Cascade Deletes for Organizations are intact");
        //    }
        //}
        //[TestMethod]
        //public void OrganizationWithMetaDataCascadeDeletes()
        //{
        //    Logger.Debug("Starting Prep work for testing Cascade Deletes from Organizations");
        //    MetaData.OrganizationMetaDataTests.InsertOrganizationMetadataTest();
        //    using (var db = new Data.DataContext())
        //    {
        //        var list = db.Organizations.Find(Organizations.OrganizationTests.NewOrgID);
        //        Logger.Debug("Starting deleting Organizations");
        //        db.Organizations.Remove(list);
        //        Logger.Debug(String.Format("Deletion Complete.  Searching Database for Organizations with ID of {0}", Organizations.OrganizationTests.NewOrgID));
        //        db.SaveChanges();
        //        var testOrg = db.Organizations.Find(Organizations.OrganizationTests.NewOrgID);
        //        //var testDMS = db.DataMarts.Where(x => x.OrganizationID == Organizations.OrganizationTests.NewOrgID).ToArray();
        //        Assert.IsTrue(testOrg.IsNull());
        //        //Assert.IsTrue(testDMS.Count() == 0);
        //        Logger.Debug("Tests Complete.  Cascade Deletes for Organizations are intact");
        //    }
        //}
        //[TestMethod]
        //public void NetworkCascadeDeletes()
        //{
        //    Logger.Debug("Starting Prep work for testing Cascade Deletes from Networks");
        //    DataMarts.DataMartTests.InsertNewDataMartTest();
        //    using (var db = new Data.DataContext())
        //    {
        //        var list = db.Networks.Find(Networks.NetworkTests.NewNetworkID);
        //        Logger.Debug("Starting deleting Organizations");
        //        db.Networks.Remove(list);
        //        db.SaveChanges();
        //        Logger.Debug(String.Format("Deletion Complete.  Searching Database for Networks with ID of {0}", Networks.NetworkTests.NewNetworkID));
        //        var testNet = db.Organizations.Find(Networks.NetworkTests.NewNetworkID);
        //        var testOrg = db.Organizations.Find(Organizations.OrganizationTests.NewOrgID);
        //        var testDMS = db.DataSources.Where(x => x.OrganizationID == Organizations.OrganizationTests.NewOrgID).ToArray();
        //        Assert.IsTrue(testNet.IsNull());
        //        Assert.IsTrue(testOrg.IsNull());
        //        Assert.IsTrue(testDMS.Count() == 0);
        //        Logger.Debug("Tests Complete.  Cascade Networks for Lists are intact");
        //    }
        //}
    }
}
