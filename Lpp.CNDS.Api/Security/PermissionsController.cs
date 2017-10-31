using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lpp.CNDS.Api.Security
{
    /// <summary>
    /// The Controller that will handle everything for Permissions
    /// </summary>
    [AllowAnonymous]
    public class PermissionsController : LppApiController<DataContext>
    {
        /// <summary>
        /// Returns a List of Permissions in the CNDS System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<PermissionDTO> List()
        {
            var permissions = (from p in DataContext.Permissions.AsNoTracking()
                               select new PermissionDTO
                               {
                                   ID = p.ID,
                                   Name = p.Name,
                                   Description = p.Description
                               });
            return permissions;
        }

        /// <summary>
        /// Checks if a User has a Permission
        /// </summary>
        /// <param name="permissionID">The Identifier of the Permission</param>
        /// <param name="userID">The Identifier of the User</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> HasPermission(Guid permissionID, Guid userID)
        {
            var per = await (from sgu in DataContext.SecurityGroupUsers.AsNoTracking()
                       join acl in DataContext.GlobalAcls.AsNoTracking() on sgu.SecurityGroupID equals acl.SecurityGroupID
                       where sgu.UserID == userID && acl.PermissionID == permissionID
                       select acl).ToArrayAsync();

            return per.Any() && per.All(a => a.Allowed);
        }

        /// <summary>
        /// Returns a List of all the User Permissions
        /// </summary>
        /// <param name="userID">The Identifier of the User</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<AssignedPermissionDTO> GetUserPermissions(Guid userID)
        {
            var permissions = (from sgu in DataContext.SecurityGroupUsers.AsNoTracking()
                    join acl in DataContext.GlobalAcls.AsNoTracking() on sgu.SecurityGroupID equals acl.SecurityGroupID
                    where sgu.UserID == userID
                    select new AssignedPermissionDTO
                    {
                        PermissionID = acl.PermissionID,
                        SecurityGroupID = acl.SecurityGroupID,
                        Allowed = acl.Allowed
                    });
            return permissions;
        }

        /// <summary>
        /// Returns a List of all the Permissions set to Security Groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<AssignedPermissionDTO> GetPermissions()
        {
            return (from acl in DataContext.GlobalAcls.AsNoTracking()
                    select new AssignedPermissionDTO
                    {
                        PermissionID = acl.PermissionID,
                        SecurityGroupID = acl.SecurityGroupID,
                        Allowed = acl.Allowed
                    });
        }

        /// <summary>
        /// Updates the Permissions set within the System
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task SetPermissions(IEnumerable<UpdateAssignedPermissionDTO> permissions)
        {
            foreach(var permission in permissions)
            {
                if(permission.Delete)
                {
                    await DataContext.Database.ExecuteSqlCommandAsync("Delete from AclGlobal where PermissionID = {0} AND SecurityGroupID = {1}", permission.PermissionID, permission.SecurityGroupID);
                }
                else
                {
                    var previousPermission = DataContext.GlobalAcls.Where(x => x.SecurityGroupID == permission.SecurityGroupID && x.PermissionID == permission.PermissionID).FirstOrDefault();
                    if(previousPermission != null)
                    {
                        previousPermission.Allowed = permission.Allowed;
                    }
                    else
                    {
                        DataContext.GlobalAcls.Add(new AclGlobal { SecurityGroupID = permission.SecurityGroupID, PermissionID = permission.PermissionID, Allowed = permission.Allowed});
                    }
                }
            }
            await DataContext.SaveChangesAsync();
        }

    }
}
