using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lpp.Dns.DataMart.Model
{
    [DataContract, Serializable]
    public class RequestMetadata
    {
        [DataMember]
        public string RequestTypeId { get; set; }
        [DataMember]
        public bool IsMetadataRequest { get; set; }
        [DataMember]
        public string DataMartId { get; set; }
        [DataMember]
        public string DataMartName { get; set; }
        [DataMember]
        public string DataMartOrganizationId { get; set; }
        [DataMember]
        public string DataMartOrganizationName { get; set; }
    }
}
