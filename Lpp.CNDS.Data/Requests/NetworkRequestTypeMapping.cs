using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data
{
    [Table("NetworkRequestTypeMapping")]
    public class NetworkRequestTypeMapping : EntityWithID
    {
        public NetworkRequestTypeMapping()
        {
            NetworkRoutes = new HashSet<NetworkRequestTypeMappingRoutes>();
        }

        /// <summary>
        /// Gets or sets the ID of the source network.
        /// </summary>
        [Index("IX_UniqueSourceRequestType", IsUnique = true, Order = 1)]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// Gets or sets the source network.
        /// </summary>        
        public virtual Network Network { get; set; }
        /// <summary>
        /// Gets or sets the source Project ID.
        /// </summary>
        [Index("IX_UniqueSourceRequestType", IsUnique = true, Order = 2)]
        public Guid ProjectID { get; set; }
        /// <summary>
        /// Gets or sets the source RequestType ID.
        /// </summary>
        [Index("IX_UniqueSourceRequestType", IsUnique = true, Order = 3)]
        public Guid RequestTypeID { get; set; }
        /// <summary>
        /// Gets or sets the collection of routes available to the source requesttype mapping.
        /// </summary>
        public virtual ICollection<NetworkRequestTypeMappingRoutes> NetworkRoutes { get; set; }

    }

    [Table("NetworkRequestTypeMappingRoutes")]
    public class NetworkRequestTypeMappingRoutes {
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RequestTypeMappingID { get; set; }

        public virtual NetworkRequestTypeMapping RequestTypeMapping { get; set; }

        [Key, Column(Order = 2), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RequestTypeDefinitionID { get; set; }

        public virtual NetworkRequestTypeDefinition RequestTypeDefinition { get; set; }
    }
}
