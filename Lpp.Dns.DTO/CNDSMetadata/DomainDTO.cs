using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.CNDS
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class DomainDTO
    {
        /// <summary>
        /// The Identifier for the Domain
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// The Domain Use Identifier
        /// </summary>
        [DataMember]
        public Guid DomainUseID { get; set; }
        /// <summary>
        /// The ID of the Parent Domain
        /// </summary>
        [DataMember]
        public Guid? ParentDomainID { get; set; }
        /// <summary>
        /// The Title of the Domain
        /// </summary>
        [DataMember, Required, MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// If the Domain Contains Children
        /// </summary>
        [DataMember, Required]
        public bool IsMultiValue { get; set; }
        /// <summary>
        /// The Enum Value of the Domain
        /// </summary>
        [DataMember]
        public short? EnumValue { get; set; }
        /// <summary>
        /// THe Entity Type of the Domain
        /// </summary>
        [DataMember]
        public Lpp.CNDS.DTO.Enums.EntityType EntityType { get; set; }
        /// <summary>
        /// The Data Type of the Domain
        /// </summary>
        [DataMember, MaxLength(255)]
        public string DataType { get; set; }
        /// <summary>
        /// Gets or sets the collection of child domain definitions.
        /// </summary>
        [DataMember]
        public IEnumerable<DomainDTO> Children { get; set; }
        /// <summary>
        /// Gets or sets the collection of DomainReferences.
        /// </summary>
        [DataMember]
        public IEnumerable<DomainReferenceDTO> DomainReferences { get; set; }
    }
}
