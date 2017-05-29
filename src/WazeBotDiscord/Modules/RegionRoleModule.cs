using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Classes.Roles;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Modules
{
    public class RegionRoleModule : ModuleBase
    {
        [Command("region", RunMode = RunMode.Async)]
        [RequireSmOrAbove]
        public async Task ToggleRegion(IUser userIn)
        {
            var exists = Region.Ids.TryGetValue(Context.Guild.Id, out var roleId);
            if (!exists)
            {
                await ReplyAsync("This server does not have a region-specific role.");
                return;
            }

            var user = (SocketGuildUser)userIn;
            var role = Context.Guild.GetRole(roleId);

            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
                await ReplyAsync($"{user.Mention}: Region-specific role removed.");
            }
            else
            {
                await user.AddRoleAsync(role);
                await ReplyAsync($"{user.Mention}: Region-specific role added.");
            }
        }
    }
}
