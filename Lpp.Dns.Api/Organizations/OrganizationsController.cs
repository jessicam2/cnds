using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using System.Data.Entity;
using Lpp.Utilities.WebSites.Controllers;
using Lpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Lpp.Dns.DTO.Security;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using log4net;
using cndsDTO = Lpp.CNDS.DTO;
using Lpp.CNDS.ApiClient;

namespace Lpp.Dns.Api.Organizations
{
	/// <summary>
	/// Controller that services the Organization
	/// </summary>
	public class OrganizationsController : LppApiDataController<Organization, OrganizationDTO, DataContext, PermissionDefinition>
	{
		static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		/// <summary>
		/// Returns a specified organization
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public override async System.Threading.Tasks.Task<OrganizationDTO> Get(Guid id)
		{
			var organization = await DataContext.Organizations.Where(o => o.ID == id && o.OrganizationType == DTO.Enums.OrganizationType.Local).AsNoTracking().Map<Organization, OrganizationDTO>().FirstOrDefaultAsync();

            if(organization == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified Organization was not found."));
            }

			//TODO: create helper to cache
			var cndsOrgID = Guid.Empty;
			Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
			try
			{
				using (var cnds = new CNDSEntityUpdater(networkID))
				{
					var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { id });
					cndsOrgID = response.Select(org => org.EntityID).FirstOrDefault();

					if (cndsOrgID == default(Guid))
					{
						throw new System.Net.Http.HttpRequestException("Organization not found in CNDS.");
					}

					var availOrgMetdata = await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'0'");
					var currentOrgMetadata = await CNDSEntityUpdater.CNDS.Organizations.ListOrganizationDomains(cndsOrgID);
                    var visibility = await CNDSEntityUpdater.CNDS.Organizations.GetDomainVisibility(cndsOrgID);
                    List<MetadataDTO> meta = new List<MetadataDTO>();
					foreach (var metadata in availOrgMetdata.Where(x => x.ParentDomainID == null))
					{
						meta.Add(cnds.GetMetadataChildren(metadata.ID, availOrgMetdata, currentOrgMetadata, visibility, cndsOrgID));
					}
					organization.Metadata = meta;
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message, ex);
				throw;
			}
			return organization;
		}

