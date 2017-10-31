using Lpp.Utilities.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Lpp.CNDS.Data
{
    [Table("DomainReference")]
    public class DomainReference : EntityWithID
    {
        /// <summary>
        /// The Identifier of the Domain
        /// </summary>
        public Guid DomainID { get; set; }
        /// <summary>
        /// The Navigational Property to the Domain
        /// </summary>
        public Domain Domain { get; set; }
        /// <summary>
        /// The Identifier of the Parent DomainReference
        /// </summary>
        public Guid? ParentDomainReferenceID { get; set; }
        /// <summary>
        /// The Navigational Property to the Parent DomainReference
        /// </summary>
        public DomainReference ParentDomainReference { get; set; }
        /// <summary>
        /// The Title of the Domain Reference
        /// </summary>
        [Required, MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// The Description of the Domain Reference
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The Value of the Domain Reference
        /// </summary>
        public string Value { get; set; }
        public bool Deleted { get; set; }
    }
    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class DomainReferenceConfiguration : EntityTypeConfiguration<DomainReference>
    {
        public DomainReferenceConfiguration()
        {

            // Mapping the Child DomainReference to its Parent
            HasOptional(dr => dr.ParentDomainReference)
                .WithMany()
                .HasForeignKey(dr => dr.ParentDomainReferenceID)
                .WillCascadeOnDelete(true);
            //Mapping Domain Reference to its Domain
            HasRequired(dr => dr.Domain)
                .WithMany(dr => dr.DomainReferences)
                .HasForeignKey(dr => dr.DomainID)
                .WillCascadeOnDelete(true);
        }
    }
}
