namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeletedToDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainReference", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Domain", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Domain", "Deleted");
            DropColumn("dbo.DomainReference", "Deleted");
        }
    }
}
