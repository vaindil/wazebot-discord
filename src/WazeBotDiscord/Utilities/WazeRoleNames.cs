using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WazeBotDiscord.Utilities
{
    public static class WazeRoleNames
    {
        public static readonly IReadOnlyCollection<string> RoleNames = new ReadOnlyCollection<string>(new List<string>
        {
            "Admin",
            "Admin Keyholder",
            "Regional Coordinator",
            "Assistant Regional Coordinator",
            "Global Champ",
            "Local Champ",
            "Bot",
            "Country Manager",
            "State Manager",
            "Large Area Manager",
            "Area Manager",
            "Mentor",
            "Level 6",
            "Level 5",
            "Level 4",
            "Level 3",
            "Level 2",
            "Level 1"
        });
    }
}
