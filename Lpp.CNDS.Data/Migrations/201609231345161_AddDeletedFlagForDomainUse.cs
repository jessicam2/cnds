namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeletedFlagForDomainUse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainUse", "Deleted", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DomainUse", "Deleted");
        }
    }
}
