using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Lpp.Utilities.Objects;
using Lpp.CNDS.DTO.Enums;

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

}
