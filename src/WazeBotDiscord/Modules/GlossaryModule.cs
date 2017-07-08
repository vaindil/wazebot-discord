using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WazeBotDiscord.Glossary;

namespace WazeBotDiscord.Modules
{
    [Group("glossary")]
    public class GlossaryModule : ModuleBase
    {
        readonly GlossaryService _glossarySvc;

        public GlossaryModule(GlossaryService glossarySvc)
        {
            _glossarySvc = glossarySvc;
        }

        [Command]
        public async Task Help()
        {
            await ReplyAsync("Use `!glossary term` to search the glossary for that term. Search terms must currently match exactly.");
        }

        [Command]
        public async Task Search([Remainder]string term)
        {
            var item = _glossarySvc.GetGlossaryItem(term.ToLowerInvariant());
            if (item == null)
            {
                await ReplyAsync($"No match for {term}.");
                return;
            }

            var embed = CreateEmbed(item);
            await ReplyAsync("", embed: embed);
        }

        Embed CreateEmbed(GlossaryItem item)
        {
            var embed = new EmbedBuilder()
            {
                Color = new Color(147, 196, 211),
                Title = item.Term,
                Url = $"https://wazeopedia.waze.com/wiki/USA/Glossary#{item.Ids[0]}",
                Description = item.Description,

                Footer = new EmbedFooterBuilder
                {
                    Text = $"Last updated on {item.ModifiedAt.Date.ToString("yyyy-MM-dd")}"
                }
            };

            return embed.Build();
        }
    }
}
