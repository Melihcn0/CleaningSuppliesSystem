using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LocationCityWithLocationDistrictController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;
        public LocationCityWithLocationDistrictController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultLocationCityWithLocationDistrictDto>(
                $"LocationCities/all-cityWithdistrict?page={page}&pageSize={pageSize}");

            return View(response);
        }

    }
}
