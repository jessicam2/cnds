namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDomainAccessTables2 : DbMigration
    {
        public override void Up()
        {
            //After previous corrections to the DomainAccess tables it looks like the FK to DomainUse got dropped from the DB even though EF thinks it is still there.
            //Drop if exists, and re-add.
            DropForeignKey("dbo.DataSourceDomainAccess", "DomainUseID", "dbo.DomainUse");
            DropForeignKey("dbo.OrganizationDomainAccess", "DomainUseID", "dbo.DomainUse");
            DropForeignKey("dbo.UserDomainAccess", "DomainUseID", "dbo.DomainUse");

            AddForeignKey("dbo.UserDomainAccess", "DomainUseID", "dbo.DomainUse", "ID", cascadeDelete: true);
            AddForeignKey("dbo.OrganizationDomainAccess", "DomainUseID", "dbo.DomainUse", "ID", cascadeDelete: true);
            AddForeignKey("dbo.DataSourceDomainAccess", "DomainUseID", "dbo.DomainUse", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
        }
    }
}
