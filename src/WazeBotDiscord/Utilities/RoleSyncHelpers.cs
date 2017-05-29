using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.Utilities
{
    public static class RoleSyncHelpers
    {
        public static async Task<SyncedRoleStatus> ToggleSyncedRolesAsync(
            IUser userIn,
            IReadOnlyDictionary<ulong, ulong> guildRoles,
            ICommandContext context)
        {
            var user = userIn as SocketGuildUser;
            var exists = guildRoles.TryGetValue(context.Guild.Id, out var roleId);
            if (!exists)
            {
                await context.Channel.SendMessageAsync("This server is not configured for that command.");
                return SyncedRoleStatus.NotConfigured;
            }

            var role = context.Guild.GetRole(roleId);

            if (user.Roles.Contains(role))
            {
                await RemoveSyncedRolesAsync(user, guildRoles, context.Client);
                return SyncedRoleStatus.Removed;
            }
            else
            {
                await AddSyncedRolesAsync(user, guildRoles, context.Client);
                return SyncedRoleStatus.Added;
            }
        }

        public static async Task RemoveSyncedRolesAsync(
            SocketGuildUser guildUser,
            IReadOnlyDictionary<ulong, ulong> guildRole,
            IDiscordClient client)
        {
            foreach (var guild in await GetUserGuildsAsync(guildUser, client))
            {
                var exists = guildRole.TryGetValue(guild.Id, out var roleId);
                if (!exists)
                    continue;

                var role = guild.GetRole(roleId);
                var thisGuildUser = guild.GetUser(guildUser.Id);

                await thisGuildUser.RemoveRoleAsync(role);
            }
        }

        public static async Task AddSyncedRolesAsync(
            SocketGuildUser guildUser,
            IReadOnlyDictionary<ulong, ulong> guildRole,
            IDiscordClient client)
        {
            foreach (var guild in await GetUserGuildsAsync(guildUser, client))
            {
                var exists = guildRole.TryGetValue(guild.Id, out var roleId);
                if (!exists)
                    continue;

                var role = guild.GetRole(roleId);
                var thisGuildUser = guild.GetUser(guildUser.Id);

                await thisGuildUser.AddRoleAsync(role);
            }
        }

        public static async Task<IEnumerable<SocketGuild>> GetUserGuildsAsync(
            SocketGuildUser guildUser,
            IDiscordClient client)
        {
            return (await client.GetGuildsAsync())
                .Select(g => g as SocketGuild)
                .Where(g => g.Users.Any(u => u.Id == guildUser.Id));
        }
    }

    public enum SyncedRoleStatus
    {
        Added,
        Removed,
        NotConfigured
    }
}
