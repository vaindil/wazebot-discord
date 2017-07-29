using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeBotDiscord.MultiPermissionChannels
{
    public class MultiPermissionChannelService
    {
        List<MultiPermissionChannel> _channels = new List<MultiPermissionChannel>();

        public async Task InitAsync()
        {
            List<DbMultiPermissionChannel> mpcs;

            using (var db = new WbContext())
            {
                mpcs = await db.MultiPermissionChannels.ToListAsync();
            }

            foreach (var c in mpcs)
            {
                var current = GetCurrent(c.ChannelId);

                if (current.RequiredRoleIds.Contains(c.RoleId))
                    continue;

                current.RequiredRoleIds.Add(c.RoleId);
            }
        }

        public List<ulong> GetAccessibleChannels(ulong userId, List<ulong> roleIds)
        {
            return _channels
                .FindAll(c => roleIds.All(r => c.RequiredRoleIds.Contains(r)))
                .Select(c => c.ChannelId)
                .ToList();
        }

        public async Task AddChannelRoleIdAsync(ulong channelId, ulong roleId)
        {
            var current = GetCurrent(channelId);
            if (current.RequiredRoleIds.Contains(roleId))
                return;

            current.RequiredRoleIds.Add(roleId);

            using (var db = new WbContext())
            {
                var existing = await db.MultiPermissionChannels
                    .FirstOrDefaultAsync(c => c.ChannelId == channelId && c.RoleId == roleId);
                if (existing != null)
                    return;

                db.MultiPermissionChannels.Add(existing);
                await db.SaveChangesAsync();
            }
        }

        public async Task RemoveChannelRoleIdAsync(ulong channelId, ulong roleId)
        {
            var current = GetCurrent(channelId);
            if (!current.RequiredRoleIds.Contains(roleId))
                return;

            current.RequiredRoleIds.Remove(roleId);

            using (var db = new WbContext())
            {
                var existing = await db.MultiPermissionChannels
                    .FirstOrDefaultAsync(c => c.ChannelId == channelId && c.RoleId == roleId);
                if (existing == null)
                    return;

                db.MultiPermissionChannels.Remove(existing);
                await db.SaveChangesAsync();
            }
        }

        MultiPermissionChannel GetCurrent(ulong channelId)
        {
            var current = _channels.Find(x => x.ChannelId == channelId);
            if (current == null)
            {
                current = new MultiPermissionChannel
                {
                    ChannelId = channelId,
                    RequiredRoleIds = new List<ulong>()
                };

                _channels.Add(current);
            }

            return current;
        }
    }
}
