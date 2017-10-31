using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DTO For if Updating Permissions
    /// </summary>
    [DataContract]
    public class CNDSUpdateAssignedPermissionDTO : CNDSAssignedPermissionDTO
    {
        /// <summary>
        /// Flag for if the Permission Should be Deleted from the CNDS System
        /// </summary>
        [DataMember]
        public bool Delete { get; set; }
    }
}
