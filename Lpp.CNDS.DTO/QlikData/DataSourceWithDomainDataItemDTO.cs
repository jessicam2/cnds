using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.QlikData
{
    /// <summary>
    /// A DTO for a DataSource DomainData definition.
    /// </summary>
    [DataContract]
    public class DataSourceWithDomainDataItemDTO : EntityWithDomainDataItemDTO
    {
        
        /// <summary>
        /// Gets or sets the ID of the DataSource.
        /// </summary>
        [DataMember]
        public Guid DataSourceID { get; set; }
        /// <summary>
        /// Gets or sets the name of the DataSource.
        /// </summary>
        [DataMember]
        public string DataSource { get; set; }
        /// <summary>
        /// Gets or sets the acronym of the DataSource.
        /// </summary>
        [DataMember]
        public string DataSourceAcronym { get; set; }
        /// <summary>
        /// Gets or sets the supported adapter ID.
        /// </summary>
        [DataMember]
        public Guid? DataSourceAdapterSupportedID { get; set; }
        /// <summary>
        /// Gets or sets the name of the supported adapter.
        /// </summary>
        [DataMember]
        public string DataSourceAdapterSupported { get; set; }
        /// <summary>
        /// Gets or sets if the DataSource supports cross network requests - IE has a adapter specified for QE requests.
        /// </summary>
        [DataMember]
        public bool SupportsCrossNetworkRequests { get; set; }
        

    }
}
