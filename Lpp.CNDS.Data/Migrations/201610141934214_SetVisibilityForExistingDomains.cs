namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetVisibilityForExistingDomains : DbMigration
    {
        public override void Up()
        {

            Sql(@"DECLARE @tempOrgs AS Table(tempID uniqueidentifier, OrganizationID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempOrgs select NEWID(), o.ID, du.ID, 0 from DomainUse as du join Organizations as o on 1=1 Where du.EntityType = 0

INSERT INTO DomainAccess(ID, DomainUseID, AccessType) select tempID, DomainUseID, AccessType from @tempOrgs

INSERT INTO OrganizationDomainAccess(ID, OrganizationID) select tempID, OrganizationID from @tempOrgs




DECLARE @tempDSs AS Table(tempID uniqueidentifier, DataSourceID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempDSs select NEWID(), o.ID, du.ID, 0 from DomainUse as du join DataSources as o on 1=1 Where du.EntityType = 2

INSERT INTO DomainAccess(ID, DomainUseID, AccessType) select tempID, DomainUseID, AccessType from @tempDSs

INSERT INTO DataSourceDomainAccess(ID, DataSourceID) select tempID, DataSourceID from @tempDSs



DECLARE @tempUsers AS Table(tempID uniqueidentifier, UserID uniqueidentifier, DomainUseID uniqueidentifier, AccessType int)

INSERT INTO @tempUsers select NEWID(), o.ID, du.ID, 0 from DomainUse as du join Users as o on 1=1 Where du.EntityType = 1

INSERT INTO DomainAccess(ID, DomainUseID, AccessType) select tempID, DomainUseID, AccessType from @tempUsers

INSERT INTO UserDomainAccess(ID, UserID) select tempID, UserID from @tempUsers

");
        }
        
        public override void Down()
        {
            Sql("delete from DataSourceDomainAccess");
            Sql("delete from UserDomainAccess");
            Sql("delete from OrganizationDomainAccess");
        }
    }
}
