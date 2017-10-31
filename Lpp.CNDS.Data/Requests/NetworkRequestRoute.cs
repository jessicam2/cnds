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
    /// A datasource in the participating network the request has been submitted to.
    /// </summary>
    /// <remarks>
    /// The ID of the NetworkRequestRoute will be set to the ID of the RequestDataMart in the participating network.
    /// </remarks>
    [Table("NetworkRequestRoutes")]
    public class NetworkRequestRoute : EntityWithID
    {

        public NetworkRequestRoute()
        {
            Responses = new HashSet<NetworkRequestResponse>();
        }
        /// <summary>
        /// Gets or sets the ID of the participant the route belongs to.
        /// </summary>
        public Guid ParticipantID { get; set; }
        /// <summary>
        /// Gets or sets th participant the route belongs to.
        /// </summary>
        public virtual NetworkRequestParticipant Participant { get; set; }
        /// <summary>
        /// Gets or sets the CNDS ID of the participating DataSource.
        /// </summary>
        public Guid DataSourceID { get; set; }
        /// <summary>
        /// Gets or sets the CNDS DataSouce.
        /// </summary>
        public virtual DataSource DataSource { get; set; }
        /// <summary>
        /// Gets or sets the ID of the RequestDataMart in the source PMN request.
        /// </summary>
        public Guid SourceRequestDataMartID { get; set; }
        /// <summary>
        /// Gets or sets the collection of responses associated to the route.
        /// </summary>
        public virtual ICollection<NetworkRequestResponse> Responses { get; set; }

    }
}
