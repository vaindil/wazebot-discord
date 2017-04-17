using System;

namespace WazeBotDiscord.Glossary
{
    public class GlossaryItem
    {
        public string Term { get; set; }

        public string Alternates { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
