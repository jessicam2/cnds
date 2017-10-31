using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO.CNDS
{
    /// <summary>
    /// DTO For Updating The DataMarts in a Request
    /// </summary>
    [DataContract]
    public class CNDSUpdateDataMartPriorityAndDueDateDTO
    {
        /// <summary>
        /// Gets or Sets the ID from the RequestDataMart. Used if coming from Source
        /// </summary>
        [DataMember]
        public Guid? RequestDataMartID { get; set; }
        /// <summary>
        /// Gets or Sets the Priority of the DataMart
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// Gets or Sets the DueDate of the DataMart
        /// </summary>
        [DataMember]
        public DateTime? DueDate { get; set; }
    }
}
