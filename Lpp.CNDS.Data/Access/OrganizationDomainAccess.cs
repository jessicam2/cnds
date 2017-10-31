using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.Data
{
    public class OrganizationDomainAccess : DomainAccess
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
