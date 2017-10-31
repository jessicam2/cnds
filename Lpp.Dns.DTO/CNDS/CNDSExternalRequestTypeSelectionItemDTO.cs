using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CNDSExternalRequestTypeSelectionItemDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public CNDSExternalRequestTypeSelectionItemDTO()
        {
            MappingDefinitions = new HashSet<CNDSNetworkProjectRequestTypeDataMartDTO>();
        }
        /// <summary>
        /// Gets or sets the current Network ID.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the Project ID.
        /// </summary>
        [DataMember]
        public Guid ProjectID { get; set; }
        /// <summary>
        /// Gets or sets the Project name.
        /// </summary>
        [DataMember]
        public string Project { get; set; }
        /// <summary>
        /// Gets or sets the RequestType ID.
        /// </summary>
        [DataMember]
        public Guid RequestTypeID { get; set; }
        /// <summary>
        /// Gets or sets the RequestType name.
        /// </summary>
        [DataMember]
        public string RequestType { get; set; }
        /// <summary>
        /// Gets or sets the CNDS mapping definitions.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSNetworkProjectRequestTypeDataMartDTO> MappingDefinitions { get; set; }
    }
}
