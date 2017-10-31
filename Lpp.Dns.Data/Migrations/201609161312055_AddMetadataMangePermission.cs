namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetadataMangePermission : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO Permissions(ID, Name, Description)VALUES('9B797195-578A-430F-A56A-F133DC98DFB9', 'Manage Metadata', 'Allows the Selected Security Group to Manage Metadata in CNDS')");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('9B797195-578A-430F-A56A-F133DC98DFB9', 0)");
        }

        public override void Down()
        {
            Sql(@"DELETE From PermissionLocations where PermissionID = '9B797195-578A-430F-A56A-F133DC98DFB9'");
            Sql(@"Delete from Permissions where ID = '9B797195-578A-430F-A56A-F133DC98DFB9'");
        }
    }
}
