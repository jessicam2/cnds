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
        [DataMember, Description("No One")]
        NoOne = 0,
        [DataMember, Description("My Network Members")]
        MyNetwork = 100,
        [DataMember, Description("All PMN Members")]
        AllPMNNetworks = 1000,
        [DataMember, Description("All PMN and CNDS Members")]
        AllNetworks = 10000,
        [DataMember, Description("Anyone")]
        Anyone = 100000
    }
}
