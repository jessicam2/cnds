using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lpp.Utilities.Objects;

namespace Lpp.CNDS.Data
{
    public abstract class DomainAccess : EntityWithID
    {
        
        public virtual DTO.Enums.AccessType AccessType { get; set; }

        [Required]
        public virtual Guid DomainUseID { get; set; }
        
        public virtual DomainUse DomainUse { get; set; }
    }
}
