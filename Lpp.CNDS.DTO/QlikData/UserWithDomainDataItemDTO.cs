using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.QlikData
{
    [DataContract]
    public class UserWithDomainDataItemDTO : EntityWithDomainDataItemDTO
    {
        /// <summary>
        /// Gets or sets the ID of the User.
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the salutation for the user.
        /// </summary>
        [DataMember]
        public string UserSalutation { get; set; }
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [DataMember]
        public string UserFirstName { get; set; }
        /// <summary>
        /// Gets or sets the user's middle name.
        /// </summary>
        [DataMember]
        public string UserMiddleName { get; set; }
        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [DataMember]
        public string UserLastName { get; set; }
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [DataMember]
        public string UserEmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        [DataMember]
        public string UserPhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the user's fax number.
        /// </summary>
        [DataMember]
        public string UserFaxNumber { get; set; }
        /// <summary>
        /// Gets or sets an indicator for if the user is active or not.
        /// </summary>
        [DataMember]
        public bool UserIsActive { get; set; }
    }
}
