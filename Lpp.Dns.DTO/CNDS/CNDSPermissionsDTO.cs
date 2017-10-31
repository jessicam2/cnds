using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DTO For Permissions
    /// </summary>
    [DataContract]
    public class CNDSPermissionsDTO
    {
        /// <summary>
        /// The Identifier of the Permission
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// The Name of the Permission
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// The Description of the Permission
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
