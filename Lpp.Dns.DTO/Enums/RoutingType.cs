using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.Enums
{
    /// <summary>
    /// Types of routings
    /// </summary>
    [DataContract]
    public enum RoutingType
    {
        /// <summary>
        /// Analysis Center routing
        /// </summary>
        [EnumMember]
        AnalysisCenter = 0,

        /// <summary>
        /// Data Partner routing
        /// </summary>
        [EnumMember]
        DataPartner = 1,

        /// <summary>
        /// Source CNDS Routing
        /// </summary>
        [EnumMember]
        SourceCNDS = 2,

        /// <summary>
        /// External CNDS Routing
        /// </summary>
        [EnumMember]
        ExternalCNDS = 3
    }
}