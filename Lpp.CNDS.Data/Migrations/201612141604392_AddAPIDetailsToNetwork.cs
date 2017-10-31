namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAPIDetailsToNetwork : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Networks", "ServiceUrl", c => c.String(maxLength: 450));
            AddColumn("dbo.Networks", "ServiceUserName", c => c.String(maxLength: 80));
            AddColumn("dbo.Networks", "ServicePassword", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Networks", "ServicePassword");
            DropColumn("dbo.Networks", "ServiceUserName");
            DropColumn("dbo.Networks", "ServiceUrl");
        }
    }
}
