using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.PromoAlertValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromoAlertController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;
        private readonly PaginationHelper _paginationHelper;
        public PromoAlertController(IHttpClientFactory clientFactory, IWebHostEnvironment env, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultPromoAlertDto>(
                $"promoAlerts?page={page}&pageSize={pageSize}");

            return View(response);
        }
        public async Task<IActionResult> CreatePromoAlert()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromoAlert(CreatePromoAlertDto dto)
        {
            var validator = new CreatePromoAlertValidator();
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

            var response = await _client.PostAsJsonAsync("promoAlerts", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ön gösterim başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Ön gösterim eklenemedi.";
            return View(dto);
        }

        public async Task<IActionResult> UpdatePromoAlert(int id)
        {
            ViewBag.ShowBackButton = true;

            var promoAlert = await _client.GetFromJsonAsync<UpdatePromoAlertDto>($"promoAlerts/{id}");
            if (promoAlert == null)
                return NotFound();

            return View(promoAlert);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePromoAlert(UpdatePromoAlertDto dto)
        {
            var validator = new UpdatePromoAlertValidator();
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

            var response = await _client.PutAsJsonAsync("promoAlerts", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ön gösterim güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Ön gösterim güncellenemedi.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeletePromoAlert(int id)
        {
            var response = await _client.DeleteAsync($"promoAlerts/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);
            else
                return BadRequest(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int promoAlertId, bool newStatus)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"promoAlerts/togglestatus?promoAlertId={promoAlertId}&newStatus={newStatus}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Ön gösterim durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}
