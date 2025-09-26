using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeBannerComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeBannerComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var banners = await _client.GetFromJsonAsync<List<ResultBannerDto>>("developerbanners");
            var activeBanner = banners?.FirstOrDefault(b => b.IsShown);

            return View(activeBanner);
        }

    }
}
