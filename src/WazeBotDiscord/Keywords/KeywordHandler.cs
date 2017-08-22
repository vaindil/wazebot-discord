using Discord.WebSocket;
using System.Linq;
using System.Text;
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

            foreach (var m in service.CheckForKeyword(msg, channel.Guild.Id, channel.Id))
            {
                if (msg.Author.Id == m.UserId
                    || !channel.Users.Any(u => u.Id == m.UserId))
                    continue;

                var reply = new StringBuilder();
                reply.Append(msg.Author.Username);
                reply.Append(" mentioned ");
                reply.Append(m.MatchedKeywords.Count);
                reply.Append(" of your keywords in ");
                reply.Append(msg.Channel.Name);
                reply.Append(", ");
                reply.Append(((SocketGuildChannel)msg.Channel).Guild.Name);
                reply.Append(".\n\nMatched keyword");
                if (m.MatchedKeywords.Count > 1)
                    reply.Append("s");
                reply.Append(":\n```\n");

                foreach (var k in m.MatchedKeywords)
                {
                    reply.Append(k);
                    reply.Append("\n");
                }

                reply.Append("```\n");
                reply.Append("Message:\n```\n");
                reply.Append(msg.Content);
                reply.Append("\n```");

                var dm = await client.GetUser(m.UserId).GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(reply.ToString());
            }
        }
    }
}
