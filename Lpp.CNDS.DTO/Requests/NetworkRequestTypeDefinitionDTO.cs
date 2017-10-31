using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class NetworkRequestTypeDefinitionDTO : EntityDtoWithID
    {

        [DataMember]
        public Guid NetworkID { get; set; }

        [DataMember]
        public string Network { get; set; }

        [DataMember]
        public Guid ProjectID { get; set; }

        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public Guid RequestTypeID { get; set; }

        [DataMember]
        public string RequestType { get; set; }

        [DataMember]
        public Guid DataSourceID { get; set; }

        [DataMember]
        public string DataSource { get; set; }
    }
}
