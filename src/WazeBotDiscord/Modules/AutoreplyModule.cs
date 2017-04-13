using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using WazeBotDiscord.Autoreplies;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Modules
{
    [Group("autoreply")]
    [Alias("autoreplies", "reply", "replies")]
    public class BaseAutoreplyModule : ModuleBase
    {
        readonly AutoreplyService _arService;

        public BaseAutoreplyModule(AutoreplyService arService)
        {
            _arService = arService;
        }

        [Command]
        public async Task ListAll()
        {
            var channelReplies = _arService.GetChannelAutoreplies(Context.Channel.Id).Select(r => r.Trigger);
            var guildReplies = _arService.GetGuildAutoreplies(Context.Guild.Id).Select(r => r.Trigger);
            var globalReplies = _arService.GetGlobalAutoreplies().Select(r => r.Trigger);

            var msg = "__Channel__\n";
            msg += string.Join(", ", channelReplies);

            msg += "\n\n__Server__\n";
            msg += string.Join(", ", guildReplies);

            msg += "\n\n__Global__\n";
            msg += string.Join(", ", globalReplies);
            
            if (msg.Length > 1500)
            {
                msg = msg.Substring(0, 1500);
                msg += "\n\n_(autoreply too long, sorry!)_";
            }

            await ReplyAsync(msg);
        }

        [Group("add")]
        [RequireSmOrAbove]
        public class AddAutoreplyModule : ModuleBase
        {
            readonly AutoreplyService _arService;

            public AddAutoreplyModule(AutoreplyService arService)
            {
                _arService = arService;
            }

            [Command("channel")]
            [Summary("Add an autoreply to this channel.")]
            public async Task AddToChannel(string trigger, [Remainder]string reply)
            {
                if (trigger.Length > 30)
                {
                    await ReplyAsync("Trigger is too long.");
                    return;
                }

                if (reply.Length > 1000)
                {
                    await ReplyAsync("Reply is too long.");
                    return;
                }

                var autoreply = new Autoreply
                {
                    ChannelId = Context.Channel.Id,
                    GuildId = Context.Guild.Id,
                    Trigger = trigger.ToLowerInvariant(),
                    Reply = reply,
                    AddedById = Context.User.Id,
                    AddedAt = DateTime.UtcNow
                };

                await _arService.AddOrModifyAutoreply(autoreply);

                await ReplyAsync($"Channel autoreply added. {autoreply.Trigger} | {autoreply.Reply}");
            }

            [Command("server")]
            [Summary("Add an autoreply to this server.")]
            public async Task AddToServer(string trigger, [Remainder]string reply)
            {
                if (trigger.Length > 30)
                {
                    await ReplyAsync("Trigger is too long.");
                    return;
                }

                if (reply.Length > 1000)
                {
                    await ReplyAsync("Reply is too long.");
                    return;
                }

                var autoreply = new Autoreply
                {
                    ChannelId = 1,
                    GuildId = Context.Guild.Id,
                    Trigger = trigger.ToLowerInvariant(),
                    Reply = reply,
                    AddedById = Context.User.Id,
                    AddedAt = DateTime.UtcNow
                };

                await _arService.AddOrModifyAutoreply(autoreply);

                await ReplyAsync($"Server autoreply added. {autoreply.Trigger} | {autoreply.Reply}");
            }

            [Command("global")]
            [Summary("Add a global autoreply.")]
            public async Task AddToGlobal(string trigger, [Remainder]string reply)
            {
                if (trigger.Length > 30)
                {
                    await ReplyAsync("Trigger is too long.");
                    return;
                }

                if (reply.Length > 1000)
                {
                    await ReplyAsync("Reply is too long.");
                    return;
                }

                var autoreply = new Autoreply
                {
                    ChannelId = 1,
                    GuildId = 1,
                    Trigger = trigger.ToLowerInvariant(),
                    Reply = reply,
                    AddedById = Context.User.Id,
                    AddedAt = DateTime.UtcNow
                };

                await _arService.AddOrModifyAutoreply(autoreply);

                await ReplyAsync($"Global autoreply added. {autoreply.Trigger} | {autoreply.Reply}");
            }
        }

        [Group("remove")]
        [Alias("delete")]
        [RequireSmOrAbove]
        public class RemoveAutoreplyModule : ModuleBase
        {
            readonly AutoreplyService _arService;

            public RemoveAutoreplyModule(AutoreplyService arService)
            {
                _arService = arService;
            }

            [Command("channel")]
            [Summary("Remove an autoreply from this channel.")]
            public async Task RemoveFromChannel(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                await _arService.RemoveAutoreply(Context.Channel.Id, Context.Guild.Id, trigger);

                await ReplyAsync("Channel autoreply removed.");
            }

            [Command("server")]
            [Summary("Remove an autoreply from this server.")]
            public async Task RemoveFromServer(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                await _arService.RemoveAutoreply(1, Context.Guild.Id, trigger);

                await ReplyAsync("Server autoreply removed.");
            }

            [Command("global")]
            [Summary("Remove a global autoreply.")]
            public async Task RemoveFromGlobal(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                await _arService.RemoveAutoreply(1, 1, trigger);

                await ReplyAsync("Global autoreply removed.");
            }
        }
    }
}
