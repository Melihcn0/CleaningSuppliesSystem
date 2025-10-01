using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeRecentProductComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeRecentProductComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allProducts = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var rnd = new Random();

            var recentProducts = allProducts?
                .Where(p => p.IsShown && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedDate)
                .Take(10) // Son eklenen 10 ürün
                .OrderBy(_ => rnd.Next()) // Bunları karıştır
                .Take(5) // Rastgele 5 tanesini al
                .ToList() ?? new List<ResultProductDto>();

            return View(recentProducts);
        }
    }
}