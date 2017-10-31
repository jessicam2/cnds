using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects.ValidationAttributes;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data
{
    [Table("Permissions")]
    public class Permission : EntityWithID
    {

        public Permission()
        {
        }

        [Required, Index(IsUnique = true), MaxLength(255)]
        public string Name { get; set; }
        
        public string Description { get; set; }

    }
}
