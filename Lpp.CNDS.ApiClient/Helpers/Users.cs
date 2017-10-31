using Lpp.CNDS.DTO;
using Lpp.Dns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient.Helpers
{
    public class Users
    {
        public static async Task NewCNDSUser(CNDSClient client, Dns.DTO.UserDTO newUser, Guid networkID, Guid cndsOrganizationID)
        {
            var sendUser = new CNDS.DTO.UserTransferDTO()
            {
                ID = newUser.ID.Value,
                UserName = newUser.UserName,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                EmailAddress = newUser.Email,
                FaxNumber = newUser.Fax,
                OrganizationID = cndsOrganizationID,
                MiddleName = newUser.MiddleName,
                PhoneNumber = newUser.Phone,
                NetworkID = networkID
            };

            sendUser.Metadata = DomainData.GetDomainData(newUser.Metadata, new List<DomainDataDTO>());

            await client.Users.Register(sendUser);
        }
        public static async Task EditCNDSUser(CNDSClient client, Dns.DTO.UserDTO editUser, Guid networkID, Guid cndsUserID, Guid cndsOrganizationID)
        {
            var sendUser = new CNDS.DTO.UserTransferDTO()
            {
                ID = cndsUserID,
                UserName = editUser.UserName,
                FirstName = editUser.FirstName,
                LastName = editUser.LastName,
                EmailAddress = editUser.Email,
                FaxNumber = editUser.Fax,
                OrganizationID = cndsOrganizationID,
                MiddleName = editUser.MiddleName,
                PhoneNumber = editUser.Phone,
                NetworkID = networkID,
                Active = editUser.Active
            };

            var currentMetaData = await client.Users.ListUserDomains(cndsUserID);

            sendUser.Metadata = DomainData.GetDomainData(editUser.Metadata, currentMetaData);

            await client.Users.Update(sendUser);
        }
    }
}
