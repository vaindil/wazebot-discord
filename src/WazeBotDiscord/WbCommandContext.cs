using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace WazeBotDiscord
{
    public class WbCommandContext : CommandContext
    {
        public WbCommandContext(IDiscordClient client, IUserMessage msg)
            : base(client, msg)
        {
        }
    }
}
