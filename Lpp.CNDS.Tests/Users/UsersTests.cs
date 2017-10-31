using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Lpp.Utilities;
using Lpp.CNDS.DTO;
using Lpp.CNDS.Api.Users;
using System.Threading.Tasks;
using Lpp.CNDS.Data;

namespace Lpp.CNDS.Tests.Users
{
    [TestClass]
    public class UsersTests
    {
        static readonly log4net.ILog Logger;
        const string ResourceFolder = "../Resources";
        public static IList<KeyValuePair<Guid, Guid>> UsersPair = new List<KeyValuePair<Guid, Guid>>();

        static UsersTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(UsersTests));
        }
       
       
        [TestMethod]
        public async Task RunAllUsersTests()
        {
            Logger.Debug("Beginning Testing of Users Inserts");
            Logger.Debug("----------------------------------------------");
            await InsertNewUserTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Users Inserts");
            Logger.Debug("Beginning Testing of Users Lists");
            Logger.Debug("----------------------------------------------");
            await GetAllUserTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Users Lists");
            Logger.Debug("Beginning Testing of Users Updates");
            Logger.Debug("----------------------------------------------");
            await UpdateNewUserTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Users Updates");
            Logger.Debug("Beginning Testing of Users Deletes");
            Logger.Debug("----------------------------------------------");
            DeleteNewUserTest();
            Logger.Debug("----------------------------------------------");
            Logger.Debug("Ending Testing of Users Deletes");
        }

        public static async Task GetAllUserTest()
        {
            using (var db = new Data.DataContext())
            {
                foreach (var pair in UsersPair)
                {
                    var lists = await db.Users.FindAsync(pair.Value);
                    Assert.IsTrue(!lists.IsNull());
                    Logger.Debug(String.Format("Users {0} was successfully found", lists.UserName));
                }
            }
        }

        public static async Task InsertNewUserTest()
        {
            await Organizations.OrganizationTests.InsertNewOrgTest();
            string filepath = System.IO.Path.Combine(ResourceFolder, "UsersRegister.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            UsersList orgs = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersList>(json, jsonSettings);
            var controller = new UsersController();
            foreach (var user in orgs.Users)
            {
                user.OrganizationID = Organizations.OrganizationTests.OrgPair.Where(x => x.Key == user.OrganizationID).Select(x => x.Value).FirstOrDefault();
                var response = await controller.Register(user);
                UsersPair.Add(new KeyValuePair<Guid, Guid>(user.ID, response.ID.Value));
                using (var db = new Data.DataContext())
                {
                    //TODO: Sections commented out wont work till User MetaData is ready
                    var usrGet = await db.Users.FindAsync(response.ID);
                    //var metaData = await db.DomainDatas.OfType<Data.UserDomainData>().Where(x => x.UserID == response.ID).ToArrayAsync();
                    Assert.IsTrue(!usrGet.IsEmpty());
                    //Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }

        public static async Task UpdateNewUserTest()
        {
            string filepath = System.IO.Path.Combine(ResourceFolder, "UsersUpdate.json");
            var json = System.IO.File.ReadAllText(filepath);
            Newtonsoft.Json.JsonSerializerSettings jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
            UsersList users = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersList>(json, jsonSettings);
            var controller = new UsersController();
            foreach (var user in users.Users)
            {
                using (var db = new Data.DataContext())
                {
                    var usrID = UsersPair.Where(x => x.Key == user.ID).Select(x => x.Value).FirstOrDefault();
                    user.ID = usrID;
                    user.OrganizationID = Organizations.OrganizationTests.OrgPair.Where(x => x.Key == user.OrganizationID).Select(x => x.Value).FirstOrDefault();

                    //TODO: The below Section is Commented out till User Metadata is Ready
                    //var metadatas = db.DomainDatas.OfType<Data.UserDomainData>().Where(x => x.User == usrID).ToArray();
                    //var newMetaDatas = new List<DomainDataDTO>();
                    //foreach (var metadata in metadatas)
                    //{
                    //    if (metadata.DomainUseID == new Guid("082C0001-C179-4630-A0C9-A6560123AFC6") || metadata.DomainReferenceID == new Guid("16C392E9-FF26-47C6-AE6F-A62600B48C85"))
                    //    {
                    //        var meta = new DomainDataDTO();
                    //        meta.Value = metadata.Value + " Update Test";
                    //        meta.SequenceNumber = 0;
                    //        meta.ID = metadata.ID;
                    //        meta.DomainUseID = metadata.DomainUseID;
                    //        if (metadata.DomainReferenceID.HasValue)
                    //            meta.DomainReferenceID = metadata.DomainReferenceID.Value;
                    //        newMetaDatas.Add(meta);
                    //    }
                    //    else if (metadata.DomainReferenceID == new Guid("BF140BF4-91BF-41F0-BA4A-A62600AED2B9") || metadata.DomainReferenceID == new Guid("B7236848-CA4E-4F5F-B482-A62600AEC6C6"))
                    //    {
                    //        //Intensionally leaving this blank to not put into newMetaDatas List.
                    //    }
                    //    else if (user.ID == new Guid("CD50B39C-9A9E-48FB-91D0-A62700F623B0"))
                    //    {
                    //        var meta = new DomainDataDTO()
                    //        {
                    //            DomainUseID = Guid.Parse("B5D00001-6085-44DA-8162-A6560124D48D"),
                    //            DomainReferenceID = Guid.Parse("95414CD3-B661-4F91-8957-A62600B243D9"),
                    //            SequenceNumber = 0
                    //        };
                    //        newMetaDatas.Add(meta);
                    //    }
                    //    else if (user.ID == new Guid("8EDD0AF4-29CE-4B82-8D85-A62700F8EFDF"))
                    //    {
                    //        var meta = new DomainDataDTO()
                    //        {
                    //            DomainUseID = Guid.Parse("B5D00001-6085-44DA-8162-A6560124D48D"),
                    //            DomainReferenceID = Guid.Parse("F108307B-B8C5-4DE4-A6E8-A62600B25065"),
                    //            Value = "LPP Types of Data",
                    //            SequenceNumber = 0
                    //        };
                    //        newMetaDatas.Add(meta);
                    //    }
                    //    else
                    //    {

                    //        //This is filling in all the rest to remain untouched
                    //        var meta = new DomainDataDTO()
                    //        {
                    //            ID = metadata.ID,
                    //            DomainUseID = metadata.DomainUseID,
                    //            Value = metadata.Value,
                    //            SequenceNumber = metadata.SequenceNumber
                    //        };
                    //        if (metadata.DomainReferenceID.HasValue)
                    //            meta.DomainReferenceID = metadata.DomainReferenceID;
                    //        newMetaDatas.Add(meta);
                    //    }
                    //}
                    //user.Metadata = newMetaDatas;



                    var response = await controller.Update(user);

                    var userGet = await db.Users.FindAsync(usrID);
                    //var metaData = await db.DomainDatas.OfType<Data.UserDomainData>().Where(x => x.UserID == usrID).ToArrayAsync();
                    Assert.IsTrue(!userGet.IsEmpty());
                    //Assert.IsTrue(metaData.Count() > 0);
                }

            }
        }

        public static void DeleteNewUserTest()
        {
            using (var db = new Data.DataContext())
            {
                Networks.NetworkTests.DeleteNewNetworkTest();
                foreach (var user in UsersPair)
                {
                    var deletedOrg = db.Users.Find(user.Value);
                    Assert.IsTrue(deletedOrg.IsEmpty());
                    Logger.Debug(String.Format("User {0} was successfully Deleted", user.Value));
                }
            }
        }

        [TestMethod]
        public void QlikBulkLoadQueryForUsers()
        {
            using(var db = new DataContext())
            {
                db.Database.Log = (s) => {
                    Logger.Debug(s);
                };

                var query = from user in db.Users
                            from def in (
                                from d in db.Domains.Where(x => x.Deleted == false)
                                join du in db.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                                join domainReference in db.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                                from dr in domainReferences.DefaultIfEmpty()
                                where du.EntityType == DTO.Enums.EntityType.User
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
                            join org in db.Organizations on user.OrganizationID equals org.ID
                            let domData = db.DomainDatas.OfType<UserDomainData>().Where(dat => user.ID == dat.UserID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                            let domVis = user.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                            where user.Deleted == false && user.OrganizationID.HasValue
                            select new 
                            {
                                NetworkID = org.NetworkID,
                                Network = org.Network.Name,
                                NetworkUrl = org.Network.Url,
                                OrganizationID = user.OrganizationID,
                                Organization = org.Name,
                                OrganizationAcronym = org.Acronym,
                                ParentOrganizationID = org.ParentOrganizationID,
                                UserID = user.ID,
                                UserName = user.UserName,
                                UserSalutation = user.Salutation,
                                UserFirstName = user.FirstName,
                                UserMiddleName = user.MiddleName,
                                UserLastName = user.LastName,
                                UserEmailAddress = user.EmailAddress,
                                UserPhoneNumber = user.PhoneNumber,
                                UserFaxNumber = user.FaxNumber,
                                UserIsActive = user.Active,
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
                                DomainDataValue = domData.Value,
                                DomainDataDomainReferenceID = domData.DomainReferenceID,
                                DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                            };



                //var users = query.Take(5).ToArray();
                //Logger.Debug(users.Length + " records returned.");

                //PMN user ID
                Guid userID = new Guid("");
                //PMN network ID for the datamart
                Guid networkID = new Guid("");

                var details = (from usr in query
                               where db.NetworkEntities.Any(ne => ne.ID == usr.UserID && ne.EntityType == DTO.Enums.EntityType.User && ne.NetworkEntityID == userID && ne.NetworkID == networkID)
                               select usr).ToArray();

                foreach (var d in details)
                {
                    Logger.Debug(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", d.UserID, d.UserName, d.DomainID, d.DomainTitle, d.DomainReferenceID, d.DomainReferenceTitle));
                }

                //var datasources = q.ToArray();
                //Logger.Debug(datasources.Length + " records returned.");

                if (System.IO.File.Exists("user_details.json"))
                    System.IO.File.Delete("user_details.json");

                using (var fs = new System.IO.StreamWriter("user_details.json", false))
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
    public class UsersList
    {
        public IEnumerable<UserTransferDTO> Users { get; set; }
    }
}
