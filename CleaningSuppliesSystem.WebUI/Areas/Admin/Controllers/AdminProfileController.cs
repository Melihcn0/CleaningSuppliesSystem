using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddresDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.AdminProfileValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminProfileController : Controller
    {
        private readonly HttpClient _client;

        public AdminProfileController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        private async Task<AdminProfileViewModel?> GetAdminProfileViewModel(UpdateAdminProfileDto? updateProfileDto = null, UpdateCompanyAddressDto? updateCompanyAddressDto = null)
        {
            // Admin profili çek
            var profileDto = await _client.GetFromJsonAsync<AdminProfileDto>("AdminProfiles/Profile");
            if (profileDto == null) return null;

            // Admin profili çek
            CompanyAddressDto? addressDto = null;
            try {
                addressDto = await _client.GetFromJsonAsync<CompanyAddressDto>("CompanyAddresses/Address");
            }
            catch {
                addressDto = null;
            }

            // Update DTO'ları al
            var updateAdminDto = updateProfileDto
                ?? await _client.GetFromJsonAsync<UpdateAdminProfileDto>("AdminProfiles/UpdateAdminProfile");


            var updateCompanyDto = updateCompanyAddressDto
                ?? await _client.GetFromJsonAsync<UpdateCompanyAddressDto>("CompanyAddresses/UpdateCompanyAddress");

            if (profileDto != null && addressDto != null)
            {
                profileDto.CompanyAddress = addressDto;
            }

            return new AdminProfileViewModel
            {
                AdminProfile = profileDto,
                CompanyAddress = addressDto,
                UpdateAdminProfile = updateAdminDto ?? new UpdateAdminProfileDto(),
                UpdateCompanyAddress = updateCompanyDto ?? new UpdateCompanyAddressDto()
            };

        }


        public async Task<IActionResult> Index(string activeTab = "pills-blank")
        {
            var model = await GetAdminProfileViewModel();
            if (model == null)
                return NotFound("Admin bilgileri bulunamadı.");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? activeTab;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdminProfile(UpdateAdminProfileDto dto)
        {
            var validator = new UpdateAdminProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-profile";
                return View("Index", await GetAdminProfileViewModel(dto));
            }

            var response = await _client.PutAsJsonAsync("AdminProfiles", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yetkili profili başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Yetkili profili güncellenemedi.";
            TempData["ActiveTab"] = "pills-edit-profile";
            return View("Index", await GetAdminProfileViewModel(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddressProfile(UpdateCompanyAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = "pills-edit-address";
                return View("Index", await GetAdminProfileViewModel(null, dto));
            }

            var response = await _client.PutAsJsonAsync("CompanyAddresses", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şirket adresi başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Şirket adresi güncellenemedi.";
            TempData["ActiveTab"] = "pills-edit-address";
            return View("Index", await GetAdminProfileViewModel(null, dto));
        }

    }
}
