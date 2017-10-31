namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDomainChildAssociation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Domain", "Domain_ID", "dbo.Domain");
            DropIndex("dbo.Domain", new[] { "Domain_ID" });
            //DropColumn("dbo.Domain", "ParentDomainID");
            //RenameColumn(table: "dbo.Domain", name: "Domain_ID", newName: "ParentDomainID");
            //AddForeignKey("dbo.Domain", "ParentDomainID", "dbo.Domain", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Domain", "ParentDomainID", "dbo.Domain");
            //RenameColumn(table: "dbo.Domain", name: "ParentDomainID", newName: "Domain_ID");
            //AddColumn("dbo.Domain", "ParentDomainID", c => c.Guid());
            //CreateIndex("dbo.Domain", "Domain_ID");
            //AddForeignKey("dbo.Domain", "Domain_ID", "dbo.Domain", "ID");
        }
    }
}
