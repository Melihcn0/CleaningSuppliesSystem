using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ServiceValidatorDto;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers
{
    [Authorize(Roles = "Developer")]
    [Area("Developer")]
    public class ServiceController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public ServiceController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        private async Task LoadUnusedServiceIconsDropdownAsync(int? selectedIconId = null)
        {
            var icons = await _client.GetFromJsonAsync<List<ResultServiceIconDto>>("developerServiceIcons");
            ViewBag.serviceIcons = new SelectList(icons, "Id", "IconName", selectedIconId);
        }


        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultServiceDto>(
                $"developerServices/active?page={page}&pageSize={pageSize}");

            return View(response);
        }
        public async Task<IActionResult> DeletedService(int page = 1, int pageSize = 10)
        {
            ViewBag.ShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultServiceDto>(
                $"developerServices/deleted?page={page}&pageSize={pageSize}");

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteService(int id)
        {
            var response = await _client.PostAsync($"developerServices/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeleteService(int id)
        {
            var response = await _client.PostAsync($"developerServices/undosoftdelete/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Hizmet geri alındı.";
            }
            else
            {
                TempData["ErrorMessage"] = "Geri alma işlemi başarısız.";
            }

            return RedirectToAction(nameof(DeletedService));
        }
        public async Task<IActionResult> CreateService()
        {
            ViewBag.ShowBackButton = true;
            await LoadUnusedServiceIconsDropdownAsync();
            return View();
        }
        public async Task<IActionResult> UpdateService(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateServiceDto>($"developerServices/{id}");
            ViewBag.ShowBackButton = true;
            await LoadUnusedServiceIconsDropdownAsync(value.ServiceIconId);
            if (value == null) return NotFound();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(CreateServiceDto createServiceDto)
        {
            var iconResponse = await _client.GetAsync($"developerServices/{createServiceDto.ServiceIconId}");
            if (iconResponse.IsSuccessStatusCode)
            {
                var iconJson = await iconResponse.Content.ReadAsStringAsync();
                var icon = JsonConvert.DeserializeObject<ResultServiceIconDto>(iconJson);

                if (icon != null && icon.IsShown)
                {
                    var servicesResponse = await _client.GetAsync("developerServices");
                    if (servicesResponse.IsSuccessStatusCode)
                    {
                        var servicesJson = await servicesResponse.Content.ReadAsStringAsync();
                        var allServices = JsonConvert.DeserializeObject<List<ResultServiceDto>>(servicesJson);

                        int relatedCount = allServices.Count(x => x.ServiceIconId == icon.Id);

                        if (relatedCount >= 5)
                        {
                            ModelState.AddModelError("ServiceIconId", "Bu ikonla en fazla 5 hizmet ilişkilendirilebilir.");
                            await LoadUnusedServiceIconsDropdownAsync();
                            return View(createServiceDto);
                        }
                    }
                }
            }

            var validator = new CreateServiceValidator();
            var result = await validator.ValidateAsync(createServiceDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await LoadUnusedServiceIconsDropdownAsync();
                return View(createServiceDto);
            }

            var response = await _client.PostAsJsonAsync("developerServices", createServiceDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Hizmet oluşturma işlemi başarısız.");
                await LoadUnusedServiceIconsDropdownAsync();
                return View(createServiceDto);
            }

            TempData["SuccessMessage"] = "Hizmet başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateService(UpdateServiceDto updateServiceDto)
        {
            var validator = new UpdateServiceValidator();
            var result = await validator.ValidateAsync(updateServiceDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await LoadUnusedServiceIconsDropdownAsync(updateServiceDto.ServiceIconId);
                return View(updateServiceDto);
            }

            var response = await _client.PutAsJsonAsync("developerServices", updateServiceDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Hizmet güncelleme işlemi başarısız.");
                await LoadUnusedServiceIconsDropdownAsync();
                return View(updateServiceDto);
            }

            TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteService(int id)
        {
            var response = await _client.DeleteAsync($"developerServices/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? Ok(content) : BadRequest(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int serviceId, bool newStatus)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"developerServices/togglestatus?serviceId={serviceId}&newStatus={newStatus}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorMessage;
            }
            else
            {
                TempData["SuccessMessage"] = "Hizmet durumu başarıyla güncellendi.";
            }

            return RedirectToAction("Index");
        }


    }
}
