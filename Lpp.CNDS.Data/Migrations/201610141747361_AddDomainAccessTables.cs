namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDomainAccessTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomainAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        DomainUseID = c.Guid(nullable: false),
                        AccessType = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainUse", t => t.DomainUseID, cascadeDelete: true)
                .Index(t => t.DomainUseID);
            
            CreateTable(
                "dbo.UserDomainAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        UserID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainAccess", t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.OrganizationDomainAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        OrganizationID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainAccess", t => t.ID)
                .ForeignKey("dbo.Organizations", t => t.OrganizationID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.OrganizationID);
            
            CreateTable(
                "dbo.DataSourceDomainAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        DataSourceID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DomainAccess", t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.DataSourceID);
            Sql(@"CREATE TABLE [dbo].[AccessTypes](
	                [ID] [Int] NOT NULL,
	                [Name] [nvarchar](50) NOT NULL,
                    CONSTRAINT [PK_AccessTypes] PRIMARY KEY CLUSTERED 
                (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]");


            Sql(@"ALTER TABLE [dbo].[DomainAccess]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DomainAccess_dbo.AccessTypes_ID] FOREIGN KEY([AccessType])
                    REFERENCES [dbo].[AccessTypes] ([ID])
                    ON DELETE CASCADE");

            Sql(@"CREATE NONCLUSTERED INDEX [IX_AccessType] ON [dbo].[DomainAccess]
                    (
                        [AccessType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");

            Sql("INSERT INTO AccessTypes(ID,Name)VALUES(0,'No One')");
            Sql("INSERT INTO AccessTypes(ID,Name)VALUES(100,'My Network Members')");
            Sql("INSERT INTO AccessTypes(ID,Name)VALUES(1000,'All PMN Members')");
            Sql("INSERT INTO AccessTypes(ID,Name)VALUES(10000,'All PMN and CNDS Members')");
            Sql("INSERT INTO AccessTypes(ID,Name)VALUES(100000,'Anyone')");

            Sql(@"CREATE TRIGGER [dbo].[DataSourceDomainAccess_Delete] 
	                ON  [dbo].[DataSourceDomainAccess] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainAccess where ID IN (SELECT deleted.ID FROM deleted)
                END");
            Sql(@"CREATE TRIGGER [dbo].[OrganizationDomainAccess_Delete] 
	                ON  [dbo].[OrganizationDomainAccess] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainAccess where ID IN (SELECT deleted.ID FROM deleted)
                END");
            Sql(@"CREATE TRIGGER [dbo].[UserDomainAccess_Delete] 
	                ON  [dbo].[UserDomainAccess] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainAccess where ID IN (SELECT deleted.ID FROM deleted)
                END");

        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DomainAccess] DROP CONSTRAINT [FK_dbo.DomainAccess_dbo.AccessTypes_ID]");
            Sql("Drop Index IX_AccessType on DomainAccess");
            DropForeignKey("dbo.DataSourceDomainAccess", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.DataSourceDomainAccess", "ID", "dbo.DomainAccess");
            DropForeignKey("dbo.OrganizationDomainAccess", "OrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationDomainAccess", "ID", "dbo.DomainAccess");
            DropForeignKey("dbo.UserDomainAccess", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserDomainAccess", "ID", "dbo.DomainAccess");
            DropForeignKey("dbo.DomainAccess", "DomainUseID", "dbo.DomainUse");
            DropIndex("dbo.DataSourceDomainAccess", new[] { "DataSourceID" });
            DropIndex("dbo.DataSourceDomainAccess", new[] { "ID" });
            DropIndex("dbo.OrganizationDomainAccess", new[] { "OrganizationID" });
            DropIndex("dbo.OrganizationDomainAccess", new[] { "ID" });
            DropIndex("dbo.UserDomainAccess", new[] { "UserID" });
            DropIndex("dbo.UserDomainAccess", new[] { "ID" });
            DropIndex("dbo.DomainAccess", new[] { "DomainUseID" });
            DropTable("dbo.DataSourceDomainAccess");
            DropTable("dbo.OrganizationDomainAccess");
            DropTable("dbo.UserDomainAccess");
            DropTable("dbo.DomainAccess");
            Sql("Drop Table AccessTypes");
        }
    }
}
