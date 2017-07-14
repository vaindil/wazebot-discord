using Discord.WebSocket;
using System.Threading.Tasks;

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

            await channel.SendMessageAsync(ar.Reply);
        }
    }
}
