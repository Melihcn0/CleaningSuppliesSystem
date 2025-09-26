using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Admin
{
    public class _OrdersNotificationComponent : ViewComponent
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        public _OrdersNotificationComponent(IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            const string cacheKey = "ongoingOrders";

            if (_cache.TryGetValue(cacheKey, out List<ExpiredOrderDto> cachedOrders))
                return View(cachedOrders);

            var allOrders = await _client.GetFromJsonAsync<List<ExpiredOrderDto>>("orders/expired")
                            ?? new List<ExpiredOrderDto>();

            var latestOrders = allOrders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();

            _cache.Set(cacheKey, latestOrders, TimeSpan.FromMinutes(2));

            return View(latestOrders);
        }
    }
}
