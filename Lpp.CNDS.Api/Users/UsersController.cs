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

namespace Lpp.CNDS.Api.Users
{
    /// <summary>
    /// WebAPI Controller for all User Actions
    /// </summary>
    [AllowAnonymous]
    public class UsersController : LppApiController<DataContext>
    {
        /// <summary>
        /// An OData Endpoint for Listing all Users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<UserDTO> List()
        {
            return DataContext.Users.Where(u => u.Active && !u.Deleted).AsQueryable().Map<User, UserDTO>();
        }

        /// <summary>
        /// Endpoint for retrieving User Information
        /// </summary>
        /// <param name="id">The GUID ID of the User</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<UserDTO> Get(Guid id)
        {
            return DataContext.Users.Where(u => u.ID == id && !u.Deleted).Map<User, UserDTO>();
        }
        /// <summary>
        /// An OData Enpoint for an Organizations Domain Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DomainDataDTO> ListUserDomains(Guid id)
        {
            return (from od in DataContext.DomainDatas.OfType<UserDomainData>()
                    join u in DataContext.Users on od.UserID equals u.ID
                    where od.UserID == id && !u.Deleted && od.DomainUse.Deleted == false
                    select new DomainDataDTO
                    {
                        ID = od.ID,
                        EntityID = od.UserID,
                        DomainUseID = od.DomainUseID,
                        DomainReferenceID = od.DomainReferenceID,
                        Value = od.Value,
                        SequenceNumber = od.SequenceNumber,
                    });
        }
        /// <summary>
        /// Endpoint for Registering a User
        /// </summary>
        /// <param name="dto">The DTO of the User</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UserDTO> Register(UserTransferDTO dto)
        {
            NetworkEntity userEntity = DataContext.NetworkEntities.Add(new NetworkEntity { NetworkID = dto.NetworkID, NetworkEntityID = dto.ID, EntityType = EntityType.User });

            Data.User user = DataContext.Users.Add(new Data.User() {
                ID = userEntity.ID,
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                PhoneNumber = dto.PhoneNumber,
                FaxNumber = dto.FaxNumber,
                EmailAddress = dto.EmailAddress,
                NetworkID = dto.NetworkID,
                OrganizationID = dto.OrganizationID,
                Salutation = dto.Salutation,
            });

            await DataContext.SaveChangesAsync();

            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                IList<Data.UserDomainData> metadata = new List<Data.UserDomainData>();
                foreach (var meta in dto.Metadata)
                {
                    var orgMeta = new UserDomainData()
                    {
                        UserID = user.ID,
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

            return user.Map<User, UserDTO>();
        }
        /// <summary>
        /// Endpoint for Updating a User
        /// </summary>
        /// <param name="dto">The User Object to be Updated</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UserDTO> Update(UserTransferDTO dto)
        {
            var user = await DataContext.Users.FindAsync(dto.ID);

            if(user.Deleted)
            {
                throw new Exception("The specified User was no found");
            }

            user.UserName = dto.UserName;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.MiddleName = dto.MiddleName;
            user.PhoneNumber = dto.PhoneNumber;
            user.FaxNumber = dto.FaxNumber;
            user.EmailAddress = dto.EmailAddress;
            user.Salutation = dto.Salutation;
            user.Active = dto.Active;

            await DataContext.SaveChangesAsync();

            var userMetadata = await DataContext.DomainDatas.OfType<UserDomainData>().Where(x => x.UserID == dto.ID).ToArrayAsync();
            if (dto.Metadata != null && dto.Metadata.Count() > 0)
            {
                IList<UserDomainData> metaDataToAdd = new List<UserDomainData>();
                foreach (var meta in dto.Metadata.Where(m => !m.ID.HasValue))
                {
                    var userMeta = new UserDomainData()
                    {
                        UserID = dto.ID,
                        DomainUseID = meta.DomainUseID,
                        Value = meta.Value,
                        SequenceNumber = meta.SequenceNumber,
                    };
                    if (meta.DomainReferenceID.HasValue)
                        userMeta.DomainReferenceID = meta.DomainReferenceID.Value;
                    metaDataToAdd.Add(userMeta);
                }
                if (metaDataToAdd.Count > 0)
                    DataContext.DomainDatas.AddRange(metaDataToAdd);

                foreach (var meta in userMetadata.Where(org => dto.Metadata.Any(m => m.ID == org.ID && (org.Value != m.Value || org.SequenceNumber != m.SequenceNumber || org.DomainReferenceID != m.DomainReferenceID))))
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
            var remove = (from d in userMetadata
                                where !metadataIDs.Contains(d.ID) && d.UserID == dto.ID
                                select d.ID).ToArray();


            if (remove.Count() > 0)
            {
                //Have to do this cause of trigger
                var removeIDs = String.Join(",", remove.Select(x => String.Format("'{0}'", x)));
                await DataContext.Database.ExecuteSqlCommandAsync(string.Format("delete from UserDomainData where ID IN ({0})", removeIDs));
            }

            return user.Map<User, UserDTO>();
        }

        /// <summary>
        /// Endpoint for Deleting a User
        /// </summary>
        /// <param name="id">The GUID ID of the User</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var user = DataContext.Users.Find(id);
            if(user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified User was not found.");
            }
            else if(user.Deleted)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified User was not found.");
            }
            else
            {
                user.Deleted = true;
                user.DeletedOn = DateTime.Now;
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
            return (from uda in DataContext.DomainAccess.OfType<UserDomainAccess>()
                    join du in DataContext.DomainUses on uda.DomainUse.ID equals du.ID
                    where uda.UserID == id 
                    select new DomainDataDTO
                    {
                        ID = du.DomainID,
                        EntityID = uda.UserID,
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
                    var previousDomain = DataContext.DomainAccess.OfType<UserDomainAccess>().FirstOrDefault(x => x.DomainUseID == domain.DomainUseID && x.UserID == domain.EntityID);

                    if (previousDomain == null)
                    {
                        DataContext.DomainAccess.Add(new UserDomainAccess
                        {
                            UserID = domain.EntityID.Value,
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
    }
}
