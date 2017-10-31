using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Lpp.Utilities;
using Lpp.CNDS.DTO;
using System.Threading.Tasks;
using Lpp.CNDS.Api.Organizations;
using Lpp.CNDS.Data;

namespace Lpp.CNDS.Tests.Organizations
{
    [TestClass]
    public class OrganizationTests
    {
        static readonly log4net.ILog Logger;
        const string ResourceFolder = "../Resources";
        public static IList<KeyValuePair<Guid, Guid>> OrgPair = new List<KeyValuePair<Guid, Guid>>();

        static OrganizationTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(OrganizationTests));
        }
        
        [TestMethod]
        public async Task RunAllOrgTests()
        {
            Logger.Debug("Beginning Testing of Organization Inserts");
            Logger.Debug("----------------------------------------------");
            await InsertNewOrgTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Organization Inserts");
            Logger.Debug("Beginning Testing of Organization Lists");
            Logger.Debug("----------------------------------------------");
            await GetAllOrgsTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Organization Lists");
            Logger.Debug("Beginning Testing of Organization Updates");
            Logger.Debug("----------------------------------------------");
            await UpdateNewOrgTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Organization Updates");
            Logger.Debug("Beginning Testing of Organization Deletes");
            Logger.Debug("----------------------------------------------");
            await DeleteNewOrgTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Organization Deletes");
        }

        public static async Task GetAllOrgsTest()
        {
            using (var db = new Data.DataContext())
            {
                foreach (var pair in OrgPair)
                {
                    var lists = await db.Organizations.FindAsync(pair.Value);
                    Assert.IsTrue(!lists.IsNull());
                    Logger.Debug(String.Format("Org {0} was successfully found", lists.Name));
                }
            }
        }

        public static async Task InsertNewOrgTest()
        {
            await Networks.NetworkTests.InsertNewNetworkTest();
            string filepath = System.IO.Path.Combine(ResourceFolder, "OrganizationsRegister.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            OrgList orgs = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgList>(json, jsonSettings);
            var controller = new OrganizationsController();
            foreach (var org in orgs.Organizations)
            {
                var response = await controller.Register(org);
                OrgPair.Add(new KeyValuePair<Guid, Guid>(org.ID, response.ID.Value));
                using (var db = new Data.DataContext())
                {
                    var orgGet = await db.Organizations.FindAsync(response.ID);
                    var metaData = await db.DomainDatas.OfType<Data.OrganizationDomainData>().Where(x => x.OrganizationID == response.ID).ToArrayAsync();
                    Assert.IsTrue(!orgGet.IsEmpty());
                    Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }

        public static async Task UpdateNewOrgTest()
        {
            string filepath = System.IO.Path.Combine(ResourceFolder, "OrganizationsUpdate.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            OrgList orgs = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgList>(json, jsonSettings);
            var controller = new OrganizationsController();
            foreach (var org in orgs.Organizations)
            {
                using (var db = new Data.DataContext())
                {
                    var orgID = OrgPair.Where(x => x.Key == org.ID).Select(x => x.Value).FirstOrDefault();
                    org.ID = orgID;
                    var metadatas = db.DomainDatas.OfType<Data.OrganizationDomainData>().Where(x => x.OrganizationID == orgID).ToArray();
                    var newMetaDatas = new List<DomainDataDTO>();
                    foreach(var metadata in metadatas)
                    {
                        if (metadata.DomainUseID == new Guid("082C0001-C179-4630-A0C9-A6560123AFC6") || metadata.DomainReferenceID == new Guid("16C392E9-FF26-47C6-AE6F-A62600B48C85"))
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
                        else if(metadata.DomainReferenceID == new Guid("BF140BF4-91BF-41F0-BA4A-A62600AED2B9") || metadata.DomainReferenceID == new Guid("B7236848-CA4E-4F5F-B482-A62600AEC6C6"))
                        {
                            //Intensionally leaving this blank to not put into newMetaDatas List.
                        }
                        else if (org.ID == new Guid("CD50B39C-9A9E-48FB-91D0-A62700F623B0"))
                        {
                            var meta = new DomainDataDTO() {
                                DomainUseID = Guid.Parse("B5D00001-6085-44DA-8162-A6560124D48D"),
                                DomainReferenceID = Guid.Parse("95414CD3-B661-4F91-8957-A62600B243D9"),
                                SequenceNumber = 0
                            };
                            newMetaDatas.Add(meta);
                        }
                        else if (org.ID == new Guid("8EDD0AF4-29CE-4B82-8D85-A62700F8EFDF"))
                        {
                            var meta = new DomainDataDTO()
                            {
                                DomainUseID = Guid.Parse("B5D00001-6085-44DA-8162-A6560124D48D"),
                                DomainReferenceID = Guid.Parse("F108307B-B8C5-4DE4-A6E8-A62600B25065"),
                                Value = "LPP Types of Data",
                                SequenceNumber = 0
                            };
                            newMetaDatas.Add(meta);
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
                    org.Metadata = newMetaDatas;



                    var response = await controller.Update(org);

                    var orgGet = await db.Organizations.FindAsync(orgID);
                    var metaData = await db.DomainDatas.OfType<Data.OrganizationDomainData>().Where(x => x.OrganizationID == orgID).ToArrayAsync();
                    Assert.IsTrue(!orgGet.IsEmpty());
                    Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }
        public static async Task DeleteNewOrgTest()
        {
            using (var db = new Data.DataContext())
            {
                Networks.NetworkTests.DeleteNewNetworkTest();
                foreach (var org in OrgPair)
                {
                    var deletedOrg = await db.Organizations.FindAsync(org.Value);
                    Assert.IsTrue(deletedOrg.IsEmpty());
                    Logger.Debug(String.Format("Org {0} was successfully Deleted", org.Value));
                }
            }
        }

        [TestMethod]
        public void QlikBulkLoadQueryForDataSources()
        {
            using(var db = new DataContext())
            {
                db.Database.Log = (s) => {
                    Logger.Debug(s);
                };

                var query = from org in db.Organizations
                            from def in (
                                from d in db.Domains.Where(x => x.Deleted == false)
                                join du in db.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                                join domainReference in db.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                                from dr in domainReferences.DefaultIfEmpty()
                                where du.EntityType == DTO.Enums.EntityType.Organization
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
                            let domData = db.DomainDatas.OfType<OrganizationDomainData>().Where(dat => org.ID == dat.OrganizationID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                            let domVis = org.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                            where org.Deleted == false
                            select new DTO.QlikData.OrganizationWithDomainDataItemDTO
                            {
                                NetworkID = org.NetworkID,
                                Network = org.Network.Name,
                                NetworkUrl = org.Network.Url,
                                OrganizationID = org.ID,
                                Organization = org.Name,
                                OrganizationAcronym = org.Acronym,
                                ParentOrganizationID = org.ParentOrganizationID,
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
                                DomainDataValue = def.DomainDataType == "boolean" ? (domData.ID == null ? "false" : "true") : domData.Value,
                                DomainDataDomainReferenceID = domData.DomainReferenceID,
                                DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                            };





                //var organizations = query.ToArray();
                //Logger.Debug(organizations.Length + " records returned.");

                //PMN organization ID
                Guid organizationID = new Guid("");
                //PMN network ID for the datamart
                Guid networkID = new Guid("");

                var details = (from org in query
                               where db.NetworkEntities.Any(ne => ne.ID == org.OrganizationID && ne.EntityType == DTO.Enums.EntityType.Organization && ne.NetworkEntityID == organizationID && ne.NetworkID == networkID)
                               select org).ToArray();

                foreach (var d in details)
                {
                    Logger.Debug(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", d.OrganizationID, d.Organization, d.DomainID, d.DomainTitle, d.DomainReferenceID, d.DomainReferenceTitle));
                }

                //var datasources = q.ToArray();
                //Logger.Debug(datasources.Length + " records returned.");

                if (System.IO.File.Exists("organization_details.json"))
                    System.IO.File.Delete("organization_details.json");

                using (var fs = new System.IO.StreamWriter("organization_details.json", false))
                {
                    var serializationSettings = new Newtonsoft.Json.JsonSerializerSettings();
                    serializationSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

                    var js = Newtonsoft.Json.JsonSerializer.Create(serializationSettings);
                    js.Serialize(fs, details);
                    fs.Flush();
                }



            }
        }
    }

    public class OrgList
    {
        public IList<OrganizationTransferDTO> Organizations { get; set; }
    }
}
