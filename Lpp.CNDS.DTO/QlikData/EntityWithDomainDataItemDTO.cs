using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.QlikData
{
    /// <summary>
    /// Base clas for DomainData export DTO's.
    /// </summary>
    [DataContract]
    public abstract class EntityWithDomainDataItemDTO
    {
        /// <summary>
        /// Gets or sets the Network ID.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the name of the Network.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// Gets or sets the the url of the Network.
        /// </summary>
        [DataMember]
        public string NetworkUrl { get; set; }
        /// <summary>
        /// Gets or sets the the Organization ID.
        /// </summary>
        [DataMember]
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// Gets or sets the name of the Organization.
        /// </summary>
        [DataMember]
        public string Organization { get; set; }
        /// <summary>
        /// Gets or sets the acronym of the Organization.
        /// </summary>
        [DataMember]
        public string OrganizationAcronym { get; set; }
        /// <summary>
        /// Gets or sets the ID of the Organizations parent organization.
        /// </summary>
        [DataMember]
        public Guid? ParentOrganizationID { get; set; }
        /// <summary>
        /// Gets or sets the the DomainUse ID.
        /// </summary>
        [DataMember]
        public Guid DomainUseID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the Domain.
        /// </summary>
        [DataMember]
        public Guid DomainID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the Domain's parent domain.
        /// </summary>
        [DataMember]
        public Guid? ParentDomainID { get; set; }
        /// <summary>
        /// Gets or sets the title of the Domain.
        /// </summary>
        [DataMember]
        public string DomainTitle { get; set; }
        /// <summary>
        /// Gets or sets if the Domain supports the selection of multiple values..
        /// </summary>
        [DataMember]
        public bool DomainIsMultiValueSelect { get; set; }
        /// <summary>
        /// Gets or sets the datatype of the Domain.
        /// </summary>
        [DataMember]
        public string DomainDataType { get; set; }
        /// <summary>
        /// Gets or sets the ID of the DomainReference item.
        /// </summary>
        [DataMember]
        public Guid? DomainReferenceID { get; set; }
        /// <summary>
        /// Gets or sets the title of the DomainReference item.
        /// </summary>
        [DataMember]
        public string DomainReferenceTitle { get; set; }
        /// <summary>
        /// Gets or sets the description of the DomainReference item.
        /// </summary>
        [DataMember]
        public string DomainReferenceDescription { get; set; }
        /// <summary>
        /// Gets or sets the value of the DomainReference item.
        /// </summary>
        [DataMember]
        public string DomainReferenceValue { get; set; }
        /// <summary>
        /// Gets or sets the value of the DomainData (implicitly specified value - first name, last name, "Other" value, etc..).
        /// </summary>
        [DataMember]
        public string DomainDataValue { get; set; }
        /// <summary>
        /// Gets or sets the DomainReferenceID of the DomainData (the ID of the selected reference item for the Domain).
        /// </summary>
        [DataMember]
        public Guid? DomainDataDomainReferenceID { get; set; }
        /// <summary>
        /// Gets or sets the integer value of the AccessType set for the Domain for the DataSource.
        /// </summary>
        [DataMember]
        public int DomainAccessValue { get; set; }
    }
}
