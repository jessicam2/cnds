using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class OrganizationTransferDTO
    {
        /// <summary>
        /// The ID of the Organization
        /// </summary>
        [DataMember, Required]
        public Guid ID { get; set; }
        /// <summary>
        /// The Name of the Organization
        /// </summary>
        [DataMember, Required, MaxLength(255)]
        public string Name { get; set; }
        /// <summary>
        /// The Acronym of the Organization
        /// </summary>
        [DataMember, Required, MaxLength(450)]
        public string Acronym { get; set; }
        /// <summary>
        /// The ID of the Parent Organization
        /// </summary>
        [DataMember]
        public Guid? ParentOrganizationID { get; set; }
        /// <summary>
        /// The NetworkID of the Organization
        /// </summary>
        [DataMember]
        public Guid? NetworkID { get; set; }
        /// <summary>
        /// A List of Metadata for the Organization
        /// </summary>
        [DataMember]
        public IEnumerable<DomainDataDTO> Metadata { get; set; }
        /// <summary>
        /// Contact Email
        /// </summary>
        [DataMember]
        public string ContactEmail { get; set; }
        /// <summary>
        /// Contact First Name
        /// </summary>
        [DataMember]
        public string ContactFirstName { get; set; }
        /// <summary>
        /// Contact Last Name
        /// </summary>
        [DataMember]
        public string ContactLastName { get; set; }
        /// <summary>
        /// Contact Phone
        /// </summary>
        [DataMember]
        public string ContactPhone { get; set; }
    }
}
