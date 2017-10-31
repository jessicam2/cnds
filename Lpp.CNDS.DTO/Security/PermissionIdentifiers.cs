using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Security;

namespace Lpp.CNDS.DTO.Security
{
    /// <summary>
    /// Permission Identifier
    /// </summary>
    public class PermissionDefinition : IPermissionDefinition
    {
        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// determines the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is PermissionDefinition)
            {
                return ((PermissionDefinition)obj).ID == this.ID;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// returns gethashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        /// <summary>
        /// returns object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is PermissionDefinition))
                return -1;

            var ob = obj as PermissionDefinition;
            return this.ID.CompareTo(ob.ID);
        }
        /// <summary>
        /// operator system
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator Guid(PermissionDefinition o) //This allows you to just compare this to a Guid. Doesn't work in Entity Framework.
        {
            return o.ID;
        }
    }

    public static class PermissionIdentifiers
    {
        static PermissionIdentifiers()
        {
            var r = Global.ManageSecurity;
        }

        /// <summary>
        /// Permission definition
        /// </summary>
        public static List<PermissionDefinition> Definitions = new List<PermissionDefinition>(100);


        public static class Global
        {

            public static readonly PermissionDefinition ManageMetadata = new PermissionDefinition { ID = new Guid("4EB90001-6F08-46E3-911D-A6BF012EBFB8") };
            public static readonly PermissionDefinition ManageSecurity = new PermissionDefinition { ID = new Guid("E3410001-B6F4-4C51-B269-A6BF012EC64D") };
            public static readonly PermissionDefinition CreateSecurityGroup = new PermissionDefinition { ID = new Guid("E2A20001-1B7F-463E-8990-A6BF012ECC72") };
            public static readonly PermissionDefinition EditSecurityGroup = new PermissionDefinition { ID = new Guid("10CF0001-451E-44ED-8388-A6BF012ED2D6") };
            public static readonly PermissionDefinition DeleteSecurityGroup = new PermissionDefinition { ID = new Guid("25D50001-03BD-4EDE-9E6F-A6BF012ED91E") };
            public static readonly PermissionDefinition ManageRequestTypeMappings = new PermissionDefinition { ID = new Guid("9AFF0001-1E18-4AEA-8C2E-A6DB01656A4B") };

            static Global()
            {
                Definitions.AddRange(new[] {
                    ManageMetadata,
                    ManageSecurity,
                    CreateSecurityGroup,
                    EditSecurityGroup,
                    DeleteSecurityGroup,
                    ManageRequestTypeMappings
                });
            }

        }


    
    }
}
