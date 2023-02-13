using analysistools.api.Models.Authentication;
using analysistools.api.Models.Continental;
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

            modelBuilder.Entity<Failure>().ToTable("IDRFailures");
            modelBuilder.Entity<Failure>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ProducedUnits>().ToTable("IDRProducedUnits");
            modelBuilder.Entity<ProducedUnits>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.FamilyIDR).WithMany(l => l.ProducedUnitsIDR).HasForeignKey(p => p.FamilyID);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
