namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterPermissions : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('4EB90001-6F08-46E3-911D-A6BF012EBFB8', 'Manage Metadata', 'Governs the ability to perform all functions related to metadata management including adding/ editing/removing domains and assigning domains to entity types.')");
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('E3410001-B6F4-4C51-B269-A6BF012EC64D', 'Manage CNDS Access', 'Governs the ability to set CNDS permission settings and assign CNDS security groups to all users. Users without this permission cannot see the \"Permissions\" option in the CNDS menu.')");
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('E2A20001-1B7F-463E-8990-A6BF012ECC72', 'Create CNDS Security Group', 'Governs the ability to create a CNDS security group.')");
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('10CF0001-451E-44ED-8388-A6BF012ED2D6', 'Edit CNDS Security Group', 'Governs the ability to edit the name of a CNDS security group.')");
            Sql("INSERT INTO dbo.[Permissions] (ID, [Name], [Description]) VALUES ('25D50001-03BD-4EDE-9E6F-A6BF012ED91E', 'Delete CNDS Security Group', 'Governs the ability to delete a CNDS security group. Deleting will remove the group from the CNDS database and all profiles to which its assigned.')");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM dbo.[Permissions]");
        }
    }
}
