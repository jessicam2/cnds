using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO For if Updating Permissions
    /// </summary>
    [DataContract]
    public class UpdateAssignedPermissionDTO : AssignedPermissionDTO
    {
        /// <summary>
        /// Flag for if the Permission Should be Deleted from the CNDS System
        /// </summary>
        [DataMember]
        public bool Delete { get; set; }
    }
}
