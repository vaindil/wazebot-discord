using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    public class KeywordService
    {
        List<KeywordRecord> _keywords;

        public async Task InitKeywordServiceAsync()
        {
            List<DbKeyword> keywords;

            using (var db = new WbContext())
            {
                keywords = await db.Keywords
                    .Include(k => k.IgnoredChannels)
                    .Include(k => k.IgnoredGuilds)
                    .ToListAsync();
            }

            _keywords = keywords.Select(k => new KeywordRecord
            {
                Id = k.Id,
                UserId = k.UserId,
                Keyword = k.Keyword,
                IgnoredChannels = k.IgnoredChannels.Select(c => c.ChannelId).ToList(),
                IgnoredGuilds = k.IgnoredGuilds.Select(g => g.GuildId).ToList()
            }).ToList();
        }

        public List<ulong> CheckForKeyword(string message)
        {
            message = message.ToLowerInvariant();
            var userIds = new List<ulong>();

            foreach (var k in _keywords)
            {
                if (message.Contains(k.Keyword))
                    userIds.Add(k.UserId);
            }

            return userIds;
        }

        public List<KeywordRecord> GetKeywordsForUser(ulong userId)
        {
            return _keywords.Where(k => k.UserId == userId).ToList();
        }

        public async Task<(KeywordRecord keyword, bool alreadyExisted)> AddKeywordAsync(ulong userId, string keyword)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record != null)
                return (record, true);

            record = new KeywordRecord
            {
                UserId = userId,
                Keyword = keyword
            };

            _keywords.Add(record);

            using (var db = new WbContext())
            {
                db.Keywords.Add(new DbKeyword
                {
                    UserId = userId,
                    Keyword = keyword
                });

                await db.SaveChangesAsync();
            }

            return (record, false);
        }

        public async Task RemoveKeywordAsync(ulong userId, string keyword)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return;

            _keywords.Remove(record);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords.FirstOrDefaultAsync(k => k.UserId == userId && k.Keyword == keyword);
                if (dbRecord == null)
                    return;

                db.Keywords.Remove(dbRecord);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> IgnoreChannelsAsync(ulong userId, string keyword, params ulong[] channelIds)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return false;

            var channels = channelIds.Distinct().Except(record.IgnoredChannels);
            var dbChannels = channels.Select(c => new DbKeywordIgnoredChannel
            {
                KeywordId = record.Id,
                ChannelId = c
            });

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredChannels)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return false;

                foreach (var c in dbChannels)
                    dbRecord.IgnoredChannels.Add(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredChannels.AddRange(channels);

            return true;
        }

        public async Task<bool> IgnoreGuildsAsync(ulong userId, string keyword, params ulong[] guildIds)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return false;

            var guilds = guildIds.Distinct().Except(record.IgnoredGuilds);
            var dbGuilds = guilds.Select(c => new DbKeywordIgnoredGuild
            {
                KeywordId = record.Id,
                GuildId = c
            });

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredGuilds)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return false;

                foreach (var c in dbGuilds)
                    dbRecord.IgnoredGuilds.Add(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredGuilds.AddRange(guilds);

            return true;
        }

        public async Task<bool> UnignoreChannelsAsync(ulong userId, string keyword, params ulong[] channelIds)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return false;

            var channels = channelIds.Distinct().Intersect(record.IgnoredChannels);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredChannels)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return false;

                foreach (var c in dbRecord.IgnoredChannels.Where(c => channels.Contains(c.ChannelId)))
                    dbRecord.IgnoredChannels.Remove(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredChannels.RemoveAll(c => channels.Contains(c));

            return true;
        }

        public async Task<bool> UnignoreGuildsAsync(ulong userId, string keyword, params ulong[] guildIds)
        {
            keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return false;

            var guilds = guildIds.Distinct().Intersect(record.IgnoredGuilds);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredGuilds)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return false;

                foreach (var g in dbRecord.IgnoredGuilds.Where(g => guilds.Contains(g.GuildId)))
                    dbRecord.IgnoredGuilds.Remove(g);

                await db.SaveChangesAsync();
            }

            record.IgnoredGuilds.RemoveAll(g => guilds.Contains(g));

            return true;
        }

        KeywordRecord GetRecord(ulong userId, string keyword)
        {
            return _keywords.Find(k => k.UserId == userId && k.Keyword == keyword);
        }
    }
}
