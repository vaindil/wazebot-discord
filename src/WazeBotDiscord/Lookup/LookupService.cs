using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
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
        
        public async Task ReloadSheetsAsync()
        {
            _sheets.Clear();

            await InitAsync();
        }
    }
}
