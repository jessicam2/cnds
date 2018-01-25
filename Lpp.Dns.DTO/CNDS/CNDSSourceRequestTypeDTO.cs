using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// A CNDS RequestType that can be created by a user in their network.
    /// </summary>
    [DataContract]
    public class CNDSSourceRequestTypeDTO
    {
        [DataMember]
        public Guid ProjectID { get; set; }

        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public Guid RequestTypeID { get; set; }

        [DataMember]
        public string RequestType { get; set; }
        /// <summary>
        /// The datamart that are part of the project the requesttype belongs to. The DataMart ID is local.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSSourceRequestTypeRoutingDTO> LocalRoutes { get; set; }
        /// <summary>
        /// The external data source routing information, the DataMart ID is CNDS.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSSourceRequestTypeRoutingDTO> ExternalRoutes { get; set; }
        /// <summary>
        /// The data source information for invalid selected data sources, the DataMart ID is CNDS.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSSourceRequestTypeRoutingDTO> InvalidRoutes { get; set; }
    }
}
