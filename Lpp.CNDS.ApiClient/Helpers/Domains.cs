using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient.Helpers
{
    public class Domains
    {
        public static async Task UpdateCNDSDomains(IEnumerable<Dns.DTO.MetadataDTO> domains, CNDSClient client)
        {
            var newDomainDTO = new List<CNDS.DTO.DomainDTO>();

            foreach(var domain in domains)
            {
                newDomainDTO.Add(PrepDomainDTOStructure(domain, null));
            }

            await client.Domain.InsertOrUpdateDomains(newDomainDTO);

            return;
        }

        static CNDS.DTO.DomainDTO PrepDomainDTOStructure(Dns.DTO.MetadataDTO domain, Guid? parentID)
        {
            var returnDTO = new CNDS.DTO.DomainDTO();

            if(domain.ChildMetadata.Count() > 0)
            {
                var newChild = new List<CNDS.DTO.DomainDTO>();
                foreach (var child in domain.ChildMetadata)
                {
                    newChild.Add(PrepDomainDTOStructure(child, domain.ID));
                }
                returnDTO.ChildMetadata = newChild;
            }
            if (domain.References.Count() > 0)
            {
                var newRef = new List<CNDS.DTO.DomainReferenceDTO>();
                foreach (var child in domain.References)
                {
                    newRef.Add(PrepDomainDTOStructure(child, domain.ID));
                }
                returnDTO.References = newRef;
            }

            returnDTO.ID = domain.ID;
            returnDTO.IsMultiValue = domain.IsMultiValue;
            returnDTO.ParentDomainID = parentID;
            returnDTO.Title = domain.Title;
            returnDTO.DataType = domain.DataType;

            return returnDTO;
        }

        static CNDS.DTO.DomainReferenceDTO PrepDomainDTOStructure(Dns.DTO.CNDS.DomainReferenceDTO domain, Guid domainID)
        {
            var returnReference = new CNDS.DTO.DomainReferenceDTO();

            returnReference.ID = domain.ID;
            returnReference.Title = domain.Title;
            returnReference.DomainID = domain.ID;
            returnReference.Description = domain.Description;
            returnReference.ParentDomainReferenceID = domain.ParentDomainReferenceID;

            return returnReference;
        }
    }
}
