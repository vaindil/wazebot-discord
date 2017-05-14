using Discord;
using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WazeBotDiscord.Modules
{
    [Group("roles")]
    [RequireOwner]
    public class RoleListModule : ModuleBase
    {
        [Command]
        public async Task ListRoles([Remainder]string unused = null)
        {
            var orderedRoles = Context.Guild.Roles.OrderByDescending(r => r.Position);

            var replySb = new StringBuilder("__Roles__\n");
            foreach (var role in orderedRoles)
            {
                var roleName = role.Name;
                if (roleName == "@everyone")
                    roleName = "(@)everyone";

                replySb.AppendLine($"{roleName}: {role.Id}");
            }

            var reply = replySb.ToString();
            reply = reply.TrimEnd('\\', 'n');

            await ReplyAsync(reply);
        }
    }
}
