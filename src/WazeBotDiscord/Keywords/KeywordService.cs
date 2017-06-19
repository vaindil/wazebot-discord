using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    public class KeywordService
    {
        List<KeywordRecord> _keywords;

        /// <summary>
        /// Initializes the keyword service from the database.
        /// </summary>
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

        /// <summary>
        /// Checks a message for any matching keywords and returns all matches.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <returns>List of matches</returns>
        public List<KeywordMatch> CheckForKeyword(string message)
        {
            message = message.ToLowerInvariant();
            var matches = new List<KeywordMatch>();

            foreach (var k in _keywords)
            {
                if (!message.Contains(k.Keyword))
                    continue;

                var existingMatch = matches.Find(m => m.UserId == k.UserId);
                if (existingMatch != null)
                    existingMatch.MatchedKeywords.Add(k.Keyword);
                else
                    matches.Add(new KeywordMatch(k.UserId, k.Keyword));

            }

            return matches;
        }

        /// <summary>
        /// Get all keywords for a user.
        /// </summary>
        /// <param name="userId">User ID to get keywords for</param>
        /// <returns>List of KeywordRecords</returns>
        public List<KeywordRecord> GetKeywordsForUser(ulong userId)
        {
            return _keywords.Where(k => k.UserId == userId).ToList();
        }

        /// <summary>
        /// Adds a new keyword to a user.
        /// </summary>
        /// <param name="userId">ID of the user to add the keyword to</param>
        /// <param name="keyword">Keyword to add</param>
        /// <returns>Tuple of the keyword and whether user was already subscribed</returns>
        public async Task<(KeywordRecord Keyword, bool AlreadyExisted)> AddKeywordAsync(ulong userId, string keyword)
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

        /// <summary>
        /// Removes a keyword from a user.
        /// </summary>
        /// <param name="userId">ID of the user to remove the keyword from</param>
        /// <param name="keyword">Keyword to remove</param>
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

        /// <summary>
        /// Ignores channels for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to add the ignores to</param>
        /// <param name="keyword">Keyword that is being ignored</param>
        /// <param name="channelIds">Channel IDs that are being ignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
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

        /// <summary>
        /// Ignores guilds for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to add the ignores to</param>
        /// <param name="keyword">Keyword that is being ignored</param>
        /// <param name="guildIds">Guild IDs that are being ignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
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

        /// <summary>
        /// Unignores channels for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to remove the ignores from</param>
        /// <param name="keyword">Keyword that is being unignored</param>
        /// <param name="channelIds">Channel IDs that are being unignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
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

        /// <summary>
        /// Unignores guilds for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to remove the ignores from</param>
        /// <param name="keyword">Keyword that is being unignored</param>
        /// <param name="guildIds">Guild IDs that are being unignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
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

        /// <summary>
        /// Gets a specific KeywordRecord from the list in memory.
        /// </summary>
        /// <param name="userId">User ID for the record</param>
        /// <param name="keyword">The keyword for the record</param>
        /// <returns>The requested KeywordRecord or null if not found</returns>
        KeywordRecord GetRecord(ulong userId, string keyword)
        {
            return _keywords.Find(k => k.UserId == userId && k.Keyword == keyword);
        }
    }
}
