using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class Region
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [313435914540154890] = 313436232568799232, // Northwest
            [301113669696356352] = 313652543529943040, // Southwest
            [313433524130545664] = 313433884534636545, // Plains
            [313431377724964876] = 313431885181091860, // South Central
            [299563059695976451] = 313399864920899585, // Great Lakes
            [300737538384199681] = 313421038539243533, // South Atlantic
            [313428729739083776] = 313429119868207104, // Southeast
            [300482201198395417] = 313425686192717824, // New England
            [300481818619150336] = 302529188521312257, // Northeast
            [299676784327393281] = 313424101291065356  // Mid Atlantic
        };
    }
}
