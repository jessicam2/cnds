using Lpp.CNDS.DTO.Enums;
using System;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class DomainDataDTO
    {
        /// <summary>
        /// The Identiifier of the Domain Data
        /// </summary>
        [DataMember]
        public Guid? ID { get; set; }
        /// <summary>
        /// The Identiifier of the Associated Entity
        /// </summary>
        [DataMember]
        public Guid? EntityID { get; set; }
        /// <summary>
        /// The Identifier of the associated Domain Use
        /// </summary>
        [DataMember]
        public Guid DomainUseID { get; set; }
        /// <summary>
        /// The Value of the Domain Data
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// The Identifier of the associated Domain Reference
        /// </summary>
        [DataMember]
        public Guid? DomainReferenceID { get; set; }
        /// <summary>
        /// The Placement of where this Domain Item should be in the List
        /// </summary>
        [DataMember]
        public int SequenceNumber { get; set; }
        /// <summary>
        /// The Visibility Set for the Domain
        /// </summary>
        [DataMember]
        public AccessType Visibility { get; set; }
    }
}
