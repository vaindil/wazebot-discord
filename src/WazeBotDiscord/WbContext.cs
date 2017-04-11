using Microsoft.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
using System;
using WazeBotDiscord.Classes;

namespace WazeBotDiscord
{
    public class WbContext : DbContext
    {
        public DbSet<SyncedRole> SyncedRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING");
            if (dbConnectionString == null)
                throw new ArgumentNullException(nameof(dbConnectionString), "DB connection string env var not found");

            optionsBuilder.UseMySQL(dbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SyncedRole>(e =>
            {
                e.ToTable("synced_role");
                e.HasKey(r => r.UserId);

                e.Ignore(r => r.WazeRole);
                e.Property(r => r.WazeRoleValue).IsRequired();

                e.Property(r => r.SetById).IsRequired();
                e.Property(r => r.SetOnServerId).IsRequired();
                e.Property(r => r.SetAt).IsRequired();
            });
        }
    }
}
