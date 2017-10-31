using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lpp.CNDS.Api.Security
{
    /// <summary>
    /// Controller for Controlling Secuirty Group and User Association
    /// </summary>
    [AllowAnonymous]
    public class SecurityGroupUsersController : LppApiController<DataContext>
    {
        /// <summary>
        /// Endpoint for Listing SecurityGroupUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<SecurityGroupUserDTO> List()
        {
            return (from sg in DataContext.SecurityGroupUsers.AsNoTracking()
                    select new SecurityGroupUserDTO {
                        UserID = sg.UserID,
                        SecurityGroups = new List<SecurityGroupDTO> { new SecurityGroupDTO { ID = sg.SecurityGroupID, Name = sg.SecurityGroup.Name } }
                    });
        }
        /// <summary>
        /// Endpoint for Listing SecurityGroupUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SecurityGroupUserDTO Get(Guid userID)
        {
            var sg = new SecurityGroupUserDTO();

            sg.UserID = userID;

            sg.SecurityGroups = (from sgs in DataContext.SecurityGroupUsers.AsNoTracking()
                                 where sgs.UserID == userID
                                 select new SecurityGroupDTO
                                 {
                                    ID = sgs.SecurityGroupID, Name = sgs.SecurityGroup.Name
                                 });

            return sg;
        }
        /// <summary>
        /// Endpont for Creating a Security Group User Association
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Update(SecurityGroupUserDTO dto)
        {
            var oldSecurityGroups = DataContext.SecurityGroupUsers.Where(x => x.UserID == dto.UserID);


            var newSecurityGroupUsers = dto.SecurityGroups.Where(d => !oldSecurityGroups.Select(x => x.SecurityGroupID).Contains(d.ID)).Select(x => new SecurityGroupUser
            {
                UserID = dto.UserID,
                SecurityGroupID = x.ID
            });

            var dtoSecurityIDs = dto.SecurityGroups.Select(x => x.ID);
            var deleteSecurityGroupUsers = (from d in oldSecurityGroups
                                            let ids = d.SecurityGroupID
                                            where !dtoSecurityIDs.Contains(ids)
                                            select d);

            if (newSecurityGroupUsers.Count() > 0)
                DataContext.SecurityGroupUsers.AddRange(newSecurityGroupUsers);

            if (deleteSecurityGroupUsers.Count() > 0)
                DataContext.SecurityGroupUsers.RemoveRange(deleteSecurityGroupUsers);

            await DataContext.SaveChangesAsync();
        }
    }
}
