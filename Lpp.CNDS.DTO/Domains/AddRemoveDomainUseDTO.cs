using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// Used for Adding and Removing Domain Uses
    /// </summary>
    [DataContract]
    public class AddRemoveDomainUseDTO
    {
        /// <summary>
        /// A Collection of Domain Uses to Add based on their EntityType. The key will be the Domain ID.
        /// </summary>
        [DataMember]
        public IEnumerable<KeyValuePair<Guid, DTO.Enums.EntityType>> AddDomainUse { get; set; }
        /// <summary>
        /// A Collection of Domain Uses to Remove based on their Entity Type. The key will be the DomainUse ID.
        /// </summary>
        [DataMember]
        public IEnumerable<KeyValuePair<Guid, DTO.Enums.EntityType>> RemoveDomainUse { get; set; }
    }
}
