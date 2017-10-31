using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// DTO For Security Groups 
    /// </summary>
    [DataContract]
    public class CNDSSecurityGroupDTO
    {
        /// <summary>
        /// The Identifier of the Security Group
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// The Name of the Security Group
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
