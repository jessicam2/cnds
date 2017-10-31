namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class INIT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataSources",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                    Acronym = c.String(nullable: false),
                    OrganizationID = c.Guid(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Organizations", t => t.OrganizationID, cascadeDelete: true)
                .Index(t => t.OrganizationID);

            CreateTable(
                "dbo.DomainData",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    DomainUseID = c.Guid(nullable: false),
                    Value = c.String(),
                    DomainReferenceID = c.Guid(),
                    SequenceNumber = c.Int(nullable: false, defaultValue: 0),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainReference", t => t.DomainReferenceID)
                .ForeignKey("dbo.DomainUse", t => t.DomainUseID, cascadeDelete: true)
                .Index(t => t.DomainUseID)
                .Index(t => t.DomainReferenceID);

            CreateTable(
                "dbo.DomainReference",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    DomainID = c.Guid(nullable: false),
                    ParentDomainReferenceID = c.Guid(),
                    Title = c.String(nullable: false, maxLength: 255),
                    Description = c.String(),
                    Value = c.String(),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Domain", t => t.DomainID, cascadeDelete: true)
                .ForeignKey("dbo.DomainReference", t => t.ParentDomainReferenceID)
                .Index(t => t.DomainID)
                .Index(t => t.ParentDomainReferenceID);

            CreateTable(
                "dbo.Domain",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ParentDomainID = c.Guid(),
                    Title = c.String(nullable: false, maxLength: 255),
                    IsMultiValue = c.Boolean(nullable: false, defaultValue: false),
                    EnumValue = c.Short(),
                    DataType = c.String(maxLength: 255),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Domain", t => t.ParentDomainID)
                .Index(t => t.ParentDomainID);

            CreateTable(
                "dbo.DomainUse",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    DomainID = c.Guid(nullable: false),
                    EntityType = c.Int(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Domain", t => t.DomainID, cascadeDelete: true)
                .Index(t => t.DomainID);

            CreateTable(
                "dbo.Organizations",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NetworkID = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 255),
                    Acronym = c.String(nullable: false, maxLength: 100),
                    ParentOrganizationID = c.Guid(),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: true)
                .ForeignKey("dbo.Organizations", t => t.ParentOrganizationID)
                .Index(t => t.NetworkID)
                .Index(t => t.ParentOrganizationID);

            CreateTable(
                "dbo.Networks",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 100),
                    Url = c.String(nullable: false, maxLength: 450),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NetworkEntities",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NetworkID = c.Guid(nullable: false),
                    EntityType = c.Int(nullable: false),
                    NetworkEntityID = c.Guid(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: true)
                .Index(t => t.NetworkID);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NetworkID = c.Guid(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 50),
                    FirstName = c.String(nullable: false, maxLength: 100),
                    LastName = c.String(nullable: false, maxLength: 100),
                    MiddleName = c.String(maxLength: 100),
                    Salutation = c.String(maxLength: 100),
                    EmailAddress = c.String(maxLength: 400),
                    PhoneNumber = c.String(maxLength: 50),
                    FaxNumber = c.String(maxLength: 50),
                    OrganizationID = c.Guid(),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Networks", t => t.NetworkID, cascadeDelete: true)
                .ForeignKey("dbo.Organizations", t => t.OrganizationID)
                .Index(t => t.NetworkID)
                .Index(t => t.OrganizationID);

            CreateTable(
                "dbo.DataSourceDomainData",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    DataSourceID = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainData", t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.DataSourceID);

            CreateTable(
                "dbo.OrganizationDomainData",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    OrganizationID = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainData", t => t.ID)
                .ForeignKey("dbo.Organizations", t => t.OrganizationID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.OrganizationID);

            CreateTable(
                "dbo.UserDomainData",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    UserID = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainData", t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.UserID);
            Sql(@"CREATE TABLE [dbo].[EntityTypes](
	                [ID] [Int] NOT NULL,
	                [Name] [nvarchar](50) NOT NULL,
                    CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED 
                (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]");
            Sql(@"ALTER TABLE [dbo].[DomainUse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DomainUse_dbo.EntityTypes_ID] FOREIGN KEY([EntityType])
                    REFERENCES [dbo].[EntityTypes] ([ID])
                    ON DELETE CASCADE");
            Sql(@"ALTER TABLE [dbo].[NetworkEntities]  WITH CHECK ADD  CONSTRAINT [FK_dbo.NetworkEntities_dbo.EntityTypes_ID] FOREIGN KEY([EntityType])
                    REFERENCES [dbo].[EntityTypes] ([ID])
                    ON DELETE CASCADE");
            Sql(@"CREATE NONCLUSTERED INDEX [IX_EntityType] ON [dbo].[NetworkEntities]
                    (
                        [EntityType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
            Sql(@"CREATE NONCLUSTERED INDEX [IX_EntityType] ON [dbo].[DomainUse]
                    (
                        [EntityType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
            Sql(@"INSERT INTO EntityTypes(ID,Name)VALUES(0,'Organization')");
            Sql(@"INSERT INTO EntityTypes(ID,Name)VALUES(1,'User')");
            Sql(@"INSERT INTO EntityTypes(ID,Name)VALUES(2, 'DataSource')");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[NetworkEntities] DROP CONSTRAINT [FK_dbo.NetworkEntities_dbo.EntityTypes_ID]");
            Sql("ALTER TABLE [dbo].[DomainUse] DROP CONSTRAINT [FK_dbo.DomainUse_dbo.EntityTypes_ID]");
            Sql("Drop Index IX_EntityType on NetworkEntities");
            Sql("Drop Index IX_EntityType on DomainUse");
            DropForeignKey("dbo.UserDomainData", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserDomainData", "ID", "dbo.DomainData");
            DropForeignKey("dbo.OrganizationDomainData", "OrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationDomainData", "ID", "dbo.DomainData");
            DropForeignKey("dbo.DataSourceDomainData", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.DataSourceDomainData", "ID", "dbo.DomainData");
            DropForeignKey("dbo.DomainReference", "ParentDomainReferenceID", "dbo.DomainReference");
            DropForeignKey("dbo.Domain", "ParentDomainID", "dbo.Domain");
            DropForeignKey("dbo.Organizations", "ParentOrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.Users", "OrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.Users", "NetworkID", "dbo.Networks");
            DropForeignKey("dbo.Organizations", "NetworkID", "dbo.Networks");
            DropForeignKey("dbo.NetworkEntities", "NetworkID", "dbo.Networks");
            DropForeignKey("dbo.DataSources", "OrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.DomainData", "DomainUseID", "dbo.DomainUse");
            DropForeignKey("dbo.DomainData", "DomainReferenceID", "dbo.DomainReference");
            DropForeignKey("dbo.DomainUse", "DomainID", "dbo.Domain");
            DropForeignKey("dbo.DomainReference", "DomainID", "dbo.Domain");
            DropIndex("dbo.UserDomainData", new[] { "UserID" });
            DropIndex("dbo.UserDomainData", new[] { "ID" });
            DropIndex("dbo.OrganizationDomainData", new[] { "OrganizationID" });
            DropIndex("dbo.OrganizationDomainData", new[] { "ID" });
            DropIndex("dbo.DataSourceDomainData", new[] { "DataSourceID" });
            DropIndex("dbo.DataSourceDomainData", new[] { "ID" });
            DropIndex("dbo.Users", new[] { "OrganizationID" });
            DropIndex("dbo.Users", new[] { "NetworkID" });
            DropIndex("dbo.NetworkEntities", new[] { "NetworkID" });
            DropIndex("dbo.Organizations", new[] { "ParentOrganizationID" });
            DropIndex("dbo.Organizations", new[] { "NetworkID" });
            DropIndex("dbo.DomainUse", new[] { "DomainID" });
            DropIndex("dbo.Domain", new[] { "ParentDomainID" });
            DropIndex("dbo.DomainReference", new[] { "ParentDomainReferenceID" });
            DropIndex("dbo.DomainReference", new[] { "DomainID" });
            DropIndex("dbo.DomainData", new[] { "DomainReferenceID" });
            DropIndex("dbo.DomainData", new[] { "DomainUseID" });
            DropIndex("dbo.DataSources", new[] { "OrganizationID" });
            DropTable("dbo.UserDomainData");
            DropTable("dbo.OrganizationDomainData");
            DropTable("dbo.DataSourceDomainData");
            DropTable("dbo.Users");
            DropTable("dbo.NetworkEntities");
            DropTable("dbo.Networks");
            DropTable("dbo.Organizations");
            DropTable("dbo.DomainUse");
            DropTable("dbo.Domain");
            DropTable("dbo.DomainReference");
            DropTable("dbo.DomainData");
            DropTable("dbo.DataSources");
            Sql("DROP TABLE EntityTypes");
        }
    }
}
