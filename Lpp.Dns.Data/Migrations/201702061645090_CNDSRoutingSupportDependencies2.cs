namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CNDSRoutingSupportDependencies2 : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE DataMartTypes SET DatamartType = 'CNDS Network Route' WHERE ID = '5B060001-2A5F-4243-980F-A70E011E7D5F'");
            Sql("IF NOT EXISTS(SELECT NULL FROM Organizations WHERE ID = '39040001-38AC-49E9-8FAC-A7120111F82E') INSERT INTO Organizations (ID, [Name], [Acronym], OrganizationType) VALUES ('39040001-38AC-49E9-8FAC-A7120111F82E', 'CNDS Network Route Owner', 'CNDS-NRO', 1)");
        }
        
        public override void Down()
        {
            Sql("UPDATE DataMartTypes SET DatamartType = 'CNDS' WHERE ID = '5B060001-2A5F-4243-980F-A70E011E7D5F'");
            Sql("DELETE FROM Organizations WHERE ID = '39040001-38AC-49E9-8FAC-A7120111F82E'");
        }
    }
}
