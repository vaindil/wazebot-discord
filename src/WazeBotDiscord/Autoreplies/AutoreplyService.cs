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

            await Task.Delay(-1);
        }

        public List<Autoreply> GetAllAutoreplies()
        {
            return _autoreplies;
        }

        public Autoreply SearchForAutoreply(string content, SocketTextChannel channel)
        {
            var autoreplyList = _autoreplies.Where(a => a.ChannelId == channel.Id).ToList();
            autoreplyList.AddRange(_autoreplies.FindAll(a => a.GuildId == channel.Guild.Id));
            autoreplyList.AddRange(_autoreplies.FindAll(a => a.GuildId == 1));

            return autoreplyList.FirstOrDefault(r => content.Contains($"!{r.Trigger}"));
        }

        public Autoreply GetAutoreply(ulong channelId, ulong guildId, string trigger)
        {
            return _autoreplies.FirstOrDefault(r => r.ChannelId == channelId
                                               && r.GuildId == guildId
                                               && string.CompareOrdinal(r.Trigger, trigger) == 0);
        }

        public async Task AddOrUpdateAutoreply(Autoreply reply)
        {
            var existing = GetAutoreply(reply.ChannelId, reply.GuildId, reply.Trigger);
            if (existing == null)
            {
                _autoreplies.Add(reply);

                using (var db = new WbContext())
                {
                    db.Autoreplies.Add(reply);
                    await db.SaveChangesAsync();
                }

                return;
            }

            existing.Reply = reply.Reply;
            existing.AddedById = reply.AddedById;
            existing.AddedAt = reply.AddedAt;

            using (var db = new WbContext())
            {
                db.Autoreplies.Update(existing);
                await db.SaveChangesAsync();
            }
        }
    }
}
