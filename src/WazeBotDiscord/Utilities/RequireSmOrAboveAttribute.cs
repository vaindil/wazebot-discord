using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Utilities
{
    public class RequireSmOrAboveAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissions(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var appInfo = await context.Client.GetApplicationInfoAsync();
            if (appInfo.Owner.Id == context.User.Id)
                return PreconditionResult.FromSuccess();

            var smRole = ((SocketGuild)context.Guild).Roles.FirstOrDefault(r => r.Name == "State Manager");
            if (smRole == null)
                return PreconditionResult.FromError("This server is not configured for that command.");

            if (((SocketGuildUser)context.Message.Author).Hierarchy >= smRole.Position)
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You do not have permission to use that command.");
        }
    }
}
