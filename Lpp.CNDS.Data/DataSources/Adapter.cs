using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data
{
    [Table("Adapters")]
    public class Adapter : EntityWithID
    {
        public Adapter()
        {
            DataSources = new HashSet<DataSource>();
        }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        public ICollection<DataSource> DataSources { get; set; }
    }

    /// <summary>
    /// The Fluent API mapping for Entity
    /// </summary>
    internal class AdapterEntityTypeConfiguration : EntityTypeConfiguration<Adapter>
    {
        public AdapterEntityTypeConfiguration()
        {

            // Mapping the Organization to its Foriegn Key OrganizationID
            HasMany(ad => ad.DataSources)
                .WithOptional(ds => ds.AdapterSupported)
                .HasForeignKey(ds => ds.AdapterSupportedID)
                .WillCascadeOnDelete(false);

        }
    }
}
