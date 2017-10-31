using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    [DataContract]
    public class CNDSProjectRequestTypeDataMartDTO
    {
        [DataMember]
        public Guid ProjectID { get; set; }
        [DataMember]
        public string Project { get; set; }
        [DataMember]
        public Guid DataMartID { get; set; }
        [DataMember]
        public string DataMart { get; set; }
        [DataMember]
        public Guid RequestTypeID { get; set; }
        [DataMember]
        public string RequestType { get; set; }
    }
}
