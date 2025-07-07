using CleaningSuppliesSystem.WebUI.DTOs.CategoryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeCategoryComponent : ViewComponent
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            var activeCategories = values.Where(x => x.IsDeleted == false).ToList();

            return View(activeCategories);
        }
    }
}
