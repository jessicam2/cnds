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
    [Table("SecurityGroups")]
    public class SecurityGroup : EntityWithID
    {
        public SecurityGroup()
        {
            Users = new HashSet<SecurityGroupUser>();
        }

        [Required, Index(IsUnique = true), MaxLength(255)]
        public string Name { get; set; }

        public virtual ICollection<SecurityGroupUser> Users { get; set; }

        public virtual ICollection<AclGlobal> GlobalAcls { get; set; }
    }

}
