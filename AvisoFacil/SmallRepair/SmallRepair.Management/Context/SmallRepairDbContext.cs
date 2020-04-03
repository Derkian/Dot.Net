using Microsoft.EntityFrameworkCore;
using SmallRepair.Management.Model;


namespace SmallRepair.Management.Context
{
    public class SmallRepairDbContext : DbContext
    {
        #region Construtor
        public SmallRepairDbContext(DbContextOptions<SmallRepairDbContext> options)
            : base(options)
        {
            //this.Database.EnsureCreated();
        } 
        #endregion

        #region DbSets
        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Baremo> Baremos { get; set; }

        public DbSet<BaremoTime> BaremoTimes { get; set; }

        public DbSet<Company> Customers { get; set; }

        public DbSet<ServiceValue> ServiceValues { get; set; }

        public DbSet<AdditionalService> AdditionalServices { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Assessment> Assessments { get; set; }

        public DbSet<AssessmentAdditionalService> AssessmentAdditionalServices { get; set; }

        public DbSet<AssessmentServiceValue> AssessmentServiceValues { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ReportTemplate> ReportTemplates { get; set; }

        public DbSet<CompanyReportTemplate> CompanyReportTemplates { get; set; }

        public DbSet<AssessmentVersion> AssessmentHistory { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Configuration.BaremoConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.BaremoTimeConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.CatalogConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.AdditionalServiceConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.AssessmentConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.AssessmentAdditionalServiceConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.AssessmentServiceValueConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.PartConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.ServiceValueConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.ReportTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.CompanyReportTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.AssessmentVersionConfiguration());
        }
    }
}
