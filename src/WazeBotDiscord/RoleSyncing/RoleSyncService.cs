using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.RoleSyncing
{
    public static class RoleSyncService
    {
        public static async Task UserUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            if (before.Roles.Count == after.Roles.Count)
                return;

            WazeRoleStatus status;
            IEnumerable<SocketRole> roles;

            if (before.Roles.Count - after.Roles.Count > 0)
            {
                status = WazeRoleStatus.Removed;
                roles = before.Roles.Except(after.Roles);
            }
            else
            {
                status = WazeRoleStatus.Added;
                roles = after.Roles.Except(before.Roles);
            }


        }
    }
}
