using System;
using System.Runtime.Serialization;

namespace Lpp.Dns.DTO.CNDS
{
    /// <summary>
    /// DTO for returning DataMarts for Search
    /// </summary>
    [DataContract]
    public class CNDSDataSourceSearchDTO
    {
        /// <summary>
        /// Gets or Sets the CNDS DataSource ID
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
        /// Gets or Sets the OrganiationID
        /// </summary>
        [DataMember]
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// Gets or Sets the Organization Name
        /// </summary>
        [DataMember]
        public string Organization { get; set; }
        /// <summary>
        /// Gets or Sets the Datasource Name
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
