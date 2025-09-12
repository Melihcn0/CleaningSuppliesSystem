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

            var shownCategories = allCategories?
                .Where(c => c.IsShown && !c.IsDeleted)
                .OrderBy(_ => rnd.Next())   // önce karıştır
                .Take(20)                   // 20 tanesini al
                .ToList() ?? new List<ResultCategoryDto>();

            // Bu 20 kategoriden 10 tanesini tekrar rastgele seç
            var latestCategories = shownCategories
                .OrderBy(_ => rnd.Next())
                .Take(10)
                .ToList();


            return View(latestCategories);
        }
    }
}