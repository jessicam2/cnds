using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lpp.CNDS.Data
{
    [Table("UserDomainData")]
    public class UserDomainData : DomainData
    {
        /// <summary>
        /// The Identifier of the Associated User
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// The Navigational Property of the User
        /// </summary>
        public virtual User User { get; set; }
    }
}
