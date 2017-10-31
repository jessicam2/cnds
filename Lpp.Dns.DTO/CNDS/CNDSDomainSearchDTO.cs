using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.CNDS
{
    /// <summary>
    /// DTO For Searching DataSources and Organizations
    /// </summary>
    [DataContract]
    public class CNDSDomainSearchDTO
    {
        /// <summary>
        /// Gets or Sets the Identifiers of Domains
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> DomainIDs { get; set; }
        /// <summary>
        /// Gets or Sets the Identifiers of References
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> DomainReferences { get; set; }
    }
}
