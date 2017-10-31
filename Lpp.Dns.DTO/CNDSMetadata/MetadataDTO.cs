using Lpp.CNDS.DTO.Enums;
using Lpp.Dns.DTO.CNDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class MetadataDTO
    {
        /// <summary>
        /// The Identifier for the Domain
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// The Identifier for the Entity
        /// </summary>
        [DataMember]
        public Guid? EntityID { get; set; }
        /// <summary>
        /// The ID of the Domain Reference
        /// </summary>
        [DataMember]
        public Guid? DomainReferenceID { get; set; }
        /// <summary>
        /// The Title of the Domain
        /// </summary>
        [DataMember, Required, MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// The Description of the Domain 
        /// </summary>
        [DataMember]
        public string Description { get; set; }
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
        /// The Data Type of the Domain
        /// </summary>
        [DataMember, MaxLength(255)]
        public string DataType { get; set; }
        /// <summary>
        /// The Data Type of the Domain
        /// </summary>
        [DataMember]
        public EntityType? EntityType { get; set; }
        /// <summary>
        /// The Domain Use ID of the Metadata Item
        /// </summary>
        [DataMember]
        public Guid? DomainUseID { get; set; }
        /// <summary>
        /// The Parent Domain Reference ID of the MetadataItem
        /// </summary>
        [DataMember]
        public Guid? ParentDomainReferenceID { get; set; }
        /// <summary>
        /// The Value of the Metadata Item
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// The Child Metadata
        /// </summary>
        [DataMember]
        public IEnumerable<MetadataDTO> ChildMetadata { get; set; }
        /// <summary>
        /// The Domain References
        /// </summary>
        [DataMember]
        public IEnumerable<DomainReferenceDTO> References { get; set; }
        /// <summary>
        /// The Visibility of the Metadata Item
        /// </summary>
        [DataMember]
        public AccessType Visibility { get; set; }
    }
}
