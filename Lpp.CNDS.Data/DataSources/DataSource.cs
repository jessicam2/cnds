using Lpp.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using Lpp.Utilities;
using Lpp.CNDS.DTO;
using System.Linq.Expressions;

namespace Lpp.CNDS.Data
{
    /// <summary>
    /// The DataMart Entity.  Inherits from EntityWithID which contains ID and TimeStamp Fields
    /// </summary>
    [Table("DataSources")]
    public class DataSource : EntityWithID
    {

        public DataSource()
        {
            DomainData = new HashSet<DataSourceDomainData>();
            DomainAccess = new HashSet<DataSourceDomainAccess>();
            NetworkRequestRoutes = new HashSet<Requests.NetworkRequestRoute>();
            Deleted = false;
        }

        /// <summary>
        /// The name of the datamart.
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The acronym of the datamart.
        /// </summary>
        [Required]
        public string Acronym { get; set; }
        /// <summary>
        /// The ID of the organization the datamart belongs to.
        /// </summary>
        [Required]
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// The organization the datamart belongs to.
        /// </summary>
        public virtual Organization Organization { get; set; }
        /// <summary>
        /// Gets or sets the supported Adapter ID.
        /// </summary>
        public Guid? AdapterSupportedID { get; set; }
        /// <summary>
        /// Gets or sets the supported Adapter.
        /// </summary>
        public virtual Adapter AdapterSupported { get; set; }
        /// <summary>
        /// Determines if the DataSource is Deleted or Not
        /// </summary>
        [Required]
        public bool Deleted { get; set; }
        /// <summary>
        /// Determines when the User Was Deleted
        /// </summary>
        public DateTime? DeletedOn { get; set; }
        /// <summary>
        /// The collection of all DomainData for the DataSource.
        /// </summary>
        public virtual ICollection<DataSourceDomainData> DomainData { get; set; }
        public virtual ICollection<DataSourceDomainAccess> DomainAccess { get; set; }

        public virtual ICollection<Requests.NetworkRequestRoute> NetworkRequestRoutes { get; set; }

    }
    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class DataSourceConfiguration : EntityTypeConfiguration<DataSource>
    {
        public DataSourceConfiguration()
        {

            // Mapping the Organization to its Foriegn Key OrganizationID
            HasRequired(dm => dm.Organization)
                .WithMany(o => o.DataSources)
                .HasForeignKey(dm => dm.OrganizationID)
                .WillCascadeOnDelete(true);

        }
    }

    internal class DataSourceDTOMappingConfiguration : EntityMappingConfiguration<DataSource, DTO.DataSourceDTO>
    {
        public override Expression<Func<DataSource, DataSourceDTO>> MapExpression
        {
            get
            {
                return (ds) => new DataSourceDTO {
                    ID = ds.ID,
                    Name = ds.Name,
                    Acronym = ds.Acronym,
                    OrganizationID = ds.OrganizationID,
                    AdapterSupportedID = ds.AdapterSupportedID,
                    AdapterSupported = ds.AdapterSupportedID.HasValue ? ds.AdapterSupported.Name : string.Empty
                };
            }
        }
    }

    internal class DataSourceExtendedDTOMappingConfiguration : EntityMappingConfiguration<DataSource, DataSourceExtendedDTO>
    {
        public override Expression<Func<DataSource, DataSourceExtendedDTO>> MapExpression
        {
            get
            {
                return (ds) => new DataSourceExtendedDTO {
                    ID = ds.ID,
                    Name = ds.Name,
                    Acronym = ds.Acronym,
                    OrganizationID = ds.OrganizationID,
                    Organization = ds.Organization.Name,
                    NetworkID = ds.Organization.NetworkID,
                    Network = ds.Organization.Network.Name,
                    AdapterSupportedID = ds.AdapterSupportedID,
                    AdapterSupported = ds.AdapterSupportedID.HasValue ? ds.AdapterSupported.Name : string.Empty
                };                
            }
        }
    }

}
