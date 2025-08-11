using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.ViewComponents.Home
{
    public class _HomeServiceComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeServiceComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var services = await _client.GetFromJsonAsync<List<ResultServiceDto>>("services/active");
            var activeServices = services?.Where(s => s.IsShown).Take(5).ToList() ?? new List<ResultServiceDto>();

            return View(activeServices);
        }

    }
}
