namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NetworkRequestParticipantRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NetworkRequestParticipants", "NetworkID", "dbo.Networks");
            AddForeignKey("dbo.NetworkRequestParticipants", "NetworkID", "dbo.Networks", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetworkRequestParticipants", "NetworkID", "dbo.Networks");
            AddForeignKey("dbo.NetworkRequestParticipants", "NetworkID", "dbo.Networks", "ID", cascadeDelete: true);
        }
    }
}
