using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Autoreplies
{
    public class AutoreplyService
    {
        List<Autoreply> _autoreplies;

        public async Task InitAutoreplyServiceAsync()
        {
            using (var db = new WbContext())
            {
                _autoreplies = await db.Autoreplies.ToListAsync();
            }
        }

        public List<Autoreply> GetAllAutoreplies(ulong channelId, ulong guildId)
        {
            return BuildList(channelId, guildId);
        }

        public Autoreply SearchForAutoreply(string content, SocketTextChannel channel)
        {
            var autoreplyList = BuildList(channel.Id, channel.Guild.Id);

            return autoreplyList.Find(r => content.StartsWith($"!{r.Trigger}"));
        }

        public Autoreply GetExactAutoreply(ulong channelId, ulong guildId, string trigger)
        {
            return _autoreplies.Find(r => r.ChannelId == channelId
                                          && r.GuildId == guildId
                                          && string.CompareOrdinal(r.Trigger, trigger) == 0);
        }

        public async Task<bool> AddOrModifyAutoreply(Autoreply reply)
        {
            var existing = GetExactAutoreply(reply.ChannelId, reply.GuildId, reply.Trigger);
            if (existing == null)
            {
                _autoreplies.Add(reply);

                using (var db = new WbContext())
                {
                    db.Autoreplies.Add(reply);
                    await db.SaveChangesAsync();
                }

                return true;
            }

            existing.Reply = reply.Reply;
            existing.AddedById = reply.AddedById;
            existing.AddedAt = reply.AddedAt;

            using (var db = new WbContext())
            {
                db.Autoreplies.Update(existing);
                await db.SaveChangesAsync();
            }

            return false;
        }

        public async Task<bool> RemoveAutoreply(ulong channelId, ulong guildId, string trigger)
        {
            var autoreply = GetExactAutoreply(channelId, guildId, trigger);
            if (autoreply == null)
                return false;

            _autoreplies.Remove(autoreply);

            using (var db = new WbContext())
            {
                db.Autoreplies.Remove(autoreply);
                await db.SaveChangesAsync();
            }

            return true;
        }

        public List<Autoreply> GetChannelAutoreplies(ulong channelId)
        {
            return _autoreplies.FindAll(a => a.ChannelId == channelId);
        }

        public List<Autoreply> GetGuildAutoreplies(ulong guildId)
        {
            return _autoreplies.FindAll(a => a.ChannelId == 1 && a.GuildId == guildId);
        }

        public List<Autoreply> GetGlobalAutoreplies()
        {
            return _autoreplies.FindAll(a => a.ChannelId == 1 && a.GuildId == 1);
        }

        List<Autoreply> BuildList(ulong channelId, ulong guildId)
        {
            var autoreplyList = _autoreplies.FindAll(a => a.ChannelId == channelId);
            autoreplyList.AddRange(_autoreplies.FindAll(a => a.ChannelId == 1 && a.GuildId == guildId));
            autoreplyList.AddRange(_autoreplies.FindAll(a => a.GuildId == 1));

            return autoreplyList;
        }
    }
}
