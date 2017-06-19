using System.Collections.Generic;

namespace WazeBotDiscord.Keywords
{
    public class KeywordMatch
    {
        public KeywordMatch() { }

        public KeywordMatch(ulong userId, string matchedKeyword)
        {
            UserId = userId;
            MatchedKeywords = new List<string> { matchedKeyword };
        }

        public ulong UserId { get; set; }

        public List<string> MatchedKeywords { get; set; }
    }
}
