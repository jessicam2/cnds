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
    public class DataSourceDomainAccess : DomainAccess
    {
        /// <summary>
        /// The Identifier of the associated DataSource
        /// </summary>
        public Guid DataSourceID { get; set; }
        /// <summary>
        /// The Navigational Property to the DataSource
        /// </summary>
        public virtual DataSource DataSource { get; set; }
    }
}
