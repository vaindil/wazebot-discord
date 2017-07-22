using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MySQL.Data.Entity.Extensions;
using System;
using WazeBotDiscord.Autoreplies;
using WazeBotDiscord.Keywords;
using WazeBotDiscord.Lookup;
using WazeBotDiscord.Twitter;

namespace WazeBotDiscord
{
    public class WbContext : DbContext
    {
        public DbSet<Autoreply> Autoreplies { get; set; }
        public DbSet<TwitterToCheck> TwittersToCheck { get; set; }
        public DbSet<SheetToSearch> SheetsToSearch { get; set; }
        public DbSet<DbKeyword> Keywords { get; set; }
        public DbSet<DbUserMutedChannel> MutedChannels { get; set; }
        public DbSet<DbUserMutedGuild> MutedGuilds { get; set; }

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

            modelBuilder.Entity<DbKeyword>(e =>
            {
                e.ToTable("keyword");
                e.HasKey(r => r.Id);

                e.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
                e.Property(r => r.Keyword).HasColumnName("keyword").IsRequired().HasMaxLength(30);

                e.HasMany(r => r.IgnoredChannels)
                    .WithOne(s => s.Keyword)
                    .HasForeignKey(s => s.KeywordId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(r => r.IgnoredGuilds)
                    .WithOne(s => s.Keyword)
                    .HasForeignKey(s => s.KeywordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DbKeywordIgnoredChannel>(e =>
            {
                e.ToTable("keyword_ignored_channel");
                e.HasKey(r => r.Id);

                e.Property(r => r.KeywordId).HasColumnName("keyword_id").IsRequired();
                e.Property(r => r.ChannelId).HasColumnName("channel_id").IsRequired();
            });

            modelBuilder.Entity<DbKeywordIgnoredGuild>(e =>
            {
                e.ToTable("keyword_ignored_guild");
                e.HasKey(r => r.Id);

                e.Property(r => r.KeywordId).HasColumnName("keyword_id").IsRequired();
                e.Property(r => r.GuildId).HasColumnName("guild_id").IsRequired();
            });

            modelBuilder.Entity<DbUserMutedChannel>(e =>
            {
                e.ToTable("keyword_user_muted_channel");
                e.HasKey(c => new { c.UserId, c.ChannelId });

                e.Property(c => c.UserId).HasColumnName("user_id");
                e.Property(c => c.ChannelId).HasColumnName("channel_id");
            });

            modelBuilder.Entity<DbUserMutedGuild>(e =>
            {
                e.ToTable("keyword_user_muted_guild");
                e.HasKey(g => new { g.UserId, g.GuildId });

                e.Property(g => g.UserId).HasColumnName("user_id");
                e.Property(g => g.GuildId).HasColumnName("guild_id");
            });
        }
    }
}
