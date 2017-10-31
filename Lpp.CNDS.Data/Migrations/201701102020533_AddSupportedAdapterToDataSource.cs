namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupportedAdapterToDataSource : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adapters",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.DataSources", "AdapterSupportedID", c => c.Guid());
            CreateIndex("dbo.DataSources", "AdapterSupportedID");
            AddForeignKey("dbo.DataSources", "AdapterSupportedID", "dbo.Adapters", "ID");

            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '321ADAA1-A350-4DD0-93DE-5DE658A507DF') INSERT INTO Adapters(ID, [Name]) VALUES ('321ADAA1-A350-4DD0-93DE-5DE658A507DF', 'Data Checker QE')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '1B0FFD4C-3EEF-479D-A5C4-69D8BA0D0154') INSERT INTO Adapters(ID, [Name]) VALUES ('1B0FFD4C-3EEF-479D-A5C4-69D8BA0D0154', 'Modular Program')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '4C8A25DC-6816-4202-88F4-6D17E72A43BC') INSERT INTO Adapters(ID, [Name]) VALUES ('4C8A25DC-6816-4202-88F4-6D17E72A43BC', 'Distributed Regression')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = 'CC14E6A2-99A8-4EF8-B4CB-779A7B93A7BB') INSERT INTO Adapters(ID, [Name]) VALUES ('CC14E6A2-99A8-4EF8-B4CB-779A7B93A7BB', 'Summary Tables')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '7C69584A-5602-4FC0-9F3F-A27F329B1113') INSERT INTO Adapters(ID, [Name]) VALUES ('7C69584A-5602-4FC0-9F3F-A27F329B1113', 'ESP Request')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '455C772A-DF9B-4C6B-A6B0-D4FD4DD98488') INSERT INTO Adapters(ID, [Name]) VALUES ('455C772A-DF9B-4C6B-A6B0-D4FD4DD98488', 'Query Composer')");
            Sql("IF NOT EXISTS(SELECT NULL FROM Adapters WHERE ID = '85EE982E-F017-4BC4-9ACD-EE6EE55D2446') INSERT INTO Adapters(ID, [Name]) VALUES ('85EE982E-F017-4BC4-9ACD-EE6EE55D2446', 'PCORnet CDM')");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataSources", "AdapterSupportedID", "dbo.Adapters");
            DropIndex("dbo.DataSources", new[] { "AdapterSupportedID" });
            DropColumn("dbo.DataSources", "AdapterSupportedID");
            DropTable("dbo.Adapters");
        }
    }
}
