using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects;

namespace Lpp.CNDS.Data
{
    [Table("SecurityGroupUsers")]
    public class SecurityGroupUser : Entity
    {
        [Key, Column(Order = 0)]
        public Guid SecurityGroupID { get; set; }

        public virtual SecurityGroup SecurityGroup { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserID { get; set; }

        public virtual User User { get; set; }
    }

    internal class SecurityGroupUsersConfiguration : EntityTypeConfiguration<SecurityGroupUser>
    {
        public SecurityGroupUsersConfiguration()
        {
            HasRequired(u => u.SecurityGroup)
                .WithMany(n => n.Users)
                .HasForeignKey(u => u.SecurityGroupID)
                .WillCascadeOnDelete(true);

            HasRequired(u => u.User)
                .WithMany(n => n.SecurityGroups)
                .HasForeignKey(u => u.UserID)
                .WillCascadeOnDelete(true);
        }
    }
}
