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
    /// A response in the participating network the request has beeen submitted to.
    /// </summary>
    /// <remarks>
    /// The ID of the NetworkRequestResponse will be set to the ID of the response created in the participating PMN network.
    /// </remarks>
    [Table("NetworkRequestResponses")]
    public class NetworkRequestResponse : EntityWithID
    {

        public NetworkRequestResponse()
        {
            Documents = new HashSet<NetworkRequestDocument>();
        }
        /// <summary>
        /// Gets or sets the ID of the response in the source request network.
        /// </summary>
        public Guid SourceResponseID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the route in the partcipating network.
        /// </summary>
        public Guid NetworkRequestRouteID { get; set; }
        /// <summary>
        /// Gets or sets the route in the participating network the response is associated to.
        /// </summary>
        public virtual NetworkRequestRoute NetworkRequestRoute { get; set; }
        /// <summary>
        /// Gets the value of the iteration the response was submitted on.
        /// </summary>
        public int IterationIndex { get; set; }
        /// <summary>
        /// Gets or sets the documents associated to the response.
        /// </summary>
        public virtual ICollection<NetworkRequestDocument> Documents { get; set; }
    }
}
