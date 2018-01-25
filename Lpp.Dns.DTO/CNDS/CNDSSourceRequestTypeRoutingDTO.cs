using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// Information about a routing for a CNDS request.
    /// </summary>
    [DataContract]
    public class CNDSSourceRequestTypeRoutingDTO
    {
        /// <summary>
        /// Gets or sets the MappingDefinition ID, will be null for a local route.
        /// </summary>
        [DataMember]
        public Guid? MappingDefinitionID { get; set; }

        /// <summary>
        /// The ID of the network.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// The name of the network.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// The ID of the project.
        /// </summary>
        [DataMember]
        public Guid ProjectID { get; set; }
        /// <summary>
        /// The name of the project.
        /// </summary>
        [DataMember]
        public string Project { get; set; }
        /// <summary>
        /// The ID of the request type.
        /// </summary>
        [DataMember]
        public Guid RequestTypeID { get; set; }
        /// <summary>
        /// The name of the requesttype.
        /// </summary>
        [DataMember]
        public string RequestType { get; set; }
        /// <summary>
        /// The ID of the datamart, it will be the CNDS datasource ID unless the datamart is local.
        /// </summary>
        [DataMember]
        public Guid DataMartID { get; set; }
        /// <summary>
        /// The name of the datamart.
        /// </summary>
        [DataMember]
        public string DataMart { get; set; }

        /// <summary>
        /// Indicates if the datamart is local to the parent source requesttype.
        /// </summary>
        [DataMember]
        public bool IsLocal { get; set; }
    }
}
