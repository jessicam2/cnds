namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDomainAccessTables : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DomainAccess] DROP CONSTRAINT [FK_dbo.DomainAccess_dbo.AccessTypes_ID]");
            Sql("Drop Index IX_AccessType on DomainAccess");
            DropForeignKey("dbo.UserDomainAccess", "ID", "dbo.DomainAccess");
            DropForeignKey("dbo.OrganizationDomainAccess", "ID", "dbo.DomainAccess");
            DropForeignKey("dbo.DataSourceDomainAccess", "ID", "dbo.DomainAccess");
            DropIndex("dbo.DomainAccess", new[] { "DomainUseID" });
            DropIndex("dbo.UserDomainAccess", new[] { "ID" });
            DropIndex("dbo.OrganizationDomainAccess", new[] { "ID" });
            DropIndex("dbo.DataSourceDomainAccess", new[] { "ID" });
            AddColumn("dbo.DataSourceDomainAccess", "AccessType", c => c.Int(nullable: false));
            AddColumn("dbo.DataSourceDomainAccess", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.DataSourceDomainAccess", "DomainUseID", c => c.Guid(nullable: false));
            AddColumn("dbo.OrganizationDomainAccess", "AccessType", c => c.Int(nullable: false));
            AddColumn("dbo.OrganizationDomainAccess", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.OrganizationDomainAccess", "DomainUseID", c => c.Guid(nullable: false));
            AddColumn("dbo.UserDomainAccess", "AccessType", c => c.Int(nullable: false));
            AddColumn("dbo.UserDomainAccess", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.UserDomainAccess", "DomainUseID", c => c.Guid(nullable: false));
            CreateIndex("dbo.UserDomainAccess", "DomainUseID");
            CreateIndex("dbo.OrganizationDomainAccess", "DomainUseID");
            CreateIndex("dbo.DataSourceDomainAccess", "DomainUseID");
            Sql("truncate Table UserDomainAccess");
            Sql("Truncate Table OrganizationDomainAccess");
            Sql("Truncate Table DataSourceDomainAccess");
            DropTable("dbo.DomainAccess");
            Sql(@"ALTER TABLE [dbo].[UserDomainAccess]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserDomainAccess_dbo.AccessTypes_ID] FOREIGN KEY([AccessType])
                    REFERENCES [dbo].[AccessTypes] ([ID])
                    ON DELETE CASCADE");

            Sql(@"CREATE NONCLUSTERED INDEX [IX_AccessType] ON [dbo].[UserDomainAccess]
                    (
                        [AccessType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");


            Sql(@"ALTER TABLE [dbo].[OrganizationDomainAccess]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrganizationDomainAccess_dbo.AccessTypes_ID] FOREIGN KEY([AccessType])
                    REFERENCES [dbo].[AccessTypes] ([ID])
                    ON DELETE CASCADE");

            Sql(@"CREATE NONCLUSTERED INDEX [IX_AccessType] ON [dbo].[OrganizationDomainAccess]
                    (
                        [AccessType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");


            Sql(@"ALTER TABLE [dbo].[DataSourceDomainAccess]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DataSourceDomainAccess_dbo.AccessTypes_ID] FOREIGN KEY([AccessType])
                    REFERENCES [dbo].[AccessTypes] ([ID])
                    ON DELETE CASCADE");

            Sql(@"CREATE NONCLUSTERED INDEX [IX_AccessType] ON [dbo].[DataSourceDomainAccess]
                    (
                        [AccessType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");

            Sql("Drop Trigger DataSourceDomainAccess_Delete");
            Sql("Drop Trigger OrganizationDomainAccess_Delete");
            Sql("Drop Trigger UserDomainAccess_Delete");

        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DataSourceDomainAccess] DROP CONSTRAINT [FK_dbo.DataSourceDomainAccess_dbo.AccessTypes_ID]");
            Sql("Drop Index IX_AccessType on DataSourceDomainAccess");
            Sql("ALTER TABLE [dbo].[OrganizationDomainAccess] DROP CONSTRAINT [FK_dbo.OrganizationDomainAccess_dbo.AccessTypes_ID]");
            Sql("Drop Index IX_AccessType on OrganizationDomainAccess");
            Sql("ALTER TABLE [dbo].[UserDomainAccess] DROP CONSTRAINT [FK_dbo.UserDomainAccess_dbo.AccessTypes_ID]");
            Sql("Drop Index IX_AccessType on UserDomainAccess");
            CreateTable(
                "dbo.DomainAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        DomainUseID = c.Guid(nullable: false),
                        AccessType = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);

            Sql(@"ALTER TABLE [dbo].[DomainAccess]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DomainAccess_dbo.AccessTypes_ID] FOREIGN KEY([AccessType])
                    REFERENCES [dbo].[AccessTypes] ([ID])
                    ON DELETE CASCADE");

            Sql(@"CREATE NONCLUSTERED INDEX [IX_AccessType] ON [dbo].[DomainAccess]
                    (
                        [AccessType] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");

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

            DropIndex("dbo.DataSourceDomainAccess", new[] { "DomainUseID" });
            DropIndex("dbo.OrganizationDomainAccess", new[] { "DomainUseID" });
            DropIndex("dbo.UserDomainAccess", new[] { "DomainUseID" });
            DropColumn("dbo.UserDomainAccess", "DomainUseID");
            DropColumn("dbo.UserDomainAccess", "Timestamp");
            DropColumn("dbo.UserDomainAccess", "AccessType");
            DropColumn("dbo.OrganizationDomainAccess", "DomainUseID");
            DropColumn("dbo.OrganizationDomainAccess", "Timestamp");
            DropColumn("dbo.OrganizationDomainAccess", "AccessType");
            DropColumn("dbo.DataSourceDomainAccess", "DomainUseID");
            DropColumn("dbo.DataSourceDomainAccess", "Timestamp");
            DropColumn("dbo.DataSourceDomainAccess", "AccessType");
            CreateIndex("dbo.DataSourceDomainAccess", "ID");
            CreateIndex("dbo.OrganizationDomainAccess", "ID");
            CreateIndex("dbo.UserDomainAccess", "ID");
            CreateIndex("dbo.DomainAccess", "DomainUseID");
            AddForeignKey("dbo.DataSourceDomainAccess", "ID", "dbo.DomainAccess", "ID");
            AddForeignKey("dbo.OrganizationDomainAccess", "ID", "dbo.DomainAccess", "ID");
            AddForeignKey("dbo.UserDomainAccess", "ID", "dbo.DomainAccess", "ID");
        }
    }
}
