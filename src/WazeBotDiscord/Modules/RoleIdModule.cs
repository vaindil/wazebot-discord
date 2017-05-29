using Discord;
using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Modules
{
    [Group("roleid")]
    [RequireOwner]
    public class RoleIdModule : ModuleBase
    {
        [Command]
        public async Task GetSpecific(IRole role) => await ReplyAsync($"Role {role.Name}: {role.Id}");

        [Command("wazeall")]
        public async Task GetAll()
        {
            var reply = new StringBuilder("__Waze roles on this server__");

            foreach (var name in WazeRoleNames.RoleNames)
            {
                var idString = "";
                var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == name);
                if (role == null)
                    idString = "(role not present)";
                else
                    idString = role.Id.ToString();

                reply.Append($"\n{name}: {idString}");
            }

            await ReplyAsync(reply.ToString());
        }
    }
}
