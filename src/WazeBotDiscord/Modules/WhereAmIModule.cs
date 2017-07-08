using Discord.Commands;
using System.Threading.Tasks;

namespace WazeBotDiscord.Modules
{
    public class WhereAmIModule : ModuleBase
    {
        [Command("whereami")]
        public async Task WhereAmI([Remainder]string unused = null)
        {
            await ReplyAsync($"Channel ID: `{Context.Channel.Id.ToString()}`\n" +
                             $"Server ID: `{Context.Guild.Id.ToString()}`");
        }
    }
}
