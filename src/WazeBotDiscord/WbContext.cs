using Microsoft.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
using System;
using WazeBotDiscord.Autoreplies;
using WazeBotDiscord.Classes;
using WazeBotDiscord.Lookup;
using WazeBotDiscord.Twitter;

namespace WazeBotDiscord
{
    public class WbContext : DbContext
    {
        public DbSet<Autoreply> Autoreplies { get; set; }
        public DbSet<SyncedRole> SyncedRoles { get; set; }
        public DbSet<TwitterToCheck> TwittersToCheck { get; set; }
        public DbSet<SheetToSearch> SheetsToSearch { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING"));
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

            modelBuilder.Entity<TwitterToCheck>(e =>
            {
                e.ToTable("twitter_to_check");
                e.HasKey(t => t.Id);

                e.Ignore(r => r.RequiredKeywords);
                e.Property(r => r.RequiredKeywordsValue).HasColumnName("required_keywords").HasMaxLength(150);
                
                e.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
                e.Property(t => t.FriendlyUsername).HasColumnName("friendly_username").IsRequired().HasMaxLength(45);
                e.Property(t => t.DiscordGuildId).HasColumnName("discord_guild_id").IsRequired();
                e.Property(t => t.DiscordChannelId).HasColumnName("discord_channel_id").IsRequired();
            });

            modelBuilder.Entity<SheetToSearch>(e =>
            {
                e.ToTable("sheet_to_search");
                e.HasKey(r => r.ChannelId);

                e.Property(r => r.ChannelId).HasColumnName("channel_id");
                e.Property(r => r.GuildId).HasColumnName("guild_id").IsRequired();
                e.Property(r => r.SheetId).HasColumnName("sheet_id").IsRequired().HasMaxLength(100);
            });
        }
    }
}
