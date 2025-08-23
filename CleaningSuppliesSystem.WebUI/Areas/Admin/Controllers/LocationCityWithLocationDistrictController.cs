using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LocationCityWithLocationDistrictController : Controller
    {
        private readonly HttpClient _client;
        public LocationCityWithLocationDistrictController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        public async Task<IActionResult> Index()
        {
            var resultDtos = await _client.GetFromJsonAsync<List<ResultLocationCityWithLocationDistrictDto>>("locationCities/all-cityWithdistrict");

            var vm = new LocationCityViewModel
            {
                CityWithDistrictViewList = resultDtos
            };

            return View(vm);
        }
    }
}
