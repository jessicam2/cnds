using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    [DataContract]
    public class CNDSNetworkProjectRequestTypeDataMartDTO : CNDSProjectRequestTypeDataMartDTO
    {
        [DataMember]
        public Guid DefinitionID { get; set; }
        [DataMember]
        public Guid NetworkID { get; set; }
        [DataMember]
        public string Network { get; set; }

    }
}
