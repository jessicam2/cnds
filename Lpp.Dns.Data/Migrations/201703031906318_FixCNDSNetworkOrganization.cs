namespace Lpp.Dns.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCNDSNetworkOrganization : DbMigration
    {
        public override void Up()
        {
            Sql(@"Update Organizations Set 
                    isDeleted = 0,
                    InpatientClaims = 0,
                    OutpatientClaims = 0,
                    OutpatientPharmacyClaims = 0,
                    ObservationalParticipation = 0,
                    ProspectiveTrials = 0,
                    EnrollmentClaims = 0,
                    DemographicsClaims = 0,
                    LaboratoryResultsClaims = 0,
                    VitalSignsClaims = 0
                    where ID = '39040001-38AC-49E9-8FAC-A7120111F82E'");
        }
        
        public override void Down()
        {
            Sql(@"Update Organizations Set 
                    isDeleted = NULL,
                    InpatientClaims = NULL,
                    OutpatientClaims = NULL,
                    OutpatientPharmacyClaims = NULL,
                    ObservationalParticipation = NULL,
                    ProspectiveTrials = NULL,
                    EnrollmentClaims = NULL,
                    DemographicsClaims = NULL,
                    LaboratoryResultsClaims = NULL,
                    VitalSignsClaims = NULL
                    where ID = '39040001-38AC-49E9-8FAC-A7120111F82E'");
        }
    }
}
