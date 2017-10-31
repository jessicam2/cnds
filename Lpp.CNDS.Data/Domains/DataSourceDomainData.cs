using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lpp.CNDS.Data
{
    [Table("DataSourceDomainData")]
    public class DataSourceDomainData : DomainData
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
