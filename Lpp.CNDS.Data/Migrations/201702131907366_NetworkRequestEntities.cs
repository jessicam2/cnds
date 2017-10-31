namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NetworkRequestEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NetworkRequests",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        NetworkID = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: true)
                .Index(t => t.NetworkID);
            
            CreateTable(
                "dbo.NetworkRequestParticipants",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        NetworkRequestID = c.Guid(nullable: false),
                        NetworkID = c.Guid(nullable: false),
                        ProjectID = c.Guid(nullable: false),
                        RequestTypeID = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: false)
                .ForeignKey("dbo.NetworkRequests", t => t.NetworkRequestID, cascadeDelete: true)
                .Index(t => t.NetworkRequestID)
                .Index(t => t.NetworkID);
            
            CreateTable(
                "dbo.NetworkRequestRoutes",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ParticipantID = c.Guid(nullable: false),
                        DataSourceID = c.Guid(nullable: false),
                        SourceRequestDataMartID = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: true)
                .ForeignKey("dbo.NetworkRequestParticipants", t => t.ParticipantID)
                .Index(t => t.ParticipantID)
                .Index(t => t.DataSourceID);
            
            CreateTable(
                "dbo.NetworkRequestResponses",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        SourceResponseID = c.Guid(nullable: false),
                        NetworkRequestRouteID = c.Guid(nullable: false),
                        IterationIndex = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NetworkRequestRoutes", t => t.NetworkRequestRouteID, cascadeDelete: true)
                .Index(t => t.NetworkRequestRouteID);
            
            CreateTable(
                "dbo.NetworkRequestDocuments",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ResponseID = c.Guid(nullable: false),
                        SourceDocumentID = c.Guid(nullable: false),
                        DestinationRevisionSetID = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NetworkRequestResponses", t => t.ResponseID, cascadeDelete: true)
                .Index(t => t.ResponseID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetworkRequestParticipants", "NetworkRequestID", "dbo.NetworkRequests");
            DropForeignKey("dbo.NetworkRequestRoutes", "ParticipantID", "dbo.NetworkRequestParticipants");
            DropForeignKey("dbo.NetworkRequestResponses", "NetworkRequestRouteID", "dbo.NetworkRequestRoutes");
            DropForeignKey("dbo.NetworkRequestDocuments", "ResponseID", "dbo.NetworkRequestResponses");
            DropForeignKey("dbo.NetworkRequestRoutes", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.NetworkRequestParticipants", "NetworkID", "dbo.Networks");
            DropForeignKey("dbo.NetworkRequests", "NetworkID", "dbo.Networks");
            DropIndex("dbo.NetworkRequestDocuments", new[] { "ResponseID" });
            DropIndex("dbo.NetworkRequestResponses", new[] { "NetworkRequestRouteID" });
            DropIndex("dbo.NetworkRequestRoutes", new[] { "DataSourceID" });
            DropIndex("dbo.NetworkRequestRoutes", new[] { "ParticipantID" });
            DropIndex("dbo.NetworkRequestParticipants", new[] { "NetworkID" });
            DropIndex("dbo.NetworkRequestParticipants", new[] { "NetworkRequestID" });
            DropIndex("dbo.NetworkRequests", new[] { "NetworkID" });
            DropTable("dbo.NetworkRequestDocuments");
            DropTable("dbo.NetworkRequestResponses");
            DropTable("dbo.NetworkRequestRoutes");
            DropTable("dbo.NetworkRequestParticipants");
            DropTable("dbo.NetworkRequests");
        }
    }
}
