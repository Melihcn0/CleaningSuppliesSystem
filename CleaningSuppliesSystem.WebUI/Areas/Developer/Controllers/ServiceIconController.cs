using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using Newtonsoft.Json;
using System.Text;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ServiceIconValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers
{
    [Authorize(Roles = "Developer")]
    [Area("Developer")]
    public class ServiceIconController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public ServiceIconController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            ViewBag.ServiceShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultServiceIconDto>(
                $"developerServiceIcons/active?page={page}&pageSize={pageSize}");

            return View(response);
        }

        public async Task<IActionResult> DeletedServiceIcon(int page = 1, int pageSize = 10)
        {
            ViewBag.ShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultServiceIconDto>(
                $"developerServiceIcons/deleted?page={page}&pageSize={pageSize}");

            return View(response);
        }

        [HttpGet]
        public IActionResult CreateServiceIcon()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServiceIcon(CreateServiceIconDto dto)
        {
            var validator = new CreateServiceIconValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                ViewBag.ShowBackButton = true;
                return View(dto);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("developerServiceIcons", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İkon başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"İkon oluşturulamadı: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateServiceIcon(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateServiceIconDto>($"developerServiceIcons/{id}");
            if (dto == null) return NotFound();
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateServiceIcon(UpdateServiceIconDto dto)
        {
            var validator = new UpdateServiceIconValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                ViewBag.ShowBackButton = true;
                return View(dto);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("developerServiceIcons", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İkon başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"İkon güncellenemedi: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteServiceIcon(int id)
        {
            var response = await _client.PostAsync($"developerServiceIcons/softdelete/{id}", null);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeleteServiceIcon(int id)
        {
            var response = await _client.PostAsync($"developerServiceIcons/undosoftdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = msg;
            }
            else
            {
                TempData["ErrorMessage"] = msg;
            }

            return RedirectToAction(nameof(DeletedServiceIcon));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteServiceIcon(int id)
        {
            var responseGet = await _client.GetAsync($"developerServiceIcons/{id}");
            if (!responseGet.IsSuccessStatusCode)
                return BadRequest("İkon bilgisi alınamadı.");

            var responseDelete = await _client.DeleteAsync($"developerServiceIcons/permanent/{id}");
            var message = await responseDelete.Content.ReadAsStringAsync();

            if (!responseDelete.IsSuccessStatusCode)
                return BadRequest(message);

            TempData["SuccessMessage"] = "İkon kalıcı olarak silindi.";
            return Ok(message);
        }
    }
}
