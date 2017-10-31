namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterManageRequestTypeMappingsPermission : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('9AFF0001-1E18-4AEA-8C2E-A6DB01656A4B', 'Manage RequestType Mappings', 'Governs the ability for a user to view and manage cross network requesttype mappings.')");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM dbo.[Permissions] WHERE ID = '9AFF0001-1E18-4AEA-8C2E-A6DB01656A4B'");
        }
    }
}
