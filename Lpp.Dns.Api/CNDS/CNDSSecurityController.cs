using Lpp.CNDS.ApiClient;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace Lpp.Dns.Api.CNDS
{
    /// <summary>
    /// Controller for handling CNDS Security Within PMN
    /// </summary>
    public class CNDSSecurityController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Endpoint for Listing Security Groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSSecurityGroupDTO>> SecurityGroupList()
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var sgs = await CNDSEntityUpdater.CNDS.SecurityGroups.List();

                    return (from sg in sgs
                            select new CNDSSecurityGroupDTO
                            {
                                ID = sg.ID,
                                Name = sg.Name
                            }).ToArray();
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Endpoint for Getting a Security Group
        /// </summary>
        /// <param name="securityGroupID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CNDSSecurityGroupDTO> SecurityGroupGet(Guid securityGroupID)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var sgs = await CNDSEntityUpdater.CNDS.SecurityGroups.Get(securityGroupID);

                    return new CNDSSecurityGroupDTO
                    {
                        ID = sgs.ID,
                        Name = sgs.Name
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Endpint for Creating Security Groups
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task InsertOrUpdateSecurityGroups(IEnumerable<CNDSSecurityGroupDTO> dtos)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    await CNDSEntityUpdater.CreateSecurityGroups(dtos.Where(x => x.ID == null || x.ID == Guid.Empty));
                    await CNDSEntityUpdater.UpdateSecurityGroups(dtos.Where(x => x.ID != null || x.ID != Guid.Empty));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Endpoint for Deleting SecurityGroups
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task SecurityGroupDelete([FromUri]IEnumerable<Guid> ids)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    foreach(var id in ids)
                        await CNDSEntityUpdater.DeleteSecurityGroup(id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Endpoint for Listing SecurityGroupUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSSecurityGroupUserDTO>> SecurityGroupUsersList()
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var sgus = await CNDSEntityUpdater.CNDS.SecurityGroupUsers.List();

                    return (from sg in sgus
                            select new CNDSSecurityGroupUserDTO
                            {
                                UserID = sg.UserID,
                                SecurityGroups = sg.SecurityGroups.Select(x => new CNDSSecurityGroupDTO { ID = x.ID, Name = x.Name })
                            }).ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CNDSSecurityGroupUserDTO> GetUserSecurityGroups(Guid userID)
        {
            if(await DataContext.Users.AnyAsync(u => u.ID == userID && u.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy))
            {
                return new CNDSSecurityGroupUserDTO();
            }

            var cndsUserID = Guid.Empty;
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { userID });
                    cndsUserID = response.Select(user => user.EntityID).FirstOrDefault();

                    if (cndsUserID == default(Guid))
                    {
                        throw new System.Net.Http.HttpRequestException("User not found in CNDS.");
                    }

                    var sgus = await CNDSEntityUpdater.CNDS.SecurityGroupUsers.Get(cndsUserID);

                    return new CNDSSecurityGroupUserDTO { UserID = sgus.UserID, SecurityGroups = sgus.SecurityGroups.Select(x => new CNDSSecurityGroupDTO { ID = x.ID, Name = x.Name }) };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Endpont for Creating a Security Group User Association
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SecurityGroupUsersUpdate(CNDSSecurityGroupUserDTO dto)
        {
            if (await DataContext.Users.AnyAsync(u => dto.UserID == u.ID && u.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "CNDS Network Proxy Users cannot be assigned to CNDS security groups."));
            }

            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    await CNDSEntityUpdater.UpdateSecurityGroupUsers(dto);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns a List of Permissions in the CNDS System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSPermissionsDTO>> ListPermissions()
        {
            try
            {
                var client = new CNDSClient(System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"]);
                var permissions = await client.Permissions.List();

                var convertedPermissions = (from p in permissions
                                            select new CNDSPermissionsDTO
                                            {
                                                ID = p.ID,
                                                Name = p.Name,
                                                Description = p.Description
                                            }).ToArray();
                return convertedPermissions;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns if User has a Permissions in the CNDS System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> HasPermissions(Guid permissionID, Guid userID)
        {
            if (await DataContext.Users.AnyAsync(u => u.ID == userID && u.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy))
            {
                return false;
            }

            var cndsUserID = Guid.Empty;
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { userID });
                    cndsUserID = response.Select(user => user.EntityID).FirstOrDefault();

                    if (cndsUserID == default(Guid))
                    {
                        throw new System.Net.Http.HttpRequestException("User not found in CNDS.");
                    }

                    return await CNDSEntityUpdater.CNDS.Permissions.HasPermission(permissionID, cndsUserID);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns a List of all the Security Group Permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSAssignedPermissionDTO>> GetSecurityGroupPermissions()
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {

                    var returnPermissions = await CNDSEntityUpdater.CNDS.Permissions.GetPermissions();
                    var convertedPermissions = (from p in returnPermissions
                                                select new CNDSAssignedPermissionDTO
                                                {
                                                    SecurityGroupID = p.SecurityGroupID,
                                                    PermissionID = p.PermissionID,
                                                    Allowed = p.Allowed
                                                }).ToArray();
                    return convertedPermissions;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Returns a List of all the Current User Permissions
        /// </summary>
        /// <param name="userID">The Identifier of the User</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSAssignedPermissionDTO>> GetCurrentUserPermissions()
        {
            if (await DataContext.Users.AnyAsync(u => u.ID == Identity.ID && u.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy))
            {
                return Enumerable.Empty<CNDSAssignedPermissionDTO>();
            }

            var cndsUserID = Guid.Empty;
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { Identity.ID });
                    cndsUserID = response.Select(user => user.EntityID).FirstOrDefault();

                    if (cndsUserID == default(Guid))
                    {
                        throw new System.Net.Http.HttpRequestException("User not found in CNDS.");
                    }

                    var returnPermissions = await CNDSEntityUpdater.CNDS.Permissions.GetUserPermissions(cndsUserID);
                    var convertedPermissions = (from p in returnPermissions
                                                select new CNDSAssignedPermissionDTO
                                                {
                                                    SecurityGroupID = p.SecurityGroupID,
                                                    PermissionID = p.PermissionID,
                                                    Allowed = p.Allowed
                                                }).ToArray();
                    return convertedPermissions;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Returns a List of all the User Permissions
        /// </summary>
        /// <param name="userID">The Identifier of the User</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSAssignedPermissionDTO>> GetUserPermissions(Guid userID)
        {
            if(await DataContext.Users.AnyAsync(u => u.ID == userID && u.UserType == DTO.Enums.UserTypes.CNDSNetworkProxy))
            {
                return Enumerable.Empty<CNDSAssignedPermissionDTO>();
            }

            var cndsUserID = Guid.Empty;
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { userID });
                    cndsUserID = response.Select(user => user.EntityID).FirstOrDefault();

                    if (cndsUserID == default(Guid))
                    {
                        throw new System.Net.Http.HttpRequestException("User not found in CNDS.");
                    }

                    var returnPermissions = await CNDSEntityUpdater.CNDS.Permissions.GetUserPermissions(cndsUserID);
                    var convertedPermissions = (from p in returnPermissions
                            select new CNDSAssignedPermissionDTO {
                                SecurityGroupID = p.SecurityGroupID,
                                PermissionID = p.PermissionID,
                                Allowed = p.Allowed
                            }).ToArray();
                    return convertedPermissions;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// Updates the Permissions set within the System
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task SetPermissions(IEnumerable<CNDSUpdateAssignedPermissionDTO> permissions)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    await CNDSEntityUpdater.SetPermissions(permissions);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
