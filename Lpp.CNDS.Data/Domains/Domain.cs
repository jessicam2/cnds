using Lpp.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Lpp.CNDS.Data
{
    [Table("Domain")]
    public class Domain : EntityWithID
    {
        public Domain()
        {
            DomainReferences = new HashSet<DomainReference>();
            DomainUses = new HashSet<DomainUse>();
            IsMultiValue = false;
            ChildrenDomains = new HashSet<Domain>();
        }
        /// <summary>
        /// The ID of the Parent Domain
        /// </summary>
        public Guid? ParentDomainID { get; set; }
        /// <summary>
        /// The Navigational Property to the Parent Domain
        /// </summary>
        public Domain ParentDomain { get; set; }
        /// <summary>
        /// The Title of the Domain
        /// </summary>
        [Required, MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// If the Domain Contains Children
        /// </summary>
        [Required]
        public bool IsMultiValue { get; set; }
        /// <summary>
        /// The Enum Value of the Domain
        /// </summary>
        public short? EnumValue { get; set; }
        /// <summary>
        /// The Data Type of the Domain
        /// </summary>
        [MaxLength(255)]
        public string DataType { get; set; }
        public bool Deleted { get; set; }
        public ICollection<Domain> ChildrenDomains { get; set; }
        public ICollection<DomainReference> DomainReferences { get; set; }
        public ICollection<DomainUse> DomainUses { get; set; }
    }
    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class DomainConfiguration : EntityTypeConfiguration<Domain>
    {
        public DomainConfiguration()
        {

            // Mapping the Child Domain to its Parent Domain
            HasOptional(dm => dm.ParentDomain)
                .WithMany(d => d.ChildrenDomains)
                .HasForeignKey(dm => dm.ParentDomainID)
                .WillCascadeOnDelete(true);

        }
    }
}
