using System.Runtime.Serialization;

namespace Lpp.CNDS.DTO.Enums
{
    [DataContract]
    public enum EntityType
    {
        [EnumMember]
        Organization = 0,
        [EnumMember]
        User = 1,
        [EnumMember]
        DataSource = 2
    }
}
