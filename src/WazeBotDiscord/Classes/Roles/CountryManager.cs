using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class CountryManager
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [300471946494214146] = 300493275037564929, // National
            [313435914540154890] = 313436128747454476, // Northwest
            [301113669696356352] = 301115472953475072, // Southwest
            [313433524130545664] = 313433692934635523, // Plains
            [313431377724964876] = 313431784572452865, // South Central
            [299563059695976451] = 299582351904866336, // Great Lakes
            [300737538384199681] = 300756688171040779, // South Atlantic
            [313428729739083776] = 313428992436731916, // Southeast
            [300482201198395417] = 313425535524929537, // New England
            [300481818619150336] = 302528524802195457, // Northeast
            [299676784327393281] = 299677234787123200, // Mid Atlantic
            [347386780074377217] = 347412542626201601  // Map Raid
        };
    }
}
