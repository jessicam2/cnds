using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO for Passing IDs to return Search Results
    /// </summary>
    [DataContract]
    public class SearchDTO
    {
        /// <summary>
        /// Gets or Sets the Identifiers of the Domains
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> DomainIDs { get; set; }
        /// <summary>
        /// Gets or Sets the Identifiers of the Domains References
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> DomainReferencesIDs { get; set; }
        /// <summary>
        /// Gets or Sets the Identifier of the Network
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
    }
}
