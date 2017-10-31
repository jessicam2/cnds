using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.Data
{
    public class AclGlobal : Lpp.Objects.Entity
    {
        [Key, Column(Order = 1)]
        public Guid SecurityGroupID { get; set; }

        public virtual SecurityGroup SecurityGroup { get; set; }

        [Key, Column(Order = 2)]
        public Guid PermissionID { get; set; }

        public virtual Permission Permission { get; set; }

        public bool Allowed { get; set; }
    }
}
