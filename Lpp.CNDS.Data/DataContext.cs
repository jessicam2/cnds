using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<NetworkEntity> NetworkEntities { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<DomainData> DomainDatas { get; set; }
        public DbSet<DomainReference> DomainReferences { get; set; }
        public DbSet<DomainUse> DomainUses { get; set; }
        public DbSet<DataSource> DataSources { get; set;}
        public DbSet<DomainAccess> DomainAccess { get; set; }
        public DbSet<SecurityGroup> SecurityGroups { get; set; }
        public DbSet<SecurityGroupUser> SecurityGroupUsers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AclGlobal> GlobalAcls { get; set; }
        public DbSet<NetworkRequestTypeMapping> NetworkRequestTypeMappings { get; set; }
        public DbSet<NetworkRequestTypeDefinition> NetworkRequestTypeDefinitions { get; set; }
        public DbSet<NetworkRequestTypeMappingRoutes> NetworkRequestTypeMappingRoutes { get; set; }
        public DbSet<Adapter> Adapters { get; set; }
        public DbSet<Requests.NetworkRequest> NetworkRequests { get; set; }
        public DbSet<Requests.NetworkRequestParticipant> NetworkRequestParticipants { get; set; }
        public DbSet<Requests.NetworkRequestRoute> NetworkRequestRoutes { get; set; }
        public DbSet<Requests.NetworkRequestResponse> NetworkRequestResponses { get; set; }
        public DbSet<Requests.NetworkRequestDocument> NetworkRequestDocuments { get; set; }

        public DataContext()
        {
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = true;
            this.DisableExtendedValidationAndSave = false;
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 999;
        }

        public DataContext(string ConnectionString)
        {
            Database.Connection.ConnectionString = ConnectionString;
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = true;
            this.DisableExtendedValidationAndSave = false;
            
        }

        public DataContext(bool EnableLazyLoading) : this()
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        public bool DisableExtendedValidationAndSave { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<DataSourceDomainData>().ToTable("DataSourceDomainData");
            modelBuilder.Entity<OrganizationDomainData>().ToTable("OrganizationDomainData");

            modelBuilder.Entity<UserDomainAccess>().Map(u => {
                u.MapInheritedProperties();
                u.ToTable("UserDomainAccess");
            });
            modelBuilder.Entity<OrganizationDomainAccess>().Map(u => {
                u.MapInheritedProperties();
                u.ToTable("OrganizationDomainAccess");
            });
            modelBuilder.Entity<DataSourceDomainAccess>().Map(u => {
                u.MapInheritedProperties();
                u.ToTable("DataSourceDomainAccess");
            });

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(
                type => type.BaseType != null &&
                    type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            foreach (object configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            {
                modelBuilder.Configurations.Add((dynamic)configurationInstance);
            }

            modelBuilder.Entity<DomainUse>().HasMany<DomainAccess>(du => du.AccessVisibility).WithRequired(da => da.DomainUse).HasForeignKey(da => da.DomainUseID).WillCascadeOnDelete(true);

            modelBuilder.Entity<DataSource>().HasMany(dm => dm.DomainData).WithRequired(m => m.DataSource).HasForeignKey(m => m.DataSourceID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Organization>().HasMany(o => o.DomainData).WithRequired(m => m.Organization).HasForeignKey(m => m.OrganizationID).WillCascadeOnDelete(true);
            modelBuilder.Entity<User>().HasMany(u => u.DomainData).WithRequired(m => m.User).HasForeignKey(m => m.UserID).WillCascadeOnDelete(true);

            modelBuilder.Entity<DataSource>().HasMany(dm => dm.DomainAccess).WithRequired(m => m.DataSource).HasForeignKey(m => m.DataSourceID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Organization>().HasMany(o => o.DomainAccess).WithRequired(m => m.Organization).HasForeignKey(m => m.OrganizationID).WillCascadeOnDelete(true);
            modelBuilder.Entity<User>().HasMany(u => u.DomainAccess).WithRequired(m => m.User).HasForeignKey(m => m.UserID).WillCascadeOnDelete(true);

            modelBuilder.Entity<NetworkRequestTypeMapping>().ToTable("NetworkRequestTypeMappings");
            modelBuilder.Entity<NetworkRequestTypeDefinition>().ToTable("NetworkRequestTypeDefinitions");
            modelBuilder.Entity<NetworkRequestTypeMappingRoutes>().ToTable("NetworkRequestTypeMappingRoutes");

            modelBuilder.Entity<NetworkRequestTypeDefinition>().HasMany(rtd => rtd.NetworkRequestTypeMappingRoutes).WithRequired(rtm => rtm.RequestTypeDefinition).HasForeignKey(t => t.RequestTypeDefinitionID).WillCascadeOnDelete(true);
            modelBuilder.Entity<NetworkRequestTypeMapping>().HasMany(rtm => rtm.NetworkRoutes).WithRequired(rtm => rtm.RequestTypeMapping).HasForeignKey(t => t.RequestTypeMappingID).WillCascadeOnDelete(true);


            modelBuilder.Entity<Requests.NetworkRequest>().ToTable("NetworkRequests");
            modelBuilder.Entity<Requests.NetworkRequestParticipant>().ToTable("NetworkRequestParticipants");
            modelBuilder.Entity<Requests.NetworkRequestRoute>().ToTable("NetworkRequestRoutes");
            modelBuilder.Entity<Requests.NetworkRequestResponse>().ToTable("NetworkRequestResponses");
            modelBuilder.Entity<Requests.NetworkRequestDocument>().ToTable("NetworkRequestDocuments");

            modelBuilder.Entity<Requests.NetworkRequest>().HasMany(r => r.Participants).WithRequired(p => p.NetworkRequest).HasForeignKey(r => r.NetworkRequestID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Requests.NetworkRequest>().HasRequired(r => r.Network);
            modelBuilder.Entity<Network>().HasMany(n => n.SourceRequests).WithRequired(r => r.Network).HasForeignKey(r => r.NetworkID).WillCascadeOnDelete(true);

            modelBuilder.Entity<Requests.NetworkRequestParticipant>().HasMany(p => p.Routes).WithRequired(rt => rt.Participant).HasForeignKey(rt => rt.ParticipantID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Requests.NetworkRequestParticipant>().HasRequired(p => p.Network);
            modelBuilder.Entity<Network>().HasMany(n => n.ParticipantRequests).WithRequired(r => r.Network).HasForeignKey(rt => rt.NetworkID).WillCascadeOnDelete(false);

            modelBuilder.Entity<Requests.NetworkRequestRoute>().HasMany(rt => rt.Responses).WithRequired(rsp => rsp.NetworkRequestRoute).HasForeignKey(rsp => rsp.NetworkRequestRouteID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Requests.NetworkRequestRoute>().HasRequired(rt => rt.Participant);
            modelBuilder.Entity<Requests.NetworkRequestRoute>().HasRequired(rt => rt.DataSource);
            modelBuilder.Entity<DataSource>().HasMany(ds => ds.NetworkRequestRoutes).WithRequired(rt => rt.DataSource).HasForeignKey(rt => rt.DataSourceID).WillCascadeOnDelete(true);

            modelBuilder.Entity<Requests.NetworkRequestResponse>().HasMany(rsp => rsp.Documents).WithRequired(d => d.Response).HasForeignKey(d => d.ResponseID).WillCascadeOnDelete(true);
        }
    }
}
