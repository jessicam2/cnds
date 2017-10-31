namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivityIdentifiersToWorkflow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Workflows", "RequestReviewWorkflowActivityID", c => c.Guid(nullable: true));
            AddColumn("dbo.Workflows", "CompleteDistributionWorkflowActivityID", c => c.Guid(nullable: true));
            CreateIndex("dbo.Workflows", "RequestReviewWorkflowActivityID");
            CreateIndex("dbo.Workflows", "CompleteDistributionWorkflowActivityID");
            AddForeignKey("dbo.Workflows", "CompleteDistributionWorkflowActivityID", "dbo.WorkflowActivities", "ID");
            AddForeignKey("dbo.Workflows", "RequestReviewWorkflowActivityID", "dbo.WorkflowActivities", "ID");

            //update Default workflow
            Sql("UPDATE Workflows SET RequestReviewWorkflowActivityID = '73740001-A942-47B0-BF6E-A3B600E7D9EC', CompleteDistributionWorkflowActivityID = 'ACBA0001-0CE4-4C00-8DD3-A3B5013A3344' WHERE ID = 'F64E0001-4F9A-49F0-BF75-A3B501396946'");

            //update Summary Tables workflow
            Sql("UPDATE Workflows SET RequestReviewWorkflowActivityID = 'CC1BCADD-4487-47C7-BDCA-1010F2C68FE0', CompleteDistributionWorkflowActivityID = '6BD20AD7-502C-4D8E-A0BB-A9A5CE388C4B' WHERE ID = '7A82FE34-BE6B-40E5-AABF-B40A3DBE73B8'");

            //update DataChecker workflow
            Sql("UPDATE Workflows SET RequestReviewWorkflowActivityID = '3FFBCA99-5801-4045-9FB4-072136A845FC', CompleteDistributionWorkflowActivityID = 'ACBA0001-0CE4-4C00-8DD3-A3B5013A3344' WHERE ID = '942A2B39-0E9C-4ECE-9E2C-C85DF0F42663'");

            //update Simple Modular workflow
            Sql("UPDATE Workflows SET CompleteDistributionWorkflowActivityID = 'D0E659B8-1155-4F44-9728-B4B6EA4D4D55' WHERE ID = '931C0001-787C-464D-A90F-A64F00FB23E7'");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Workflows", "RequestReviewWorkflowActivityID", "dbo.WorkflowActivities");
            DropForeignKey("dbo.Workflows", "CompleteDistributionWorkflowActivityID", "dbo.WorkflowActivities");
            DropIndex("dbo.Workflows", new[] { "CompleteDistributionWorkflowActivityID" });
            DropIndex("dbo.Workflows", new[] { "RequestReviewWorkflowActivityID" });
            DropColumn("dbo.Workflows", "CompleteDistributionWorkflowActivityID");
            DropColumn("dbo.Workflows", "RequestReviewWorkflowActivityID");
        }
    }
}
