namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCNDSRoutingSupportDependencies : DbMigration
    {
        static readonly Guid DataMartTypeID = new Guid("5B060001-2A5F-4243-980F-A70E011E7D5F");

        public override void Up()
        {
            AddColumn("dbo.Organizations", "OrganizationType", c => c.Int(nullable: false, defaultValue: 0));
            Sql("IF(NOT EXISTS(SELECT NULL FROM DataMartTypes WHERE ID = '" + DataMartTypeID.ToString("D") + "')) INSERT INTO DataMartTypes (ID, [DatamartType]) VALUES ('" + DataMartTypeID.ToString("D") + "', 'CNDS')");
        }
        
        public override void Down()
        {
            //cleanup any routes, datamarts, or organizations that have been imported from CNDS
            Sql("DELETE FROM RequestDataMarts WHERE EXISTS(SELECT NULL FROM DataMarts dm WHERE dm.DataMartTypeID = '" + DataMartTypeID.ToString("D") + "' AND dm.ID = RequestDataMarts.DataMartID)");
            Sql("DELETE FROM DataMarts WHERE DataMartTypeID = '" + DataMartTypeID.ToString("D") + "'");
            Sql("DELETE FROM DataMartTypes WHERE ID = '" + DataMartTypeID.ToString("D") + "'");
            Sql("DELETE FROM Organizations WHERE OrganizationType != 0");
            DropColumn("dbo.Organizations", "OrganizationType");
        }
    }
}
