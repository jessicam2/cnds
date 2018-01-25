using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Lpp.Utilities.Objects;
using Lpp.CNDS.DTO;
using Lpp.CNDS.DTO.Enums;
using Lpp.Utilities;

namespace Lpp.CNDS.Data
{ 
    [Table("NetworkEntities")]
    public class NetworkEntity : EntityWithID
    {
        [Required]
        public Guid NetworkID { get; set; }

        public virtual Network Network { get; set; }
        public EntityType EntityType { get; set; }

        public Guid NetworkEntityID { get; set; }
    }

    internal class NetworkEntityConfiguration : EntityTypeConfiguration<NetworkEntity>
    {
        public NetworkEntityConfiguration()
        {
            HasRequired(ne => ne.Network)
                .WithMany(n => n.NetworkEntities)
                .HasForeignKey(ne => ne.NetworkID)
                .WillCascadeOnDelete(true);
           
        }
            
    }

    internal class NetworkEntityDtoMappingConfiguration : EntityMappingConfiguration<NetworkEntity, NetworkEntityDTO>
    {
        public override System.Linq.Expressions.Expression<Func<NetworkEntity, NetworkEntityDTO>> MapExpression
        {
            get
            {
                return (o) => new NetworkEntityDTO
                {
                    ID = o.ID,
                    EntityType = o.EntityType,
                    Network = o.Network.Name,
                    NetworkEntityID = o.NetworkEntityID,
                    NetworkID = o.NetworkID,
                    Timestamp = o.Timestamp
                };
            }
        }
    }

}
