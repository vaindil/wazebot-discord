using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class StateManager
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [300471946494214146] = 300493722175668227, // National
            [313435914540154890] = 313436150121627649, // Northwest
            [301113669696356352] = 301115500891734017, // Southwest
            [313433524130545664] = 313433730385444864, // Plains
            [313431377724964876] = 313431806168793099, // South Central
            [299563059695976451] = 299566441751904258, // Great Lakes
            [300737538384199681] = 300755483726446592, // South Atlantic
            [313428729739083776] = 313429019431534596, // Southeast
            [300482201198395417] = 313425574204932116, // New England
            [300481818619150336] = 302528666456555520, // Northeast
            [299676784327393281] = 299677955540647937, // Mid Atlantic
            [347386780074377217] = 347412653221609484  // Map Raid
        };
    }
}
