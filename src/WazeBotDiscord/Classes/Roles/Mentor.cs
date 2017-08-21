using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class Mentor
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [300471946494214146] = 300495035898658817, // National
            [313435914540154890] = 313436215993171978, // Northwest
            [301113669696356352] = 313653535327649792, // Southwest
            [313433524130545664] = 313433864766750720, // Plains
            [313431377724964876] = 313431865979830274, // South Central
            [299563059695976451] = 313399797396668418, // Great Lakes
            [300737538384199681] = 300763117019725825, // South Atlantic
            [313428729739083776] = 313429093674647552, // Southeast
            [300482201198395417] = 313425649362665472, // New England
            [300481818619150336] = 302528989552181248, // Northeast
            [299676784327393281] = 313424374524936202, // Mid Atlantic
            [347386780074377217] = 347907828792426506  // Map Raid
        };
    }
}
