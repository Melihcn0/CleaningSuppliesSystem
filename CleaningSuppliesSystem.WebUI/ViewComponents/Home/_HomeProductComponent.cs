using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeProductComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeProductComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allProducts = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");

            var rnd = new Random();
            var products = allProducts?
                .Where(p => p.IsShown && !p.IsDeleted)
                .OrderBy(x => rnd.Next())
                .Take(5)                
                .ToList()
                ?? new List<ResultProductDto>();

            return View(products);
        }
    }
}
