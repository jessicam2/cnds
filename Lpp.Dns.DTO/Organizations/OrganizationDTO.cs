using Lpp.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects.ValidationAttributes;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DTO for Organization
    /// </summary>
    [DataContract]
    public class OrganizationDTO : EntityDtoWithID
    {
        /// <summary>
        /// Organization Name
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        ///Organization Acronym
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string Acronym { get; set; }
        /// <summary>
        /// Gets or sets the indicator to specify if Organization is Deleted 
        /// </summary>
        [DataMember]
        public bool Deleted { get; set; }
        /// <summary>
        /// Gets or sets the indicator to specify if Primary Organization is selected 
        /// </summary>
        [DataMember]
        public bool Primary { get; set; }
        /// <summary>
        /// ID of Parent Organization
        /// </summary>
        [DataMember]
        public Guid? ParentOrganizationID { get; set; }
        /// <summary>
        /// Parent Organization
        /// </summary>
        [DataMember, ReadOnly(true)]
        public string ParentOrganization { get; set; }
        /// <summary>
        /// The Listing of all the Available Metadata for the Organization
        /// </summary>
        [DataMember]
        public IEnumerable<MetadataDTO> Metadata { get; set; }
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
