using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lpp.CNDS.Data
{
    [Table("OrganizationDomainData")]
    public class OrganizationDomainData : DomainData
    {
        /// <summary>
        /// The Identifier of the associated Organization
        /// </summary>
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// The Navigational Property to the Organization
        /// </summary>
        public virtual Organization Organization { get; set; }
    }
}
