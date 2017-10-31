using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data
{
    [Table("NetworkRequestTypeDefinitions")]
    public class NetworkRequestTypeDefinition : EntityWithID
    {
        public NetworkRequestTypeDefinition() : base()
        {
            NetworkRequestTypeMappingRoutes = new HashSet<NetworkRequestTypeMappingRoutes>();
        }

        [Index("IX_UniqueDefinition", IsUnique = true, Order = 1)]
        public Guid NetworkID { get; set; }
        
        public virtual Network Network { get; set; }

        [Index("IX_UniqueDefinition", IsUnique = true, Order = 2)]
        public Guid ProjectID { get; set; }

        [Index("IX_UniqueDefinition", IsUnique = true, Order = 3)]
        public Guid RequestTypeID { get; set; }

        [Index("IX_UniqueDefinition", IsUnique = true, Order = 4)]
        public Guid DataSourceID { get; set; }

        public virtual DataSource DataSource { get; set; }

        public virtual ICollection<NetworkRequestTypeMappingRoutes> NetworkRequestTypeMappingRoutes { get; set; }
    }
}
