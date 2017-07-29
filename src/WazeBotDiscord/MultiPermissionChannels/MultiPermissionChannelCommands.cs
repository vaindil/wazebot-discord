using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WazeBotDiscord.MultiPermissionChannels
{
    [Group("permissions")]
    [Alias("permission", "perms", "perm")]
    public class MultiPermissionChannelCommands : ModuleBase
    {
        readonly MultiPermissionChannelService _mpcSvc;

        public MultiPermissionChannelCommands(MultiPermissionChannelService mpcSvc)
        {
            _mpcSvc = mpcSvc;
        }

        public async Task ChannelListById(ulong channelId)
        {

        }

        public async Task ChannelList(ITextChannel channel)
        {

        }
    }
}
