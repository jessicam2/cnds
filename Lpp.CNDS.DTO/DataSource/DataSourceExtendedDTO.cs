using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class DataSourceExtendedDTO : DataSourceDTO
    {
        /// <summary>
        /// Gets or sets the name of the Organization.
        /// </summary>
        [DataMember]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Network.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }

        /// <summary>
        /// Gets or sets the name of the Network.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
    }
}
