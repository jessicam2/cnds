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
    /// A document associated to the a network response.
    /// </summary>
    /// <remarks>
    /// The ID of the NetworkRequestDocument will be set to the ID of the document in the destination network.
    /// </remarks>
    [Table("NetworkRequestDocuments")]
    public class NetworkRequestDocument : EntityWithID
    {
        /// <summary>
        /// Gets or set the ID of the response associated to the document.
        /// </summary>
        public Guid ResponseID { get; set; }
        /// <summary>
        /// Gets or set the response associated to the document.
        /// </summary>
        public virtual NetworkRequestResponse Response { get; set; }
        /// <summary>
        /// Gets or sets the ID of the source document.
        /// </summary>
        public Guid SourceDocumentID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the document revision set in the destination network.
        /// </summary>
        public Guid DestinationRevisionSetID { get; set; }

    }
}
