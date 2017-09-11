using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Modules
{
    public class FiresideModule : ModuleBase
    {
        //[Command("mifireside")]
        public async Task ToggleFireside(IUser user)
        {
            var author = (SocketGuildUser)Context.Message.Author;

            await Context.Message.DeleteAsync();

            if (!author.Roles.Any(r => r.Id == 0))
                return;

            if (author.Hierarchy < Context.Guild.GetRole(0).Position)
                return;

            var channel = (SocketTextChannel)(await Context.Guild.GetChannelAsync(0));
            if (channel.GetPermissionOverwrite(user) != null)
                await channel.RemovePermissionOverwriteAsync(user);
            else
                await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(readMessages: PermValue.Allow));
        }
    }
}