		/// <summary>
		/// Returns All Available Organization Metadata
		/// </summary>
		/// <param></param>
		/// <returns></returns>
		[HttpGet]
		public async System.Threading.Tasks.Task<IEnumerable<MetadataDTO>> GetAvailableOrganizationMetadata()
		{
            IList<MetadataDTO> orgMetadata = new List<MetadataDTO>();
			Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
			try
			{
				using (var cnds = new CNDSEntityUpdater(networkID))
				{

					var availOrgMetdata = await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'0'");

					foreach (var metadata in availOrgMetdata.Where(x => x.ParentDomainID == null))
					{
                        orgMetadata.Add(cnds.GetMetadataChildren(metadata.ID, availOrgMetdata, new List<cndsDTO.DomainDataDTO>(), null, null));
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message, ex);
				throw;
			}
			return orgMetadata;
		}

		/// <summary>
		/// Gets a secure list of Organizations
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public override IQueryable<OrganizationDTO> List()
		{
            //does not hit CNDS, DTO is stripped of all old metadata.
            //return base.List().Where(l => !l.Deleted);
            var org = (from o in DataContext.Secure<Organization>(Identity) where o.Deleted == false && o.OrganizationType == DTO.Enums.OrganizationType.Local select o).AsNoTracking().Map<Organization, OrganizationDTO>();

            return org;
        }

        /// <summary>
        /// Gets the organization for a specific user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<OrganizationDTO> GetForUser(Guid userID)
        {
            var orgs = (from o in DataContext.Secure<Organization>(Identity)
                        let user = DataContext.Users.Where(u => u.ID == userID).FirstOrDefault()
                        where o.Deleted == false
                        && ((user.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy && o.ID == user.OrganizationID) || (user.UserType != DTO.Enums.UserTypes.CNDSNetworkProxy && o.OrganizationType == DTO.Enums.OrganizationType.Local))
                        select o).AsNoTracking().Map<Organization, OrganizationDTO>();
            return orgs;
        }

		/// <summary>
		/// Returns a secure list of organizations by Group
		/// </summary>
		/// <param name="groupID"></param>
		/// <returns></returns>
		[HttpGet]
		public IQueryable<OrganizationDTO> ListByGroupMembership(Guid groupID)
		{
			var result = (from o in DataContext.Secure<Organization>(Identity) where o.Groups.Any(g => g.GroupID == groupID) && o.OrganizationType == DTO.Enums.OrganizationType.Local select o).Map<Organization, OrganizationDTO>();

			return result;
		}

		/// <summary>
		/// Returns a specified copied organization
		/// </summary>
		/// <param name="organizationID"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<Guid> Copy(Guid organizationID)
		{
			var existing = await (from o in DataContext.Organizations where o.ID == organizationID && o.OrganizationType == DTO.Enums.OrganizationType.Local select o).FirstOrDefaultAsync();

			if (existing == null)
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The Organization could not be found."));

			if (!await DataContext.HasPermissions<Organization>(Identity, existing.ID, PermissionIdentifiers.Organization.Copy))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have permission to copy the specified organization."));

			string newAcronym = "New " + existing.Acronym;
			string newName = "New " + existing.Name;

			while (await (from p in DataContext.Organizations where !p.Deleted && (p.Name == newName && p.Acronym == newAcronym) select p).AnyAsync())
			{
				newAcronym = "New " + newAcronym;
				newName = "New " + newName;
			}

			var organization = new Organization
			{
				Acronym = newAcronym,
				Name = newName,
				ParentOrganizationID = existing.ParentOrganizationID,
				ApprovalRequired = existing.ApprovalRequired,
				ContactEmail = existing.ContactEmail,
				ContactFirstName = existing.ContactFirstName,
				ContactLastName = existing.ContactLastName,
				ContactPhone = existing.ContactPhone,
				SpecialRequirements = existing.SpecialRequirements,
				UsageRestrictions = existing.UsageRestrictions,
				HealthPlanDescription = existing.HealthPlanDescription,
				EnableClaimsAndBilling = existing.EnableClaimsAndBilling,
				EnableEHRA = existing.EnableEHRA,
				EnableRegistries = existing.EnableRegistries,
				DataModelESP = existing.DataModelESP,
				DataModelHMORNVDW = existing.DataModelHMORNVDW,
				DataModelI2B2 = existing.DataModelI2B2,
				DataModelMSCDM = existing.DataModelMSCDM,
				DataModelOMOP = existing.DataModelOMOP,
				DataModelOther = existing.DataModelOther,
				DataModelOtherText = existing.DataModelOtherText,
				PragmaticClinicalTrials = existing.PragmaticClinicalTrials,
				Biorepositories = existing.Biorepositories,
				PatientReportedBehaviors = existing.PatientReportedBehaviors,
				PatientReportedOutcomes = existing.PatientReportedOutcomes,
				PrescriptionOrders = existing.PrescriptionOrders,
				InpatientEHRApplication = existing.InpatientEHRApplication,
				OutpatientEHRApplication = existing.OutpatientEHRApplication,
				OtherInpatientEHRApplication = existing.OtherInpatientEHRApplication,
				OtherOutpatientEHRApplication = existing.OtherOutpatientEHRApplication,
				InpatientClaims = existing.InpatientClaims,
				OutpatientClaims = existing.OutpatientClaims,
				ObservationalParticipation = existing.ObservationalParticipation,
				ProspectiveTrials = existing.ProspectiveTrials,
				EnrollmentClaims = existing.EnrollmentClaims,
				DemographicsClaims = existing.DemographicsClaims,
				LaboratoryResultsClaims = existing.LaboratoryResultsClaims,
				VitalSignsClaims = existing.LaboratoryResultsClaims,
				OtherClaims = existing.OtherClaims,
				OtherClaimsText = existing.OtherClaimsText,
				ObservationClinicalExperience = existing.ObservationClinicalExperience,
                OrganizationType = existing.OrganizationType
			};

			DataContext.Organizations.Add(organization);

            var orgDTO = new OrganizationDTO()
            {
                ID = organization.ID,
                Name = organization.Name,
                Acronym = organization.Acronym,
                ParentOrganizationID = organization.ParentOrganizationID,
                ContactEmail = organization.ContactEmail,
                ContactFirstName = organization.ContactFirstName,
                ContactLastName = organization.ContactLastName,
                ContactPhone = organization.ContactPhone
            };

            #region CNDS
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { organizationID });
                        var cndsOrgID = response.Select(org => org.EntityID).FirstOrDefault();

                        if (cndsOrgID == default(Guid))
                        {
                            throw new System.Net.Http.HttpRequestException("Organization not found in CNDS.");
                        }

                        var availOrgMetdata = await CNDSEntityUpdater.CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'0'");
                        var currentOrgMetadata = await CNDSEntityUpdater.CNDS.Organizations.ListOrganizationDomains(cndsOrgID);

                        List<MetadataDTO> meta = new List<MetadataDTO>();
                        foreach (var metadata in availOrgMetdata.Where(x => x.ParentDomainID == null))
                        {
                            meta.Add(cnds.GetMetadataChildren(metadata.ID, availOrgMetdata, currentOrgMetadata, null, null));
                        }
                        orgDTO.Metadata = meta;
                        await CNDSEntityUpdater.RegisterOrUpdateOrganizations(orgDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }

            #endregion CNDS

            //Registries
            var existingRegistries = await (from reg in DataContext.OrganizationRegistries where reg.OrganizationID == existing.ID select reg).ToArrayAsync();
			foreach (var existingRegistry in existingRegistries)
			{

				var reg = new OrganizationRegistry
				{
					RegistryID = existingRegistry.RegistryID,
					OrganizationID = organization.ID,
					Description = existingRegistry.Description
				};
				DataContext.OrganizationRegistries.Add(reg);
			}

			//Security Groups
			var existingSecurityGroups = await (from sg in DataContext.SecurityGroups.Include(x => x.Users) where sg.OwnerID == existing.ID orderby sg.ParentSecurityGroupID select sg).ToArrayAsync();
			var SecurityGroupMap = new Dictionary<Guid, Guid>();

			CopySecurityGroups(existingSecurityGroups, ref SecurityGroupMap, null, organization);

			await DataContext.SaveChangesAsync();

			//All of these are done this way with a conditional if because the triggers cause inserts that entity framework is not aware of. Note that they are parameterized to ensure no sql injections.

			foreach (var user in existingSecurityGroups.SelectMany(u => u.Users).DistinctBy(u => new {u.SecurityGroupID, u.UserID}))
			{
				await DataContext.Database.ExecuteSqlCommandAsync(@"IF NOT EXISTS(SELECT NULL FROM SecurityGroupUsers WHERE UserID = @UserID AND SecurityGroupID = @SecurityGroupID)
	INSERT INTO SecurityGroupUsers (UserID, SecurityGroupID, Overridden) VALUES (@UserID, @SecurityGroupID, 0)", new SqlParameter("UserID", user.UserID), new SqlParameter("SecurityGroupID", SecurityGroupMap[user.SecurityGroupID]));

			}


			

			//Org Acls
			var existingSecurityGroupIDs = SecurityGroupMap.Select(gm => gm.Key).ToArray();
			var existingOrganizationAcls = await (from a in DataContext.OrganizationAcls where a.OrganizationID == existing.ID && existingSecurityGroupIDs.Contains(a.SecurityGroupID) select a).Distinct().ToArrayAsync();

			foreach (var existingOrganizationAcl in existingOrganizationAcls)
			{
				if (!SecurityGroupMap.ContainsKey(existingOrganizationAcl.SecurityGroupID))
					SecurityGroupMap.Add(existingOrganizationAcl.SecurityGroupID, existingOrganizationAcl.SecurityGroupID);

				var count = await DataContext.Database.ExecuteSqlCommandAsync(@"IF NOT EXISTS(SELECT NULL FROM AclOrganizations WHERE OrganizationID = @OrganizationID AND SecurityGroupID = @SecurityGroupID AND PermissionID = @PermissionID)
	INSERT INTO AclOrganizations (OrganizationID, SecurityGroupID, PermissionID, Allowed, Overridden) VALUES (@OrganizationID, @SecurityGroupID, @PermissionID, @Allowed, 1)", new SqlParameter("OrganizationID", organization.ID), new SqlParameter("SecurityGroupID", SecurityGroupMap[existingOrganizationAcl.SecurityGroupID]), new SqlParameter("PermissionID", existingOrganizationAcl.PermissionID), new SqlParameter("Allowed", existingOrganizationAcl.Allowed));

			}

			//Org Event Acls
			var existingOrganizationEventAcls = await (from a in DataContext.OrganizationEvents where a.OrganizationID == existing.ID select a).ToArrayAsync();
			foreach (var existingOrganizationEventAcl in existingOrganizationEventAcls)
			{
				if (!SecurityGroupMap.ContainsKey(existingOrganizationEventAcl.SecurityGroupID))
					SecurityGroupMap.Add(existingOrganizationEventAcl.SecurityGroupID, existingOrganizationEventAcl.SecurityGroupID);

				await DataContext.Database.ExecuteSqlCommandAsync(@"IF NOT EXISTS(SELECT NULL FROM OrganizationEvents WHERE OrganizationID = @OrganizationID AND SecurityGroupID = @SecurityGroupID AND EventID = @EventID)
	INSERT INTO OrganizationEvents (OrganizationID, SecurityGroupID, EventID, Allowed, Overridden) VALUES (@OrganizationID, @SecurityGroupID, @EventID, @Allowed, 0)", new SqlParameter("OrganizationID", organization.ID), new SqlParameter("SecurityGroupID", SecurityGroupMap[existingOrganizationEventAcl.SecurityGroupID]), new SqlParameter("EventID", existingOrganizationEventAcl.EventID), new SqlParameter("Allowed", existingOrganizationEventAcl.Allowed));
			}

			return organization.ID;
		}

		private void CopySecurityGroups(SecurityGroup[] existingSecurityGroups, ref Dictionary<Guid, Guid> securityGroupMap, Guid? parentSecurityGroupID, Organization organization)
		{
			foreach (var existingSecurityGroup in existingSecurityGroups.Where(sg => (sg.ParentSecurityGroupID == null && parentSecurityGroupID == null) || (parentSecurityGroupID == null && sg.ParentSecurityGroupID.HasValue && !existingSecurityGroups.Any(esg => esg.ID == sg.ParentSecurityGroupID.Value)) || sg.ParentSecurityGroupID == parentSecurityGroupID))
			{

				//If there is a parent, and the parent isn't a group from the organization itself and it isn't in the map yet, then it's external and not changing, so the map is the same.
				if (existingSecurityGroup.ParentSecurityGroupID.HasValue && !securityGroupMap.ContainsKey(existingSecurityGroup.ParentSecurityGroupID.Value) && !existingSecurityGroups.Any(group => group.ID == existingSecurityGroup.ParentSecurityGroupID.Value))
					securityGroupMap.Add(existingSecurityGroup.ParentSecurityGroupID.Value, existingSecurityGroup.ParentSecurityGroupID.Value); 

				var sg = new SecurityGroup
				{
					Kind = existingSecurityGroup.Kind,
					Name = "New " + existingSecurityGroup.Name,
					OwnerID = organization.ID,
					Owner = organization.Acronym,
					Type = DTO.Enums.SecurityGroupTypes.Organization,
					Path = organization.Acronym + @"\" + existingSecurityGroup.Name,
					ParentSecurityGroupID = existingSecurityGroup.ParentSecurityGroupID.HasValue && existingSecurityGroup.ParentSecurityGroupID.Value != existingSecurityGroup.ID ? securityGroupMap[existingSecurityGroup.ParentSecurityGroupID.Value] : (Guid?)null
				};
				DataContext.SecurityGroups.Add(sg);

				securityGroupMap.Add(existingSecurityGroup.ID, sg.ID);

				CopySecurityGroups(existingSecurityGroups, ref securityGroupMap, existingSecurityGroup.ID, organization);
			}
		}

		/// <summary>
		/// Flags the organization as deleted.
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[HttpDelete]
		public override async Task Delete([FromUri] IEnumerable<Guid> ID)
		{
			if (!await DataContext.CanDelete<Organization>(Identity, ID.ToArray()))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have permission to delete this Organization."));

			var orgs = await (from o in DataContext.Organizations where ID.Contains(o.ID) && o.OrganizationType == DTO.Enums.OrganizationType.Local select o).ToArrayAsync();
            var childOrgs = await (from o in DataContext.Organizations where o.ParentOrganizationID.HasValue && ID.Contains(o.ParentOrganizationID.Value) && o.OrganizationType == DTO.Enums.OrganizationType.Local select o).ToArrayAsync();
            foreach(var child in childOrgs)
            {
                child.ParentOrganizationID = null;
            }

            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        var networkEntities = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(orgs.Select(x => x.ID).ToArray());
                        foreach (var org in orgs)
                        {
                            await CNDSEntityUpdater.CNDS.Organizations.Delete(networkEntities.Where(x => x.NetworkEntityID == org.ID).Select(x => x.EntityID).FirstOrDefault());
                            org.Deleted = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw;
                }
            }

            var users = await (from u in DataContext.Users where ID.Contains(u.OrganizationID.Value) select u).ToArrayAsync();
            foreach (var user in users)
            {
                user.Deleted = true;
            }

            var dms = await (from d in DataContext.DataMarts where ID.Contains(d.OrganizationID) select d).ToArrayAsync();
            foreach (var dm in dms)
            {
                dm.Deleted = true;
            }

            await DataContext.SaveChangesAsync();


            var securityGroups = await (from sg in DataContext.SecurityGroups where ID.Contains(sg.OwnerID) select sg.ID).ToArrayAsync();
            if (securityGroups.Any())
            {
                var securityGroupIDs = String.Join(",", securityGroups.Select(x => String.Format("'{0}'", x)));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclGlobal where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclDataMarts where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclDataMartRequestTypes where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclOrganizations where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclOrganizationDataMarts where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclOrganizationUsers where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclProjects where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclProjectOrganizations where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclProjectDataMarts where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclProjectDataMartRequestTypes where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclRegistries where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclRequests where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclRequestSharedFolders where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclRequestTypes where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclUsers where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from AclRequestTypes where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from GlobalEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from DataMartEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from GroupEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from OrganizationEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from ProjectEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from ProjectOrganizationEvents where SecurityGroupID IN ({0})", securityGroupIDs));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from ProjectDataMartEvents where SecurityGroupID IN ({0})", securityGroupIDs));
            }
        }

