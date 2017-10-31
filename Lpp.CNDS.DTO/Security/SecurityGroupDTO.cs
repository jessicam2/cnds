using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO For Security Groups 
    /// </summary>
    [DataContract]
    public class SecurityGroupDTO
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
