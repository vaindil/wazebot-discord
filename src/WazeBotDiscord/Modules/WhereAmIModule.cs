using Discord.Commands;
using System.Threading.Tasks;

namespace WazeBotDiscord.Modules
{
    [Group("whereami")]
    public class WhereAmIModule : ModuleBase
    {
        [Command]
        public async Task WhereAmI([Remainder]string unused = null)
        {
            await ReplyAsync($"Channel ID: `{Context.Channel.Id}`\n" +
                             $"Server ID: `{Context.Guild.Id}`");
        }
    }
}
