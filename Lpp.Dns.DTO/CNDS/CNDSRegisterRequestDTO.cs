using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.DTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CNDSRegisterRequestDTO
    {
        /// <summary>
        /// Gets or sets the ID of the source network the request is coming from.
        /// </summary>
        [DataMember]
        public Guid SourceNetworkID { get; set; }
        /// <summary>
        /// Gets or sets the name of the source network the request is coming from.
        /// </summary>
        [DataMember]
        public string SourceNetworkName { get; set; }
        /// <summary>
        /// Gets or sets the CNDS Network Participant ID, which is the equivalent of the destination Request ID.
        /// </summary>
        [DataMember]
        public Guid ParticipantID { get; set; }
        /// <summary>
        /// Gets or sets the Project ID.
        /// </summary>
        [DataMember]
        public Guid ProjectID { get; set; }
        /// <summary>
        /// Gets or sets the requesttype ID.
        /// </summary>
        [DataMember]
        public Guid RequestTypeID { get; set; }
        /// <summary>
        /// Gets or sets the serialized source request details.
        /// </summary>
        [DataMember]
        public string RequestDetails { get; set; }
        /// <summary>
        /// Gets or sets the collection of DataMartIDs to create routes for - this will be the PMN ID of the CNDS DataSource.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSRegisterRouteDTO> Routes { get; set; }
        /// <summary>
        /// Gets or sets the collection of input documents for the request.
        /// </summary>
        [DataMember]
        public IEnumerable<CNDSRegisterDocumentDTO> Documents { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CNDSRegisterRouteDTO
    {
        /// <summary>
        /// Gets or sets the RequestDataMart ID for the new PMN route.
        /// </summary>
        [DataMember]
        public Guid RouteID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the DataMart.
        /// </summary>
        [DataMember]
        public Guid DataMartID { get; set; }
        /// <summary>
        /// Gets or sets the due date for the route.
        /// </summary>
        [DataMember]
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Gets or sets the priority for the route.
        /// </summary>
        [DataMember]
        public DTO.Enums.Priorities Priority { get; set; }
        /// <summary>
        /// Gets or sets the ID of the new PMN response to be created.
        /// </summary>
        [DataMember]
        public Guid ResponseID { get; set; }
        /// <summary>
        /// Gets or sets the collection of Document IDs for the input documents for the route.
        /// </summary>
        [DataMember]
        public IEnumerable<Guid> DocumentIDs { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CNDSRegisterDocumentDTO {
        /// <summary>
        /// Gets or sets the ID of the source document.
        /// </summary>
        [DataMember]
        public Guid SourceDocumentID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the source document revision set.
        /// </summary>
        [DataMember]
        public Guid SourceRevisionSetID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the document.
        /// </summary>
        [DataMember]
        public Guid DocumentID { get; set; }
        /// <summary>
        /// Gets or set the revision ID for the document.
        /// </summary>
        [DataMember]
        public Guid RevisionSetID { get; set; }
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

}
