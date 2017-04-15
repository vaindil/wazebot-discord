using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Utilities
{
    public class RequireLcOrAboveAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissions(
            ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var appInfo = await context.Client.GetApplicationInfoAsync();
            if (appInfo.Owner.Id == context.User.Id)
                return PreconditionResult.FromSuccess();

            var lcRole = ((SocketGuild)context.Guild).Roles.FirstOrDefault(r => r.Name == "Local Champ");
            if (lcRole == null)
                return PreconditionResult.FromError("This server is not configured for that command.");

            if (((SocketGuildUser)context.Message.Author).Hierarchy >= lcRole.Position)
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You do not have permission to use that command.");
        }
    }
}
