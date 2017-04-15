using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WazeBotDiscord.Twitter
{
    public class TwitterToCheck
    {
        public long UserId { get; set; }

        public string FriendlyUsername { get; set; }

        public int Frequency { get; set; }

        public long DiscordGuildId { get; set; }

        public long DiscordChannelId { get; set; }
        
        public List<string> RequiredKeywords { get; set; }

        public Regex RequiredKeywordsMatch { get; set; }

        public string RequiredKeywordsValue
        {
            get
            {
                var tmp = string.Join("||", RequiredKeywords);
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
                    value.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
    }
}
