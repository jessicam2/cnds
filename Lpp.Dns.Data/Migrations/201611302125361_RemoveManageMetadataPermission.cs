namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManageMetadataPermission : DbMigration
    {
        public override void Up()
        {
            //9B797195-578A-430F-A56A-F133DC98DFB9
            Sql("delete from AclGlobal where PermissionID = '9B797195-578A-430F-A56A-F133DC98DFB9'");
            Sql("delete from PermissionLocations where PermissionID = '9B797195-578A-430F-A56A-F133DC98DFB9'");
            Sql("delete from Permissions where ID = '9B797195-578A-430F-A56A-F133DC98DFB9'");
        }
        
        public override void Down()
        {
        }
    }
}
