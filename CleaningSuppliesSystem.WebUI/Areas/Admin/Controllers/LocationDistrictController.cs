using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LocationValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LocationDistrictController : Controller
    {
        private readonly HttpClient _client;
        public LocationDistrictController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        private async Task LoadCityDropdownAsync(int? selectedCategoryId = null)
        {
            var categories = await _client.GetFromJsonAsync<List<ResultLocationCityDto>>("locationCities/active");
            ViewBag.cities = new SelectList(categories, "Id", "CityName", selectedCategoryId);
        }

        public async Task<IActionResult> Index()
        {
            var resultDtos = await _client.GetFromJsonAsync<List<ResultLocationDistrictDto>>("LocationDistricts/active");

            var vm = new LocationDistrictViewModel
            {
                DistrictViewList = resultDtos
            };

            return View(vm);
        }

        public async Task<IActionResult> DeletedDistricts()
        {
            ViewBag.ShowBackButton = true;
            var resultdeletedDtos = await _client.GetFromJsonAsync<List<ResultLocationDistrictDto>>("LocationDistricts/deleted");
            var vm = new LocationDistrictViewModel
            {
                DistrictViewList = resultdeletedDtos
            };

            return View(vm);
        }

        public async Task<IActionResult> CreateDistrict()
        {
            await LoadCityDropdownAsync();
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDistrict(CreateLocationDistrictDto dto)
        {
            var validator = new CreateLocationDistrictValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCityDropdownAsync(dto.CityId);
                return View(dto);
            }

            var response = await _client.PostAsJsonAsync("LocationDistricts", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İlçe lokasyonu başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            await LoadCityDropdownAsync(dto.CityId);
            TempData["ErrorMessage"] = "İlçe lokasyonu oluşturulamadı.";
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeletedDistrict(int id)
        {
            var response = await _client.PostAsync($"LocationDistricts/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeletedDistrict(int id)
        {
            var response = await _client.PostAsync($"LocationDistricts/undosoftdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "İlçe Lokasyonu başarıyla geri alındı.";
            else
                TempData["ErrorMessage"] = "Geri alma işlemi başarısız.";

            return RedirectToAction(nameof(DeletedDistricts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteDistrict(int id)
        {
            var response = await _client.DeleteAsync($"LocationDistricts/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);
            else
                return BadRequest(content);
        }
    }
}
