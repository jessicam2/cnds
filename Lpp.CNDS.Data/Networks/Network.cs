using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Lpp.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lpp.CNDS.Data
{
    /// <summary>
    /// The Network Entity.  Inherits from EntityWithID which Contains Fields for ID and TimeStamp
    /// </summary>
    [Table("Networks")]
    public class Network : EntityWithID
    {

        public Network()
        {
            Organizations = new HashSet<Organization>();
            NetworkEntities = new HashSet<NetworkEntity>();
            SourceRequests = new HashSet<Requests.NetworkRequest>();
            ParticipantRequests = new HashSet<Requests.NetworkRequestParticipant>();
        }
        /// <summary>
        /// The Name of the Network
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// The URL to the API of the Network
        /// </summary>
        [Required, MaxLength(450)]
        public string Url { get; set; }
        /// <summary>
        /// The url to the root of the PMN API.
        /// </summary>
        [MaxLength(450)]
        public string ServiceUrl { get; set; }
        /// <summary>
        /// The username to use when accessing the networks API.
        /// </summary>
        [MaxLength(80)]
        public string ServiceUserName { get; set; }
        /// <summary>
        /// The password to use when accessing the networks API.
        /// </summary>
        [MaxLength(255)]
        public string ServicePassword { get; set; }
        /// <summary>
        /// A Collection of Organizations that this Network Corresponds to
        /// </summary>
        public virtual ICollection<Organization> Organizations { get; set; }
        /// <summary>
        /// A Collection of Users that this Network Corresponds to
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
        /// <summary>
        /// A collection of ID mappings for entities imported from a network.
        /// </summary>
        public virtual ICollection<NetworkEntity> NetworkEntities { get; set; }

        public virtual ICollection<Requests.NetworkRequest> SourceRequests { get; set; }

        public virtual ICollection<Requests.NetworkRequestParticipant> ParticipantRequests { get; set; }
    }
    /// <summary>
    /// The Mapping for Network to its DTO NetworkItemDTO
    /// </summary>
    internal class NetworkDtoMappingConfiguration : EntityMappingConfiguration<Network, NetworkDTO>
    {
        public override System.Linq.Expressions.Expression<Func<Network, NetworkDTO>> MapExpression
        {
            get
            {
                return (o) => new NetworkDTO
                {
                    ID = o.ID,
                    Name = o.Name,
                    Url = o.Url,
                    ServiceUrl = o.ServiceUrl,
                    ServiceUserName = o.ServiceUserName,
                    Timestamp = o.Timestamp
                };
            }
        }
    }
}
