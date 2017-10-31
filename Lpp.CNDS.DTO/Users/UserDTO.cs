using Lpp.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO
{
    /// <summary>
    /// DTO of the User
    /// </summary>
    [DataContract]
    public class UserDTO : EntityDtoWithID
    {
        /// <summary>
        /// The Guid of the The Network the User Belongs to
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        [DataMember, Required, MaxLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// The user's salutation.
        /// </summary>
        [DataMember, MaxLength(100)]
        public string Salutation { get; set; }
        /// <summary>
        /// User First Name
        /// </summary>
        [DataMember, MaxLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// The user's middle name.
        /// </summary>
        [DataMember, MaxLength(100)]
        public string MiddleName { get; set; }
        /// <summary>
        /// User Last Name
        /// </summary>
        [DataMember, MaxLength(100)]
        public string LastName { get; set; }        
        /// <summary>
        /// The user's email address.
        /// </summary>
        [DataMember, MaxLength(400)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// The user's phone number.
        /// </summary>
        [DataMember, MaxLength(50)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// The user's fax number.
        /// </summary>
        [DataMember, MaxLength(50)]
        public string FaxNumber { get; set; }
        /// <summary>
        /// The ID of the organization the user is associated with.
        /// </summary>
        [DataMember]
        public Guid? OrganizationID { get; set; }
        /// <summary>
        /// Determines if the User is Active
        /// </summary>
        [DataMember]
        public bool Active { get; set; }
    }
}
