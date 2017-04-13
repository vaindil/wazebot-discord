using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Utilities
{
    public class RequireSmOrAboveAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(
            ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var smRole = ((SocketGuild)context.Guild).Roles.FirstOrDefault(r => r.Name == "State Manager (SM)");
            if (smRole == null)
                return Task.FromResult(PreconditionResult.FromError("This server is not configured for that command."));

            if (((SocketGuildUser)context.Message.Author).Hierarchy >= smRole.Position)
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError("You do not have permission to use that command."));
        }
    }
}
