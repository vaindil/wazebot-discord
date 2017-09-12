using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    [Group("keyword")]
    [Alias("keywords", "kwd", "kwds", "subscription", "subscriptions", "sub", "subs")]
    public class KeywordModule : ModuleBase
    {
        readonly KeywordService _kwdSvc;
        readonly string _helpLink = "<https://wazeopedia.waze.com/wiki/USA/Wazebot#Keyword_Subscriptions>";

        public KeywordModule(KeywordService kwdSvc)
        {
            _kwdSvc = kwdSvc;
        }

        [Command]
        [Alias("help")]
        public async Task Help()
        {
            await ReplyAsync($"{Context.Message.Author.Mention}: For help with this command, see the Wazeopedia page: {_helpLink}");
        }

        [Command("list")]
        public async Task List([Remainder]string unused = null)
        {
            var keywords = _kwdSvc.GetKeywordsForUser(Context.Message.Author.Id);
            var reply = new StringBuilder();

            if (keywords.Count == 0)
            {
                reply.Append(Context.Message.Author.Mention);
                reply.Append(": You have no keywords.");
            }
            else
            {
                reply.Append("__");
                reply.Append(Context.Message.Author.Mention);
                reply.Append("'s Keywords__\n```");

                foreach (var k in keywords)
                {
                    reply.Append(k.Keyword);
                    reply.Append("\n");
                }

                reply.Remove(reply.Length - 1, 1);
                reply.Append("```");
            }

            await ReplyAsync(reply.ToString());
        }

        [Command("add")]
        [Alias("sub", "subscribe")]
        public async Task Add([Remainder]string keyword = null)
        {
            if (keyword == null)
            {
                await ReplyAsync($"{Context.Message.Author.Mention}: You must specify a keyword. For more help, see {_helpLink}.");
                return;
            }

            if (keyword.Length < 2)
            {
                await ReplyAsync($"{Context.Message.Author.Mention}: Your keyword must be at least 2 characters long.");
                return;
            }

            if (keyword.Length > 40)
            {
                await ReplyAsync($"{Context.Message.Author.Mention}: Your keyword cannot be longer than 40 characters.");
                return;
            }

            var result = await _kwdSvc.AddKeywordAsync(Context.Message.Author.Id, keyword);
            if (result.AlreadyExisted)
            {
                await ReplyAsync($"{Context.Message.Author.Mention}: " +
                    $"You were already subscribed to the keyword `{keyword}`. No change has been made.");
                return;
            }

            var reply = $"{Context.Message.Author.Mention}: Added keyword `{keyword}`.";
            if (keyword.Contains(" "))
                reply += "\n\n**Note that your keyword contains spaces.** It will only match if all words are matched exactly " +
                    "as you typed them. If you meant to add these as individual keywords, please remove this entry and " +
                    "run the command separately for each individual keyword.";

            await ReplyAsync(reply);
        }

        [Command("remove")]
        [Alias("unsub", "unsubscribe")]
        public async Task Remove([Remainder]string keyword = null)
        {
            if (keyword == null)
            {
                await ReplyAsync($"{Context.Message.Author.Mention}: " +
                    $"You must specify a keyword. For more help, see {_helpLink}.");
                return;
            }

            var existed = await _kwdSvc.RemoveKeywordAsync(Context.Message.Author.Id, keyword);

            if (!existed)
                await ReplyAsync($"{Context.Message.Author.Mention}: " +
                    "You were not subscribed to that keyword. No change was made.");
            else
                await ReplyAsync($"{Context.Message.Author.Mention}: Subscription to `{keyword}` removed.");
        }

        [Group("ignore")]
        public class IgnoreKeywordModule : ModuleBase
        {
            readonly KeywordService _kwdSvc;
            readonly string _helpLink = "<https://wazeopedia.waze.com/wiki/USA/Wazebot#Keyword_Subscriptions>";

            public IgnoreKeywordModule(KeywordService kwdSvc)
            {
                _kwdSvc = kwdSvc;
            }

            [Command("server")]
            [Alias("guild")]
            public async Task IgnoreGuild(ulong guildId, [Remainder]string keyword = null)
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                if (guild == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That server ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                if (keyword == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                switch (await _kwdSvc.IgnoreGuildsAsync(Context.Message.Author.Id, keyword, guildId))
                {
                    case IgnoreResult.Success:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            $"Ignored keyword {keyword} in server {guild.Name}.");
                        break;

                    case IgnoreResult.AlreadyIgnored:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're already ignoring that keyword in that server. No change made.");
                        break;

                    case IgnoreResult.NotSubscribed:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're not subscribed to that keyword. No change made.");
                        break;
                }
            }

            [Command("channel")]
            public async Task IgnoreChannel(ulong channelId, [Remainder]string keyword = null)
            {
                var rawChannel = await Context.Client.GetChannelAsync(channelId);
                if (rawChannel == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That channel ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                if (keyword == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                var channel = rawChannel as SocketTextChannel;

                switch (await _kwdSvc.IgnoreChannelsAsync(Context.Message.Author.Id, keyword, channelId))
                {
                    case IgnoreResult.Success:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            $"Ignored keyword {keyword} in channel {channel.Mention} (server {channel.Guild.Name}).");
                        break;

                    case IgnoreResult.AlreadyIgnored:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're already ignoring that keyword in that channel. No change made.");
                        break;

                    case IgnoreResult.NotSubscribed:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're not subscribed to that keyword. No change made.");
                        break;
                }
            }
        }

        [Group("unignore")]
        [Alias("unsub", "unsubscribe")]
        public class UnignoreKeywordModule : ModuleBase
        {
            readonly KeywordService _kwdSvc;
            readonly string _helpLink = "<https://wazeopedia.waze.com/wiki/USA/Wazebot#Keyword_Subscriptions>";

            public UnignoreKeywordModule(KeywordService kwdSvc)
            {
                _kwdSvc = kwdSvc;
            }

            [Command("server")]
            [Alias("guild")]
            public async Task UnignoreGuild(ulong guildId, [Remainder]string keyword = null)
            {
                if (keyword == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                switch (await _kwdSvc.UnignoreGuildsAsync(Context.Message.Author.Id, keyword, guildId))
                {
                    case UnignoreResult.Success:
                        var guild = await Context.Client.GetGuildAsync(guildId);
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "Unignored keyword `{keyword}` in server {guild.Name}.");
                        break;

                    case UnignoreResult.NotIgnored:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "That keyword was not ignored in that server. No change made.");
                        break;

                    case UnignoreResult.NotSubscribed:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're not subscribed to that keyword. No change made.");
                        break;
                }
            }

            [Command("channel")]
            public async Task UnignoreChannel(ulong channelId, [Remainder]string keyword = null)
            {
                if (keyword == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                switch (await _kwdSvc.UnignoreChannelsAsync(Context.Message.Author.Id, keyword, channelId))
                {
                    case UnignoreResult.Success:
                        var channel = (await Context.Client.GetChannelAsync(channelId)) as SocketTextChannel;
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            $"Unignored keyword `{keyword}` in channel {channel.Mention} (server {channel.Guild.Name}).");
                        break;

                    case UnignoreResult.NotIgnored:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "That keyword was not ignored in that channel. No change made.");
                        break;

                    case UnignoreResult.NotSubscribed:
                        await ReplyAsync($"{Context.Message.Author.Mention}: " +
                            "You're not subscribed to that keyword. No change made.");
                        break;
                }
            }
        }

        [Group("mute")]
        [Alias("silence", "quiet")]
        public class MuteKeywordsModule : ModuleBase
        {
            readonly KeywordService _kwdSvc;
            readonly string _helpLink = "<https://wazeopedia.waze.com/wiki/USA/Wazebot#Keyword_Subscriptions>";

            public MuteKeywordsModule(KeywordService kwdSvc)
            {
                _kwdSvc = kwdSvc;
            }

            [Command("server")]
            [Alias("guild")]
            public async Task MuteGuild(ulong guildId)
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                if (guild == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That server ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                await _kwdSvc.MuteGuildAsync(Context.Message.Author.Id, guildId);
                await ReplyAsync($"{Context.Message.Author.Mention}: Muted {guild.Name}.");
            }

            [Command("channel")]
            public async Task MuteChannel(ulong channelId)
            {
                var channel = await Context.Client.GetChannelAsync(channelId);
                if (channel == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That channel ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                await _kwdSvc.MuteChannelAsync(Context.Message.Author.Id, channelId);
                await ReplyAsync($"{Context.Message.Author.Mention}: Muted {channel.Name}.");
            }
        }

        [Group("unmute")]
        [Alias("unsilence", "unquiet")]
        public class UnmuteKeywordsModule : ModuleBase
        {
            readonly KeywordService _kwdSvc;
            readonly string _helpLink = "<https://wazeopedia.waze.com/wiki/USA/Wazebot#Keyword_Subscriptions>";

            public UnmuteKeywordsModule(KeywordService kwdSvc)
            {
                _kwdSvc = kwdSvc;
            }

            [Command("server")]
            [Alias("guild")]
            public async Task UnmuteGuild(ulong guildId)
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                if (guild == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That server ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                await _kwdSvc.UnmuteGuildAsync(Context.Message.Author.Id, guildId);
                await ReplyAsync($"{Context.Message.Author.Mention}: Unmuted {guild.Name}.");
            }

            [Command("channel")]
            public async Task UnmuteChannel(ulong channelId)
            {
                var channel = await Context.Client.GetChannelAsync(channelId);
                if (channel == null)
                {
                    await ReplyAsync($"{Context.Message.Author.Mention}: " +
                        $"That channel ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                await _kwdSvc.UnmuteChannelAsync(Context.Message.Author.Id, channelId);
                await ReplyAsync($"{Context.Message.Author.Mention}: Unmuted {channel.Name}.");
            }
        }
    }
}
