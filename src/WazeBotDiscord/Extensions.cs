using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WazeBotDiscord
{
    public static class Extensions
    {
        public static Regex ToWordCheckRegex(this string toCheck)
        {
            return new Regex($"(^|\\s){toCheck}($|\\s)", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public static Regex ToWordCheckRegex(this List<string> toCheck)
        {
            if (toCheck.Count == 0)
                return new Regex("^.*$", RegexOptions.Compiled | RegexOptions.Multiline);

            return new Regex($"(^|\\s)({string.Join("|", toCheck)})($|\\s)", RegexOptions.Compiled | RegexOptions.Multiline);
        }
    }
}
