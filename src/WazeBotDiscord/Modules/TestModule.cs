using Discord.Commands;
using System.Threading.Tasks;

namespace WazeBotDiscord.Modules
{
    public class TestModule : ModuleBase
    {
        [Command("test")]
        public async Task TestCommand([Remainder]string unused = null)
        {
            await ReplyAsync("Your test has succeeded.");
        }
    }
}
