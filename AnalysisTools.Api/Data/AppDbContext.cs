using analysistools.api.Models.Authentication;
using analysistools.api.Models.Continental;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.PRODUCTS;
using analysistools.api.Models.IDR;
using analysistools.api.Models.Optical;
using analysistools.api.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace analysistools.api.Data
{
    /// <summary>
    /// This class allows to query data from an sqlite database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Registered tables.
        public DbSet<User> Users { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<WindowsCredential> WindowsCredentials { get; set; }
        public DbSet<OpticalStation> OpticalStations { get; set; }
        public DbSet<TicketServer> TicketServers { get; set; }
        public DbSet<GoalTarget> GoalTargets { get; set; }
        public DbSet<FamilyIDR> FamiliesIDR { get; set; }
        public DbSet<LineIDR> LinesIDR { get; set; }
        public DbSet<StationIDR> StationsIDR { get; set; }
        public DbSet<Failure> Failures { get; set; }
        public DbSet<ProducedUnits> ProducedUnits { get; set; }

        //-----------------FPY-------------------//
        public DbSet<ProducedAndFilteredFPY> ProducedAndFilteredFPYs { get; set; }
        public DbSet<ProducedRAWFPY> ProducedRAWFPY { get; set; }
        public DbSet<FailureFPY> FailuresFPY { get; set; }


        public DbSet<FamilyFPY> FamiliesFPY { get; set; }
        public DbSet<LineFPY> LinesFPY { get; set; }
        public DbSet<ProcessFPY> ProcessesFPY { get; set; }
        public DbSet<StationFPY> StationsFPY { get; set; }
        public DbSet<ModelFPY> ModelsFPY { get; set; }
        public DbSet<FPYLDM2PRO> ProducedUnitsLDM2FPY { get; set; }
        public DbSet<FPYLDM2FAIL> FailsUnitsLDM2FPY { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=AppB.db;Password=Notelasabes.123!",
                sqliteOptionsAction: op =>
                {
                    op.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Family>().ToTable("Families");
            modelBuilder.Entity<Family>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<WindowsCredential>().ToTable("WindowsCredentials");
            modelBuilder.Entity<WindowsCredential>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<OpticalStation>().ToTable("OpticalStations");
            modelBuilder.Entity<OpticalStation>(entity =>
            {
                entity.HasKey(e => e.Id);
                // Foreign keys, one to many sql relation
                entity.HasOne(e => e.Family).WithMany(f => f.OpticalStations).HasForeignKey(f => f.FamilyId);
                entity.HasOne(e => e.Credential).WithMany(w => w.OpticalStations).HasForeignKey(w => w.CredentialId);            
            });

            modelBuilder.Entity<TicketServer>().ToTable("TicketServers");
            modelBuilder.Entity<TicketServer>(entity =>
            {
                entity.HasKey(e => e.Id);
                // Foreign keys, one to many sql relation
                entity.HasOne(e => e.Family).WithMany(f => f.TicketServers).HasForeignKey(f => f.FamilyId);
                entity.HasOne(e => e.Credential).WithMany(w => w.TicketServers).HasForeignKey(w => w.CredentialId);
            });

            modelBuilder.Entity<GoalTarget>().ToTable("GoalTargets");
            modelBuilder.Entity<GoalTarget>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            //------------------------------------------------------------------//
            //-------------------------Controllers by IDR-----------------------//
            //------------------------------------------------------------------//

            modelBuilder.Entity<ProducedUnits>().ToTable("IDR01ProducedUnits");
            modelBuilder.Entity<ProducedUnits>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.FamilyIDR).WithMany(l => l.ProducedUnitsIDR).HasForeignKey(p => p.FamilyID);
            });

            modelBuilder.Entity<FamilyIDR>().ToTable("IDRFamilies");
            modelBuilder.Entity<FamilyIDR>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<LineIDR>().ToTable("IDRProductionLines");
            modelBuilder.Entity<LineIDR>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.FamilyIDR).WithMany(f => f.LinesIDR).HasForeignKey(l => l.FamilyId);
                //entity.HasMany(e => e.Stations).WithOne(s => s.Line).HasPrincipalKey(l => l.Id);
            });

            modelBuilder.Entity<StationIDR>().ToTable("IDRStations");
            modelBuilder.Entity<StationIDR>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.LineIDR).WithMany(l => l.StationsIDR).HasForeignKey(s => s.LineId);
            });

            modelBuilder.Entity<Failure>().ToTable("IDR02Failures");
            modelBuilder.Entity<Failure>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            

            //------------------------------------------------------------------//
            //-------------------------Controllers by FPY-----------------------//
            //------------------------------------------------------------------//

            modelBuilder.Entity<ProducedAndFilteredFPY>().ToTable("FPY01ProducedANDFilter");
            modelBuilder.Entity<ProducedAndFilteredFPY>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
            modelBuilder.Entity<ProducedRAWFPY>().ToTable("FPY02ProducedRAW");
            modelBuilder.Entity<ProducedRAWFPY>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<FailureFPY>().ToTable("FPY03Fails");
            modelBuilder.Entity<FailureFPY>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<FamilyFPY>().ToTable("FPY04Families");
            modelBuilder.Entity<FamilyFPY>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<LineFPY>().ToTable("FPY05ProductionLines");
            modelBuilder.Entity<LineFPY>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.FamilyFPY).WithMany(f => f.LinesFPY).HasForeignKey(l => l.FamilyId);
                //entity.HasMany(e => e.Stations).WithOne(s => s.Line).HasPrincipalKey(l => l.Id);
            });

            modelBuilder.Entity<ProcessFPY>().ToTable("FPY06Process");
            modelBuilder.Entity<ProcessFPY>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.LineFPY).WithMany(l => l.ProcessesFPY).HasForeignKey(s => s.LineID);
            });

            modelBuilder.Entity<StationFPY>().ToTable("FPY07Stations");
            modelBuilder.Entity<StationFPY>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.ProcessFPY).WithMany(l => l.StationsFPY).HasForeignKey(s => s.ProcessID);
            });

            modelBuilder.Entity<ModelFPY>().ToTable("FPY08Models");
            modelBuilder.Entity<ModelFPY>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.StationFPY).WithMany(l => l.ModelsFPY).HasForeignKey(s => s.StationID);
            });

            base.OnModelCreating(modelBuilder);
        }

        
    }
}
