using Lpp.CNDS.DTO.Enums;
using Lpp.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO
{
    [DataContract]
    public class NetworkEntityDTO : EntityDtoWithID
    {
        /// <summary>
        /// The ID of the Network the entity belongs to.
        /// </summary>
        [DataMember]
        public Guid NetworkID { get; set; }
        /// <summary>
        /// The name of the network the entity belongs to.
        /// </summary>
        [DataMember]
        public string Network { get; set; }
        /// <summary>
        /// The type of entity.
        /// </summary>
        [DataMember]
        public EntityType EntityType { get; set; }
        /// <summary>
        /// The ID of the entity in the PMN instance.
        /// </summary>
        [DataMember]
        public Guid NetworkEntityID { get; set; }
    }
}
