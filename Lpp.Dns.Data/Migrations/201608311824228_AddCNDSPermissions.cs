namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCNDSPermissions : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO Permissions(ID, Name, Description)VALUES('B76D3582-2D61-4847-9CA1-6C72DFD798AB', 'Search CNDS', 'Allows the Selected Security Group to Search CNDS')");
            Sql(@"INSERT INTO PermissionLocations(PermissionID, Type)VALUES('B76D3582-2D61-4847-9CA1-6C72DFD798AB', 0)");
        }
        
        public override void Down()
        {
            Sql(@"DELETE From PermissionLocations where PermissionID = 'B76D3582-2D61-4847-9CA1-6C72DFD798AB'");
            Sql(@"Delete from Permissions where ID = 'B76D3582-2D61-4847-9CA1-6C72DFD798AB'");
        }
    }
}
