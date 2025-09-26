using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeSecondaryBannerComponent : ViewComponent
    {
        private readonly HttpClient _client;
        public _HomeSecondaryBannerComponent(IHttpClientFactory clientFactory)
        {
             _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allBanners = await _client.GetFromJsonAsync<List<ResultSecondaryBannerDto>>("developerSecondarybanners");

            var banner = allBanners?
                .FirstOrDefault(b => b.IsShown && !b.IsDeleted);

            return View(banner);
        }
    }
}
