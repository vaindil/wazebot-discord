using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    public class KeywordService
    {
        List<KeywordRecord> _keywords = new List<KeywordRecord>();
        List<UserMutedChannels> _mutedChannels = new List<UserMutedChannels>();
        List<UserMutedGuilds> _mutedGuilds = new List<UserMutedGuilds>();

        readonly Regex _botMsg = new Regex(@"\*\*.+\*\*: ");

        /// <summary>
        /// Initializes the keyword service from the database.
        /// </summary>
        public async Task InitKeywordServiceAsync()
        {
            List<DbKeyword> keywords;
            List<DbUserMutedChannel> mutedChannels;
            List<DbUserMutedGuild> mutedGuilds;

            using (var db = new WbContext())
            {
                keywords = await db.Keywords
                    .Include(k => k.IgnoredChannels)
                    .Include(k => k.IgnoredGuilds)
                    .ToListAsync();

                mutedChannels = await db.MutedChannels.ToListAsync();
                mutedGuilds = await db.MutedGuilds.ToListAsync();
            }

            foreach (var k in keywords)
            {
                Regex regexKeyword = null;
                if (k.Keyword.StartsWith("/") && k.Keyword.EndsWith("/"))
                    regexKeyword = CreateRegex(k.Keyword);

                _keywords.Add(new KeywordRecord
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    Keyword = k.Keyword,
                    RegexKeyword = regexKeyword,
                    IgnoredChannels = k.IgnoredChannels.Select(c => c.ChannelId).ToList(),
                    IgnoredGuilds = k.IgnoredGuilds.Select(g => g.GuildId).ToList()
                });
            }

            var mcUserIds = mutedChannels.Select(c => c.UserId).Distinct();
            _mutedChannels = mcUserIds.Select(i => new UserMutedChannels
            {
                UserId = i,
                ChannelIds = mutedChannels.Where(c => c.UserId == i).Select(c => c.ChannelId).ToList()
            }).ToList();

            var mgUserIds = mutedGuilds.Select(g => g.UserId).Distinct();
            _mutedGuilds = mgUserIds.Select(i => new UserMutedGuilds
            {
                UserId = i,
                GuildIds = mutedGuilds.Where(g => g.UserId == i).Select(g => g.GuildId).ToList()
            }).ToList();
        }

        /// <summary>
        /// Checks a message for any matching keywords and returns all matches.
        /// </summary>
        /// <param name="msg">The message to check</param>
        /// <param name="guildId">ID of the guild the message was sent in</param>
        /// <param name="channelId">ID of the channel the message was sent in</param>
        /// <returns>List of matches</returns>
        public List<KeywordMatch> CheckForKeyword(SocketMessage msg, ulong guildId, ulong channelId)
        {
            var message = msg.Content.ToLowerInvariant();
            var matches = new List<KeywordMatch>();

            if (msg.Author.Id == 333960669839884290)
                message = _botMsg.Replace(message, "");

            foreach (var k in _keywords)
            {
                if (k.IgnoredGuilds.Contains(guildId) || k.IgnoredChannels.Contains(channelId))
                    continue;

                var mutedGuilds = _mutedGuilds.Find(g => g.UserId == k.UserId);
                var mutedChannels = _mutedChannels.Find(c => c.UserId == k.UserId);

                if ((mutedGuilds?.GuildIds.Contains(guildId) == true)
                    || (mutedChannels?.ChannelIds.Contains(channelId) == true))
                    continue;

                if (k.RegexKeyword != null)
                {
                    if (k.RegexKeyword?.IsMatch(message) == false)
                        continue;
                }
                else if (!message.Contains(k.Keyword))
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
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record != null)
                return (record, true);

            if (keyword.StartsWith("/") && keyword.EndsWith("/"))
                record = new KeywordRecord(userId, keyword, CreateRegex(keyword));
            else
                record = new KeywordRecord(userId, keyword);

            var dbKeyword = new DbKeyword
            {
                UserId = userId,
                Keyword = keyword
            };

            using (var db = new WbContext())
            {
                db.Keywords.Add(dbKeyword);
                await db.SaveChangesAsync();
            }

            record.Id = dbKeyword.Id;
            _keywords.Add(record);

            return (record, false);
        }

        /// <summary>
        /// Removes a keyword from a user.
        /// </summary>
        /// <param name="userId">ID of the user to remove the keyword from</param>
        /// <param name="keyword">Keyword to remove</param>
        /// <returns>true if the keyword existed and was removed, or false if the user was not subscribed</returns>
        public async Task<bool> RemoveKeywordAsync(ulong userId, string keyword)
        {
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return false;

            _keywords.Remove(record);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords.FirstOrDefaultAsync(k => k.UserId == userId && k.Keyword == keyword);
                if (dbRecord == null)
                    return true;

                db.Keywords.Remove(dbRecord);
                await db.SaveChangesAsync();
            }

            return true;
        }

        /// <summary>
        /// Ignores channels for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to add the ignores to</param>
        /// <param name="keyword">Keyword that is being ignored</param>
        /// <param name="channelIds">Channel IDs that are being ignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword or 
        /// it is already being ignored</returns>
        public async Task<IgnoreResult> IgnoreChannelsAsync(ulong userId, string keyword, params ulong[] channelIds)
        {
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return IgnoreResult.NotSubscribed;
            if (channelIds.All(c => record.IgnoredChannels.Contains(c)))
                return IgnoreResult.AlreadyIgnored;

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
                    return IgnoreResult.NotSubscribed;

                foreach (var c in dbChannels)
                    dbRecord.IgnoredChannels.Add(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredChannels.AddRange(channels);

            return IgnoreResult.Success;
        }

        /// <summary>
        /// Ignores guilds for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to add the ignores to</param>
        /// <param name="keyword">Keyword that is being ignored</param>
        /// <param name="guildIds">Guild IDs that are being ignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword or 
        /// it is already being ignored</returns>
        public async Task<IgnoreResult> IgnoreGuildsAsync(ulong userId, string keyword, params ulong[] guildIds)
        {
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return IgnoreResult.NotSubscribed;
            if (guildIds.All(g => record.IgnoredGuilds.Contains(g)))
                return IgnoreResult.AlreadyIgnored;

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
                    return IgnoreResult.NotSubscribed;

                foreach (var c in dbGuilds)
                    dbRecord.IgnoredGuilds.Add(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredGuilds.AddRange(guilds);

            return IgnoreResult.Success;
        }

        /// <summary>
        /// Unignores channels for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to remove the ignores from</param>
        /// <param name="keyword">Keyword that is being unignored</param>
        /// <param name="channelIds">Channel IDs that are being unignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
        public async Task<UnignoreResult> UnignoreChannelsAsync(ulong userId, string keyword, params ulong[] channelIds)
        {
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return UnignoreResult.NotSubscribed;
            if (channelIds.All(c => !record.IgnoredGuilds.Contains(c)))
                return UnignoreResult.NotIgnored;

            var channels = channelIds.Distinct().Intersect(record.IgnoredChannels);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredChannels)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return UnignoreResult.NotSubscribed;

                foreach (var c in dbRecord.IgnoredChannels.Where(c => channels.Contains(c.ChannelId)).ToList())
                    dbRecord.IgnoredChannels.Remove(c);

                await db.SaveChangesAsync();
            }

            record.IgnoredChannels.RemoveAll(c => channels.Contains(c));

            return UnignoreResult.Success;
        }

        /// <summary>
        /// Unignores guilds for a given user keyword.
        /// </summary>
        /// <param name="userId">ID of the user to remove the ignores from</param>
        /// <param name="keyword">Keyword that is being unignored</param>
        /// <param name="guildIds">Guild IDs that are being unignored</param>
        /// <returns>True if success, false if the user isn't subscribed to the provided keyword</returns>
        public async Task<UnignoreResult> UnignoreGuildsAsync(ulong userId, string keyword, params ulong[] guildIds)
        {
            if (!keyword.StartsWith("/") || !keyword.EndsWith("/"))
                keyword = keyword.ToLowerInvariant();

            var record = GetRecord(userId, keyword);
            if (record == null)
                return UnignoreResult.NotSubscribed;
            if (guildIds.All(g => !record.IgnoredGuilds.Contains(g)))
                return UnignoreResult.NotIgnored;

            var guilds = guildIds.Distinct().Intersect(record.IgnoredGuilds);

            using (var db = new WbContext())
            {
                var dbRecord = await db.Keywords
                    .Include(k => k.IgnoredGuilds)
                    .FirstOrDefaultAsync(k => k.Id == record.Id);
                if (dbRecord == null)
                    return UnignoreResult.NotSubscribed;

                foreach (var g in dbRecord.IgnoredGuilds.Where(g => guilds.Contains(g.GuildId)).ToList())
                    dbRecord.IgnoredGuilds.Remove(g);

                await db.SaveChangesAsync();
            }

            record.IgnoredGuilds.RemoveAll(g => guilds.Contains(g));

            return UnignoreResult.Success;
        }

        /// <summary>
        /// Mutes a channel for the given user.
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <param name="channelId">Channel's ID</param>
        public async Task MuteChannelAsync(ulong userId, ulong channelId)
        {
            var userRecord = _mutedChannels.Find(c => c.UserId == userId);
            if (userRecord?.ChannelIds.Contains(channelId) == true)
                return;

            if (userRecord == null)
                userRecord = new UserMutedChannels
                {
                    UserId = userId,
                    ChannelIds = new List<ulong>()
                };

            userRecord.ChannelIds.Add(channelId);
            _mutedChannels.Add(userRecord);

            using (var db = new WbContext())
            {
                db.MutedChannels.Add(new DbUserMutedChannel
                {
                    UserId = userId,
                    ChannelId = channelId
                });

                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Mutes a guild for the given user.
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <param name="guildId">Guild's ID</param>
        public async Task MuteGuildAsync(ulong userId, ulong guildId)
        {
            var userRecord = _mutedGuilds.Find(c => c.UserId == userId);
            if (userRecord?.GuildIds.Contains(guildId) == true)
                return;

            if (userRecord == null)
                userRecord = new UserMutedGuilds
                {
                    UserId = userId,
                    GuildIds = new List<ulong>()
                };

            userRecord.GuildIds.Add(guildId);
            _mutedGuilds.Add(userRecord);

            using (var db = new WbContext())
            {
                db.MutedGuilds.Add(new DbUserMutedGuild
                {
                    UserId = userId,
                    GuildId = guildId
                });

                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Unmutes a channel for the given user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task UnmuteChannelAsync(ulong userId, ulong channelId)
        {
            var userRecord = _mutedChannels.Find(c => c.UserId == userId);
            if (userRecord == null || !userRecord.ChannelIds.Contains(channelId))
                return;

            userRecord.ChannelIds.Remove(channelId);

            using (var db = new WbContext())
            {
                var record = await db.MutedChannels
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ChannelId == channelId);
                if (record == null)
                    return;

                db.MutedChannels.Remove(record);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Unmutes a guild for the given user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public async Task UnmuteGuildAsync(ulong userId, ulong guildId)
        {
            var userRecord = _mutedGuilds.Find(c => c.UserId == userId);
            if (userRecord == null || !userRecord.GuildIds.Contains(guildId))
                return;

            userRecord.GuildIds.Remove(guildId);

            using (var db = new WbContext())
            {
                var record = await db.MutedGuilds
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.GuildId == guildId);
                if (record == null)
                    return;

                db.MutedGuilds.Remove(record);
                await db.SaveChangesAsync();
            }
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

        Regex CreateRegex(string keyword)
        {
            keyword = keyword.Trim('/');
            return new Regex(keyword,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline,
                new TimeSpan(0, 0, 0, 0, 500));
        }
    }

    public enum IgnoreResult
    {
        Success,
        NotSubscribed,
        AlreadyIgnored
    }

    public enum UnignoreResult
    {
        Success,
        NotSubscribed,
        NotIgnored
    }
}
