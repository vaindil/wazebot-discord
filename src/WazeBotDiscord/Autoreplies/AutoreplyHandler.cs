using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Autoreplies
{
    public static class AutoreplyHandler
    {
        public static async Task HandleAutoreplyAsync(SocketMessage inMsg, AutoreplyService arService)
        {
            if (inMsg.Channel is SocketDMChannel)
                return;

            var msg = (SocketUserMessage)inMsg;
            var content = msg.Content.ToLowerInvariant();
            var channel = (SocketTextChannel)msg.Channel;

            var ar = arService.SearchForAutoreply(content, channel);
            if (ar == null)
                return;

            if (RestrictedRegion.Ids.Contains(channel.Guild.Id))
            {
                await channel.SendMessageAsync("The bot is currently disabled on this server. " +
                    "For more info, see here: <https://github.com/vaindil/wazebot-discord/issues/12>");
                return;
            }

            await channel.SendMessageAsync(ar.Reply);
        }
    }
}
