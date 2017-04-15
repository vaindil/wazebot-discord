using System;

namespace WazeBotDiscord.Autoreplies
{
    public class Autoreply
    {
        /// <summary>
        /// Internal ID of the command.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the channel in which this reply is active. Set to 1 to be global to the guild.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// ID of the guild in which this reply is active. Set to 1 to be global.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The string to trigger the reply with no leading !.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// The reply to send when triggered.
        /// </summary>
        public string Reply { get; set; }

        /// <summary>
        /// User who created the command.
        /// </summary>
        public ulong AddedById { get; set; }

        /// <summary>
        /// Datetime when the command was created.
        /// </summary>
        public DateTime AddedAt { get; set; }
    }
}
