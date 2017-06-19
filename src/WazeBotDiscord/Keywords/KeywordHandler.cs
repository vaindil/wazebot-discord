using Discord.WebSocket;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    public static class KeywordHandler
    {
        public static async Task HandleKeywordAsync(SocketMessage msg, KeywordService service, DiscordSocketClient client)
        {
            if (msg.Author.Id == client.CurrentUser.Id)
                return;

            foreach (var m in service.CheckForKeyword(msg.Content))
            {
                var dm = await client.GetUser(m.UserId).GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"{msg.Author.Mention} mentioned one or more of your keywords in " +
                    $"{((SocketTextChannel)msg.Channel).Mention}.\n\n{msg.Content}");
            }
        }
    }
}
