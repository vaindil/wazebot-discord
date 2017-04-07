using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Modules
{
    public class FiresideModule : ModuleBase
    {
        [Command("fireside")]
        public async Task ToggleFireside([Remainder]string unused = null)
        {
            var role = (SocketRole)Context.Guild.Roles.FirstOrDefault(r => r.Name == "Fireside");
            if (role == null)
            {
                await ReplyAsync("This server doesn't have the Fireside role set up.");
                return;
            }

            var author = (SocketGuildUser)Context.Message.Author;
            var msg = author.Username + ": Fireside role ";

            if (author.Roles.Contains(role))
            {
                await author.RemoveRoleAsync(role);
                msg += "removed.";
            }
            else
            {
                await author.AddRoleAsync(role);
                msg += "added.";
            }

            await ReplyAsync(msg);
        }
    }
}
