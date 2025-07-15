using GuestBookApp.Core.Models;
using System.Net.Http.Json;

namespace GuestBookApp.Web.Services
{
    public class GuestBookApiClient
    {
        private readonly HttpClient _httpClient;

        public GuestBookApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GuestBookEntry>?> GetGuestBookEntriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GuestBookEntry>>("/guestbook");
        }

        public async Task<GuestBookEntry?> AddGuestBookEntryAsync(GuestBookEntry entry)
        {
            var response = await _httpClient.PostAsJsonAsync("/guestbook", entry);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GuestBookEntry>();
        }
    }
}