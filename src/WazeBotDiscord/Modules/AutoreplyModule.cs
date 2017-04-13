using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using WazeBotDiscord.Autoreplies;

namespace WazeBotDiscord.Modules
{
    [Group("autoreply")]
    public class AutoreplyModule : ModuleBase
    {
        readonly AutoreplyService _arService;

        public AutoreplyModule(AutoreplyService arService)
        {
            _arService = arService;
        }

        [Command]
        public async Task ListAll()
        {
            var all = _arService.GetAllAutoreplies(Context.Channel.Id, Context.Guild.Id);

            var msg = new StringBuilder();
        }
    }
}
