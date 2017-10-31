using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.CNDSMetadata
{
    /// <summary>
    /// Returns a Domain use to Send to CNDS
    /// </summary>
    [DataContract]
    public class DomainUseReturnDTO
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid? DomainUseID { get; set; }
        [DataMember]
        public bool Checked { get; set; }
        [DataMember]
        public Lpp.CNDS.DTO.Enums.EntityType EntityType { get; set; }
    }
}
