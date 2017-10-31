using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO for Security Group Users
    /// </summary>
    [DataContract]
    public class SecurityGroupUserDTO
    {
        /// <summary>
        /// The Identifier of the User
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }
        /// <summary>
        /// The Identifier of the Security Group
        /// </summary>
        [DataMember]
        public IEnumerable<SecurityGroupDTO> SecurityGroups { get; set; }
    }
}
