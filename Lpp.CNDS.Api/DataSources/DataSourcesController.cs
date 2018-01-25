using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.CNDS.DTO.Enums;
using Lpp.Utilities;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lpp.CNDS.Api.DataSources
{
    [AllowAnonymous]
    public class DataSourcesController : LppApiController<DataContext>
    {
        /// <summary>
        /// An OData Enpoint for Listing all Datamarts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataSourceDTO> List()
        {
            return DataContext.DataSources.Where(dm => !dm.Deleted).AsQueryable().Map<DataSource, DataSourceDTO>();
        }

        /// <summary>
        /// Returns all DataSources with extended details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataSourceExtendedDTO> ListExtended()
        {
            var q = DataContext.DataSources.Where(dm => !dm.Deleted).Map<DataSource, DataSourceExtendedDTO>();
            return q;
        }

        /// <summary>
        /// Endpoint for Retieving Details about a specific Datamart
        /// </summary>
        /// <param name="id">The Guid ID of a Datamart</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DataSourceDTO> Get(Guid id)
        {
            return DataContext.DataSources.Where(dm => dm.ID == id && !dm.Deleted).Map<DataSource, DataSourceDTO>();
        }
        /// <summary>
        /// An OData Enpoint for a DataSource Domain Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDataDTO> ListDataSourceDomains(Guid id)
        {
            var q = from od in DataContext.DomainDatas.OfType<DataSourceDomainData>()
                    join dm in DataContext.DataSources on od.DataSourceID equals dm.ID
                    where od.DataSourceID == id && !dm.Deleted && od.DomainUse.Deleted == false
                    select new DomainDataDTO
                    {
                        ID = od.ID,
                        EntityID = od.DataSourceID,
                        DomainUseID = od.DomainUseID,
                        DomainReferenceID = od.DomainReferenceID,
                        Value = od.Value,
                        SequenceNumber = od.SequenceNumber
                    };

            return q;
        }
        /// <summary>
        /// Endpoint for Inserting a DataSource
        /// </summary>
        /// <param name="dto">The DTO for the particular DataSource</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DataSourceDTO> Register(DataSourceTransferDTO dto)
        {

            NetworkEntity datamartEntity = DataContext.NetworkEntities.Add(new NetworkEntity { NetworkID = dto.NetworkID.Value, NetworkEntityID = dto.ID, EntityType = EntityType.DataSource });

            Data.DataSource datamart = DataContext.DataSources.Add(new Data.DataSource()
            {
                ID = datamartEntity.ID,
                Name = dto.Name,
                Acronym = dto.Acronym,
                OrganizationID = dto.OrganizationID,
                AdapterSupportedID = dto.AdapterSupportedID
            });

            await DataContext.SaveChangesAsync();

            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                IList<Data.DataSourceDomainData> metadata = new List<Data.DataSourceDomainData>();
                foreach (var meta in dto.Metadata)
                {
                    var orgMeta = new DataSourceDomainData()
                    {
                        DataSourceID = datamart.ID,
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

            await DataContext.Entry(datamart).Reference(dm => dm.AdapterSupported).LoadAsync();

            return datamart.Map<DataSource, DataSourceDTO>();
        }
        /// <summary>
        /// Endpoint for Updating a DataSource
        /// </summary>
        /// <param name="dto">The DTO for the particular DataSource</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DataSourceDTO> Update(DataSourceTransferDTO dto)
        {
            var datamart = await DataContext.DataSources.FindAsync(dto.ID);

            if (datamart.Deleted)
            {
                throw new Exception("The Specified DataSource was not found");
            }

            datamart.Name = dto.Name;
            datamart.Acronym = dto.Acronym;
            datamart.OrganizationID = dto.OrganizationID;
            datamart.AdapterSupportedID = dto.AdapterSupportedID;

            await DataContext.SaveChangesAsync();


            var dataMartMetaData = await DataContext.DomainDatas.OfType<DataSourceDomainData>().Where(x => x.DataSourceID == dto.ID).ToArrayAsync();
            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                

                IList<DataSourceDomainData> metaDataToAdd = new List<DataSourceDomainData>();
                foreach (var meta in dto.Metadata.Where(m => !m.ID.HasValue))
                {
                    var dsMeta = new DataSourceDomainData()
                    {
                        DataSourceID = dto.ID,
                        DomainUseID = meta.DomainUseID,
                        Value = meta.Value,
                        SequenceNumber = meta.SequenceNumber,
                    };
                    if (meta.DomainReferenceID.HasValue)
                        dsMeta.DomainReferenceID = meta.DomainReferenceID.Value;
                    metaDataToAdd.Add(dsMeta);
                }
                if (metaDataToAdd.Count > 0)
                    DataContext.DomainDatas.AddRange(metaDataToAdd);

                foreach (var meta in dataMartMetaData.Where(org => dto.Metadata.Any(m => m.ID == org.ID && (org.Value != m.Value || org.SequenceNumber != m.SequenceNumber || org.DomainReferenceID != m.DomainReferenceID))))
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
            var remove = (from d in dataMartMetaData
                                where !metadataIDs.Contains(d.ID) && d.DataSourceID == dto.ID
                                select d.ID).ToArray();


            if (remove.Count() > 0)
            {
                //Have to do this cause of trigger
                var removeIDs = String.Join(",", remove.Select(x => String.Format("'{0}'", x)));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from DataSourceDomainData where ID IN ({0})", removeIDs));
            }

            await DataContext.Entry(datamart).Reference(dm => dm.AdapterSupported).LoadAsync();

            return datamart.Map<DataSource, DataSourceDTO>();
        }

        /// <summary>
        /// Endpoint for Deleting a DataSource
        /// </summary>
        /// <param name="id">The Guid ID of the DataSource</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var dm = DataContext.DataSources.Find(id);
            if (dm == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified DataSource was not found.");
            }
            else if (dm.Deleted)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified DataSource was not found.");
            }
            else
            {
                dm.Deleted = true;
                dm.DeletedOn = DateTime.Now;
                await DataContext.SaveChangesAsync();

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
            var data = (from uda in DataContext.DomainAccess.OfType<DataSourceDomainAccess>()
                    join du in DataContext.DomainUses on uda.DomainUse.ID equals du.ID
                    where uda.DataSourceID == id
                    && du.Deleted == false && du.Domain.Deleted == false
                    select new DomainDataDTO
                    {
                        ID = du.DomainID,
                        EntityID = uda.DataSourceID,
                        DomainUseID = du.ID,
                        Visibility = uda.AccessType
                    });

            return data;
        }

        [HttpPost]
        public async Task UpdateDomainVisibility(IEnumerable<DomainDataDTO> domains)
        {
            try
            {
                var entities = domains.Select(d => d.EntityID).Distinct().ToArray();
                var domainUses = domains.Select(d => d.DomainUseID).Distinct().ToArray();

                var domainAccess = await (from dsda in DataContext.DomainAccess.OfType<DataSourceDomainAccess>()
                                   where entities.Contains(dsda.DataSourceID)
                                   && domainUses.Contains(dsda.DomainUseID)
                                   select dsda).ToArrayAsync();

                foreach(var dom in domains)
                {
                    var domain = domainAccess.Where(da => da.DataSourceID == dom.EntityID.Value && da.DomainUseID == dom.DomainUseID).FirstOrDefault();
                    if(domain == null)
                    {
                        DataContext.DomainAccess.Add(new DataSourceDomainAccess {
                            DataSourceID = dom.EntityID.Value,
                            DomainUseID = dom.DomainUseID,
                            AccessType = dom.Visibility
                        });
                    }
                    else
                    {
                        domain.AccessType = dom.Visibility;
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
        public async Task<IEnumerable<DataSourceSearchDTO>> DataSourceSearch(SearchDTO ids)
        {
            //*****Summary of Execution*******
            // ALL Domains and DomainReferences are ANDed together except for Container 
            // (due to its DomainUseID will never be in the DomainData Table cause it will never have a value).
            // Now in a specific occasion References will be ommited, and that is if none of its children are selected.



            var domainUseIDs = from d in DataContext.Domains.AsNoTracking()
                                join du in DataContext.DomainUses on d.ID equals du.DomainID
                                where ids.DomainIDs.Contains(d.ID) && d.DataType != "container"
                                && du.EntityType == EntityType.DataSource && !d.Deleted && !du.Deleted
                                select new {
                                    DomainUseID = du.ID,
                                    DataType = d.DataType
                                };

            var filterBasedDomainReferences = from d in DataContext.Domains.AsNoTracking()
                      join dr in DataContext.DomainReferences on d.ID equals dr.DomainID
                      join du in DataContext.DomainUses on d.ID equals du.DomainID
                      where ids.DomainReferencesIDs.Contains(dr.ID) && du.EntityType == EntityType.DataSource && !d.Deleted && !du.Deleted
                      select du.ID;


            List<Guid> DomainUseToRemove = new List<Guid>();

            foreach (var id in domainUseIDs.Where(x => x.DataType == "reference").Select(x => x.DomainUseID))
            {
                bool toBeFiltered = filterBasedDomainReferences.Any(x => x == id);
                if (!toBeFiltered)
                    DomainUseToRemove.Add(id);
            }
                

            var newDomainUseID = domainUseIDs.Where(x => !DomainUseToRemove.Contains(x.DomainUseID)).Select(x => x.DomainUseID);

            var query =  from d in DataContext.DomainDatas.OfType<DataSourceDomainData>().AsNoTracking()
                            join dda in DataContext.DomainAccess.OfType<DataSourceDomainAccess>().AsNoTracking() on new { d.DomainUseID, d.DataSourceID } equals new { dda.DomainUseID, dda.DataSourceID }
                            join ds in DataContext.DataSources on dda.DataSourceID equals ds.ID
                            join ne in DataContext.NetworkEntities on ds.ID equals ne.ID
                            where d.Value != "false" && d.Value.Trim() != ""
                            && (ids.DomainReferencesIDs.Count() == 0 ? newDomainUseID.Contains(d.DomainUseID) : true)
                            && (ids.DomainReferencesIDs.Count() > 0 ? newDomainUseID.Contains(d.DomainUseID) || ids.DomainReferencesIDs.Contains(d.DomainReferenceID.Value) : true)
                            && (dda.AccessType == AccessType.NoOne ? false : dda.AccessType == AccessType.MyNetwork ? ne.NetworkID == ids.NetworkID : true)
                            group new { d.DomainUseID, d.DomainReferenceID } by d.DataSourceID into res
                            select new
                            {
                                ID = res.Key,
                                DomainUseIDs = res.Where(x => x.DomainUseID != null).Select(x => x.DomainUseID).Distinct().ToList(),
                                DomainReferenceIDs = res.Where(x => x.DomainReferenceID != null && ids.DomainReferencesIDs.Contains(x.DomainReferenceID.Value)).Select(x => x.DomainReferenceID).Distinct().ToList()
                            };

            var dsIDs = query.Where(x =>
                                    (ids.DomainReferencesIDs.Count() == 0 ? x.DomainUseIDs.Count() == newDomainUseID.Count() : true)
                                    && (ids.DomainReferencesIDs.Count() > 0 ? x.DomainUseIDs.Count() == newDomainUseID.Count() && x.DomainReferenceIDs.Count() == ids.DomainReferencesIDs.Count() : true
                                    )).Select(x => x.ID);

            return await (from d in DataContext.DataSources.AsNoTracking()
                   join ne in DataContext.NetworkEntities.AsNoTracking() on d.ID equals ne.ID
                   join n in DataContext.Networks.AsNoTracking() on ne.NetworkID equals n.ID
                   join o in DataContext.Organizations.AsNoTracking() on d.OrganizationID equals o.ID
                   where dsIDs.Contains(d.ID)
                   select new DataSourceSearchDTO
                   {
                       ID = d.ID,
                       Name = d.Name,
                       Network = n.Name,
                       NetworkID = n.ID,
                       Organization = o.Name,
                       OrganizationID = o.ID,
                       ContactInformation = "<span>" + o.ContactFirstName + " " + o.ContactLastName + "</span><p><span>" + o.ContactEmail + "</span><p><span>" + o.ContactPhone + "</span>"
                   }).Distinct().OrderByDescending(x => x.Name).ToArrayAsync();
        }
    }
}



//var duVisibility = await(from dda in DataContext.DomainAccess.OfType<DataSourceDomainAccess>().AsNoTracking()
//                         join d in DataContext.DataSources on dda.DataSourceID equals d.ID
//                         join ne in DataContext.NetworkEntities on d.ID equals ne.ID
//                         where domainUseIDs.Contains(dda.DomainUseID) //&& dda.AccessType != AccessType.NoOne
//                         && (dda.AccessType == AccessType.NoOne ? false : dda.AccessType == AccessType.MyNetwork ? ne.NetworkID == ids.NetworkID : true)
//                         select dda.DomainUseID).ToListAsync();