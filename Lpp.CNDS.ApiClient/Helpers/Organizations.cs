using Lpp.CNDS.DTO;
using Lpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient.Helpers
{
    public class Organizations
    {
        public static async Task RegisterCNDSOrg(CNDSClient client, Dns.DTO.OrganizationDTO newOrg, Guid networkID, Guid? cndsParentOrgID)
        {
            var sendOrg = new CNDS.DTO.OrganizationTransferDTO()
            {
                ID = newOrg.ID.Value,
                Name = newOrg.Name,
                Acronym = newOrg.Acronym,
                ParentOrganizationID = cndsParentOrgID.HasValue ? cndsParentOrgID : null,
                NetworkID = networkID,
                ContactEmail = newOrg.ContactEmail,
                ContactFirstName = newOrg.ContactFirstName,
                ContactLastName = newOrg.ContactLastName,
                ContactPhone = newOrg.ContactPhone
            };
            sendOrg.Metadata = DomainData.GetDomainData(newOrg.Metadata, new List<DomainDataDTO>());


            await client.Organizations.Register(sendOrg);

        }

        public static async Task UpdateCNDSOrg(CNDSClient client, Dns.DTO.OrganizationDTO editOrgs, Guid networkID, Guid cndsOrganizationID, Guid? cndsParentOrgID)
        {
            var sendOrg = new CNDS.DTO.OrganizationTransferDTO()
            {
                ID = cndsOrganizationID,
                Name = editOrgs.Name,
                Acronym = editOrgs.Acronym,
                ParentOrganizationID = cndsParentOrgID.HasValue ? cndsParentOrgID : null,
                NetworkID = networkID,
                ContactEmail = editOrgs.ContactEmail,
                ContactFirstName = editOrgs.ContactFirstName,
                ContactLastName = editOrgs.ContactLastName,
                ContactPhone = editOrgs.ContactPhone
            };

            var currentMetaData = await client.Organizations.ListOrganizationDomains(cndsOrganizationID);

            sendOrg.Metadata = DomainData.GetDomainData(editOrgs.Metadata, currentMetaData);

            await client.Organizations.Update(sendOrg);
        }
    }
}
