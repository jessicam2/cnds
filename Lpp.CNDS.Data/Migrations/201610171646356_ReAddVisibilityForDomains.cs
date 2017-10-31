namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddVisibilityForDomains : DbMigration
    {
        public override void Up()
        {
            Sql(@"DECLARE @tempOrgs AS Table(tempID uniqueidentifier, OrganizationID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempOrgs select NEWID(), o.ID, du.ID, 0 from DomainUse as du join Organizations as o on 1=1 Where du.EntityType = 0

INSERT INTO OrganizationDomainAccess(ID,OrganizationID, DomainUseID, AccessType) select tempID,OrganizationID, DomainUseID, AccessType from @tempOrgs



DECLARE @tempDSs AS Table(tempID uniqueidentifier, DataSourceID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempDSs select NEWID(), o.ID, du.ID, 0 from DomainUse as du join DataSources as o on 1=1 Where du.EntityType = 2

INSERT INTO DataSourceDomainAccess(ID,DataSourceID, DomainUseID, AccessType) select tempID,DataSourceID, DomainUseID, AccessType from @tempDSs


DECLARE @tempUsers AS Table(tempID uniqueidentifier, UserID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempUsers select NEWID(), o.ID, du.ID, 0 from DomainUse as du join Users as o on 1=1 Where du.EntityType = 1

INSERT INTO UserDomainAccess(ID,UserID, DomainUseID, AccessType) select tempID,UserID, DomainUseID, AccessType from @tempUsers


");
        }
        
        public override void Down()
        {
            Sql("truncate table UserDomainAccess");
            Sql("truncate table DataSourceDomainAccess");
            Sql("truncate table OrganizationDomainAccess");
        }
    }
}
