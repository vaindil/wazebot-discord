using System;

namespace WazeBotDiscord.RoleSyncing
{
    public class SyncedRole
    {
        public ulong UserId { get; set; }

        public int WazeRoleValue
        {
            get
            {
                return (int)WazeRole;
            }
            set
            {
                WazeRole = (WazeRole)Enum.ToObject(typeof(WazeRole), value);
            }
        }

        public WazeRole WazeRole { get; set; }

        public ulong SetById { get; set; }

        public ulong SetInGuildId { get; set; }

        public DateTime SetAt { get; set; }
    }

    public enum WazeRole
    {
        CountryManager = 1,
        StateManager,
        LargeAreaManager,
        AreaManager,
        Mentor,
        Level6,
        Level5,
        Level4,
        Level3,
        Level2,
        Level1
    }

    public enum WazeRoleStatus
    {
        Added,
        Removed
    }
}
