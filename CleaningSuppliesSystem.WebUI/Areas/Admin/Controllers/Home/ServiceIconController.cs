using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using Newtonsoft.Json;
using System.Text;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ServiceIconValidatorDto;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers.Home
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ServiceIconController : Controller
    {
        private readonly HttpClient _client;

        public ServiceIconController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IActionResult> Index()
        {
            var serviceIcons = await _client.GetFromJsonAsync<List<ResultServiceIconDto>>("ServiceIcons/active");
            return View(serviceIcons);
        }

        public async Task<IActionResult> DeletedServiceIcon()
        {
            ViewBag.ShowBackButton = true;
            var deletedIcons = await _client.GetFromJsonAsync<List<ResultServiceIconDto>>("ServiceIcons/deleted");
            return View(deletedIcons);
        }

        [HttpGet]
        public IActionResult CreateServiceIcon()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
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
            var response = await _client.PostAsync("ServiceIcons", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Icon başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Icon oluşturulamadı: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateServiceIcon(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateServiceIconDto>($"ServiceIcons/{id}");
            if (dto == null) return NotFound();
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpPost]
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
            var response = await _client.PutAsync("ServiceIcons", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Icon başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Icon güncellenemedi: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteServiceIcon(int id)
        {
            var response = await _client.PostAsync($"ServiceIcons/softdelete/{id}", null);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPost]
        public async Task<IActionResult> UndoSoftDeleteServiceIcon(int id)
        {
            var response = await _client.PostAsync($"ServiceIcons/undosoftdelete/{id}", null);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Icon geri alındı.";
                return RedirectToAction(nameof(DeletedServiceIcon));
            }

            TempData["ErrorMessage"] = "Geri alma işlemi başarısız.";
            return RedirectToAction(nameof(DeletedServiceIcon));
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteServiceIcon(int id)
        {
            var responseGet = await _client.GetAsync($"ServiceIcons/{id}");
            if (!responseGet.IsSuccessStatusCode)
                return BadRequest("Icon bilgisi alınamadı.");

            var responseDelete = await _client.DeleteAsync($"ServiceIcons/permanent/{id}");
            var message = await responseDelete.Content.ReadAsStringAsync();

            if (!responseDelete.IsSuccessStatusCode)
                return BadRequest(message);

            TempData["SuccessMessage"] = "Icon kalıcı olarak silindi.";
            return Ok(message);
        }
    }
}
