using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// Details for a routing status change notification.
    /// </summary>
    [DataContract]
    public class CNDSUpdateRoutingStatusDTO
    {
        /// <summary>
        /// Gets or sets the ID of the Network sending the routing status change.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the name of the Network sending the routing status change.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// Gets or sets the ID of the response belonging to the route to update the status.
        /// </summary>
        [DataMember]
        public Guid ResponseID { get; set; }
        /// <summary>
        /// Gets or sets the new routing status.
        /// </summary>
        [DataMember]
        public DTO.Enums.RoutingStatus RoutingStatus { get; set; }
        /// <summary>
        /// Gets or sets the status change message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
