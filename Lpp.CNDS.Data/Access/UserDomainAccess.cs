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
    public class UserDomainAccess: DomainAccess
    {
        /// <summary>
        /// The Identifier of the associated User
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// The Navigational Property to the User
        /// </summary>
        public virtual User User { get; set; }
    }
}
