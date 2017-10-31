using Lpp.CNDS.DTO.Enums;
using Lpp.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Lpp.CNDS.Data
{
    [Table("DomainUse")]
    public class DomainUse : EntityWithID
    {
        public DomainUse()
        {
            DomainDatas = new HashSet<DomainData>();
            AccessVisibility = new HashSet<DomainAccess>();
        }
        public Guid DomainID { get; set; }
        public Domain Domain { get; set; }
        public EntityType EntityType { get; set; }
        public bool Deleted { get; set; }
        public ICollection<DomainData> DomainDatas { get; set; }
        public ICollection<DomainAccess> AccessVisibility { get; set; }

    }
    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class DomainUseConfiguration : EntityTypeConfiguration<DomainUse>
    {
        public DomainUseConfiguration()
        {
            //Mapping Domain Use to its Domain
            HasRequired(dr => dr.Domain)
                .WithMany(dr => dr.DomainUses)
                .HasForeignKey(dr => dr.DomainID)
                .WillCascadeOnDelete(true);
        }
    }
}
