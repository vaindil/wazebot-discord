using Microsoft.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
using System;
using WazeBotDiscord.Autoreplies;
using WazeBotDiscord.Classes;

namespace WazeBotDiscord
{
    public class WbContext : DbContext
    {
        public DbSet<Autoreply> Autoreplies { get; set; }
        public DbSet<SyncedRole> SyncedRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING");
            optionsBuilder.UseMySQL(dbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Autoreply>(e =>
            {
                e.ToTable("autoreply");
                e.HasKey(r => r.Id);
                e.Property(r => r.Id).HasColumnName("id");

                e.Property(r => r.ChannelId).HasColumnName("channel_id").IsRequired();
                e.Property(r => r.GuildId).HasColumnName("guild_id").IsRequired();
                e.Property(r => r.Trigger).HasColumnName("trigger").IsRequired().HasMaxLength(20);
                e.Property(r => r.Reply).HasColumnName("reply").IsRequired().HasMaxLength(1500);
                e.Property(r => r.AddedById).HasColumnName("added_by_id").IsRequired();
                e.Property(r => r.AddedAt).HasColumnName("added_at").IsRequired();
            });

            modelBuilder.Entity<SyncedRole>(e =>
            {
                e.ToTable("synced_role");
                e.HasKey(r => r.UserId);
                e.Property(r => r.UserId).HasColumnName("user_id");

                e.Ignore(r => r.WazeRole);
                e.Property(r => r.WazeRoleValue).HasColumnName("waze_role").IsRequired();

                e.Property(r => r.SetById).HasColumnName("set_by_id").IsRequired();
                e.Property(r => r.SetInGuildId).HasColumnName("set_in_guild_id").IsRequired();
                e.Property(r => r.SetAt).HasColumnName("set_at").IsRequired();
            });
        }
    }
}
