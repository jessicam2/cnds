using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    [DataContract]
    public class CNDSMappingItemDTO
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember, Required]
        public string Name { get; set; }
        [DataMember]
        public IEnumerable<CNDSMappingItemDTO> Children { get; set; }
    }
}
