using System.Collections.Generic;

namespace WazeBotDiscord.Classes.Roles
{
    public static class LargeAreaManager
    {
        public static IReadOnlyDictionary<ulong, ulong> Ids = new Dictionary<ulong, ulong>
        {
            [300471946494214146] = 300571943197736972, // National
            [313435914540154890] = 313436172665749504, // Northwest
            // Southwest
            [313433524130545664] = 313433712148873216, // Plains
            [313431377724964876] = 313431828478558208, // South Central
            [299563059695976451] = 299566346599923713, // Great Lakes
            // South Atlantic
            [313428729739083776] = 313429048275763201, // Southeast
            [300482201198395417] = 313425600046039051, // New England
            [300481818619150336] = 302528706042265602, // Northeast
            [299676784327393281] = 313425600046039051  // Mid Atlantic
        };
    }
}
