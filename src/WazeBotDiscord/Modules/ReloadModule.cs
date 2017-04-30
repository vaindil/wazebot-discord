using Discord.Commands;
using System.Threading.Tasks;
using WazeBotDiscord.Lookup;
using WazeBotDiscord.Twitter;

namespace WazeBotDiscord.Modules
{
    [Group("reload")]
    [RequireOwner]
    public class ReloadModule : ModuleBase
    {
        readonly TwitterService _twitterSvc;
        readonly LookupService _lookupSvc;

        public ReloadModule(TwitterService twitterSvc, LookupService lookupSvc)
        {
            _twitterSvc = twitterSvc;
            _lookupSvc = lookupSvc;
        }

        [Command("twitter")]
        public async Task ReloadTwitter()
        {
            _twitterSvc.StopAllStreams();
            await _twitterSvc.InitTwitterServiceAsync();

            await ReplyAsync("Twitter reloaded.");
        }

        [Command("lookup")]
        public async Task ReloadLookup()
        {
            await _lookupSvc.ReloadSheetsAsync();
            await ReplyAsync("Lookup reloaded.");
        }
    }
}
