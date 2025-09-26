using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LocationValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LocationCityController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;
        public LocationCityController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultLocationCityDto>(
                $"LocationCities/active?page={page}&pageSize={pageSize}");

            return View(response);
        }

        public async Task<IActionResult> DeletedCities(int page = 1, int pageSize = 10)
        {
            ViewBag.ShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultLocationCityDto>(
                $"LocationCities/deleted?page={page}&pageSize={pageSize}");

            return View(response);
        }

        public async Task<IActionResult> CreateCity()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(CreateLocationCityDto dto)
        {
            var validator = new CreateLocationCityValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            var response = await _client.PostAsJsonAsync("locationCities", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şehir başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Şehir eklenemedi.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeletedCity(int id)
        {
            var response = await _client.PostAsync($"locationCities/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeletedCity(int id)
        {
            var response = await _client.PostAsync($"locationCities/undosoftdelete/{id}", null);
            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Şehir lokasyonu başarıyla çöp kutusundan geri alındı.";
            else
                TempData["ErrorMessage"] = "Şehir lokasyonunun çöp kutusundan geri alma işlemi başarısız.";

            return RedirectToAction(nameof(DeletedCities));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteCity(int id)
        {
            var response = await _client.DeleteAsync($"locationCities/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);
            else
                return BadRequest(content);
        }
    }
}
