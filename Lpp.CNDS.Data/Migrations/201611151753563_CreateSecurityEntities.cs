namespace Lpp.CNDS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSecurityEntities : DbMigration
    {
        public override void Up()
        {

            CreateTable(
                "dbo.SecurityGroups",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 255),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.SecurityGroupUsers",
                c => new
                    {
                        SecurityGroupID = c.Guid(nullable: false),
                        UserID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SecurityGroupID, t.UserID })
                .ForeignKey("dbo.SecurityGroups", t => t.SecurityGroupID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.SecurityGroupID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Name = c.String(nullable:false, maxLength:255),
                        Description = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.AclGlobal",
                c => new
                {
                    SecurityGroupID = c.Guid(nullable: false),
                    PermissionID = c.Guid(nullable: false),
                    Allowed = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => new { t.SecurityGroupID, t.PermissionID })
                .ForeignKey("dbo.Permissions", t => t.PermissionID, cascadeDelete: true)
                .ForeignKey("dbo.SecurityGroups", t => t.SecurityGroupID, cascadeDelete: true)
                .Index(t => t.SecurityGroupID)
                .Index(t => t.PermissionID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecurityGroupUsers", "UserID", "dbo.Users");
            DropForeignKey("dbo.SecurityGroupUsers", "SecurityGroupID", "dbo.SecurityGroups");
            DropForeignKey("dbo.AclGlobal", "SecurityGroupID", "dbo.SecurityGroups");
            DropForeignKey("dbo.AclGlobal", "PermissionID", "dbo.Permissions");
            DropIndex("dbo.Permissions", new[] { "Name" });
            DropIndex("dbo.AclGlobal", new[] { "PermissionID" });
            DropIndex("dbo.AclGlobal", new[] { "SecurityGroupID" });
            DropIndex("dbo.SecurityGroups", new[] { "Name" });
            DropIndex("dbo.SecurityGroupUsers", new[] { "UserID" });
            DropIndex("dbo.SecurityGroupUsers", new[] { "SecurityGroupID" });
            DropTable("dbo.AclGlobal");
            DropTable("dbo.Permissions");
            DropTable("dbo.SecurityGroupUsers");
            DropTable("dbo.SecurityGroups");            
        }
    }
}
