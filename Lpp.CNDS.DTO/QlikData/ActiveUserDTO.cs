using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.QlikData
{
    /// <summary>
    /// Details about a specific user.
    /// </summary>
    [DataContract]
    public class ActiveUserDTO
    {
        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the user in the PMN instance they belong to.
        /// </summary>
        [DataMember]
        public Guid PmnID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the organization the user belongs to.
        /// </summary>
        [DataMember]
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the organization in the PMN instance it belongs to.
        /// </summary>
        [DataMember]
        public Guid PmnOrganizationID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the network the user belongs to.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the name of the network the user belongs to.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// Gets or sets the name of the organization the user belongs to.
        /// </summary>
        [DataMember]
        public string Organization { get; set; }
    }
}
