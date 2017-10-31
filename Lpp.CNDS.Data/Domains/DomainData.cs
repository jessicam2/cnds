using Lpp.Utilities.Objects;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Lpp.CNDS.Data
{
    [Table("DomainData")]
    public abstract class DomainData : EntityWithID
    {
        public DomainData()
        {
            SequenceNumber = 0;
        }
        /// <summary>
        /// The Identifier of the associated Domain Use
        /// </summary>
        public Guid DomainUseID { get; set; }
        /// <summary>
        /// The Navigational Property to the Domain Use
        /// </summary>
        public DomainUse DomainUse { get; set; }
        /// <summary>
        /// The Value of the Domain Data
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The Identifier of the associated Domain Reference
        /// </summary>
        public Guid? DomainReferenceID { get; set; }
        /// <summary>
        /// The Navigational Property to the Domain Reference
        /// </summary>
        public DomainReference DomainReference { get; set; }
        /// <summary>
        /// The Placement of where this Domain Item should be in the List
        /// </summary>
        public int SequenceNumber { get; set; }

    }
    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class DomainDataConfiguration : EntityTypeConfiguration<DomainData>
    {
        public DomainDataConfiguration()
        {
            //Mapping Domain Data to its Domain Use
            HasRequired(dr => dr.DomainUse)
                .WithMany(dr => dr.DomainDatas)
                .HasForeignKey(dr => dr.DomainUseID)
                .WillCascadeOnDelete(true);
            
        }
    }
}
