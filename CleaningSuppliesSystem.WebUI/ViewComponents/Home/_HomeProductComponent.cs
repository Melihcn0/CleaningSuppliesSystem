using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeProductComponent : ViewComponent
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allProducts = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var lastFive = allProducts.Where(x => x.IsDeleted == false).OrderByDescending(x => x.CreatedAt).Take(5).ToList();
            return View(lastFive);
        }
    }
}
