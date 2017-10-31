using Lpp.CNDS.DTO;
using Lpp.Dns.DTO;
using Lpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient.Helpers
{
    public class DataSources
    {
        public static async Task RegisterCNDSDataSource(CNDS.ApiClient.CNDSClient client, DataMartDTO dm, Guid networkID, Guid organizationID)
        {
            var sendDM = new CNDS.DTO.DataSourceTransferDTO()
            {
                ID = dm.ID.Value,
                Acronym = dm.Acronym,
                Name = dm.Name,
                OrganizationID = organizationID,
                NetworkID = networkID,
                AdapterSupportedID = dm.AdapterID
            };
            sendDM.Metadata = DomainData.GetDomainData(dm.Metadata, new List<DomainDataDTO>());

            await client.DataSources.Register(sendDM);
        }

        public static async Task UpdateCNDSDataSource(CNDS.ApiClient.CNDSClient client, DataMartDTO dm, Guid networkID, Guid cndsDataSourceID, Guid organizationID)
        {
            var sendDM = new CNDS.DTO.DataSourceTransferDTO()
            {
                ID = cndsDataSourceID,
                Acronym = dm.Acronym,
                Name = dm.Name,
                OrganizationID = organizationID,
                NetworkID = networkID,
                AdapterSupportedID = dm.AdapterID
            };
            var currentMetaData = await client.DataSources.ListDataSourceDomains(cndsDataSourceID);
            sendDM.Metadata = DomainData.GetDomainData(dm.Metadata, currentMetaData);
            await client.DataSources.Update(sendDM);
        }
    }
}
