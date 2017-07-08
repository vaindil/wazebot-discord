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
            await ReplyAsync($"For help with this command, see the Wazeopedia page: {_helpLink}");
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
                reply.Append("'s Keywords__\n");

                foreach (var k in keywords)
                {
                    reply.Append(k.Keyword);
                    reply.Append("\n");
                }
            }

            await ReplyAsync(reply.ToString().TrimEnd('\\', 'n'));
        }

        [Command("add")]
        [Alias("sub", "subscribe")]
        public async Task Add([Remainder]string keyword = null)
        {
            if (keyword == null)
            {
                await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                return;
            }

            if (keyword.Length < 3)
            {
                await ReplyAsync("Your keyword must be at least 3 characters long.");
                return;
            }

            if (keyword.Length > 40)
            {
                await ReplyAsync("Your keyword cannot be longer than 40 characters.");
                return;
            }

            var result = await _kwdSvc.AddKeywordAsync(Context.Message.Author.Id, keyword);
            if (result.AlreadyExisted)
            {
                await ReplyAsync($"You were already subscribed to the keyword `{keyword}`. No change has been made.");
                return;
            }

            var reply = $"Added keyword `{keyword}`.";
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
                await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                return;
            }

            var existed = await _kwdSvc.RemoveKeywordAsync(Context.Message.Author.Id, keyword);

            if (!existed)
                await ReplyAsync("You were not subscribed to that keyword. No change was made.");
            else
                await ReplyAsync($"Subscription to `{keyword}` removed.");
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
                    await ReplyAsync($"That server ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                if (keyword == null)
                {
                    await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                var success = await _kwdSvc.IgnoreGuildsAsync(Context.Message.Author.Id, keyword, guildId);
                if (!success)
                    await ReplyAsync("You are not subscribed to that keyword or you're already " +
                        "ignoring it in that server. No change made.");
                else
                    await ReplyAsync($"Ignored keyword {keyword} in server {guild.Name}.");
            }

            [Command("channel")]
            public async Task IgnoreChannel(ulong channelId, [Remainder]string keyword = null)
            {
                var rawChannel = await Context.Client.GetChannelAsync(channelId);
                if (rawChannel == null)
                {
                    await ReplyAsync($"That channel ID is invalid. For more help, see {_helpLink}.");
                    return;
                }

                if (keyword == null)
                {
                    await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                var channel = rawChannel as SocketTextChannel;

                var success = await _kwdSvc.IgnoreChannelsAsync(Context.Message.Author.Id, keyword, channelId);
                if (!success)
                    await ReplyAsync("You are not subscribed to that keyword or you're already " +
                        "ignoring it in that channel. No change made.");
                else
                    await ReplyAsync($"Ignored keyword {keyword} in channel {channel.Mention} (server {channel.Guild.Name}).");
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
                    await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                var success = await _kwdSvc.UnignoreGuildsAsync(Context.Message.Author.Id, keyword, guildId);

                if (!success)
                    await ReplyAsync("That keyword was not ignored in that server. No change made.");
                else
                {
                    var guild = await Context.Client.GetGuildAsync(guildId);
                    await ReplyAsync($"Unignored keyword `{keyword}` in server {guild.Name}.");
                }
            }

            [Command("channel")]
            public async Task UnignoreChannel(ulong channelId, [Remainder]string keyword = null)
            {
                if (keyword == null)
                {
                    await ReplyAsync($"You must specify a keyword. For more help, see {_helpLink}.");
                    return;
                }

                var success = await _kwdSvc.UnignoreChannelsAsync(Context.Message.Author.Id, keyword, channelId);

                if (!success)
                    await ReplyAsync("That keyword was not ignored in that server. No change made.");
                else
                {
                    var channel = (await Context.Client.GetChannelAsync(channelId)) as SocketTextChannel;
                    await ReplyAsync($"Unignored keyword `{keyword}` in channel {channel.Mention} (server {channel.Guild.Name}).");
                }
            }
        }
    }
}
