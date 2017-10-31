using Lpp.Utilities.Objects;
using Lpp.Utilities.Security;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Lpp.Utilities;
using Lpp.CNDS.DTO;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Lpp.CNDS.Data
{
    /// <summary>
    /// The User Domain Object
    /// </summary>
    [Table("Users")]
    public class User : EntityWithID, IUser
    {
        public User()
        {
            DomainData = new HashSet<UserDomainData>();
            DomainAccess = new HashSet<UserDomainAccess>();
            Deleted = false;
            Active = true;
            SecurityGroups = new HashSet<SecurityGroupUser>();
        }
        /// <summary>
        /// The ID of the network the user belongs to.
        /// </summary>
        public Guid NetworkID { get; set; }
        /// <summary>
        /// The network the user belongs to.
        /// </summary>
        public Network Network { get; set; }
        /// <summary>
        /// The user's username.
        /// </summary>
        [Required, MaxLength(150)]
        public string UserName { get; set; }
        /// <summary>
        /// The user's first name.
        /// </summary>
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// The the user's last name.
        /// </summary>
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        /// <summary>
        /// Used to Resolve IUser Property LastOrCompanyName.  Maps to Property {LastName} of the User  
        /// </summary>
        string IUser.LastOrCompanyName
        {
            get
            {
                return LastName;
            }
            set
            {
                LastName = value;
            }
        }

        /// <summary>
        /// The user's middle name.
        /// </summary>
        [MaxLength(100)]
        public string MiddleName { get; set; }
        /// <summary>
        /// The user's salutation.
        /// </summary>
        [MaxLength(100)]
        public string Salutation { get; set; }
        /// <summary>
        /// The user's email address.
        /// </summary>
        [MaxLength(400)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// The user's phone number.
        /// </summary>
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// The user's fax number.
        /// </summary>
        [MaxLength(50)]
        public string FaxNumber { get; set; }
        /// <summary>
        /// The ID of the organization the user is associated with.
        /// </summary>
        public Guid? OrganizationID { get; set; }
        /// <summary>
        /// Determines if the User is Active or Not
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Determines if the User is Deleted or Not
        /// </summary>
        [Required]
        public bool Deleted { get; set; }
        /// <summary>
        /// Determines when the User Was Deleted
        /// </summary>
        public DateTime? DeletedOn { get; set; }
        /// <summary>
        /// The organization the user is associated with.
        /// </summary>
        public virtual Organization Organization { get; set; }
        public ICollection<UserDomainData> DomainData { get; set; }
        public ICollection<UserDomainAccess> DomainAccess { get; set; }

        public ICollection<SecurityGroupUser> SecurityGroups { get; set; }

    }
    /// <summary>
    /// The Fluent API Definition of the User Object
    /// </summary>
    internal class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            /// Mapping the Network to its Foriegn Key NetworkID
            HasRequired(u => u.Network)
                 .WithMany(n => n.Users)
                 .HasForeignKey(u => u.NetworkID)
                 .WillCascadeOnDelete(true);

            HasOptional(t => t.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(t => t.OrganizationID)
                .WillCascadeOnDelete(true);
        }
    }
    /// <summary>
    /// The DTO Mapping for the User Domain Object to the Data Transfer Object Class UserDTO
    /// </summary>
    internal class UserDtoMappingConfiguration : EntityMappingConfiguration<User, UserDTO>
    {
        public override System.Linq.Expressions.Expression<Func<User, UserDTO>> MapExpression
        {
            get
            {
                return (u) => new UserDTO
                {
                    ID = u.ID,
                    UserName = u.UserName,
                    Salutation = u.Salutation,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    NetworkID = u.NetworkID,
                    Timestamp = u.Timestamp,
                    EmailAddress = u.EmailAddress,
                    PhoneNumber = u.PhoneNumber,
                    FaxNumber = u.FaxNumber,                    
                    OrganizationID = u.OrganizationID,
                    Active = u.Active
                };
            }
        }
    }
}
