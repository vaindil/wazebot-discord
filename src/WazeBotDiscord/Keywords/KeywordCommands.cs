using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WazeBotDiscord.Keywords
{
    [Group("keyword")]
    [Alias("keywords", "kwd", "kwds", "subscription", "subscriptions", "sub", "subs")]
    public class KeywordModule : ModuleBase
    {
        readonly KeywordService _kwdSvc;

        public KeywordModule(KeywordService kwdSvc)
        {
            _kwdSvc = kwdSvc;
        }

        [Command]
        [Alias("help")]
        public async Task Help([Remainder]string unused = null)
        {
            await ReplyAsync(_helpLink);
        }

        readonly string _helpLink = "https://vaindil.com/nothinghereyet";

        readonly string _helpMsg =
            "You can subscribe to keywords and the bot will notify you if they're used in a channel that you're in. Usage:\n" +
            "`!keyword sub <keyword>`: Subscribe to a keyword. Example: `!keyword sub testing`\n" +
            "`!keyword unsub <keyword`: Unsubscribe from a keyword.\n" +
            "`!keyword ignore server serverid`: Ignore all keywords from the specified server. " +
            "Example: `!keyword ignore server 12345431`\n" +
            "`!keyword ignore channel channelid`: Ignore all keywords from the specified channel.\n" +
            "`!keyword unignore <server/channel> <server/channel ID>`: Unignore keywords from the specified server/channel.\n" +
            "\n" +
            "You can get the server and channel IDs..." +
            "THIS IS GOING TO BE MOVED EVENTUALLY BUT I'M LEAVING IT HERE FOR THIS COMMIT SO I DON'T HAVE TO DELETE IT.";
    }
}
