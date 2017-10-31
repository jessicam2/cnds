using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class DataSourceTransferDTO
    {
        /// <summary>
        /// The ID of the DataMart
        /// </summary>
        [DataMember, Required]
        public Guid ID { get; set; }
        /// <summary>
        /// The Name of the DataMart
        /// </summary>
        [DataMember, Required]
        public string Name { get; set; }
        /// <summary>
        /// The Acronym of the DataMart
        /// </summary>
        [DataMember,Required]
        public string Acronym { get; set; }
        /// <summary>
        /// Gets or sets the ID of the supported adapter.
        /// </summary>
        [DataMember]
        public Guid? AdapterSupportedID { get; set; }
        /// <summary>
        /// The ID of the network.
        /// </summary>
        [DataMember]
        public Guid? NetworkID { get; set; }
        /// <summary>
        /// The Guid of the Organization the DataMart Belongs to
        /// </summary>
        [DataMember,Required]
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// A List of Metadata for the DataMart
        /// </summary>
        [DataMember]
        public IEnumerable<DomainDataDTO> Metadata { get; set; }
    }
}
