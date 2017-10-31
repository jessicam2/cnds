namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTriggers : DbMigration
    {
        public override void Up()
        {
            Sql(@"Create TRIGGER [dbo].[OrganiztionDomainData_Delete] 
	                ON  [dbo].[OrganizationDomainData] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainData where ID IN (SELECT deleted.ID FROM deleted)
                END");
            Sql(@"Create TRIGGER [dbo].[DataSourceDomainData_Delete] 
	                ON  [dbo].[DataSourceDomainData] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainData where ID IN (SELECT deleted.ID FROM deleted)
                END");
            Sql(@"Create TRIGGER [dbo].[UserDomainData_Delete] 
	                ON  [dbo].[UserDomainData] 
                   For Delete
                AS 
                BEGIN
	                Delete from DomainData where ID IN (SELECT deleted.ID FROM deleted)
                END");
        }
        
        public override void Down()
        {
        }
    }
}
