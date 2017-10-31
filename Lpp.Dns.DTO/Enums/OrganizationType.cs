using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.Enums
{
    /// <summary>
    /// Indicates the type of Organization.
    /// </summary>
    [DataContract]
    public enum OrganizationType
    {
        /// <summary>
        /// Indicates the Organization belongs to the current Network.
        /// </summary>
        [EnumMember]
        Local = 0,
        /// <summary>
        /// Indicates the Organization belongs to a different Network, and has been imported into the current network.
        /// </summary>
        [EnumMember]
        Network = 1
    }
}
