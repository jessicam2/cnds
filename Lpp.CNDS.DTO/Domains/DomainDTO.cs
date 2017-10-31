using Lpp.CNDS.DTO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
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
        public Guid? DomainUseID { get; set; }
        /// <summary>
        /// The ID of the Parent Domain
        /// </summary>
        [DataMember]
        public Guid? ParentDomainID { get; set; }
        /// <summary>
        /// The Title of the Domain
        /// </summary>
        [DataMember,Required, MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// If the Domain Contains Children
        /// </summary>
        [DataMember,Required]
        public bool IsMultiValue { get; set; }
        /// <summary>
        /// The Enum Value of the Domain
        /// </summary>
        [DataMember]
        public short? EnumValue { get; set; }
        /// <summary>
        /// The Data Type of the Domain
        /// </summary>
        [DataMember,MaxLength(255)]
        public string DataType { get; set; }
        // <summary>
        /// The Data Type of the Domain
        /// </summary>
        [DataMember]
        public EntityType? EntityType { get; set; }
        /// <summary>
        /// The Child Metadata
        /// </summary>
        [DataMember]
        public IEnumerable<DomainDTO> ChildMetadata { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IEnumerable<DomainReferenceDTO> References { get; set; }
    }
}
