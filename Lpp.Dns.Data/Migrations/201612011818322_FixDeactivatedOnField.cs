namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDeactivatedOnField : DbMigration
    {
        public override void Up()
        {
            Sql("Update Users Set DeactivatedOn = null where isActive = 1");
        }
        
        public override void Down()
        {
        }
    }
}
