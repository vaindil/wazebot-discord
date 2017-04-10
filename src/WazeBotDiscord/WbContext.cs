using Microsoft.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
using System;
using WazeBotDiscord.SyncedRoles;

namespace WazeBotDiscord
{
    public class WbContext : DbContext
    {
        public DbSet<RegionalCoordinator> RegionalCoordinators { get; set; }
        public DbSet<AssistantRegionalCoordinator> AssistantRegionalCoordinators { get; set; }
        public DbSet<GlobalChamp> GlobalChamps { get; set; }
        public DbSet<LocalChamp> LocalChamps { get; set; }
        public DbSet<CountryManager> CountryManagers { get; set; }
        public DbSet<StateManager> StateManagers { get; set; }
        public DbSet<LargeAreaManager> LargeAreaManagers { get; set; }
        public DbSet<AreaManager> AreaManagers { get; set; }
        public DbSet<Level6Wazer> Level6Wazers { get; set; }
        public DbSet<Level5Wazer> Level5Wazers { get; set; }
        public DbSet<Level4Wazer> Level4Wazers { get; set; }
        public DbSet<Level3Wazer> Level3Wazers { get; set; }
        public DbSet<Level2Wazer> Level2Wazers { get; set; }
        public DbSet<Level1Wazer> Level1Wazers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING");
            if (dbConnectionString == null)
                throw new ArgumentNullException(nameof(dbConnectionString), "DB connection strin env var not found");

            optionsBuilder.UseMySQL(dbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // the most fun programming experience I've ever had right here PepoThink

            modelBuilder.Entity<RegionalCoordinator>(e =>
            {
                e.ToTable("regional_coordinator");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<AssistantRegionalCoordinator>(e =>
            {
                e.ToTable("assistant_regional_coordinator");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<GlobalChamp>(e =>
            {
                e.ToTable("global_champ");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<LocalChamp>(e =>
            {
                e.ToTable("local_champ");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<CountryManager>(e =>
            {
                e.ToTable("country_manager");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<StateManager>(e =>
            {
                e.ToTable("state_manager");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<LargeAreaManager>(e =>
            {
                e.ToTable("large_area_manager");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<AreaManager>(e =>
            {
                e.ToTable("area_manager");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level6Wazer>(e =>
            {
                e.ToTable("level_6_wazer");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level5Wazer>(e =>
            {
                e.ToTable("level_5_wazer");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level4Wazer>(e =>
            {
                e.ToTable("level_4_wazer");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level3Wazer>(e =>
            {
                e.ToTable("level_3_wazer");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level2Wazer>(e =>
            {
                e.ToTable("level_2_wazer");
                e.HasKey(r => r.UserId);
            });

            modelBuilder.Entity<Level1Wazer>(e =>
            {
                e.ToTable("level_1_wazer");
                e.HasKey(r => r.UserId);
            });
        }
    }
}
