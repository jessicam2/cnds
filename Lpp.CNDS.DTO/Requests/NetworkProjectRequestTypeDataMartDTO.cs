using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class NetworkProjectRequestTypeDataMartDTO
    {
        [DataMember]
        public Guid NetworkID { get; set; }
        [DataMember]
        public string Network { get; set; }
        [DataMember]
        public Guid ProjectID { get; set; }
        [DataMember]
        public string Project { get; set; }
        /// <summary>
        /// The CNDS ID of the datamart.
        /// </summary>
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
