using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using Lpp.CNDS.DTO.Enums;
using System.Data.SqlClient;

namespace Lpp.CNDS.Api.Organizations
{
    /// <summary>
    /// WebAPI Controller for all Organization Actions
    /// </summary>
    [AllowAnonymous]
    public class OrganizationsController : LppApiController<DataContext>
    {
        /// <summary>
        /// An OData Enpoint for Listing all Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<OrganizationDTO> List()
        {
            return DataContext.Organizations.Where(o => !o.Deleted).Map<Organization, OrganizationDTO>();
        }
        /// <summary>
        /// An Endpoint for Listing information about a specific Organization
        /// </summary>
        /// <param name="id">The Guid ID of the Organization</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<OrganizationDTO> Get(Guid id)
        {
            return DataContext.Organizations.Where(o => o.ID == id && !o.Deleted).Map<Organization, OrganizationDTO>();
        }
        /// <summary>
        /// An OData Enpoint for an Organizations Domain Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDataDTO> ListOrganizationDomains(Guid id)
        {
            return (from od in DataContext.DomainDatas.OfType<OrganizationDomainData>()
                    join o in DataContext.Organizations on od.OrganizationID equals o.ID
                    where od.OrganizationID == id && !o.Deleted && od.DomainUse.Deleted == false
                    select new DomainDataDTO {
                        ID = od.ID,
                        EntityID = od.OrganizationID,
                        DomainUseID = od.DomainUseID,
                        DomainReferenceID = od.DomainReferenceID,
                        Value = od.Value,
                        SequenceNumber = od.SequenceNumber,
                    });
        }
        /// <summary>
        /// Endpoint for Registering an Organization
        /// </summary>
        /// <param name="dto">The DTO for Transfering all Organization Requirements</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OrganizationDTO> Register(OrganizationTransferDTO dto)
        {
            var organizationEntity = DataContext.NetworkEntities.Add(new NetworkEntity { NetworkID = dto.NetworkID.Value, NetworkEntityID = dto.ID, EntityType = EntityType.Organization });
            await DataContext.SaveChangesAsync();

            Data.Organization organization = DataContext.Organizations.Add(new Data.Organization()
            {
                ID = organizationEntity.ID,
                Name = dto.Name,
                NetworkID = dto.NetworkID.Value,
                Acronym = dto.Acronym,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                ContactFirstName = dto.ContactFirstName,
                ContactLastName = dto.ContactLastName
            });

            if (dto.ParentOrganizationID.HasValue)
            {
                organization.ParentOrganizationID = dto.ParentOrganizationID.Value;
            }

            await DataContext.SaveChangesAsync();
            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                IList<Data.OrganizationDomainData> metadata = new List<Data.OrganizationDomainData>();
                foreach (var meta in dto.Metadata)
                {
                    var orgMeta = new OrganizationDomainData()
                    {
                        OrganizationID = organization.ID,
                        DomainUseID = meta.DomainUseID,
                        Value = meta.Value,
                        SequenceNumber = meta.SequenceNumber,
                    };
                    if (meta.DomainReferenceID.HasValue)
                        orgMeta.DomainReferenceID = meta.DomainReferenceID.Value;
                    metadata.Add(orgMeta);
                }
                if (metadata.Count > 0)
                    DataContext.DomainDatas.AddRange(metadata);
                await DataContext.SaveChangesAsync();
            }

            return organization.Map<Organization, OrganizationDTO>();
        }
        /// <summary>
        /// Enpoint for Updating an Organization
        /// </summary>
        /// <param name="dto">The Organization Object to be updated</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OrganizationDTO> Update(OrganizationTransferDTO dto) {
            var organizationMetadata = await DataContext.DomainDatas.OfType<OrganizationDomainData>().Where(x => x.OrganizationID == dto.ID).ToArrayAsync();
            var organization = await DataContext.Organizations.FindAsync(dto.ID);

