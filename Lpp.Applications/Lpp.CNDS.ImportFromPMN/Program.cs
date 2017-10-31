using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Utilities;
using Lpp.CNDS.Data;
using System.Data.SqlClient;
using Lpp.CNDS.DTO.Enums;

namespace Lpp.CNDS.ImportFromPMN
{
    class Program
    {
        static readonly log4net.ILog Logger;
        
        static Program()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(Program));
        }

        static void Main(string[] args)
        {
            Logger.Debug("Starting import.");
            try
            {
                MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Application exception occurred!", ex);
                Console.ReadLine();
            }

            Logger.Debug("Bye!");

        }

        static async Task MainAsync()
        {
            Guid networkID = Properties.Settings.Default.NetworkID;

            await DeleteEntities(networkID);

            await ImportNetwork(networkID);

            await ImportOrganizations(networkID);

            await ImportUsers(networkID);

            await ImportDataMarts(networkID);
        }

        static async Task DeleteEntities(Guid networkID)
        {
            Logger.Info("Deleting all existing entities for the network.");

            using (var db = new DataContext())
            {
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Users WHERE EXISTS(SELECT NULL FROM Organizations o WHERE o.NetworkID = @networkID AND Users.OrganizationID = o.ID)", new SqlParameter("networkID", networkID));
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM DataSources WHERE EXISTS(SELECT NULL FROM Organizations o WHERE o.NetworkID = @networkID AND DataSources.OrganizationID = o.ID)", new SqlParameter("networkID", networkID));
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Organizations WHERE NetworkID = @networkID", new SqlParameter("networkID", networkID));
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Networks WHERE ID = @networkID", new SqlParameter("networkID", networkID));
            }

        }

        static async Task ImportNetwork(Guid networkID)
        {
            Logger.Info("Importing network, ID: " + networkID.ToString("D"));
            using (var conn = OpenPMNConnection())
            using (var cmd = conn.CreateCommand())
            using (var db = new DataContext())
            {
                cmd.CommandText = "SELECT [Name], Url FROM Networks WHERE ID = '" + networkID.ToString("D") + "'";
                using (var reader = cmd.ExecuteReader())
                {
                    await reader.ReadAsync();

                    db.Networks.Add(new Network { ID = networkID, Name = reader.GetValue(0).ToStringEx(), Url = reader.GetValue(1).ToStringEx() });
                }

                await db.SaveChangesAsync();
            }
        }

        static async Task ImportOrganizations(Guid networkID)
        {
            using (var conn = OpenPMNConnection())
            using (var cmd = conn.CreateCommand())
            using (var db = new DataContext())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Organizations o WHERE o.IsDeleted = 0";
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                Logger.Info(count + " Organizations were found to import.");
                if (count == 0)
                {
                    return;
                }

                List<NetworkEntity> networkEntities = new List<NetworkEntity>();
                List<Organization> organizations = new List<Organization>();
                
                cmd.CommandText = @"SELECT ID, [Name], Acronym, ContactEmail, ContactFirstName, ContactLastName, ContactPhone, OrganizationDescription as [Description], SpecialRequirements AS CollaborationRequirements, 
        UsageRestrictions as ResearchCapabilities, ObservationalParticipation, ProspectiveTrials, PragmaticClinicalTrials,
        InpatientClaims, OutpatientClaims, OutpatientPharmacyClaims, EnrollmentClaims, DemographicsClaims, LaboratoryResultsClaims, VitalSignsClaims, Biorepositories, PatientReportedOutcomes, PatientReportedBehaviors, PrescriptionOrders, OtherClaims, OtherClaimsText,
        DataModelMSCDM, DataModelHMORNVDW, DataModelESP, DataModelI2B2, DataModelOMOP, DataModelPCORI, DataModelOther, DataModelOtherText
        FROM Organizations o WHERE o.IsDeleted = 0 ORDER BY ID";
                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        var entityLink = db.NetworkEntities.Add(new NetworkEntity { NetworkID = networkID, EntityType = EntityType.Organization, NetworkEntityID = reader.GetGuid(0) });

                        var organization = db.Organizations.Add(
                            new Organization
                            {
                                ID = entityLink.ID,
                                NetworkID = networkID,
                                Name = reader.GetValue(1).ToStringEx(),
                                Acronym = reader.GetValue(2).ToStringEx(),
                            }
                         );
                        // Contact Information

                        //Add FirstName
                        if (!reader.GetValue(4).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("82790001-6318-423F-B1BF-A65601238941"), null, reader.GetValue(4).ToStringEx(), 0));
                        //Add LastName
                        if (!reader.GetValue(5).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("33F30001-8B67-4BD9-917D-A656012392F1"), null, reader.GetValue(5).ToStringEx(), 0));
                        //Add Email
                        if (!reader.GetValue(3).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("06CE0001-376B-4674-B087-A6560123A494"), null, reader.GetValue(3).ToStringEx(), 0));
                        //Add Phone
                        if (!reader.GetValue(6).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("CAB30001-7FC6-40B7-B508-A65601239BBE"), null, reader.GetValue(6).ToStringEx(), 0));
                        //Add Description
                        if (!reader.GetValue(7).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("082C0001-C179-4630-A0C9-A6560123AFC6"), null, reader.GetValue(7).ToStringEx(), 0));
                        // Add CollaborationRequirements
                        if (!reader.GetValue(8).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("86010001-1B19-4238-94F7-A6560123BA32"), null, reader.GetValue(8).ToStringEx(), 0));
                        // Add ResearchCapabilities
                        if (!reader.GetValue(9).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("A0F00001-4282-4165-8839-A6560123C376"), null, reader.GetValue(9).ToStringEx(), 0));

                        //Willing To Participate In
                        if (reader.GetBoolean(reader.GetOrdinal("ObservationalParticipation")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("7F970001-CDC6-4D64-8144-A6560124D9C7"), new Guid("836C4F41-483D-471D-B7FC-A62600AEB78F"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("ProspectiveTrials")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("7F970001-CDC6-4D64-8144-A6560124D9C7"), new Guid("B7236848-CA4E-4F5F-B482-A62600AEC6C6"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("PragmaticClinicalTrials")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("7F970001-CDC6-4D64-8144-A6560124D9C7"), new Guid("BF140BF4-91BF-41F0-BA4A-A62600AED2B9"), null, 0));

                        //Types of Data Collected
                        if (reader.GetBoolean(reader.GetOrdinal("InpatientClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("CCFC567D-08E5-42EE-A99E-A62600B1D585"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("OutpatientClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("C7BAD56C-99D1-4263-BD04-A62600B18878"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("OutpatientPharmacyClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("A16B5588-2768-4122-BE3A-A62600B1DFCE"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("EnrollmentClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("1FF2DA55-1C5C-4693-B613-A62600B1AE12"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DemographicsClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("A72E3F30-3FCA-49D9-8D3E-A62600B1E91A"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("LaboratoryResultsClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("AFEA017F-9B90-4D69-8306-A62600B1B688"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("VitalSignsClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("2216ECD8-6D98-4726-959B-A62600B1F2D1"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("Biorepositories")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("F3DF108B-E3A2-46BF-9E68-A62600B1C14A"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("PatientReportedOutcomes")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("75E54C21-A27C-4893-8A7C-A62600B1FBF4"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("PatientReportedBehaviors")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("4618B313-B73C-4518-A426-A62600B1CBE2"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("PrescriptionOrders")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("95414CD3-B661-4F91-8957-A62600B243D9"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("OtherClaims")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("B5D00001-6085-44DA-8162-A6560124D48D"), new Guid("F108307B-B8C5-4DE4-A6E8-A62600B25065"), reader.GetValue(reader.GetOrdinal("OtherClaimsText")).ToStringEx(), 0));

                        //DataModels
                        if (reader.GetBoolean(reader.GetOrdinal("DataModelESP")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("2F01EC11-35C0-401A-81CE-A62600B453E6"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelHMORNVDW")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("860E7DB0-0E98-4870-BFB5-A62600B46045"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelMSCDM")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("C2B41962-FF78-493B-AF9E-A62600B46986"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelI2B2")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("95CDDE93-5CDB-4838-B71C-A62600B4748F"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelOMOP")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("035BFFCC-0091-4FD7-9362-A62600B47E58"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelPCORI")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("BD017965-00EC-4239-8559-A62600B496EF"), null, 0));

                        if (reader.GetBoolean(reader.GetOrdinal("DataModelOther")))
                            db.DomainDatas.Add(CreateOrgDomainData(entityLink.ID, new Guid("FD760001-8047-4FDB-A347-A65601249C71"), new Guid("16C392E9-FF26-47C6-AE6F-A62600B48C85"), reader.GetValue(reader.GetOrdinal("DataModelOtherText")).ToStringEx(), 0));

                        organizations.Add(organization);
                        networkEntities.Add(entityLink);
                    }
                }


                Logger.Info("Update imported organizations parent organization associations.");

                //now update the parent organization IDs
                cmd.CommandText = "SELECT ID, ParentOrganizationID FROM Organizations WHERE ParentOrganizationID IS NOT NULL AND IsDeleted = 0 ORDER BY ID";
                using (var reader = cmd.ExecuteReader())
                {
                    Organization org = null;

                    while (await reader.ReadAsync())
                    {
                        Guid organizationID = reader.GetGuid(0);
                        if (org == null || org.ID != organizationID)
                        {
                            org = (from ne in networkEntities
                                   join o in organizations on ne.ID equals o.ID
                                   where ne.NetworkEntityID == organizationID
                                   select o).FirstOrDefault();
                        }

                        if (org != null)
                        {
                            var ne = networkEntities.FirstOrDefault(o => o.NetworkEntityID == reader.GetGuid(1));
                            if (ne != null)
                                org.ParentOrganizationID = ne.ID;
                        }
                    }
                }

                var validationErrors = db.GetValidationErrors();
                if (validationErrors.Any())
                {
                    foreach (var err in validationErrors)
                    {
                        Logger.Error("Validation Error for Organization: " + err.Entry.CurrentValues["ID"] + "/r/n" + string.Join(Environment.NewLine, err.ValidationErrors.Select(e => string.Format("{0}: {1}", e.PropertyName, e.ErrorMessage)).ToArray()));
                        throw new Exception("Validation error occurred, canceling import.");
                    }
                }

                await db.SaveChangesAsync();

                Logger.Info(count + " organizations successfully imported.");
            }
        }

        static async Task ImportUsers(Guid networkID)
        {
            using (var conn = OpenPMNConnection())
            using (var cmd = conn.CreateCommand())
            using (var db = new DataContext())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Users u WHERE u.isDeleted = 0 AND u.isActive = 1 AND u.UserType = 0 AND EXISTS(SELECT NULL FROM Organizations o WHERE o.ID = u.OrganizationID AND o.IsDeleted = 0)";
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                Logger.Info(count + " active Users were found to import.");
                if (count == 0)
                {
                    return;
                }

                cmd.CommandText = "SELECT u.ID, u.Title as Salutation, u.Username, u.FirstName, u.MiddleName, u.LastName, u.Email, u.Phone, u.Fax, u.OrganizationID FROM Users u join Organizations o on u.OrganizationID = o.ID WHERE u.isDeleted = 0 AND u.isActive = 1 AND u.UserType = 0 and o.IsDeleted = 0 ORDER BY u.ID";
                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        var networkLink = db.NetworkEntities.Add(new NetworkEntity { NetworkID = networkID, EntityType = EntityType.User, NetworkEntityID = reader.GetGuid(0) });

                        var user = db.Users.Add(new User
                        {
                            NetworkID = networkID,
                            ID = networkLink.ID,
                            Salutation = reader.GetValue(1).ToStringEx(),
                            UserName = reader.GetValue(2).ToStringEx(),
                            FirstName = reader.GetValue(3).ToStringEx() ?? reader.GetValue(2).ToStringEx(),
                            MiddleName = reader.GetValue(4).ToStringEx(),
                            LastName = reader.GetValue(5).ToStringEx() ?? reader.GetValue(2).ToStringEx(),
                            EmailAddress = reader.GetValue(6).ToStringEx(),
                            PhoneNumber = reader.GetValue(7).ToStringEx(),
                            FaxNumber = reader.GetValue(8).ToStringEx()
                        });

                        if (string.IsNullOrWhiteSpace(user.FirstName))
                            user.FirstName = user.UserName;

                        if (string.IsNullOrWhiteSpace(user.LastName))
                            user.LastName = user.UserName;

                        if (reader.GetValue(9).IsNull() == false)
                        {
                            Guid orgID = reader.GetGuid(9);
                            Guid organizationID = await db.NetworkEntities.Where(ne => ne.NetworkEntityID == orgID).Select(ne => ne.ID).FirstOrDefaultAsync();
                            user.OrganizationID = organizationID;
                        }
                    }

                    var validationErrors = db.GetValidationErrors();
                    if (validationErrors.Any())
                    {
                        foreach (var err in validationErrors)
                        {
                            Logger.Error("Validation Error for User: " + err.Entry.CurrentValues["ID"] + "/r/n" + string.Join(Environment.NewLine, err.ValidationErrors.Select(e => string.Format("{0}: {1}", e.PropertyName, e.ErrorMessage)).ToArray()));
                            throw new Exception("Validation error occurred, canceling import.");
                        }
                    }

                    await db.SaveChangesAsync();
                }

                Logger.Info(count + " users successfully imported.");
            }
        }

        static async Task ImportDataMarts(Guid networkID)
        {
            using (var conn = OpenPMNConnection())
            using (var cmd = conn.CreateCommand())
            using (var db = new DataContext())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM DataMarts dm JOIN Organizations o ON dm.OrganizationID = o.ID WHERE o.IsDeleted = 0 AND dm.isDeleted = 0 AND dm.IsLocal = 0";
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                Logger.Info(count + " DataMarts were found to import.");
                if (count == 0)
                {
                    return;
                }

                var organizations = await db.NetworkEntities.Where(ne => ne.NetworkID == networkID && ne.EntityType == EntityType.Organization).AsNoTracking().ToArrayAsync();

                List<DataSource> datamarts = new List<DataSource>();
                List<Guid[]> secondaryUpdates = new List<Guid[]>();

                cmd.CommandText = @"SELECT dm.ID, dm.OrganizationID, dm.Name, dm.Acronym, dm.ContactFirstName, dm.ContactLastName, dm.ContactPhone, dm.ContactEmail, dm.Description, dm.SpecialRequirements as CollaborationRequirements, dm.StartDate, dm.EndDate, dm.DataUpdateFrequency, dm.DataModel, dm.OtherDataModel,
        dm.InpatientEncountersAny, dm.InpatientEncountersEncounterID, dm.InpatientDatesOfService, dm.InpatientEncountersProviderIdentifier, dm.InpatientICD9Procedures, dm.InpatientICD10Procedures, dm.InpatientICD9Diagnosis, dm.InpatientICD10Diagnosis,
        dm.InpatientSNOMED, dm.InpatientHPHCS, dm.InpatientDisposition, dm.InpatientDischargeStatus, dm.InpatientOther, dm.InpatientOtherText,
        dm.DemographicsAny, dm.DemographicsSex, dm.DemographicsDateOfBirth, dm.DemographicsDateOfDeath, dm.DemographicsAddressInfo, dm.DemographicsRace, dm.DemographicsEthnicity, dm.DemographicsOther, dm.DemographicsOtherText
        FROM DataMarts dm JOIN Organizations o ON dm.OrganizationID = o.ID WHERE o.IsDeleted = 0 AND dm.isDeleted = 0 AND dm.IsLocal = 0 ORDER BY dm.ID";

                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        var networkLink = db.NetworkEntities.Add(new NetworkEntity { NetworkID = networkID, EntityType = EntityType.DataSource, NetworkEntityID = reader.GetGuid(0) });
                        var datasource = db.DataSources.Add(
                            new DataSource
                            {
                                ID = networkLink.ID,
                                OrganizationID = organizations.Single(o => o.NetworkEntityID == reader.GetGuid(1)),
                                Name = reader.GetValue(2).ToStringEx(),
                                Acronym = reader.GetValue(3).ToStringEx(),
                            }
                        );


                        // Contact Information

                        // Add First Name
                        if (!reader.GetValue(4).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("E1EE0001-9417-48EC-A8C4-A65601238D61"), null, reader.GetValue(4).ToStringEx(), 0));
                        // Add Last Name
                        if (!reader.GetValue(5).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("87E60001-7423-49BD-AB0A-A65601239766"), null, reader.GetValue(5).ToStringEx(), 0));
                        // Add Phone
                        if (!reader.GetValue(6).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("039F0001-CC80-435E-939B-A6560123A074"), null, reader.GetValue(6).ToStringEx(), 0));
                        // Add Email
                        if (!reader.GetValue(7).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("981D0001-60F6-4557-9D57-A6560123A998"), null, reader.GetValue(7).ToStringEx(), 0));
                        // Add Description
                        if (!reader.GetValue(8).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("86560001-6947-4384-8CB1-A6560123B537"), null, reader.GetValue(8).ToStringEx(), 0));
                        // Add Special Requirements
                        if (!reader.GetValue(9).ToStringEx().IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("92D50001-F4D8-4712-95E7-A6560123BE90"), null, reader.GetValue(9).ToStringEx(), 0));
                        // Add Start Year
                        var startYear = reader.GetValue(10).ToStringEx().IsNullOrWhiteSpace() ? null : ((DateTime)reader.GetValue(10)).Year.ToStringEx();
                        if (!startYear.IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6ED40001-D97A-46C3-9FCF-A6560124AD5F"), null, startYear.ToStringEx(), 0));
                        // Add End Year
                        var endYear = reader.GetValue(11).ToStringEx().IsNullOrWhiteSpace() ? null : ((DateTime)reader.GetValue(11)).Year.ToStringEx();
                        if (!endYear.IsNullOrWhiteSpace())
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("7BF20001-B5BB-44C3-9EC8-A6560124A804"), null, endYear.ToStringEx(), 0));

                        // Data Update Frequency
                        var updateFreq = reader.GetValue(12) == null ? null : reader.GetValue(12).ToStringEx();
                        if (!updateFreq.IsNullOrWhiteSpace())
                        {
                            if (string.Equals(updateFreq, "None", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("318E72A4-863C-4115-B4E6-A62600BA1BD9"), null, 0));
                            else if (string.Equals(updateFreq, "Daily", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("142BAFEF-8050-4A2D-9E25-A62600BA261C"), null, 0));
                            else if (string.Equals(updateFreq, "Weekly", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("4DE3C196-15D4-409E-8BCC-A62600BA3118"), null, 0));
                            else if (string.Equals(updateFreq, "Monthly", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("491F5EA2-D892-425B-AD5D-A62600BA3E0A"), null, 0));
                            else if (string.Equals(updateFreq, "Quarterly", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("B59299CB-263F-493A-A9CC-A62600BA4746"), null, 0));
                            else if (string.Equals(updateFreq, "Semi-Annually", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("AED999BB-97E4-4920-ACBC-A62600BA510E"), null, 0));
                            else if (string.Equals(updateFreq, "Annually", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("F72D2835-9011-4E16-AF1E-A62600BA5C03"), null, 0));
                            else
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("420C0001-5D5A-4CFF-A8C6-A6560124B28D"), new Guid("5EA4E0ED-09E5-4A43-922E-A62600BA6596"), updateFreq, 0));
                        }

                        // Add Data Models
                        var dataModels = reader.GetValue(13) == null ? null : reader.GetValue(13).ToStringEx();
                        if (!dataModels.IsNullOrWhiteSpace())
                        {
                            if (string.Equals(dataModels, "None", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("7F4E0001-2FE7-4CCD-AED1-A656010911B1"), null, 0));
                            else if (string.Equals(dataModels, "MSCDM", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("F2330001-5DC3-4F7D-AD73-A65601091886"), null, 0));
                            else if (string.Equals(dataModels, "HMORNVDW", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("45AE0001-A022-48FD-A8AF-A65601091D85"), null, 0));
                            else if (string.Equals(dataModels, "ESP", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("86910001-7965-45C7-AAAE-A65601092368"), null, 0));
                            else if (string.Equals(dataModels, "I2B2", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("8F6A0001-945C-4A34-AFC6-A656010929ED"), null, 0));
                            else if (string.Equals(dataModels, "PCORI", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("696F0001-A010-49FA-8FED-A65601093649"), null, 0));
                            else if (string.Equals(dataModels, "OMOP", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("7B290001-0BD2-48C3-8268-A65601093005"), null, 0));
                            else if (string.Equals(dataModels, "Other", StringComparison.OrdinalIgnoreCase))
                                db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("5A140001-1E80-46F9-B868-A65601246870"), new Guid("5B300001-E456-4B3B-A347-A65601093CA0"), reader.GetValue(14).ToStringEx(), 0));
                        }

                        // Add Inpaitent Data
                        bool anyEncounter = reader.GetBoolean(reader.GetOrdinal("InpatientEncountersAny"));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientDatesOfService")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("14B4756B-3C11-4612-84FB-A62600C4DFE5"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientDischargeStatus")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("7685B801-7974-4574-8136-A62600C56ECD"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientDisposition")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("D1A4AEE3-413A-4B8A-B849-A62600C5652A"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientEncountersEncounterID")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("B1F19D53-06AB-419A-A523-A62600C4D3EF"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientEncountersProviderIdentifier")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("5FBB5511-475C-4FB7-9578-A62600C51B3A"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientHPHCS")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("E05D9033-73F9-4DC9-8614-A62600C55AFB"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientICD10Diagnosis")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("12157696-E9E3-4F90-8EE5-A62600C549E0"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientICD10Procedures")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("145CADC4-BE69-4123-A75F-A62600C533DC"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientICD9Diagnosis")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("056018AC-F164-49BA-88B1-A62600C53CFF"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientICD9Procedures")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("0F7A9156-F9A6-460B-A7E7-A62600C525C2"), null, 0));
                        if (anyEncounter || reader.GetBoolean(reader.GetOrdinal("InpatientSNOMED")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("1B4B6393-2399-4CFD-8318-A62600C5523F"), null, 0));
                        if (reader.GetBoolean(reader.GetOrdinal("InpatientOther")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("34650001-74EF-492D-84B9-A65601246E5F"), new Guid("28D4092C-A14A-4C4D-B089-A62600C57733"), reader.GetValue(reader.GetOrdinal("InpatientOtherText")).ToStringEx(), 0));


                        // Add Demographics Data
                        bool demographicAny = reader.GetBoolean(reader.GetOrdinal("DemographicsAny"));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsAddressInfo")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("556A46AD-BEBF-468D-B834-A62600C59E4C"), null, 0));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsDateOfBirth")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("32A60FCE-F703-46E1-BE88-A62600C58DBE"), null, 0));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsDateOfDeath")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("A4A26325-3921-4819-A8C6-A62600C59625"), null, 0));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsEthnicity")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("68B94EC3-88B4-4E6F-BA48-A62600C5B137"), null, 0));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsRace")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("62B1F51B-EC64-45CA-B183-A62600C5A7CF"), null, 0));
                        if (demographicAny || reader.GetBoolean(reader.GetOrdinal("DemographicsSex")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("BB90ED02-2DF6-4219-A495-A62600C583AD"), null, 0));
                        if (reader.GetBoolean(reader.GetOrdinal("DemographicsOther")))
                            db.DomainDatas.Add(CreateDataSourceDomainData(networkLink.ID, new Guid("6C9E0001-D50F-406D-A099-A6560124734C"), new Guid("71E44F23-1DCB-4E6B-83C7-A62600C5B923"), reader.GetValue(reader.GetOrdinal("DemographicsOtherText")).ToStringEx(), 0));

                        datamarts.Add(datasource);
                    }
                }

                var validationErrors = db.GetValidationErrors();
                if (validationErrors.Any())
                {
                    foreach (var err in validationErrors)
                    {
                        Utilities.Objects.EntityWithID errObj = (Utilities.Objects.EntityWithID)err.Entry.Entity;
                        Logger.Error("Validation Error for DataSources: " + errObj.ID + Environment.NewLine + string.Join(Environment.NewLine, err.ValidationErrors.Select(e => string.Format("{0}: {1}", e.PropertyName, e.ErrorMessage)).ToArray()));
                        throw new Exception("Validation error occurred, canceling import.");
                    }
                }

                await db.SaveChangesAsync();

                Logger.Info(count + " datamarts succesfully imported.");

            }
        }

        static System.Data.SqlClient.SqlConnection OpenPMNConnection()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PMN"].ConnectionString;
            var conn = new System.Data.SqlClient.SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        static DomainData CreateOrgDomainData(Guid entityID, Guid domainUseID, Guid? domainReferenceID, string value = null, int sequeceNumber = 0)
        {
            return new Data.OrganizationDomainData()
            {
                OrganizationID = entityID,
                DomainUseID = domainUseID,
                DomainReferenceID = domainReferenceID,
                Value = value,
                SequenceNumber = sequeceNumber
            };
        }

        static DomainData CreateUserDomainData(Guid entityID, Guid domainUseID, Guid? domainReferenceID, string value = null, int sequeceNumber = 0)
        {
            return new Data.UserDomainData()
            {
                UserID = entityID,
                DomainUseID = domainUseID,
                DomainReferenceID = domainReferenceID,
                Value = value,
                SequenceNumber = sequeceNumber
            };
        }

        static DomainData CreateDataSourceDomainData(Guid entityID, Guid domainUseID, Guid? domainReferenceID, string value = null, int sequeceNumber = 0)
        {
            return new Data.DataSourceDomainData()
            {
                DataSourceID = entityID,
                DomainUseID = domainUseID,
                DomainReferenceID = domainReferenceID,
                Value = value,
                SequenceNumber = sequeceNumber
            };
        }
    }
}
