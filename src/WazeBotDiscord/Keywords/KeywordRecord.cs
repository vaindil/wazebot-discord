using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WazeBotDiscord.Keywords
{
    public class KeywordRecord
    {
        public KeywordRecord() { }

        public KeywordRecord(ulong userId, string keyword, Regex regexKeyword = null)
        {
            UserId = userId;
            Keyword = keyword;
            RegexKeyword = regexKeyword;
            IgnoredChannels = new List<ulong>();
            IgnoredGuilds = new List<ulong>();
        }

        public int Id { get; set; }

        public ulong UserId { get; set; }

        public string Keyword { get; set; }

        public Regex RegexKeyword { get; set; }

        public List<ulong> IgnoredChannels { get; set; }

        public List<ulong> IgnoredGuilds { get; set; }
    }

    public class UserMutedGuilds
    {
        public ulong UserId { get; set; }

        public List<ulong> GuildIds { get; set; }
    }

    public class UserMutedChannels
    {
        public ulong UserId { get; set; }

        public List<ulong> ChannelIds { get; set; }
    }

    public class DbKeyword
    {
        public int Id { get; set; }

        public ulong UserId { get; set; }

        public string Keyword { get; set; }

        public ICollection<DbKeywordIgnoredChannel> IgnoredChannels { get; set; }

        public ICollection<DbKeywordIgnoredGuild> IgnoredGuilds { get; set; }
    }

    public class DbKeywordIgnoredChannel
    {
        public int Id { get; set; }

        public int KeywordId { get; set; }

        public ulong ChannelId { get; set; }

        public DbKeyword Keyword { get; set; }
    }

    public class DbKeywordIgnoredGuild
    {
        public int Id { get; set; }

        public int KeywordId { get; set; }

        public ulong GuildId { get; set; }

        public DbKeyword Keyword { get; set; }
    }

    public class DbUserMutedChannel
    {
        public ulong UserId { get; set; }

        public ulong ChannelId { get; set; }
    }

    public class DbUserMutedGuild
    {
        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }
    }
}
