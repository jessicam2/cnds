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
    /// A request submitted for distribution through CNDS.
    /// </summary>
    /// <remarks>
    /// The ID of the NetworkRequest will be set to the ID of the source PMN Request.
    /// </remarks>
    [Table("NetworkRequests")]
    public class NetworkRequest : EntityWithID
    {
        public NetworkRequest()
        {
            Participants = new HashSet<NetworkRequestParticipant>();
        }

        /// <summary>
        /// Gets or sets the ID of the source requests network.
        /// </summary>
        public Guid NetworkID { get; set; }

        /// <summary>
        /// Gets or sets the network of the source request.
        /// </summary>
        public virtual Network Network { get; set; }

        /// <summary>
        /// Gets or sets the collection of network participants (Network/Project/RequestType combinations).
        /// </summary>
        public virtual ICollection<NetworkRequestParticipant> Participants { get; set; }
    }
}
