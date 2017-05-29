using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Utilities
{
    public class RequireChampInNationalGuildAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissions(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var appInfo = await context.Client.GetApplicationInfoAsync();
            if (appInfo.Owner.Id == context.User.Id)
                return PreconditionResult.FromSuccess();

            if (context.Guild.Id != 300471946494214146)
                return PreconditionResult.FromError("That command can only be used on the national server.");

            if (((SocketGuildUser)context.Message.Author).Roles.Any(r => r.Id == 300494132839841792 || r.Id == 300494182403801088))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You must be a champ to use that command.");
        }
    }
}
