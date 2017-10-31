using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO for DataSources.
    /// </summary>
    [DataContract]
    public class DataSourceDTO
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
    }
}
