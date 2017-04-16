using System.Collections.Generic;
using System.Linq;

namespace WazeBotDiscord.Twitter
{
    public class TwitterToCheck
    {
        public int Id { get; set; }

        public long UserId { get; set; }

        public string FriendlyUsername { get; set; }

        public ulong DiscordGuildId { get; set; }

        public ulong DiscordChannelId { get; set; }

        public List<string> RequiredKeywords { get; set; }

        public string RequiredKeywordsValue
        {
            get
            {
                var tmp = string.Join(" ", RequiredKeywords);
                if (string.IsNullOrEmpty(tmp))
                    return null;

                return tmp;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RequiredKeywords = new List<string>();
                    return;
                }

                RequiredKeywords =
                    value.Split(' ').ToList();
            }
        }
    }
}
