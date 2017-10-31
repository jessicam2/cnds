using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.CNDS
{
    /// <summary>
    /// DTO Used for Returning the metadata for searching
    /// </summary>
    [DataContract]
    public class CNDSSearchMetaDataDTO
    {
        /// <summary>
        /// Gets or Sets the Identifier of the Domain
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or Sets the Identifier of the Domain
        /// </summary>
        [DataMember]
        public Guid? ParentDomainID { get; set; }
        /// <summary>
        /// Gets or Sets the Title of the Domain
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// The Child Metadata
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSSearchMetaDataDTO> ChildMetadata { get; set; }
        /// <summary>
        /// The Domain References
        /// </summary>
        [DataMember]
        public IEnumerable<DomainReferenceDTO> References { get; set; }
        /// <summary>
        /// Gets or Sets the Display Name of when the Domain is Selected
        /// </summary>
        [DataMember]
        public string SelectedDisplay { get; set; }
    }
}
