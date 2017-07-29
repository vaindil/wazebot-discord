using System.Collections.Generic;

namespace WazeBotDiscord.MultiPermissionChannels
{
    public class MultiPermissionChannel
    {
        public ulong ChannelId { get; set; }

        public List<ulong> RequiredRoleIds { get; set; }
    }

    public class DbMultiPermissionChannel
    {
        public ulong ChannelId { get; set; }

        public ulong RoleId { get; set; }
    }
}
