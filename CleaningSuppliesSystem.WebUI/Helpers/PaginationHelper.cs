using CleaningSuppliesSystem.WebUI.Models;
using System.Text.Json;

namespace CleaningSuppliesSystem.WebUI.Helpers
{
    public class PaginationHelper
    {
        private readonly HttpClient _client;

        public PaginationHelper(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<PagedResponse<T>> GetPagedDataAsync<T>(string url)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await _client.GetFromJsonAsync<PagedResponse<T>>(url, options);
        }
    }
}
