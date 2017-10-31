namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactInfoToOrganizations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "ContactEmail", c => c.String(maxLength: 510));
            AddColumn("dbo.Organizations", "ContactFirstName", c => c.String(maxLength: 100));
            AddColumn("dbo.Organizations", "ContactLastName", c => c.String(maxLength: 100));
            AddColumn("dbo.Organizations", "ContactPhone", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "ContactPhone");
            DropColumn("dbo.Organizations", "ContactLastName");
            DropColumn("dbo.Organizations", "ContactFirstName");
            DropColumn("dbo.Organizations", "ContactEmail");
        }
    }
}
