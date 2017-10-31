using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DTO for Permissions Assigned to a SecurityGroup
    /// </summary>
    [DataContract]
    public class CNDSAssignedPermissionDTO
    {
        /// <summary>
        /// The Identifier for the Security Group
        /// </summary>
        [DataMember]
        public Guid SecurityGroupID { get; set; }
        /// <summary>
        /// The Identifier for the Permission
        /// </summary>
        [DataMember]
        public Guid PermissionID { get; set; }
        /// <summary>
        /// Flag for if the Security Group has is allowed to the Permission
        /// </summary>
        [DataMember]
        public bool Allowed { get; set; }
    }
}
