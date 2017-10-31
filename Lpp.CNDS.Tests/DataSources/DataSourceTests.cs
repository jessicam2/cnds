using Lpp.CNDS.Api.DataSources;
using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.CNDS.Data;

namespace Lpp.CNDS.Tests.DataSources
{

    [TestClass]
    public class DataSourceTests
    {
        static readonly log4net.ILog Logger;
        const string ResourceFolder = "../Resources";
        public static IList<KeyValuePair<Guid, Guid>> DataSourcePair = new List<KeyValuePair<Guid, Guid>>();

        static DataSourceTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(DataSourceTests));
        }
        

        [TestMethod]
        public async Task RunAllDataMartTests()
        {
            Logger.Debug("Beginning Testing of DataSources Inserts");
            Logger.Debug("----------------------------------------------");
            await InsertNewDataMartTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of DataSources Inserts");
            Logger.Debug("Beginning Testing of DataSources Lists");
            Logger.Debug("----------------------------------------------");
            await GetAllDataMartTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of DataSources Lists");
            Logger.Debug("Beginning Testing of DataSources Updates");
            Logger.Debug("----------------------------------------------");
            await UpdateNewDataMartTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of DataSources Updates");
            Logger.Debug("Beginning Testing of DataSources Deletes");
            Logger.Debug("----------------------------------------------");
            DeleteNewDataMartTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of DataSources Deletes");
        }

        public async Task GetAllDataMartTest()
        {
            using (var db = new Data.DataContext())
            {
                foreach (var pair in DataSourcePair)
                {
                    var lists = await db.DataSources.FindAsync(pair.Value);
                    Assert.IsTrue(!lists.IsNull());
                    Logger.Debug(String.Format("DataSource {0} was successfully found", lists.Name));
                }
            }
        }

        public static async Task InsertNewDataMartTest()
        {
            await Organizations.OrganizationTests.InsertNewOrgTest();
            string filepath = System.IO.Path.Combine(ResourceFolder, "DataSourcesRegister.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            DataSourceList datasources = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSourceList>(json, jsonSettings);
            var controller = new DataSourcesController();
            foreach (var ds in datasources.DataSources)
            {
                ds.OrganizationID = Organizations.OrganizationTests.OrgPair.Where(x => x.Key == ds.OrganizationID).Select(x => x.Value).FirstOrDefault();
                var response = await controller.Register(ds);
                DataSourcePair.Add(new KeyValuePair<Guid, Guid>(ds.ID, response.ID.Value));
                using (var db = new Data.DataContext())
                {
                    var dsGet = await db.DataSources.FindAsync(response.ID);
                    var metaData = await db.DomainDatas.OfType<Data.DataSourceDomainData>().Where(x => x.DataSourceID == response.ID).ToArrayAsync();
                    Assert.IsTrue(!dsGet.IsEmpty());
                    Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }

        public static async Task UpdateNewDataMartTest()
        {
            string filepath = System.IO.Path.Combine(ResourceFolder, "DataSourcesUpdate.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            DataSourceList dataSources = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSourceList>(json, jsonSettings);
            var controller = new DataSourcesController();
            foreach (var ds in dataSources.DataSources)
            {
                using (var db = new Data.DataContext())
                {
                    var dsID = DataSourcePair.Where(x => x.Key == ds.ID).Select(x => x.Value).FirstOrDefault();
                    ds.ID = dsID;
                    ds.OrganizationID = Organizations.OrganizationTests.OrgPair.Where(x => x.Key == ds.OrganizationID).Select(x => x.Value).FirstOrDefault();
                    var metadatas = db.DomainDatas.OfType<Data.DataSourceDomainData>().Where(x => x.DataSourceID == dsID).ToArray();
                    var newMetaDatas = new List<DomainDataDTO>();
                    foreach (var metadata in metadatas)
                    {
                        if (metadata.DomainUseID == new Guid("86560001-6947-4384-8CB1-A6560123B537") || metadata.DomainReferenceID == new Guid("6C9E0001-D50F-406D-A099-A6560124734C"))
                        {
                            var meta = new DomainDataDTO();
                            meta.Value = metadata.Value + " Update Test";
                            meta.SequenceNumber = 0;
                            meta.ID = metadata.ID;
                            meta.DomainUseID = metadata.DomainUseID;
                            if (metadata.DomainReferenceID.HasValue)
                                meta.DomainReferenceID = metadata.DomainReferenceID.Value;
                            newMetaDatas.Add(meta);
                        }
                        else if (metadata.DomainReferenceID == new Guid("BB90ED02-2DF6-4219-A495-A62600C583AD"))
                        {
                            //Intensionally leaving this blank to not put into newMetaDatas List.
                        }
                        else
                        {

                            //This is filling in all the rest to remain untouched
                            var meta = new DomainDataDTO()
                            {
                                ID = metadata.ID,
                                DomainUseID = metadata.DomainUseID,
                                Value = metadata.Value,
                                SequenceNumber = metadata.SequenceNumber
                            };
                            if (metadata.DomainReferenceID.HasValue)
                                meta.DomainReferenceID = metadata.DomainReferenceID;
                            newMetaDatas.Add(meta);
                        }
                    }
                    var addMeta = new DomainDataDTO()
                    {
                        DomainUseID = Guid.Parse("B5D00001-6085-44DA-8162-A6560124D48D"),
                        DomainReferenceID = Guid.Parse("28D4092C-A14A-4C4D-B089-A62600C57733"),
                        Value = "LPP Inpatient Encounter",
                        SequenceNumber = 0
                    };
                    newMetaDatas.Add(addMeta);
                    ds.Metadata = newMetaDatas;



                    var response = await controller.Update(ds);

                    var dsGet = await db.DataSources.FindAsync(dsID);
                    var metaData = await db.DomainDatas.OfType<Data.DataSourceDomainData>().Where(x => x.DataSourceID == dsID).ToArrayAsync();
                    Assert.IsTrue(!dsGet.IsEmpty());
                    Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }

        public static void DeleteNewDataMartTest()
        {
            using (var db = new Data.DataContext())
            {
                Networks.NetworkTests.DeleteNewNetworkTest();
                foreach (var org in DataSourcePair)
                {
                    var deletedOrg = db.Organizations.Find(org.Value);
                    Assert.IsTrue(deletedOrg.IsEmpty());
                    Logger.Debug(String.Format("Datamart {0} was successfully Deleted", org.Value));
                }
            }
        }


        [TestMethod]
        public void QlikBulkLoadQueryForDataSources()
        {

            using(var db = new Data.DataContext())
            {
                db.Database.Log = (s) => {
                    Logger.Debug(s);
                };


                var q = from ds in db.DataSources
                        from def in (
                            from d in db.Domains.Where(x => x.Deleted == false)
                            join du in db.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                            join domainReference in db.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                            from dr in domainReferences.DefaultIfEmpty()
                            where du.EntityType == DTO.Enums.EntityType.DataSource
                            select new
                            {
                                DomainUseID = du.ID,
                                DomainID = du.DomainID,
                                ParentDomainID = d.ParentDomainID,
                                DomainTitle = d.Title,
                                DomainIsMultiValueSelect = d.IsMultiValue,
                                DomainDataType = d.DataType,
                                DomainReferenceID = (Guid?)dr.ID,
                                DomainReferenceTItle = dr.Title,
                                DomainReferenceDescription = dr.Description,
                                DomainReferenceValue = dr.Value
                            }

                        )
                        join org in db.Organizations on ds.OrganizationID equals org.ID
                        let domData = db.DomainDatas.OfType<DataSourceDomainData>().Where(dat => ds.ID == dat.DataSourceID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                        let domVis = ds.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                        where ds.Deleted == false
                        select new {
                            NetworkID = org.NetworkID,
                            Network = org.Network.Name,
                            NetworkUrl = org.Network.Url,
                            OrganizationID = ds.OrganizationID,
                            Organization = org.Name,
                            OrganizationAcronym = org.Acronym,
                            ParentOrganizationID = org.ParentOrganizationID,
                            DataSourceID = ds.ID,
                            DataSource = ds.Name,
                            DataSourceAcronym = ds.Acronym,
                            DataSourceAdapterSupportedID = ds.AdapterSupportedID,
                            DataSourceAdapterSupported = ds.AdapterSupported.Name,
                            SupportsCrossNetworkRequests = ds.AdapterSupportedID.HasValue,
                            DomainUseID = def.DomainUseID,
                            DomainID = def.DomainID,
                            ParentDomainID = def.ParentDomainID,
                            DomainTitle = def.DomainTitle,
                            DomainIsMultiValueSelect = def.DomainIsMultiValueSelect,
                            DomainDataType = def.DomainDataType,
                            DomainReferenceID = def.DomainReferenceID,
                            DomainReferenceTitle = def.DomainReferenceTItle,
                            DomainReferenceDescription = def.DomainReferenceDescription,
                            DomainReferenceValue = def.DomainReferenceValue,
                            DomainDataValue = def.DomainDataType == "boolean" ? (domData.ID == null ? "false" : "true"): domData.Value,
                            DomainDataDomainReferenceID = domData.DomainReferenceID,
                            DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                        };

                //var query = q.ToString();

                //PMN datamart ID
                Guid datamartID = new Guid("");
                //PMN network ID for the datamart
                Guid networkID = new Guid("");

                var details = (from ds in q
                              where db.NetworkEntities.Any(ne => ne.ID == ds.DataSourceID && ne.EntityType == DTO.Enums.EntityType.DataSource && ne.NetworkEntityID == datamartID && ne.NetworkID == networkID)
                              select ds).ToArray();

                foreach(var d in details)
                {
                    Logger.Debug(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", d.DataSourceID, d.DataSource, d.DomainID, d.DomainTitle, d.DomainReferenceID, d.DomainReferenceTitle));
                }

                //var datasources = q.ToArray();
                //Logger.Debug(datasources.Length + " records returned.");

                if (System.IO.File.Exists("datasource_details.json"))
                    System.IO.File.Delete("datasource_details.json");

                using(var fs = new System.IO.StreamWriter("datasource_details.json", false))
                {
                    var serializationSettings = new Newtonsoft.Json.JsonSerializerSettings();
                    serializationSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

                    var js = Newtonsoft.Json.JsonSerializer.Create(serializationSettings);
                    js.Serialize(fs, details);
                    fs.Flush();
                }


            }

        }        

        [TestMethod]
        public void GetDataSourceVisibility()
        {
            using (var db = new Data.DataContext())
            {
                var q = from da in db.DomainAccess.OfType<DataSourceDomainAccess>()
                        select new {
                            da.AccessType,
                            da.DomainUse.Domain.Title,
                            da.DataSource.Name
                        };

                foreach(var da in q.Take(10))
                {
                    Logger.Debug(string.Format("{0}\t{1}\t{2}", da.AccessType, da.Title, da.Name));
                }
            
            }
        }

        [TestMethod]
        public void ChangeDataSourceVisibility()
        {
            using(var db = new DataContext())
            {
                foreach(var da in db.DomainAccess.OfType<DataSourceDomainAccess>().Take(5))
                {
                    da.AccessType = DTO.Enums.AccessType.AllPMNNetworks;
                }

                db.SaveChanges();
            }
        }

        [TestMethod]
        public void HetroVisibilityQuery()
        {
            using (var db = new DataContext())
            {
                db.Database.Log = (s) => { Logger.Debug(s); };

                //Logger.Debug(db.DomainAccess.Count());

                var q = db.DomainAccess.Where(da => da.DomainUse.DomainID == new Guid("428D0001-57A8-4A69-B02E-A6560108C757"));

                var qq = from da in q
                         select new {
                             da.AccessType,
                             da.DomainUse.Domain.Title,
                             DataType = (da as DataSourceDomainAccess) == null ? ((da as OrganizationDomainAccess) == null ? "User Visibility" : "Organization Visibility") : "DataSource Visibility"
                         };

                foreach(var r in qq)
                {
                    Logger.Debug(string.Format("{0}\t{1}\t{2}", r.AccessType, r.Title, r.DataType));
                }
            }
        }




    }

    public class DataSourceList
    {
        public IEnumerable<DataSourceTransferDTO> DataSources { get; set; }
    }
}
