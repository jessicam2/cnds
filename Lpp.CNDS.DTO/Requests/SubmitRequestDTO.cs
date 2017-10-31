using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.Requests
{
    /// <summary>
    /// A network request submission dto, contains information required to be able to register a request in CNDS and participating networks.
    /// </summary>
    [DataContract]
    public class SubmitRequestDTO
    {
        /// <summary>
        /// Gets or sets the source network ID.
        /// </summary>
        [DataMember]
        public Guid SourceNetworkID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the source request.
        /// </summary>
        [DataMember]
        public Guid SourceRequestID { get; set; }
        /// <summary>
        /// Gets or sets the serialized source request details.
        /// </summary>
        [DataMember]
        public string SerializedSourceRequest { get; set; }
        /// <summary>
        /// Gets or sets the collection of participating routes for the request.
        /// </summary>
        [DataMember]
        public IEnumerable<SubmitRouteDTO> Routes { get; set; }
        /// <summary>
        /// Gets or sets the collection of document details for all the input documents to all the submitted routes.
        /// </summary>
        [DataMember]
        public IEnumerable<SubmitRequestDocumentDetailsDTO> Documents { get; set; }
    }

    /// <summary>
    /// A participating route details.
    /// </summary>
    [DataContract]
    public class SubmitRouteDTO {
        /// <summary>
        /// Gets or sets the ID of the route definition.
        /// </summary>
        [DataMember]
        public Guid NetworkRouteDefinitionID { get; set; }
        /// <summary>
        /// Gets or sets the due date the specific route.
        /// </summary>
        [DataMember]
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Gets or sets the Priority value for the specific route.
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// Gets or sets the ID of the route in the source network.
        /// </summary>
        [DataMember]
        public Guid SourceRequestDataMartID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the response being submitted in the source network.
        /// </summary>
        [DataMember]
        public Guid SourceResponseID { get; set; }
        /// <summary>
        /// Gets or sets the collection of document IDs representing the input request documents for the response being submited.
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> RequestDocumentIDs { get; set; }

    }

    /// <summary>
    /// Metadata about the request input document being registered in a participating network.
    /// </summary>
    [DataContract]
    public class SubmitRequestDocumentDetailsDTO
    {
        /// <summary>
        /// Gets or Sets the SourceRequestDataSourceID of the document
        /// </summary>
        [DataMember]
        public Guid SourceRequestDataSourceID { get; set; }
        /// <summary>
        /// Gets or sets the RevisionSet ID of the document.
        /// </summary>
        [DataMember]
        public Guid RevisionSetID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the source PMN document.
        /// </summary>
        [DataMember]
        public Guid DocumentID { get; set; }
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets if the document is renderable by PMN.
        /// </summary>
        [DataMember]
        public bool IsViewable { get; set; }
        /// <summary>
        /// Gets or sets the kind of document.
        /// </summary>
        [DataMember]
        public string Kind { get; set; }
        /// <summary>
        /// Gets or sets the MimeType.
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }
        /// <summary>
        /// Gets or sets the filename of the document.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the length of the document in bytes.
        /// </summary>
        [DataMember]
        public long Length { get; set; }
        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        
    }

    /// <summary>
    /// Routing status change details.
    /// </summary>
    [DataContract]
    public class SetRoutingStatusDTO {
        /// <summary>
        /// The ID of the response that changed triggering the status change request - the "local" response.
        /// </summary>
        /// <remarks>
        /// "Local" depends upon the sender: If the participating network is sending the notification the ResponseID is the response in the participating network, if the source network changed the routing status for an external route the ResponseID is for the source request's response.
        /// </remarks>
        [DataMember]
        public Guid ResponseID { get; set; }
        /// <summary>
        /// The status to update the reciprocal route to.
        /// </summary>
        [DataMember]
        public int RoutingStatus { get; set; }
        /// <summary>
        /// A message to set on the reciprocal response detailing the status change.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }

}
