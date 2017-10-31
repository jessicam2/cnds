namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVisibilityPermissions : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO Permissions(ID, Name, Description)VALUES('0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0', 'Manage CNDS Visibility', 'Allows the Selected Security Group to see the Visibility of CNDS MetaData')");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0', 0)");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0', 1)");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0', 3)");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0', 9)");
        }

        public override void Down()
        {
            Sql(@"DELETE From PermissionLocations where PermissionID = '0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0'");
            Sql(@"Delete from Permissions where ID = '0D2FD719-B3A9-40FF-9C3A-EFE13FE485A0'");
        }
    }
}
