using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.MultiPermissionChannels
{
    public static class UserRoleMpcEvent
    {
        public static async Task GrantPermissionsAsync(
            SocketGuildUser user, IDiscordClient client, MultiPermissionChannelService service)
        {
            var channelIds = service.GetAccessibleChannels(user.Id, user.Roles.Select(r => r.Id).ToList());

            foreach (var channel in user.Guild.Channels)
            {
                var overwrite = channel.GetPermissionOverwrite(user);
                if (overwrite == null && channelIds.Contains(channel.Id))
                {
                    var grant = new OverwritePermissions(readMessages: PermValue.Allow);
                    await channel.AddPermissionOverwriteAsync(user, grant);
                }
                else if (overwrite != null && !channelIds.Contains(channel.Id))
                {
                    await channel.RemovePermissionOverwriteAsync(user);
                }
            }
        }
    }
}
