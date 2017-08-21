using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class AreaManager
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [300471946494214146] = 300572092187934721, // National
            [313435914540154890] = 313436199157235713, // Northwest
            [301113669696356352] = 301115538556321802, // Southwest
            [313433524130545664] = 313433787583430656, // Plains
            [313431377724964876] = 313431851194908683, // South Central
            [299563059695976451] = 299565817471696897, // Great Lakes
            [300737538384199681] = 300757047534813194, // South Atlantic
            [313428729739083776] = 313429074771181570, // Southeast
            [300482201198395417] = 313425629833854989, // New England
            [300481818619150336] = 302528840901591041, // Northeast
            [299676784327393281] = 299678085706547210, // Mid Atlantic
            [347386780074377217] = 347412734951817218  // Map Raid
        };
    }
}
