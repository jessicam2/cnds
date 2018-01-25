using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.DTO.Enums
{
    /// <summary>
    /// Enum for the Access Type to the Domain
    /// </summary>
    [DataContract]
    public enum AccessType
    {
        [EnumMember, Description("No One")]
        NoOne = 0,
        [EnumMember, Description("My Network Members")]
        MyNetwork = 100,
        [EnumMember, Description("All PMN Members")]
        AllPMNNetworks = 1000,
        [EnumMember, Description("All PMN and CNDS Members")]
        AllNetworks = 10000,
        [EnumMember, Description("Anyone")]
        Anyone = 100000
    }
}
