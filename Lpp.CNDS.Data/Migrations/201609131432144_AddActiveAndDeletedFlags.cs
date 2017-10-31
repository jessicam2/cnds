namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveAndDeletedFlags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataSources", "Deleted", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.DataSources", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.Organizations", "Deleted", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Organizations", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.Users", "Active", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Users", "Deleted", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Users", "DeletedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DeletedOn");
            DropColumn("dbo.Users", "Deleted");
            DropColumn("dbo.Users", "Active");
            DropColumn("dbo.Organizations", "DeletedOn");
            DropColumn("dbo.Organizations", "Deleted");
            DropColumn("dbo.DataSources", "DeletedOn");
            DropColumn("dbo.DataSources", "Deleted");
        }
    }
}
