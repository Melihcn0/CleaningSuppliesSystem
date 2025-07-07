using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeRecentProductComponent : ViewComponent
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allProducts = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var random = new Random();
            var selectedProducts = allProducts.Where(x => x.IsDeleted == false).OrderBy(x => random.Next()).Take(Math.Min(15, allProducts.Count)).ToList();
            return View(selectedProducts);
        }
    }
}
