using Lpp.Dns.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects.ValidationAttributes;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DataMart List
    /// </summary>
    [DataContract]
    public class DataMartDTO : DataMartListDTO
    {
        /// <summary>
        /// DataMArt DTO
        /// </summary>
        public DataMartDTO() { }
        /// <summary>
        /// Determines the approval for DataMarts list
        /// </summary>
        [DataMember]
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// The ID of DataMart Type
        /// </summary>
        [DataMember]
        public Guid DataMartTypeID { get; set; }
        /// <summary>
        /// DataMart Type
        /// </summary>
        [DataMember]
        public string DataMartType { get; set; }
        /// <summary>
        /// Determines whether it's deleted
        /// </summary>
        [DataMember]
        public bool Deleted { get; set; }
        /// <summary>
        /// Determines whether DataMart is Grouped or not
        /// </summary>
        [DataMember]
        public bool? IsGroupDataMart { get; set; }
        /// <summary>
        /// Unattended Mode
        /// </summary>
        [DataMember]
        public UnattendedModes? UnattendedMode { get; set; }
 
        /// <summary>
        /// Determines whether DataMart is Local or not
        /// </summary>
        [DataMember]
        public bool IsLocal { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        [DataMember, MaxLength(255)]
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the ID of the data adapter this datamart supports.
        /// </summary>
        [DataMember]
        public Guid? AdapterID { get; set; }
        /// <summary>
        /// Gets or sets the name of the data adapter the datamart supports.
        /// </summary>
        [DataMember]
        public string Adapter { get; set; }

        /// <summary>
        /// Gets or sets the adapter ID on the datamart.
        /// </summary>
        [DataMember]
        public Guid? ProcessorID { get; set; }
        /// <summary>
        /// Metadata of the Datamart
        /// </summary>
        [DataMember]
        public IEnumerable<MetadataDTO> Metadata { get; set; }
    }
}
