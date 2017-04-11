using System;

namespace WazeBotDiscord.Classes
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

        public ulong SetOnServerId { get; set; }

        public DateTime SetAt { get; set; }
    }

    public enum WazeRole
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        AreaManager,
        LargeAreaManager,
        StateManager,
        CountryManager,
        LocalChamp,
        GlobalChamp,
        AssistantRegionalCoordinator,
        RegionalCoordinator
    }
}
