namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNetworkRequestTypeMappingTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NetworkRequestTypeMappings",
                c => new
                    {
                        NetworkOneID = c.Guid(nullable: false),
                        NetworkTwoID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.NetworkOneID, t.NetworkTwoID })
                .ForeignKey("dbo.NetworkRequestTypeDefinitions", t => t.NetworkOneID, cascadeDelete: false)
                .ForeignKey("dbo.NetworkRequestTypeDefinitions", t => t.NetworkTwoID, cascadeDelete: false)
                .Index(t => t.NetworkOneID)
                .Index(t => t.NetworkTwoID);
            
            CreateTable(
                "dbo.NetworkRequestTypeDefinitions",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        NetworkID = c.Guid(nullable: false),
                        ProjectID = c.Guid(nullable: false),
                        RequestTypeID = c.Guid(nullable: false),
                        DataSourceID = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: false)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: false)
                .Index(t => new { t.NetworkID, t.ProjectID, t.RequestTypeID, t.DataSourceID }, unique: true, name: "IX_UniqueDefinition");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetworkRequestTypeMappings", "NetworkTwoID", "dbo.NetworkRequestTypeDefinitions");
            DropForeignKey("dbo.NetworkRequestTypeMappings", "NetworkOneID", "dbo.NetworkRequestTypeDefinitions");
            DropForeignKey("dbo.NetworkRequestTypeDefinitions", "NetworkID", "dbo.Networks");
            DropForeignKey("dbo.NetworkRequestTypeDefinitions", "DataSourceID", "dbo.DataSources");
            DropIndex("dbo.NetworkRequestTypeDefinitions", "IX_UniqueDefinition");
            DropIndex("dbo.NetworkRequestTypeMappings", new[] { "NetworkTwoID" });
            DropIndex("dbo.NetworkRequestTypeMappings", new[] { "NetworkOneID" });
            DropTable("dbo.NetworkRequestTypeDefinitions");
            DropTable("dbo.NetworkRequestTypeMappings");
        }
    }
}
