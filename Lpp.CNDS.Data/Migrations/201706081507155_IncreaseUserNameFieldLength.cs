namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseUserNameFieldLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
