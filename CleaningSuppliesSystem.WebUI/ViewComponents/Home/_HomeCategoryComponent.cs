using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeCategoryComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeCategoryComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allCategories = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            var rnd = new Random();

            var latestCategories = allCategories?
                .Where(c => c.IsShown && !c.IsDeleted)
                .OrderBy(_ => rnd.Next())
                .Take(20)
                .ToList() ?? new List<ResultCategoryDto>();

            return View(latestCategories);
        }
    }
}