            if (organization.Deleted)
            {
               throw new Exception("The specified Organization was not found.");
            }
            organization.Name = dto.Name;
            organization.Acronym = dto.Acronym;
            organization.ContactEmail = dto.ContactEmail;
            organization.ContactPhone = dto.ContactPhone;
            organization.ContactFirstName = dto.ContactFirstName;
            organization.ContactLastName = dto.ContactLastName;

            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                IList<OrganizationDomainData> metaDataToAdd = new List<OrganizationDomainData>();
                foreach (var meta in dto.Metadata.Where(m => !m.ID.HasValue))
                {
                    var orgMeta = new OrganizationDomainData()
                    {
                        OrganizationID = organization.ID,
                        DomainUseID = meta.DomainUseID,
                        Value = meta.Value,
                        SequenceNumber = meta.SequenceNumber,
                    };
                    if (meta.DomainReferenceID.HasValue)
                        orgMeta.DomainReferenceID = meta.DomainReferenceID.Value;
                    metaDataToAdd.Add(orgMeta);
                }
                if (metaDataToAdd.Count > 0)
                    DataContext.DomainDatas.AddRange(metaDataToAdd);

                foreach (var meta in organizationMetadata.Where(org => dto.Metadata.Any(m => m.ID == org.ID && (org.Value != m.Value || org.SequenceNumber != m.SequenceNumber || org.DomainReferenceID != m.DomainReferenceID))))
                {
                    var diff = dto.Metadata.Where(m => m.ID == meta.ID).FirstOrDefault();
                    DataContext.DomainDatas.Attach(meta);
                    if (meta.Value != diff.Value)
                        meta.Value = diff.Value;
                    if (meta.SequenceNumber != diff.SequenceNumber)
                        meta.SequenceNumber = diff.SequenceNumber;
                    if (meta.DomainReferenceID != diff.DomainReferenceID)
                        meta.DomainReferenceID = diff.DomainReferenceID;

                }
                await DataContext.SaveChangesAsync();

            }

            var metadataIDs = dto.Metadata.Where(x => x.ID.HasValue).Select(x => x.ID.Value);
            var remove = (from d in organizationMetadata
                                where !metadataIDs.Contains(d.ID) && d.OrganizationID == dto.ID
                                select d.ID).ToArray();


            if (remove.Count() > 0)
            {
                //Have to do this cause of trigger
                var removeIDs = String.Join(",", remove.Select(x => String.Format("'{0}'", x)));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from OrganizationDomainData where ID IN ({0})", removeIDs));
            }

            return organization.Map<Organization, OrganizationDTO>();
        }
        /// <summary>
        /// An Endpoint for Deleting a specific Organization
        /// </summary>
        /// <param name="id">The Guid ID of the Organization</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var org = DataContext.Organizations.Find(id);
            if (org == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified Organization was not found.");
            }
            else
            {
                if (org.Deleted)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified org was not found.");
                }
                var deletedTime = DateTime.Now;
                org.Deleted = true;
                org.DeletedOn = deletedTime;
                await DataContext.SaveChangesAsync();

                await DataContext.Database.ExecuteSqlCommandAsync("UPDATE Users SET Deleted = 1, DeletedOn = @deletedon Where OrganizationID = @orgID", new SqlParameter("@deletedon", deletedTime), new SqlParameter("@orgID", org.ID));
                await DataContext.Database.ExecuteSqlCommandAsync("UPDATE DataSources SET Deleted = 1, DeletedOn = @deletedon Where OrganizationID = @orgID", new SqlParameter("@deletedon", deletedTime), new SqlParameter("@orgID", org.ID));

                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
        /// <summary>
        /// Endpoint for Retreving Visibility of Domains
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDataDTO> GetDomainVisibility(Guid id)
        {
            return (from uda in DataContext.DomainAccess.OfType<OrganizationDomainAccess>()
                    join du in DataContext.DomainUses on uda.DomainUse.ID equals du.ID
                    where uda.OrganizationID == id
                    select new DomainDataDTO
                    {
                        ID = du.DomainID,
                        EntityID = uda.OrganizationID,
                        DomainUseID = du.ID,
                        Visibility = uda.AccessType
                    });
        }
        [HttpPost]
        public async Task UpdateDomainVisibility(IEnumerable<DomainDataDTO> domains)
        {
            try
            {
                foreach (var domain in domains)
                {
                    var previousDomain = DataContext.DomainAccess.OfType<OrganizationDomainAccess>().FirstOrDefault(x => x.DomainUseID == domain.DomainUseID && x.OrganizationID == domain.EntityID);

                    if (previousDomain == null)
                    {
                        DataContext.DomainAccess.Add(new OrganizationDomainAccess
                        {
                            OrganizationID = domain.EntityID.Value,
                            DomainUseID = domain.DomainUseID,
                            AccessType = domain.Visibility,
                        });
                    }
                    else
                    {
                        previousDomain.AccessType = domain.Visibility;
                    }

                }
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IEnumerable<OrganizationSearchDTO>> OrganizationSearch(SearchDTO ids)
        {

            var domainUseIDs = await (from d in DataContext.Domains.AsNoTracking()
                                      join du in DataContext.DomainUses on d.ID equals du.DomainID
                                      where ids.DomainIDs.Contains(d.ID) && d.DataType != "container" && d.DataType != "booleanGroup" && du.EntityType == EntityType.Organization
                                      select du.ID).ToListAsync();

            var query = await (from odd in DataContext.DomainDatas.OfType<OrganizationDomainData>().AsNoTracking()
                               join ooa in DataContext.DomainAccess.OfType<OrganizationDomainAccess>().AsNoTracking() on new { odd.DomainUseID, odd.OrganizationID } equals new { ooa.DomainUseID, ooa.OrganizationID }
                               join o in DataContext.Organizations on ooa.OrganizationID equals o.ID
                               join ne in DataContext.NetworkEntities on o.ID equals ne.ID
                               where odd.Value != "false" && odd.Value.Trim() != ""
                               && (ids.DomainReferencesIDs.Count() == 0 ? domainUseIDs.Contains(odd.DomainUseID) : true)
                               && (ids.DomainReferencesIDs.Count() > 0 ? domainUseIDs.Contains(odd.DomainUseID) || ids.DomainReferencesIDs.Contains(odd.DomainReferenceID.Value) : true)
                               && (ooa.AccessType == AccessType.NoOne ? false : ooa.AccessType == AccessType.MyNetwork ? ne.NetworkID == ids.NetworkID : true)
                               group new { odd.DomainUseID, odd.DomainReferenceID } by odd.OrganizationID into res
                               select new
                               {
                                   ID = res.Key,
                                   DomainUseIDs = res.Where(x => x.DomainUseID != null).Select(x => x.DomainUseID).Distinct().ToList(),
                                   DomainReferenceIDs = res.Where(x => x.DomainReferenceID != null && ids.DomainReferencesIDs.Contains(x.DomainReferenceID.Value)).Select(x => x.DomainReferenceID).Distinct().ToList()
                               }).ToArrayAsync();

            var dsIDs = query.Where(x =>
                                    (ids.DomainReferencesIDs.Count() == 0 ? x.DomainUseIDs.Count() == domainUseIDs.Count() : true)
                                    && (ids.DomainReferencesIDs.Count() > 0 ? x.DomainUseIDs.Count() == domainUseIDs.Count() && x.DomainReferenceIDs.Count() == ids.DomainReferencesIDs.Count() : true
                                    )).Select(x => x.ID).ToList();


            return await (from o in DataContext.Organizations.AsNoTracking()
                          join ne in DataContext.NetworkEntities.AsNoTracking() on o.ID equals ne.ID
                          join n in DataContext.Networks.AsNoTracking() on ne.NetworkID equals n.ID
                          where dsIDs.Contains(o.ID)
                          select new OrganizationSearchDTO
                          {
                              ID = o.ID,
                              Name = o.Name,
                              Network = n.Name,
                              NetworkID = n.ID,
                              ContactInformation = "<span>" + o.ContactFirstName + " " + o.ContactLastName + "</span><p><span>" + o.ContactEmail + "</span><p><span>" + o.ContactPhone + "</span>"
                          }).Distinct().OrderByDescending(x => x.Name).ToArrayAsync();
        }
    }
}
