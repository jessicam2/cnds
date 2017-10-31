using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Lpp.CNDS.Api.Domains
{
    public class DomainController : LppApiController<DataContext>
    {
        /// <summary>
        /// Endoint for Querying Domains and Their Domain Refernces.  Supports OData Queries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDTO> List()
        {
            var domainUses = (from du in DataContext.DomainUses
                              join d in DataContext.Domains on du.DomainID equals d.ID
                              let drs = (from dr in DataContext.DomainReferences where d.ID == dr.DomainID && d.Deleted == false && dr.Deleted == false select new DomainReferenceDTO {
                                  ID= dr.ID,
                                  Title = dr.Title,
                                  Description = dr.Description,
                                  Value = dr.Value,
                                  ParentDomainReferenceID = dr.ParentDomainReferenceID,
                                  DomainID = dr.DomainID,
                              })
                              where du.Deleted == false
                              && d.Deleted == false
                              select new DomainDTO {
                                  ID = d.ID,
                                  DomainUseID = du.ID,
                                  DataType = d.DataType,
                                  Title = d.Title,
                                  ParentDomainID = d.ParentDomainID,
                                  EnumValue = d.EnumValue,
                                  IsMultiValue = d.IsMultiValue,
                                  EntityType = du.EntityType,
                                  References = drs
                              });
            return domainUses;
        }
        /// <summary>
        /// Endoint for Querying Domains and Their Domain Refernces.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDTO> ListDomains()
        {
            var domains = ( from d in DataContext.Domains
                            let drs = (from dr in DataContext.DomainReferences
                                       where d.ID == dr.DomainID && d.Deleted == false && dr.Deleted == false
                                       select dr).DefaultIfEmpty()
                            where d.Deleted == false
                            select new DomainDTO
                            {
                                ID = d.ID,
                                DataType = d.DataType,
                                Title = d.Title,
                                ParentDomainID = d.ParentDomainID,
                                EnumValue = d.EnumValue,
                                IsMultiValue = d.IsMultiValue,
                                References = drs.Where(dr => dr != null).Select(dr => new DomainReferenceDTO
                                {
                                    ID = dr.ID,
                                    Title = dr.Title,
                                    Description = dr.Description,
                                    Value = dr.Value,
                                    ParentDomainReferenceID = dr.ParentDomainReferenceID,
                                    DomainID = dr.DomainID,
                                })
                            });


            return domains;
        }

        /// <summary>
        /// Insert or Update Metadata and their References
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task InsertOrUpdateDomains(IEnumerable<DomainDTO> domainDatas)
        {
            var originalDomains = DataContext.Domains.Include(x=> x.ChildrenDomains).Include(x => x.DomainReferences).ToArray();
            var orginalDomainReferences = DataContext.DomainReferences.ToArray();

            var domainIDs = new List<Guid>();
            var domainRefIDs = new List<Guid>();
            foreach(var domain in domainDatas)
            {
                var recurse = RecursiveGetDomainID(domain);
                if (recurse.DomainIDs != null && recurse.DomainIDs.Count() > 0)
                    domainIDs.AddRange(recurse.DomainIDs);
                if (recurse.DomainRefIDs != null && recurse.DomainRefIDs.Count() > 0)
                    domainRefIDs.AddRange(recurse.DomainRefIDs);
            }

            var removeDomains = (from d in originalDomains
                                 where !domainIDs.Contains(d.ID)
                                 select d).ToArray();

            var removeDomainRefs =  (from d in orginalDomainReferences
                                      where !domainRefIDs.Contains(d.ID)
                                      select d).ToArray();

            if (removeDomainRefs.Count() > 0 || removeDomains.Count() > 0)
            {
                if (removeDomainRefs.Count() > 0)
                {
                    var domainRefs = String.Join(",", removeDomainRefs.Select(x => String.Format("'{0}'", x.ID)));
                    try
                    {
                        await DataContext.Database.ExecuteSqlCommandAsync(String.Format("Update DomainReference SET Deleted = 1 Where ID in ({0})", domainRefs));

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    
                }
                    
                if (removeDomains.Count() > 0)
                {
                    var domainsID = String.Join(",", removeDomains.Select(x => String.Format("'{0}'", x.ID)));
                    try
                    {
                        await DataContext.Database.ExecuteSqlCommandAsync(String.Format("Update Domain SET Deleted = 1 Where ID in ({0})", domainsID));
                        await DataContext.Database.ExecuteSqlCommandAsync(String.Format("Update DomainUse SET Deleted = 1 Where DomainID in ({0})", domainsID));

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    
                }
                 
                await DataContext.SaveChangesAsync();

            }


            foreach (var domain in domainDatas)
            {
                await RecusiveDomainsAdd(domain, DataContext, originalDomains);
            }

            await DataContext.SaveChangesAsync();

        }

        /// <summary>
        /// Insert or Update Metadata and their References
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task InsertOrUpdateDomainUses(AddRemoveDomainUseDTO changes)
        {
            //delete first
            if(changes.RemoveDomainUse != null && changes.RemoveDomainUse.Any())
            {
                var removeIDs = changes.RemoveDomainUse.Select(k => k.Key).ToArray();
                var items = await (from du in DataContext.DomainUses
                            let referenceCount = du.DomainDatas.Count
                            where removeIDs.Contains(du.ID)
                            select new {
                                Item = du,
                                Count = referenceCount
                            }).ToArrayAsync();

                var softDeleteItems = items.Where(i => i.Count > 0);
                foreach(var item in softDeleteItems)
                {
                    item.Item.Deleted = true;
                }

                DataContext.DomainUses.RemoveRange(items.Where(i => i.Count == 0).Select(i => i.Item));
            }

            //undelete or insert
            if (changes.AddDomainUse != null && changes.AddDomainUse.Any())
            {
                //undelete first
                var query = DataContext.DomainUses.Where(i => i.Deleted);
                foreach(var item in changes.AddDomainUse)
                {
                    query = query.Where(i => i.DomainID == item.Key && i.EntityType == item.Value);
                }

                var toUndelete = await query.ToArrayAsync();
                foreach(var item in toUndelete)
                {
                    item.Deleted = false;
                }

                //get the domain IDs that need to be inserted
                var toAddDomainIDs = changes.AddDomainUse.Where(i => toUndelete.Select(x => x.DomainID).Contains(i.Key) == false);

                //get any existing - double check
                var existingQuery = DataContext.DomainUses.AsQueryable();
                foreach (var item in toAddDomainIDs)
                {
                    existingQuery = existingQuery.Where(i => i.DomainID == item.Key && i.EntityType == item.Value);
                }
                var existingDomainUses = await existingQuery.ToArrayAsync();

                foreach (var item in toAddDomainIDs)
                {
                    var existing = existingDomainUses.FirstOrDefault(i => i.DomainID == item.Key && i.EntityType == item.Value);
                    if (existing != null)
                    {
                        if (existing.Deleted)
                            existing.Deleted = false;
                    }
                    else
                    {
                        DataContext.DomainUses.Add(new DomainUse
                        {
                            DomainID = item.Key,
                            EntityType = item.Value
                        });
                    }
                }

            }

            await DataContext.SaveChangesAsync();
        }

        //Change ParentID to the actual Parent Object
        static async Task<Domain> RecusiveDomainsAdd(DomainDTO domain, DataContext db, IEnumerable<Domain> originalDomains)
        {
            var domainCheck = originalDomains.Where(x => x.ID == domain.ID).FirstOrDefault();
            if (domainCheck == null)
            {
                
                domainCheck = new Domain()
                {
                    ID = Lpp.Utilities.DatabaseEx.NewGuid(),
                    DataType = domain.DataType,
                    EnumValue = domain.EnumValue,
                    IsMultiValue = domain.IsMultiValue,
                    Title = domain.Title,
                };
                db.Domains.Add(domainCheck);
            }
            else
            {

                domainCheck.DataType = domain.DataType;
                domainCheck.EnumValue = domain.EnumValue;
                domainCheck.IsMultiValue = domain.IsMultiValue;
                domainCheck.ParentDomainID = domain.ParentDomainID;
                domainCheck.Title = domain.Title;
            }

            if (domain.References != null && domain.References.Count() > 0)
            {

                foreach (var reff in domain.References)
                {
                    var refCheck = domainCheck.DomainReferences.Where(x => x.ID == reff.ID).FirstOrDefault();
                    if (refCheck == null)
                    {
                        refCheck = new DomainReference()
                        {
                            ID = Lpp.Utilities.DatabaseEx.NewGuid(),
                            Title = reff.Title,
                            Description = reff.Description,
                            DomainID = domainCheck.ID,
                            ParentDomainReferenceID = reff.ParentDomainReferenceID
                        };
                        domainCheck.DomainReferences.Add(refCheck);
                    }
                    else
                    {
                        refCheck.Title = reff.Title;
                        refCheck.Description = reff.Description;
                        refCheck.DomainID = domainCheck.ID;
                        refCheck.ParentDomainReferenceID = reff.ParentDomainReferenceID;
                    }
                }
            }

            if (domain.ChildMetadata != null && domain.ChildMetadata.Count() > 0)
            {
                foreach (var child in domain.ChildMetadata)
                {
                    var cc = await RecusiveDomainsAdd(child, db, originalDomains);
                    cc.ParentDomainID = domainCheck.ID;
                    domainCheck.ChildrenDomains.Add(cc);
                }
            }
            
            //await db.SaveChangesAsync();
            return domainCheck;
        }

        static ForRemoval RecursiveGetDomainID(DomainDTO domain)
        {
            var domainIDs = new List<Guid>();
            var domainRefIDs = new List<Guid>();


            domainIDs.Add(domain.ID);
            if (domain.ChildMetadata != null && domain.ChildMetadata.Count() > 0)
            {
                foreach (var child in domain.ChildMetadata)
                {
                    var childDomain = RecursiveGetDomainID(child);
                    if(childDomain.DomainIDs != null && childDomain.DomainIDs.Count() > 0)
                        domainIDs.AddRange(childDomain.DomainIDs);
                    if (childDomain.DomainRefIDs != null && childDomain.DomainRefIDs.Count() > 0)
                        domainRefIDs.AddRange(childDomain.DomainRefIDs);
                }
            }
            if (domain.References != null && domain.References.Count() > 0)
            {
                foreach (var child in domain.References)
                    domainRefIDs.Add(child.ID);
            }

            return new ForRemoval {
                DomainIDs = domainIDs,
                DomainRefIDs = domainRefIDs
            };
        }

    }

    public class ForRemoval
    {
        public IEnumerable<Guid> DomainIDs { get; set; }
        public IEnumerable<Guid> DomainRefIDs { get; set; }
    }
}
