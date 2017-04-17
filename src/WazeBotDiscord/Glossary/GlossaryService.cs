using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WazeBotDiscord.Glossary
{
    public class GlossaryService
    {
        readonly HttpClient _httpClient;

        List<GlossaryItem> _items;

        public GlossaryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _items = new List<GlossaryItem>();
        }

        public async Task InitAsync()
        {
            await UpdateGlossaryItemsAsync();
        }

        public GlossaryItem GetGlossaryItem(string term)
        {
            return _items.FirstOrDefault(i => i.Term.ToLower() == term);
        }

        async Task UpdateGlossaryItemsAsync()
        {
            var parser = new HtmlParser();
            var body = await _httpClient.GetStringAsync("https://wazeopedia.waze.com/wiki/USA/Glossary");
            var doc = await parser.ParseAsync(body);

            var tblRows = doc.QuerySelectorAll("tr[valign=\"top\"]");

            _items.Clear();

            foreach (var thisRow in tblRows)
            {
                var row = (IHtmlTableRowElement)thisRow;
                //var row = (HtmlTableRowElement)thisRow;

                var dtString = row.Cells[3].TextContent.Trim();
                dtString = dtString.Split(null)[0];

                var dt = DateTime.ParseExact(dtString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                var alternates = row.Cells[1].TextContent.Trim();
                if (string.IsNullOrEmpty(alternates) || alternates == "~")
                    alternates = "_(none)_";

                _items.Add(new GlossaryItem
                {
                    Term = row.Cells[0].TextContent.Trim(),
                    Alternates = alternates,
                    Description = row.Cells[2].TextContent.Trim(),
                    ModifiedAt = dt
                });
            }
        }
    }
}
