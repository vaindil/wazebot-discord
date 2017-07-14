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

            guildReplies = guildReplies.Select(r => channelReplies.Contains(r) ? "~~" + r + "~~" : r);
            globalReplies = globalReplies.Select(r => channelReplies.Contains(r) ? "~~" + r + "~~" : r);
            globalReplies = globalReplies.Select(r => guildReplies.Contains(r) ? "~~" + r + "~~" : r);

            var channelRepliesString = string.Join(", ", channelReplies);
            var guildRepliesString = string.Join(", ", guildReplies);
            var globalRepliesString = string.Join(", ", globalReplies);

            if (string.IsNullOrEmpty(channelRepliesString))
                channelRepliesString = "_(none)_";

            if (string.IsNullOrEmpty(guildRepliesString))
                guildRepliesString = "_(none)_";

            if (string.IsNullOrEmpty(globalRepliesString))
                globalRepliesString = "_(none)_";

            var msg = "__Channel__\n";
            msg += channelRepliesString;

            msg += "\n\n__Server__\n";
            msg += guildRepliesString;

            msg += "\n\n__Global__\n";
            msg += globalRepliesString;

            if (msg.Length > 1500)
            {
                msg = msg.Substring(0, 1500);
                msg += "\n\n_(autoreply too long, sorry!)_";
            }

            await ReplyAsync(msg);
        }

        [Group("add")]
        public class AddAutoreplyModule : ModuleBase
        {
            readonly AutoreplyService _arService;
            readonly CommandService _cmdSvc;

            public AddAutoreplyModule(AutoreplyService arService, CommandService cmdSvc)
            {
                _arService = arService;
                _cmdSvc = cmdSvc;
            }

            [Command("channel")]
            [Summary("Add an autoreply to this channel.")]
            [RequireSmOrAbove]
            public async Task AddToChannel(string trigger, [Remainder]string reply)
            {
                if (_cmdSvc.Search(Context, trigger).IsSuccess)
                {
                    await ReplyAsync("That trigger matches a bot command and cannot be used.");
                    return;
                }

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

                var newlyAdded = await _arService.AddOrModifyAutoreply(autoreply);
                var resultString = newlyAdded ? "added" : "edited";

                await ReplyAsync($"Channel autoreply {resultString}. {autoreply.Trigger} | {autoreply.Reply}");
            }

            [Command("server")]
            [Summary("Add an autoreply to this server.")]
            [RequireSmOrAbove]
            public async Task AddToServer(string trigger, [Remainder]string reply)
            {
                if (_cmdSvc.Search(Context, trigger).IsSuccess)
                {
                    await ReplyAsync("That trigger matches a bot command and cannot be used.");
                    return;
                }

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

                var newlyAdded = await _arService.AddOrModifyAutoreply(autoreply);
                var resultString = newlyAdded ? "added" : "edited";

                await ReplyAsync($"Server autoreply {resultString}. {autoreply.Trigger} | {autoreply.Reply}");
            }

            [Command("global")]
            [Summary("Add a global autoreply.")]
            [RequireChampInNationalGuild]
            public async Task AddToGlobal(string trigger, [Remainder]string reply)
            {
                if (_cmdSvc.Search(Context, trigger).IsSuccess)
                {
                    await ReplyAsync("That trigger matches a bot command and cannot be used.");
                    return;
                }

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

                var newlyAdded = await _arService.AddOrModifyAutoreply(autoreply);
                var resultString = newlyAdded ? "added" : "edited";

                await ReplyAsync($"Global autoreply {resultString}. {autoreply.Trigger} | {autoreply.Reply}");
            }
        }

        [Group("remove")]
        [Alias("delete")]
        public class RemoveAutoreplyModule : ModuleBase
        {
            readonly AutoreplyService _arService;

            public RemoveAutoreplyModule(AutoreplyService arService)
            {
                _arService = arService;
            }

            [Command("channel")]
            [Summary("Remove an autoreply from this channel.")]
            [RequireSmOrAbove]
            public async Task RemoveFromChannel(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                var removed = await _arService.RemoveAutoreply(Context.Channel.Id, Context.Guild.Id, trigger);

                if (removed)
                    await ReplyAsync("Channel autoreply removed.");
                else
                    await ReplyAsync("Channel autoreply does not exist.");
            }

            [Command("server")]
            [Summary("Remove an autoreply from this server.")]
            [RequireSmOrAbove]
            public async Task RemoveFromServer(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                var removed = await _arService.RemoveAutoreply(1, Context.Guild.Id, trigger);

                if (removed)
                    await ReplyAsync("Server autoreply removed.");
                else
                    await ReplyAsync("Server autoreply does not exist.");
            }

            [Command("global")]
            [Summary("Remove a global autoreply.")]
            [RequireChampInNationalGuild]
            public async Task RemoveFromGlobal(string trigger)
            {
                trigger = trigger.ToLowerInvariant();

                var removed = await _arService.RemoveAutoreply(1, 1, trigger);

                if (removed)
                    await ReplyAsync("Global autoreply removed.");
                else
                    await ReplyAsync("Global autoreply does not exist.");
            }
        }
    }
}
