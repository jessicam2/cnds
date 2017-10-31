namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGroupDataType : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE Domain SET DataType = 'container' where DataType = 'group'");
        }
        
        public override void Down()
        {
            Sql(@"UPDATE Domain SET DataType = 'group' where DataType = 'container'");
        }
    }
}
