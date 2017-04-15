using Discord.Commands;
using System.Threading.Tasks;
using WazeBotDiscord.Twitter;

namespace WazeBotDiscord.Modules
{
    [Group("reload")]
    [RequireOwner]
    public class ReloadModule : ModuleBase
    {
        readonly TwitterService _twitterSvc;

        public ReloadModule(TwitterService twitterSvc)
        {
            _twitterSvc = twitterSvc;
        }

        [Command("twitter")]
        public async Task ReloadTwitterAsync()
        {
            await _twitterSvc.ReloadAsync();
            await ReplyAsync("Twitter reloaded");
        }
    }
}
