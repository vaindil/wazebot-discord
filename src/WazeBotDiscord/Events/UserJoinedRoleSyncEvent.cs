using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Classes.Roles;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Events
{
    public static class UserJoinedRoleSyncEvent
    {
        public static async Task SyncRoles(SocketGuildUser user, IDiscordClient client)
        {
            var newGuild = user.Guild;

            // here we go boiz
            foreach (var guild in await RoleSyncHelpers.GetUserGuildsAsync(user, client))
            {
                var roleSet = false;
                var guildUser = guild.GetUser(user.Id);

                // loop through each role we're tracking in the guild
                foreach (var role in _roleIds)
                {
                    // check if the new guild has this role
                    if (!role.TryGetValue(newGuild.Id, out var newRoleId))
                        continue;

                    // check if the guild being checked has this role
                    if (!role.TryGetValue(guild.Id, out var roleId))
                        continue;

                    // check if this user has this role in the guild being checked
                    if (!guildUser.Roles.Any(r => r.Id == roleId))
                        continue;

                    // at this point, we've verified that:
                    // - the new guild has this role
                    // - the guild being checked has this role
                    // - the user has this role in the guild being checked
                    //
                    // this means we can sync the role

                    // first, get the role in the new guild
                    var newRole = newGuild.GetRole(newRoleId);

                    // now add the role to the user in the new guild
                    await user.AddRoleAsync(newRole);

                    // set this to true so the top loop is broken
                    roleSet = true;

                    break;
                }

                if (roleSet)
                    break;
            }
        }

        static readonly List<IReadOnlyDictionary<ulong, ulong>> _roleIds =
            new List<IReadOnlyDictionary<ulong, ulong>>
        {
            CountryManager.Ids,
            StateManager.Ids,
            LargeAreaManager.Ids,
            AreaManager.Ids,
            Mentor.Ids,
            Level6.Ids,
            Level5.Ids,
            Level4.Ids,
            Level3.Ids,
            Level2.Ids,
            Level1.Ids
        };
    }
}
