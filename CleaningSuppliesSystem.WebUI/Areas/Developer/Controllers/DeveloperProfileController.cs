using CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.DeveloperProfileValidatorDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers
{
    [Authorize(Roles = "Developer")]
    [Area("Developer")]
    public class DeveloperProfileController : Controller
    {
        private readonly HttpClient _client;

        public DeveloperProfileController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        public async Task<IActionResult> Index(string activeTab = "pills-blank")
        {
            var model = await GetDeveloperProfileViewModel();
            if (model == null)
                return NotFound("Geliştirici bilgileri bulunamadı.");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? activeTab;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDeveloperProfile(UpdateDeveloperProfileDto dto)
        {
            var validator = new UpdateDeveloperProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-profile";
                var model = await GetDeveloperProfileViewModel(updateProfileDto: dto);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("developerProfiles", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Geliştirici profili başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Geliştirici profili güncellenemedi.";
            ViewBag.ActiveTab = "pills-edit-profile";
            var errorModel = await GetDeveloperProfileViewModel(updateProfileDto: dto);
            return View("Index", errorModel);
        }
        private async Task<DeveloperProfileViewModel> GetDeveloperProfileViewModel(UpdateDeveloperProfileDto? updateProfileDto = null)
        {
            var profileDto = await _client.GetFromJsonAsync<DeveloperProfileDto>("developerProfiles/Profile");
            if (profileDto == null) return null;

            var updateDto = updateProfileDto
                ?? await _client.GetFromJsonAsync<UpdateDeveloperProfileDto>("developerProfiles/UpdateDeveloperProfile");

            return new DeveloperProfileViewModel
            {
                DeveloperProfile = profileDto,
                UpdateDeveloperProfile = updateDto
            };
        }


    }
}