		/// <summary>
		/// Insert or updates list of organizations
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		[HttpPost]
		public override async Task<IEnumerable<OrganizationDTO>> InsertOrUpdate(IEnumerable<OrganizationDTO> values)
		{
			await CheckForDuplicates(values);
			var orgDTO = values.FirstOrDefault();
			if (orgDTO.ID == null || orgDTO.ID == Guid.Empty)
				orgDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();
			#region CNDS
			Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
			if (CNDSEntityUpdater.CanUpdateCNDS)
			{
				try
				{
					using (var cnds = new CNDSEntityUpdater(networkID))
					{
						await CNDSEntityUpdater.RegisterOrUpdateOrganizations(orgDTO);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex.Message, ex);
				}
			}


			var org = DataContext.Organizations.Find(orgDTO.ID);
			if (org == null)
			{
				org = new Organization
				{
					ID = orgDTO.ID.Value,
					Name = orgDTO.Name,
					Acronym = orgDTO.Acronym,
					ParentOrganizationID = orgDTO.ParentOrganizationID,
                    ContactEmail = orgDTO.ContactEmail,
                    ContactFirstName = orgDTO.ContactFirstName,
                    ContactLastName = orgDTO.ContactLastName,
                    ContactPhone = orgDTO.ContactPhone
                };
				DataContext.Organizations.Add(org);
			}
			else
			{
				org.Name = orgDTO.Name;
				org.Acronym = orgDTO.Acronym;
				org.ParentOrganizationID = orgDTO.ParentOrganizationID;
                org.ContactEmail = orgDTO.ContactEmail;
                org.ContactFirstName = orgDTO.ContactFirstName;
                org.ContactLastName = orgDTO.ContactLastName;
                org.ContactPhone = orgDTO.ContactPhone;

            }

			DataContext.SaveChanges();
			#endregion CNDS
			IList<OrganizationDTO> returnOrg = new List<OrganizationDTO>();
			returnOrg.Add(orgDTO);
			return returnOrg;
		}
		/// <summary>
		///  Insert or updates list of organizations
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		[HttpPost]
		public override async Task<IEnumerable<OrganizationDTO>> Insert(IEnumerable<OrganizationDTO> values)
		{
            await CheckForDuplicates(values);
            var orgDTO = values.FirstOrDefault();
            if (orgDTO.ID == null || orgDTO.ID == Guid.Empty)
                orgDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();
            #region CNDS
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        await CNDSEntityUpdater.RegisterOrUpdateOrganizations(orgDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }


            var org = new Organization
            {
                ID = orgDTO.ID.Value,
                Name = orgDTO.Name,
                Acronym = orgDTO.Acronym,
                ParentOrganizationID = orgDTO.ParentOrganizationID,
                ContactEmail = orgDTO.ContactEmail,
                ContactFirstName = orgDTO.ContactFirstName,
                ContactLastName = orgDTO.ContactLastName,
                ContactPhone = orgDTO.ContactPhone
            };
            DataContext.Organizations.Add(org);

            DataContext.SaveChanges();
            #endregion CNDS
            IList<OrganizationDTO> returnOrg = new List<OrganizationDTO>();
            returnOrg.Add(orgDTO);
            return returnOrg;
        }
		/// <summary>
		///  Insert or updates list of organizations
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		[HttpPut]
		public override async Task<IEnumerable<OrganizationDTO>> Update(IEnumerable<OrganizationDTO> values)
		{
            await CheckForDuplicates(values);
            var orgDTO = values.FirstOrDefault();
            if (orgDTO.ID == null || orgDTO.ID == Guid.Empty)
                orgDTO.ID = Lpp.Utilities.DatabaseEx.NewGuid();
            #region CNDS
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            if (CNDSEntityUpdater.CanUpdateCNDS)
            {
                try
                {
                    using (var cnds = new CNDSEntityUpdater(networkID))
                    {
                        await CNDSEntityUpdater.RegisterOrUpdateOrganizations(orgDTO);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }


            var org = DataContext.Organizations.Find(orgDTO.ID);
            if (org == null)
            {
                org = new Organization
                {
                    ID = orgDTO.ID.Value,
                    Name = orgDTO.Name,
                    Acronym = orgDTO.Acronym,
                    ParentOrganizationID = orgDTO.ParentOrganizationID,
                    ContactEmail = orgDTO.ContactEmail,
                    ContactFirstName = orgDTO.ContactFirstName,
                    ContactLastName = orgDTO.ContactLastName,
                    ContactPhone = orgDTO.ContactPhone
                };
                DataContext.Organizations.Add(org);
            }
            else
            {
                org.Name = orgDTO.Name;
                org.Acronym = orgDTO.Acronym;
                org.ParentOrganizationID = orgDTO.ParentOrganizationID;
                org.ContactEmail = orgDTO.ContactEmail;
                org.ContactFirstName = orgDTO.ContactFirstName;
                org.ContactLastName = orgDTO.ContactLastName;
                org.ContactPhone = orgDTO.ContactPhone;
            }

            DataContext.SaveChanges();
            #endregion CNDS
            IList<OrganizationDTO> returnOrg = new List<OrganizationDTO>();
            returnOrg.Add(orgDTO);
            return returnOrg;
        }

        private async Task CheckForDuplicates(IEnumerable<OrganizationDTO> updates)
		{
			var ids = updates.Where(u => u.ID.HasValue).Select(u => u.ID.Value).ToArray();
			var names = updates.Select(u => u.Name).ToArray();
			var acronyms = updates.Where(u => u.Acronym != null && u.Acronym != "").Select(u => u.Acronym).ToArray();

			if (updates.GroupBy(u => u.Acronym).Any(u => u.Count() > 1))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Acronym of Organizations must be unique."));

			if (updates.GroupBy(u => u.Name).Any(u => u.Count() > 1))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Name of Organizations must be unique."));

			if (await (from o in DataContext.Organizations where !o.Deleted && !ids.Contains(o.ID) && (names.Contains(o.Name) || acronyms.Contains(o.Acronym)) select o).AnyAsync())
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The Name and Acronym of Organizations must be unique."));

            
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgMetadata"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateOrganizationVisibility(IEnumerable<MetadataDTO> orgMetadata)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var cnds = new CNDSEntityUpdater(networkID))
            {
                await CNDSEntityUpdater.RegisterOrUpdateOrganizationDomainVisibility(orgMetadata);
            }
        }
    }
}
