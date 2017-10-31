using System;
using System.Runtime.Serialization;

namespace Lpp.Dns.DTO.CNDS
{
    /// <summary>
    /// DTO for returning Organization for Search
    /// </summary>
    [DataContract]
    public class CNDSOrganizationSearchDTO
    {
        /// <summary>
        /// Gets or Sets the CNDS Organization ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or Sets the Network ID
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or Sets the Network Name
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// Gets or Sets the Organization Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the Contact Information
        /// </summary>
        [DataMember]
        public string ContactInformation { get; set; }
    }
}
