using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data.Requests
{
    /// <summary>
    /// A participant that is not the same Network/Project/RequestType combination as the source request.
    /// </summary>
    /// <remarks>
    /// The ID of the NetworkRequestParticipant will be set to the ID of the request created in the participating network.
    /// </remarks>
    [Table("NetworkRequestParticipants")]
    public class NetworkRequestParticipant : EntityWithID
    {


        public NetworkRequestParticipant()
        {
            Routes = new HashSet<NetworkRequestRoute>();
        }

        /// <summary>
        /// Gets or sets the ID of the source request.
        /// </summary>
        public Guid NetworkRequestID { get; set; }
        /// <summary>
        /// Gets or sets the source request.
        /// </summary>
        public virtual NetworkRequest NetworkRequest { get; set; }
        /// <summary>
        /// Gets or sets the ID of the participating network.
        /// </summary>
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the participating network.
        /// </summary>
        public virtual Network Network { get; set; }
        /// <summary>
        /// Gets or sets the ID of the participating network project.
        /// </summary>
        public Guid ProjectID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the participating network project requesttype.
        /// </summary>
        public Guid RequestTypeID { get; set; }
        /// <summary>
        /// Gets or sets the collection of the routes submitted to the participating network.
        /// </summary>
        public virtual ICollection<NetworkRequestRoute> Routes { get; set; }

    }
}
