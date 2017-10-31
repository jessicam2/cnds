using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Utilities;
using System.Data.Entity;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lpp.Dns.DTO.Security;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using cndsDTO = Lpp.CNDS.DTO;
using Lpp.CNDS.ApiClient;

namespace Lpp.Dns.Api.DataMarts
{
    /// <summary>
    /// Controller that services requests related to DataMarts.
    /// </summary>
    public class DataMartsController : LppApiDataController<DataMart, DataMartDTO, DataContext, PermissionDefinition>
    {
        /// <summary>
        /// The ID of the DataMartType denoting it was imported from CNDS.
        /// </summary>
        public static readonly Guid CNDSImportedDataMartTypeID = new Guid("5B060001-2A5F-4243-980F-A70E011E7D5F");

        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Gets a specific DataMart by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override async System.Threading.Tasks.Task<DataMartDTO> Get(Guid id)
        {
            var datamart = await DataContext.DataMarts.Where(o => o.ID == id && o.DataMartTypeID != CNDSImportedDataMartTypeID).AsNoTracking().Map<DataMart, DataMartDTO>().FirstOrDefaultAsync();
            if (datamart == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified DataMart was not found."));

            //TODO: create helper to cache
            var cndsDMID = Guid.Empty;
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { id });
                    cndsDMID = response.Select(org => org.EntityID).FirstOrDefault();

                    if (cndsDMID == default(Guid))
                    {
                        throw new System.Net.Http.HttpRequestException("DataMart not found in CNDS.");
                    }

                    var availDMMetadata = (await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'2'")).ToArray();
                    var currentDmMetadata = (await CNDSEntityUpdater.CNDS.DataSources.ListDataSourceDomains(cndsDMID)).ToArray();
                    var visibility = await CNDSEntityUpdater.CNDS.DataSources.GetDomainVisibility(cndsDMID);

                    List<MetadataDTO> meta = new List<MetadataDTO>();
                    foreach (var metadata in availDMMetadata.Where(x => x.ParentDomainID == null))
                    {
                        meta.Add(cnds.GetMetadataChildren(metadata.ID, availDMMetadata, currentDmMetadata, visibility, cndsDMID));
                    }
                    datamart.Metadata = meta;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
            return datamart;
        }

        /// <summary>
		/// Returns All Available DataMart Metadata
		/// </summary>
		/// <param></param>
		/// <returns></returns>
		[HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<MetadataDTO>> GetAvailableDataMartMetadata()
        {
            IList<MetadataDTO> dmMetadata = new List<MetadataDTO>();
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {

                    var availDMMetdata = await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'2'");

                    foreach (var metadata in availDMMetdata.Where(x => x.ParentDomainID == null))
                    {
                        dmMetadata.Add(cnds.GetMetadataChildren(metadata.ID, availDMMetdata, new List<cndsDTO.DomainDataDTO>(), null, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
            return dmMetadata;
        }

        /// <summary>
        /// Gets a specific DataMart for a RequestDataMart (routing).
        /// </summary>
        /// <param name="requestDataMartID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataMartDTO> GetByRoute(Guid requestDataMartID)
        {
            //allowing get of imported DataMarts since if calling this it is likely from the context of an existing request.
            return await DataContext.Secure<DataMart>(Identity).Where(dm => dm.Requests.Any(rdm => rdm.ID == requestDataMartID)).Map<DataMart, DataMartDTO>().FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns a list of Data Marts the user has access to that are filterable using OData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IQueryable<DataMartDTO> List()
        {
            var datamarts = (from dm in DataContext.Secure<DataMart>(Identity) where dm.Deleted == false && dm.DataMartTypeID != CNDSImportedDataMartTypeID select dm).AsNoTracking().Map<DataMart, DataMartDTO>();

            return datamarts;
        }

        /// <summary>
        /// Returns a list of Data Marts the user has access to using as DataMartListDTO's.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataMartListDTO> ListBasic()
        {
            return DataContext.Secure<DataMart>(Identity).Where(dm => !dm.Deleted && dm.DataMartTypeID != CNDSImportedDataMartTypeID).Map<DataMart, DataMartListDTO>();
        }

        /// <summary>
        /// Returns a list of DataMart types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataMartTypeDTO> DataMartTypeList()
        {
            return (from r in DataContext.DataMartTypes
                    select new DataMartTypeDTO
                    {
                        ID = r.ID,
                        Name = r.Name
                    }).AsQueryable();
        }

        /// <summary>
        /// Returns a list of request types based on the data mart.
        /// </summary>
        /// <param name="DataMartId"></param>
        /// <returns></returns>
        [HttpPost]
        public IQueryable<RequestTypeDTO> GetRequestTypesByDataMarts(IEnumerable<Guid> DataMartId)
        {            
            var results = (from dm in DataContext.Secure<DataMart>(Identity)
                    let legacyRequestTypes = dm.Models.Distinct().Select(m => m.Model.RequestTypes).SelectMany(m => m.Where(s => s.RequestType.WorkflowID.HasValue == false).Select(s => s.RequestType))
                    let qeRequestTypes = dm.Adapter.RequestTypes.Select(s => s.RequestType)
                    let requestTypes = DataContext.RequestTypes.Where(rr => rr.Models.Any() == false && rr.WorkflowID.HasValue && dm.AdapterID.HasValue)
                    where DataMartId.Contains(dm.ID)
                    select legacyRequestTypes.Concat(qeRequestTypes).Concat(requestTypes))
                    .SelectMany(t => t).Distinct().Map<RequestType, RequestTypeDTO>();

            return results;
        }

        /// <summary>
        /// Returns a list of installed models based on the data mart.
        /// </summary>
        /// <param name="DataMartId"></param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataMartInstalledModelDTO> GetInstalledModelsByDataMart(Guid DataMartId)
        {
            var results = (from m in DataContext.DataMartModels join dm in DataContext.Secure<DataMart>(Identity) on m.DataMartID equals dm.ID where DataMartId == m.DataMartID select m).Map<DataMartInstalledModel, DataMartInstalledModelDTO>();

            return results;
        }

        /// <summary>
        /// Returns a list of Data Marts the user has access to that are filterable using OData
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public IQueryable<DataModelDTO> ListDataModels()
        //{
        //    return (from m in DataContext.DataModels select m).Map<DataModel, DataModelDTO>();
        //}

        /// <summary>
        /// Deletes the specified data model from the datamart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UninstallModel(DataMartInstalledModelDTO model)
        {
            var dataModels = await (from dm in DataContext.DataMartModels where dm.ModelID == model.ModelID && dm.DataMartID == model.DataMartID select dm).ToArrayAsync();

            if (!await DataContext.HasPermissions<DataMart>(Identity, await (from p in DataContext.Secure<DataMart>(Identity) where dataModels.First().DataMartID == p.ID select p.ID).ToArrayAsync(), PermissionIdentifiers.DataMart.Edit))
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have permission to alter one or more datamarts referenced");

            //clear out all the request type permissions applicable to the installed model
            var datamartRequestTypeAcls = from a in DataContext.DataMartRequestTypeAcls
                                          where a.DataMartID == model.DataMartID && a.RequestType.Models.Any(m => m.DataModelID == model.ModelID)
                                          //make sure the request type is not serviced by a different installed model as well
                                          &&  !a.RequestType.Models.Any(m => m.DataModelID != model.ModelID && a.DataMart.Models.Any(x => x.ModelID == m.DataModelID))
                                          select a;
            DataContext.DataMartRequestTypeAcls.RemoveRange(datamartRequestTypeAcls);

            var projectDatamartRequestTypeAcls = from a in DataContext.ProjectDataMartRequestTypeAcls
                                                 where a.DataMartID == model.DataMartID && a.RequestType.Models.Any(m => m.DataModelID == model.ModelID)
                                                 && !a.RequestType.Models.Any(m => m.DataModelID != model.ModelID && a.DataMart.Models.Any(x => x.ModelID == m.DataModelID))
                                                 select a;
            DataContext.ProjectDataMartRequestTypeAcls.RemoveRange(projectDatamartRequestTypeAcls);

            var projectRequestTypeAcls = from a in DataContext.ProjectRequestTypeAcls
                                         where a.RequestType.Models.Any(m => m.DataModelID == model.ModelID)
                                         && a.Project.DataMarts.Any(dm => dm.DataMartID == model.DataMartID)
                                         && !a.Project.DataMarts.Any(dm => dm.DataMartID != model.DataMartID && dm.DataMart.Models.Any(m => m.ModelID == model.ModelID))
                                         select a;
            DataContext.ProjectRequestTypeAcls.RemoveRange(projectRequestTypeAcls);

            if (model.ModelID == new Guid("455C772A-DF9B-4C6B-A6B0-D4FD4DD98488"))
            {
                //if the model being removed is the QueryComposer model, reset the Adapter Supported back to null.
                var datamart = await DataContext.DataMarts.FindAsync(model.DataMartID);
                datamart.AdapterID = null;
            }

            DataContext.DataMartModels.Remove(dataModels.First());

            await DataContext.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Copies the specified datamart and returns the ID of the new datamart.
        /// </summary>
        /// <param name="datamartID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Guid> Copy(Guid datamartID)
        {
            var existing = await (from dm in DataContext.DataMarts where dm.ID == datamartID && dm.DataMartTypeID != CNDSImportedDataMartTypeID select dm).SingleOrDefaultAsync();

            if (existing == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The DataMart could not be found."));

            if (!await DataContext.HasPermissions<DataMart>(Identity, existing, PermissionIdentifiers.DataMart.Copy))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have permission to copy the specified DataMart."));

            string newAcronym = "New " + existing.Acronym;
            string newName = "New " + existing.Name;

            while (await (from p in DataContext.DataMarts where !p.Deleted && (p.Name == newName && p.Acronym == newAcronym) select p).AnyAsync())
            {
                newAcronym = "New " + newAcronym;
                newName = "New " + newName;
            }

            var datamart = new DataMart
            {
                Acronym = newAcronym,
                StartDate = DateTime.Today,
                Name = newName,
                DataMartType = existing.DataMartType,
                DataMartTypeID = existing.DataMartTypeID,
                AvailablePeriod = existing.AvailablePeriod,
                ContactEmail = existing.ContactEmail,
                ContactFirstName = existing.ContactFirstName,
                ContactLastName = existing.ContactLastName,
                ContactPhone = existing.ContactPhone,
                SpecialRequirements = existing.SpecialRequirements,
                UsageRestrictions = existing.UsageRestrictions,
                Deleted = existing.Deleted,
                HealthPlanDescription = existing.HealthPlanDescription,
                OrganizationID = existing.OrganizationID,
                Organization = existing.Organization,
                IsGroupDataMart = existing.IsGroupDataMart,
                UnattendedMode = existing.UnattendedMode,
                Description = existing.Description,
                EndDate = existing.EndDate,
                DataUpdateFrequency = existing.DataUpdateFrequency,
                InpatientEHRApplication = existing.InpatientEHRApplication,
                OutpatientEHRApplication = existing.OutpatientEHRApplication,
                LaboratoryResultsAny = existing.LaboratoryResultsAny,
                LaboratoryResultsClaims = existing.LaboratoryResultsClaims,
                LaboratoryResultsTestName = existing.LaboratoryResultsTestName,
                LaboratoryResultsDates = existing.LaboratoryResultsDates,
                LaboratoryResultsTestLOINC = existing.LaboratoryResultsTestLOINC,
                LaboratoryResultsTestSNOMED = existing.LaboratoryResultsTestSNOMED,
                LaboratoryResultsSpecimenSource = existing.LaboratoryResultsSpecimenSource,
                LaboratoryResultsTestDescriptions = existing.LaboratoryResultsTestDescriptions,
                LaboratoryResultsOrderDates = existing.LaboratoryResultsOrderDates,
                LaboratoryResultsTestResultsInterpretation = existing.LaboratoryResultsTestResultsInterpretation,
                LaboratoryResultsTestOther = existing.LaboratoryResultsTestOther,
                LaboratoryResultsTestOtherText = existing.LaboratoryResultsTestOtherText,
                InpatientEncountersAny = existing.InpatientEncountersAny,
                InpatientEncountersEncounterID = existing.InpatientEncountersEncounterID,
                InpatientEncountersProviderIdentifier = existing.InpatientEncountersProviderIdentifier,
                InpatientDatesOfService = existing.InpatientDatesOfService,
                InpatientICD9Procedures = existing.InpatientICD9Procedures,
                InpatientICD10Procedures = existing.InpatientICD10Procedures,
                InpatientICD9Diagnosis = existing.InpatientICD9Diagnosis,
                InpatientICD10Diagnosis = existing.InpatientICD10Diagnosis,
                InpatientSNOMED = existing.InpatientSNOMED,
                InpatientHPHCS = existing.InpatientHPHCS,
                InpatientDisposition = existing.InpatientDisposition,
                InpatientDischargeStatus = existing.InpatientDischargeStatus,
                InpatientOther = existing.InpatientOther,
                InpatientOtherText = existing.InpatientOtherText,
                OutpatientEncountersAny = existing.OutpatientEncountersAny,
                OutpatientEncountersEncounterID = existing.OutpatientEncountersEncounterID,
                OutpatientEncountersProviderIdentifier = existing.OutpatientEncountersProviderIdentifier,
                OutpatientClinicalSetting = existing.OutpatientClinicalSetting,
                OutpatientDatesOfService = existing.OutpatientDatesOfService,
                OutpatientICD9Procedures = existing.OutpatientICD9Procedures,
                OutpatientICD10Procedures = existing.OutpatientICD10Procedures,
                OutpatientICD9Diagnosis = existing.OutpatientICD9Diagnosis,
                OutpatientICD10Diagnosis = existing.OutpatientICD10Diagnosis,
                OutpatientSNOMED = existing.OutpatientSNOMED,
                OutpatientHPHCS = existing.OutpatientHPHCS,
                OutpatientOther = existing.OutpatientOther,
                OutpatientOtherText = existing.OutpatientOtherText,
                ERPatientID = existing.ERPatientID,
                EREncounterID = existing.EREncounterID,
                EREnrollmentDates = existing.EREnrollmentDates,
                EREncounterDates = existing.EREncounterDates,
                ERClinicalSetting = existing.ERClinicalSetting,
                ERICD9Diagnosis = existing.ERICD9Diagnosis,
                ERICD10Diagnosis = existing.ERICD10Diagnosis,
                ERHPHCS = existing.ERHPHCS,
                ERNDC = existing.ERNDC,
                ERSNOMED = existing.ERSNOMED,
                ERProviderIdentifier = existing.ERProviderIdentifier,
                ERProviderFacility = existing.ERProviderFacility,
                EREncounterType = existing.EREncounterType,
                ERDRG = existing.ERDRG,
                ERDRGType = existing.ERDRGType,
                EROther = existing.EROther,
                EROtherText = existing.EROtherText,
                DemographicsAny = existing.DemographicsAny,
                PatientBehaviorInstruments = existing.PatientBehaviorInstruments,
                PharmacyDispensingAny = existing.PharmacyDispensingAny,
                OtherClaims = existing.OtherClaims,
                Registeries = existing.Registeries,
                OtherInpatientEHRApplication = existing.OtherInpatientEHRApplication,
                OtherOutpatientEHRApplication = existing.OtherOutpatientEHRApplication,
                DataModel = existing.DataModel,
                OtherDataModel = existing.OtherDataModel,
                IsLocal = existing.IsLocal,
                Url = existing.Url ?? ""
            };

            DataContext.DataMarts.Add(datamart);

            var dmDTO = new DataMartDTO {
                ID = datamart.ID,
                Name = datamart.Name,
                Acronym = datamart.Acronym,
                OrganizationID = datamart.OrganizationID,
                AdapterID = datamart.AdapterID
            };

            
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {

                        IList<Guid> ids = new List<Guid>();
                        ids.Add(datamartID);
                        ids.Add(datamart.OrganizationID);
                        var networkEntity = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(ids);
                        Guid? cndsOrganizationID = networkEntity.Where(x => x.NetworkEntityID == datamart.OrganizationID).Select(x => x.EntityID).FirstOrDefault();
                        var cndsDataSourceID = networkEntity.Where(x => x.NetworkEntityID == datamartID).Select(o => o.EntityID).FirstOrDefault();

                        var availDMMetadata = await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'2'");
                        var oldMetadata = await CNDSEntityUpdater.CNDS.DataSources.ListDataSourceDomains(cndsDataSourceID);
                        List<MetadataDTO> meta = new List<MetadataDTO>();
                        foreach (var metadata in availDMMetadata.Where(x => x.ParentDomainID == null))
                        {
                            meta.Add(cnds.GetMetadataChildren(metadata.ID, availDMMetadata, oldMetadata, null, null));
                        }
                        dmDTO.Metadata = meta;

                        await CNDSEntityUpdater.RegisterOrUpdateDataSources(dmDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }
                        
            await DataContext.SaveChangesAsync();

            //DataMart Acls
            var existingDataMartAcls = await (from a in DataContext.DataMartAcls where a.DataMartID == existing.ID select a).ToArrayAsync();
            foreach (var existingDataMartAcl in existingDataMartAcls)
            {

                var count = await DataContext.Database.ExecuteSqlCommandAsync(@"IF NOT EXISTS(SELECT NULL FROM AclDataMarts WHERE DataMartID = @DataMartID AND SecurityGroupID = @SecurityGroupID AND PermissionID = @PermissionID)
	INSERT INTO AclDataMarts (DataMartID, SecurityGroupID, PermissionID, Allowed, Overridden) VALUES (@DataMartID, @SecurityGroupID, @PermissionID, @Allowed, 1)", new SqlParameter("DataMartID", datamart.ID), new SqlParameter("SecurityGroupID", existingDataMartAcl.SecurityGroupID), new SqlParameter("PermissionID", existingDataMartAcl.PermissionID), new SqlParameter("Allowed", existingDataMartAcl.Allowed));
            }

            return datamart.ID;
        }

        /// <summary>
        /// Flags the datamart as deleted.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete]
        public override async Task Delete([FromUri]IEnumerable<Guid> ID)
        {
            if (!await DataContext.CanDelete<DataMart>(Identity, ID.ToArray()))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have permission to delete this project."));

            var datamarts = await (from dm in DataContext.DataMarts where ID.Contains(dm.ID) && dm.DataMartTypeID != CNDSImportedDataMartTypeID select dm).ToArrayAsync();

            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        var networkEntities = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(datamarts.Select(x => x.ID).ToArray());

                        foreach (var datamart in datamarts)
                        {
                           
                            await CNDSEntityUpdater.CNDS.DataSources.Delete(networkEntities.Where(x => x.NetworkEntityID == datamart.ID).Select(x => x.EntityID).FirstOrDefault());
                            datamart.Deleted = true;
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }
           

            await DataContext.SaveChangesAsync();
        }
        /// <summary>
        /// insert
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        public override async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Lpp.Dns.DTO.DataMartDTO>> Insert(System.Collections.Generic.IEnumerable<Lpp.Dns.DTO.DataMartDTO> values)
        {
            await CheckForDuplicates(values);
            var dmDTO = values.FirstOrDefault();
            if (dmDTO.ID == null || dmDTO.ID == Guid.Empty)
                dmDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();

            
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        await CNDSEntityUpdater.RegisterOrUpdateDataSources(dmDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }

            var clientBasedTypeID = DataContext.DataMartTypes.Where(t => t.Name == "Client based").Select(t => t.ID).FirstOrDefault();
            var dm = new DataMart
            {
                ID = dmDTO.ID.Value,
                Name = dmDTO.Name,
                Acronym = dmDTO.Acronym,
                OrganizationID = dmDTO.OrganizationID.Value,
                DataMartTypeID = dmDTO.DataMartTypeID == Guid.Empty ? clientBasedTypeID : dmDTO.DataMartTypeID,
                IsGroupDataMart = dmDTO.IsGroupDataMart,
                UnattendedMode = dmDTO.UnattendedMode,
                IsLocal = dmDTO.IsLocal,
                Url = dmDTO.Url,
            };
            if (dmDTO.AdapterID.HasValue)
                dm.AdapterID = dmDTO.AdapterID;

            if (dmDTO.ProcessorID.HasValue)
                dm.ProcessorID = dmDTO.ProcessorID;

            DataContext.DataMarts.Add(dm);

            DataContext.SaveChanges();
            

            IList<DataMartDTO> returnDM = new List<DataMartDTO>();
            returnDM.Add(dmDTO);
            return returnDM;
        }
        /// <summary>
        /// insert or update datamarts
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<IEnumerable<DataMartDTO>> InsertOrUpdate(IEnumerable<DataMartDTO> values)
        {
            await CheckForDuplicates(values);

            var dmDTO = values.FirstOrDefault();
            if (dmDTO.ID == null || dmDTO.ID == Guid.Empty)
                dmDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();
            
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        await CNDSEntityUpdater.RegisterOrUpdateDataSources(dmDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }

            var clientBasedTypeID = DataContext.DataMartTypes.Where(t => t.Name == "Client based").Select(t => t.ID).FirstOrDefault();
            var dm = DataContext.DataMarts.Find(dmDTO.ID);
            if (dm == null)
            {
                dm = new DataMart
                {
                    ID = dmDTO.ID.Value,
                    Name = dmDTO.Name,
                    Acronym = dmDTO.Acronym,
                    OrganizationID = dmDTO.OrganizationID.Value,
                    DataMartTypeID = dmDTO.DataMartTypeID == Guid.Empty ? clientBasedTypeID : dmDTO.DataMartTypeID,
                    IsGroupDataMart = dmDTO.IsGroupDataMart,
                    UnattendedMode = dmDTO.UnattendedMode,
                    IsLocal = dmDTO.IsLocal,
                    Url = dmDTO.Url,
                };
                if (dmDTO.AdapterID.HasValue)
                    dm.AdapterID = dmDTO.AdapterID;
                else
                {
                    dm.AdapterID = null;
                }

                if (dmDTO.ProcessorID.HasValue)
                    dm.ProcessorID = dmDTO.ProcessorID;
                else
                {
                    dm.ProcessorID = null;
                }

                DataContext.DataMarts.Add(dm);
            }
            else
            {
                dm.Name = dmDTO.Name;
                dm.Acronym = dmDTO.Acronym;
                dm.OrganizationID = dmDTO.OrganizationID.Value;
                dm.DataMartTypeID = dmDTO.DataMartTypeID == Guid.Empty ? clientBasedTypeID : dm.DataMartTypeID;
                dm.IsGroupDataMart = dmDTO.IsGroupDataMart;
                dm.UnattendedMode = dmDTO.UnattendedMode;
                dm.IsLocal = dmDTO.IsLocal;
                dm.Url = dmDTO.Url;

                if (dmDTO.AdapterID.HasValue)
                    dm.AdapterID = dmDTO.AdapterID;
                else
                {
                    dm.AdapterID = null;
                }

                if (dmDTO.ProcessorID.HasValue)
                    dm.ProcessorID = dmDTO.ProcessorID;
                else
                {
                    dm.ProcessorID = null;
                }
            }

            DataContext.SaveChanges();
            

            IList<DataMartDTO> returnDM = new List<DataMartDTO>();
            returnDM.Add(dmDTO);
            return returnDM;
        }
        /// <summary>
        /// update datamart
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPut]
        public override async Task<IEnumerable<DataMartDTO>> Update(IEnumerable<DataMartDTO> values)
        {
            await CheckForDuplicates(values);

            var dmDTO = values.FirstOrDefault();
            if (dmDTO.ID == null || dmDTO.ID == Guid.Empty)
                dmDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();
            
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        await CNDSEntityUpdater.RegisterOrUpdateDataSources(dmDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }

            var clientBasedTypeID = DataContext.DataMartTypes.Where(t => t.Name == "Client based").Select(t => t.ID).FirstOrDefault();
            var dm = DataContext.DataMarts.Find(dmDTO.ID);
            if (dm == null)
            {
                dm = new DataMart
                {
                    ID = dmDTO.ID.Value,
                    Name = dmDTO.Name,
                    Acronym = dmDTO.Acronym,
                    OrganizationID = dmDTO.OrganizationID.Value,
                    DataMartTypeID = dmDTO.DataMartTypeID == Guid.Empty ? clientBasedTypeID : dmDTO.DataMartTypeID,
                    IsGroupDataMart = dmDTO.IsGroupDataMart,
                    UnattendedMode = dmDTO.UnattendedMode,
                    IsLocal = dmDTO.IsLocal,
                    Url = dmDTO.Url,
                };
                if (dmDTO.AdapterID.HasValue)
                    dm.AdapterID = dmDTO.AdapterID;

                if (dmDTO.ProcessorID.HasValue)
                    dm.ProcessorID = dmDTO.ProcessorID;

                DataContext.DataMarts.Add(dm);
            }
            else
            {
                dm.Name = dmDTO.Name;
                dm.Acronym = dmDTO.Acronym;
                dm.OrganizationID = dmDTO.OrganizationID.Value;
                dm.DataMartTypeID = dmDTO.DataMartTypeID == Guid.Empty ? clientBasedTypeID : dm.DataMartTypeID;
                dm.IsGroupDataMart = dmDTO.IsGroupDataMart;
                dm.UnattendedMode = dmDTO.UnattendedMode;
                dm.IsLocal = dmDTO.IsLocal;
                dm.Url = dmDTO.Url;

                if (dmDTO.AdapterID.HasValue)
                    dm.AdapterID = dmDTO.AdapterID;

                if (dmDTO.ProcessorID.HasValue)
                    dm.ProcessorID = dmDTO.ProcessorID;
            }

            DataContext.SaveChanges();
            
            IList<DataMartDTO> returnDM = new List<DataMartDTO>();
            returnDM.Add(dmDTO);
            return returnDM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmMetadata"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateDataMartVisibility(IEnumerable<MetadataDTO> dmMetadata)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var cnds = new CNDSEntityUpdater(networkID))
            {
                await CNDSEntityUpdater.RegisterOrUpdateDatamartDomainVisibility(dmMetadata);
            }
        }

        private async Task CheckForDuplicates(IEnumerable<DataMartDTO> updates)
        {
            var ids = updates.Where(u => u.ID.HasValue).Select(u => u.ID.Value).ToArray();
            var names = updates.Select(u => u.Name).ToArray();
            var acronyms = updates.Where(u => !u.Deleted).Where(u => u.Acronym != null && u.Acronym != "").Select(u => u.Acronym).ToArray();

            if (updates.GroupBy(u => u.Acronym).Any(u => u.Count() > 1))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Acronym of DataMarts must be unique."));

            if (updates.GroupBy(u => u.Name).Any(u => u.Count() > 1))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Name of DataMarts must be unique."));

            if (await (from p in DataContext.DataMarts where !p.Deleted && !ids.Contains(p.ID) && (names.Contains(p.Name) && acronyms.Contains(p.Acronym)) select p).AnyAsync())
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Name and Acronym of DataMarts must be unique."));
        }

    }
}
