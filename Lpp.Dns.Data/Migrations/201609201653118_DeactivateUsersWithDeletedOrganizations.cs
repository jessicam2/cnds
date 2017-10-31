namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeactivateUsersWithDeletedOrganizations : DbMigration
    {
        public override void Up()
        {
            //deactivate any users that have had their organizations deleted and the cascade to delete associated users and data sources did not happen.
            Sql("UPDATE Users SET OrganizationID = NULL, isActive = 0, ActivatedOn = null WHERE EXISTS(SELECT NULL FROM Organizations o WHERE o.ID = OrganizationID AND o.IsDeleted = 1) AND isDeleted = 0");

            //mark any datamarts as deleted that have had their organizations deleted and the cascade to delete associated users and data sources did not happen.
            Sql("UPDATE DataMarts SET isDeleted = 1 WHERE EXISTS(SELECT NULL FROM Organizations o WHERE o.ID = OrganizationID AND o.IsDeleted = 1) AND isDeleted = 0");
        }
        
        public override void Down()
        {
        }
    }
}
