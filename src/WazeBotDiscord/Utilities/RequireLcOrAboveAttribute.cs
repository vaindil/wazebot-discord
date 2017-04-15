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
            if ((await context.Client.GetApplicationInfoAsync()).Owner == context.Message.Author)
                return PreconditionResult.FromSuccess();

            var lcRole = ((SocketGuild)context.Guild).Roles.FirstOrDefault(r => r.Name == "Local Champ (LC)");
            if (lcRole == null)
                return PreconditionResult.FromError("This server is not configured for that command.");

            if (((SocketGuildUser)context.Message.Author).Hierarchy >= lcRole.Position)
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You do not have permission to use that command.");
        }
    }
}
