using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WazeBotDiscord.Lookup
{
    public class LookupService
    {
        readonly HttpClient _client;
        List<SheetToSearch> _sheets;

        public LookupService(HttpClient client)
        {
            _client = client;
        }

        public async Task InitAsync()
        {
            using (var db = new WbContext())
            {
                _sheets = await db.SheetsToSearch.ToListAsync();
            }
        }

        public string GetChannelSheetUrl(ulong channelId)
        {
            var sheet = _sheets.Find(s => s.ChannelId == channelId);
            if (sheet == null)
                return "This chanel is not configured to search a spreadsheet.";

            return $"<https://docs.google.com/spreadsheets/d/{sheet.SheetId}/edit>";
        }

        public async Task<string> SearchSheetAsync(ulong channelId, string origSearchString)
        {
            var searchString = origSearchString.ToLowerInvariant();

            var sheet = _sheets.Find(s => s.ChannelId == channelId);
            if (sheet == null)
                return "This chanel is not configured to search a spreadsheet.";

            var parser = new HtmlParser();
            var resp = await _client.GetAsync(
                $"https://docs.google.com/spreadsheets/d/{sheet.SheetId}/pubhtml");

            if (!resp.IsSuccessStatusCode)
                return "Spreadsheet is not configured correctly.";

            var doc = await parser.ParseAsync(await resp.Content.ReadAsStringAsync());

            var tblHeader = doc.QuerySelectorAll("table.waffle > tbody > tr:first-of-type");
            var headerRowRaw = tblHeader.FirstOrDefault();
            if (headerRowRaw == null)
                return "Spreadsheet is not configured correctly.";

            var headerRow = (IHtmlTableRowElement)headerRowRaw;
            var headerFields = headerRow.Cells
                .Where(c => !string.IsNullOrWhiteSpace(c.TextContent))
                .Select(c => c.TextContent)
                .ToList();

            var contentRows = doc.QuerySelectorAll("table.waffle > tbody > tr:not(:first-of-type)");
            var matches = new List<List<string>>();

            foreach (var thisRow in contentRows)
            {
                var row = (IHtmlTableRowElement)thisRow;
                var match = false;

                foreach (var cell in row.Cells)
                {
                    if (cell.TextContent.ToLowerInvariant().Contains(searchString))
                    {
                        match = true;
                        break;
                    }
                }

                if (match)
                    matches.Add(row.Cells.Select(c => c.TextContent).ToList());
            }

            return GenerateResult(headerFields, matches, origSearchString);
        }

        string GenerateResult(List<string> headers, List<List<string>> matches, string searchString)
        {
            var result = new StringBuilder();

            var matchCount = matches.Count;
            if (matchCount == 0)
            {
                return $"No results found for `{searchString}`.";
            }
            else if (matchCount > 4)
            {
                matchCount = 4;
                result.Append($"Total of {matches.Count} results for `{searchString}`; only the top four are shown.\n");
            }
            else
                result.Append($"{matchCount} results found for `{searchString}`.\n");

            for (var i = 0; i < matchCount; i++)
            {
                result.AppendLine("```");

                for (var j = 0; j < matches[i].Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(matches[i][j]))
                        continue;

                    result.Append(matches[i][j]);
                    result.Append(" | ");
                }

                result.Remove(result.Length - 3, 3);

                result.AppendLine("```");
            }
            
            return result.ToString();
        }
        
        public async Task ReloadSheetsAsync()
        {
            _sheets.Clear();

            await InitAsync();
        }
    }
}
