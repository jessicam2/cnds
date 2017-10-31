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
    /// Controller for Accessing Security Groups
    /// </summary>
    [AllowAnonymous]
    public class SecurityGroupsController :  LppApiController<DataContext>
    {
        /// <summary>
        /// Endpoint for Listing Security Groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<SecurityGroupDTO> List()
        {
            return (from sg in DataContext.SecurityGroups.AsNoTracking()
                    select new SecurityGroupDTO
                    {
                        ID = sg.ID,
                        Name = sg.Name
                    });
        }
        /// <summary>
        /// Endpoint for Getting a Security Group
        /// </summary>
        /// <param name="securityGroupID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<SecurityGroupDTO> Get(Guid securityGroupID)
        {
            var sg = await DataContext.SecurityGroups.FindAsync(securityGroupID);

            if (sg == null)
                throw new Exception("Security Group Not Found");

            return new SecurityGroupDTO {
                ID = sg.ID,
                Name = sg.Name
            };
        }
        /// <summary>
        /// Endpint for Creating Security Groups
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Create(IEnumerable<SecurityGroupDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                if (dto == null || dto.Name == null || dto.Name == "")
                    throw new Exception("The Required Fields are not filled out");

                DataContext.SecurityGroups.Add(new SecurityGroup
                {
                    Name = dto.Name
                });
            }
            await DataContext.SaveChangesAsync();
        }
        /// <summary>
        /// Endpint for Updating Security Groups
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Update(IEnumerable<SecurityGroupDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                if (dto.ID == null || dto.ID == Guid.Empty || dto.Name == null || dto.Name == "")
                    throw new Exception("The Required Fields are not filled out");

                var sg = await DataContext.SecurityGroups.FindAsync(dto.ID);

                sg.Name = dto.Name;
            }
            await DataContext.SaveChangesAsync();
        }
        /// <summary>
        /// Endpoint for Deleting SecurityGroups
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete(Guid id)
        {
            var sg = await DataContext.SecurityGroups.FindAsync(id);

            if (sg == null)
                throw new Exception("Security Group does not Exist");

            DataContext.SecurityGroups.Remove(sg);

            await DataContext.SaveChangesAsync();
        }
    }
}
