using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Classes;

namespace WazeBotDiscord.Events
{
    public static class AutoreplyHandler
    {
        public static async Task<List<Autoreply>> InitAutoreplyAsync()
        {
            using (var db = new WbContext())
            {
                return await db.Autoreplies.ToListAsync();
            }
        }

        public static async Task HandleAutoreplyAsync(SocketMessage inMsg, List<Autoreply> autoreplies)
        {
            var msg = (SocketUserMessage)inMsg;
            var content = msg.Content.ToLowerInvariant();
            var channel = (SocketTextChannel)msg.Channel;

            var autoreplyList = autoreplies.Where(a => a.ChannelId == channel.Id).ToList();
            autoreplyList.AddRange(autoreplies.FindAll(a => a.GuildId == channel.Guild.Id));
            autoreplyList.AddRange(autoreplies.FindAll(a => a.GuildId == 1));

            var ar = autoreplyList.FirstOrDefault(a => content.Contains($"!{a.Trigger}"));
            if (ar == null)
                return;

            await channel.SendMessageAsync(ar.Reply);
        }
    }
}
