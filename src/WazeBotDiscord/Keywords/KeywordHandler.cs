using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    public static class KeywordHandler
    {
        public static async Task HandleKeywordAsync(SocketMessage msg, KeywordService service, DiscordSocketClient client)
        {
            if (msg.Author.Id == client.CurrentUser.Id || msg.Channel is SocketDMChannel)
                return;

            var channel = msg.Channel as SocketTextChannel;

            foreach (var m in service.CheckForKeyword(msg.Content, channel.Guild.Id, channel.Id))
            {
                if (msg.Author.Id == m.UserId
                    || !channel.Users.Any(u => u.Id == m.UserId))
                    continue;

                var dm = await client.GetUser(m.UserId).GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"{msg.Author.Mention} mentioned one or more of your keywords in " +
                    $"{((SocketTextChannel)msg.Channel).Mention}.\n\n```{msg.Content}```");
            }
        }
    }
}
