using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Lpp.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Lpp.CNDS.Data
{
    /// <summary>
    /// The Organization Entity.  Inherits from EntityWithID which contains the Fields ID and Timestamp
    /// </summary>
    [Table("Organizations")]
    public class Organization : EntityWithID
    {
        public Organization()
        {
            DomainData = new HashSet<OrganizationDomainData>();
            DomainAccess = new HashSet<OrganizationDomainAccess>();
            Users = new HashSet<User>();
            DataSources = new HashSet<DataSource>();
            Deleted = false;
        }
        /// <summary>
        /// The ID of the network the organization belongs to.
        /// </summary>
        [Required]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// The network the organization belongs to.
        /// </summary>
        public virtual Network Network { get; set; }
        /// <summary>
        /// The name of the organization
        /// </summary>
        [MaxLength(255), Required]
        public string Name { get; set; }
        /// <summary>
        /// The acronym of the organization.
        /// </summary>
        [MaxLength(100), Required(AllowEmptyStrings = true)]
        public string Acronym { get; set; }
        [MaxLength(510)]
        public string ContactEmail { get; set; }

        [MaxLength(100)]//to nvarchar
        public string ContactFirstName { get; set; }

        [MaxLength(100)]//to nvarchar
        public string ContactLastName { get; set; }

        [MaxLength(15)]//to nvarchar
        public string ContactPhone { get; set; }
        /// <summary>
        /// The ID of the organization that this organization belongs to.
        /// </summary>
        public Guid? ParentOrganizationID { get; set; }
        /// <summary>
        /// The parent organization this organization belongs to.
        /// </summary>
        public virtual Organization ParentOrganization { get; set; }
        /// <summary>
        /// Determines if the Organization is Deleted or Not
        /// </summary>
        [Required]
        public bool Deleted { get; set; }
        /// <summary>
        /// Determines when the User Was Deleted
        /// </summary>
        public DateTime? DeletedOn { get; set; }
        /// <summary>
        /// The collection of all metadata for the organziation.
        /// </summary>
        public ICollection<OrganizationDomainData> DomainData { get; set; }

        public ICollection<OrganizationDomainAccess> DomainAccess { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<DataSource> DataSources { get; set; }
    }
    /// <summary>
    /// The Fluent API Mapping for the Entity
    /// </summary>
    internal class OrganizationConfiguration : EntityTypeConfiguration<Organization>
    {
        public OrganizationConfiguration()
        {
            // Mapping the ParentOrganization to its Foriegn Key ParentOrganizationID
            HasOptional(t => t.ParentOrganization)
                .WithMany()
                .HasForeignKey(t => t.ParentOrganizationID)
                .WillCascadeOnDelete(true);
            
            // Mapping the Network to its Foriegn Key NetworkID
            HasRequired(t => t.Network)
                .WithMany(t => t.Organizations)
                .HasForeignKey(t => t.NetworkID)
                .WillCascadeOnDelete(true);

            HasMany(o => o.DataSources)
                .WithRequired(o => o.Organization)
                .HasForeignKey(o => o.OrganizationID)
                .WillCascadeOnDelete(true);

        }
    }
    /// <summary>
    /// The Mapping for Organization to its DTO OrganizationDTO
    /// </summary>
    //internal class OrganizationDtoMappingConfiguration : EntityMappingConfiguration<Organization, OrganizationDTO>
    //{
    //    public override System.Linq.Expressions.Expression<Func<Organization, OrganizationDTO>> MapExpression
    //    {
    //        get
    //        {
    //            return (o) => new OrganizationDTO
    //            {
    //                ID = o.ID,
    //                Name = o.Name,
    //                Acronym = o.Acronym,
    //                ContactEmail = o.ContactEmail,
    //                ContactFirstName = o.ContactFirstName,
    //                ContactLastName = o.ContactLastName,
    //                ContactPhone = o.ContactPhone,
    //                NetworkID = o.NetworkID,
    //                CollaborationRequirements = o.CollaborationRequirements,
    //                Description = o.Description,
    //                ResearchCapabilities = o.ResearchCapabilities,
    //                ParentOrganizationID = o.ParentOrganizationID,
    //                Timestamp = o.Timestamp
    //            };
    //        }
    //    }
    //}
}
