using Discord.Commands;
using System.Threading.Tasks;
using WazeBotDiscord.Lookup;

namespace WazeBotDiscord.Modules
{
    [Group("lookup")]
    [Alias("spreadsheet", "sheet")]
    public class LookupModule : ModuleBase
    {
        readonly LookupService _lookupSvc;

        public LookupModule(LookupService lookupSvc)
        {
            _lookupSvc = lookupSvc;
        }

        [Command]
        public async Task GetUrl()
        {
            await ReplyAsync(_lookupSvc.GetChannelSheetUrl(Context.Channel.Id));
        }

        [Command(RunMode = RunMode.Async)]
        public async Task Search([Remainder]string searchString)
        {
            if (searchString.Length < 4)
            {
                await ReplyAsync("Your search term must be at least four characters long.");
                return;
            }

            await ReplyAsync(await _lookupSvc.SearchSheetAsync(Context.Channel.Id, searchString));
        }
    }
}
