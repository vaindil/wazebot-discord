using System;
using System.Collections.Generic;

namespace WazeBotDiscord.Glossary
{
    public class GlossaryItem
    {
        public List<string> Ids { get; set; }

        public string Term { get; set; }

        public string Alternates { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
