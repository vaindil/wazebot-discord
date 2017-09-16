using System;
using System.Collections.Generic;
using System.Text;

namespace WazeBotDiscord.Utilities
{
    public static class RestrictedRegion
    {
        public static IReadOnlyList<ulong> Ids = new List<ulong>
        {
            299563059695976451, // GLR
            299676784327393281  // MAR
        };
    }
}
