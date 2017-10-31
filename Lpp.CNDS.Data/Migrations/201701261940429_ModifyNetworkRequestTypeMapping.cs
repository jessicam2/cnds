namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyNetworkRequestTypeMapping : DbMigration
    {
        public override void Up()
        {            
            DropForeignKey("dbo.NetworkRequestTypeMappings", "NetworkOneID", "dbo.NetworkRequestTypeDefinitions");
            DropForeignKey("dbo.NetworkRequestTypeMappings", "NetworkTwoID", "dbo.NetworkRequestTypeDefinitions");
            DropIndex("dbo.NetworkRequestTypeMappings", new[] { "NetworkOneID" });
            DropIndex("dbo.NetworkRequestTypeMappings", new[] { "NetworkTwoID" });
            DropPrimaryKey("dbo.NetworkRequestTypeMappings");
            Sql("TRUNCATE TABLE NetworkRequestTypeMappings");
            CreateTable(
                "dbo.NetworkRequestTypeMappingRoutes",
                c => new
                    {
                        RequestTypeMappingID = c.Guid(nullable: false),
                        RequestTypeDefinitionID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RequestTypeMappingID, t.RequestTypeDefinitionID })
                .ForeignKey("dbo.NetworkRequestTypeMappings", t => t.RequestTypeMappingID, cascadeDelete: true)
                .ForeignKey("dbo.NetworkRequestTypeDefinitions", t => t.RequestTypeDefinitionID, cascadeDelete: true)
                .Index(t => t.RequestTypeMappingID)
                .Index(t => t.RequestTypeDefinitionID);
            
            AddColumn("dbo.NetworkRequestTypeMappings", "ID", c => c.Guid(nullable: false));
            AddColumn("dbo.NetworkRequestTypeMappings", "NetworkID", c => c.Guid(nullable: false));
            AddColumn("dbo.NetworkRequestTypeMappings", "ProjectID", c => c.Guid(nullable: false));
            AddColumn("dbo.NetworkRequestTypeMappings", "RequestTypeID", c => c.Guid(nullable: false));
            AddColumn("dbo.NetworkRequestTypeMappings", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddPrimaryKey("dbo.NetworkRequestTypeMappings", "ID");
            CreateIndex("dbo.NetworkRequestTypeMappings", new[] { "NetworkID", "ProjectID", "RequestTypeID" }, unique: true, name: "IX_UniqueSourceRequestType");
            AddForeignKey("dbo.NetworkRequestTypeMappings", "NetworkID", "dbo.Networks", "ID", cascadeDelete: true);
            DropColumn("dbo.NetworkRequestTypeMappings", "NetworkOneID");
            DropColumn("dbo.NetworkRequestTypeMappings", "NetworkTwoID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NetworkRequestTypeMappings", "NetworkTwoID", c => c.Guid(nullable: false));
            AddColumn("dbo.NetworkRequestTypeMappings", "NetworkOneID", c => c.Guid(nullable: false));
            DropForeignKey("dbo.NetworkRequestTypeMappingRoutes", "RequestTypeDefinitionID", "dbo.NetworkRequestTypeDefinitions");
            DropForeignKey("dbo.NetworkRequestTypeMappingRoutes", "RequestTypeMappingID", "dbo.NetworkRequestTypeMappings");
            DropForeignKey("dbo.NetworkRequestTypeMappings", "NetworkID", "dbo.Networks");
            DropIndex("dbo.NetworkRequestTypeMappings", "IX_UniqueSourceRequestType");
            DropIndex("dbo.NetworkRequestTypeMappingRoutes", new[] { "RequestTypeDefinitionID" });
            DropIndex("dbo.NetworkRequestTypeMappingRoutes", new[] { "RequestTypeMappingID" });
            DropPrimaryKey("dbo.NetworkRequestTypeMappings");
            DropColumn("dbo.NetworkRequestTypeMappings", "Timestamp");
            DropColumn("dbo.NetworkRequestTypeMappings", "RequestTypeID");
            DropColumn("dbo.NetworkRequestTypeMappings", "ProjectID");
            DropColumn("dbo.NetworkRequestTypeMappings", "NetworkID");
            DropColumn("dbo.NetworkRequestTypeMappings", "ID");
            DropTable("dbo.NetworkRequestTypeMappingRoutes");
            AddPrimaryKey("dbo.NetworkRequestTypeMappings", new[] { "NetworkOneID", "NetworkTwoID" });
            CreateIndex("dbo.NetworkRequestTypeMappings", "NetworkTwoID");
            CreateIndex("dbo.NetworkRequestTypeMappings", "NetworkOneID");
            AddForeignKey("dbo.NetworkRequestTypeMappings", "NetworkTwoID", "dbo.NetworkRequestTypeDefinitions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NetworkRequestTypeMappings", "NetworkOneID", "dbo.NetworkRequestTypeDefinitions", "ID", cascadeDelete: true);
        }
    }
}
