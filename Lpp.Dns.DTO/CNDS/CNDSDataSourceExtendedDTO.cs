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
    /// DataSource DTO with organization and network information included.
    /// </summary>
    [DataContract]
    public class CNDSDataSourceExtendedDTO
    {
        /// <summary>
        /// The Identifier of the DataSource 
        /// </summary>
        [DataMember]
        public Guid? ID { get; set; }
        /// <summary>
        /// The Name of the DataMart
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// The Acronym of the DataMart
        /// </summary>
        [Required, DataMember]
        public string Acronym { get; set; }
        /// <summary>
        /// The Guid of the Organization the DataMart Belongs to
        /// </summary>
        [Required, DataMember]
        public Guid OrganizationID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the supported Adapter.
        /// </summary>
        [DataMember]
        public Guid? AdapterSupportedID { get; set; }
        /// <summary>
        /// Gets the name of the supported adapter if set.
        /// </summary>
        [DataMember]
        public string AdapterSupported { get; set; }

        /// <summary>
        /// Gets or sets the name of the Organization.
        /// </summary>
        [DataMember]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Network.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }

        /// <summary>
        /// Gets or sets the name of the Network.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
    }
    
}
