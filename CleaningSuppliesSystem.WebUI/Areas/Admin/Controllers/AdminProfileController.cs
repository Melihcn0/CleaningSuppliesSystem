using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.AdminProfileValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CompanyAddressValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CompanyBankValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using FluentValidation;
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

        private async Task<AdminProfileViewModel?> GetAdminProfileViewModel(UpdateAdminProfileDto? updateProfileDto = null, UpdateCompanyAddressDto? updateCompanyAddressDto = null, UpdateCompanyBankDto? updateCompanyBankDto = null)
        {
            var profileDto = await _client.GetFromJsonAsync<AdminProfileDto>("AdminProfiles/Profile");
            var addressDto = await _client.GetFromJsonAsync<CompanyAddressDto>("CompanyAddresses/Address");
            var bankDto = await _client.GetFromJsonAsync<CompanyBankDto>("CompanyBanks/Bank");

            var updateAdminDto = await _client.GetFromJsonAsync<UpdateAdminProfileDto>("AdminProfiles/UpdateAdminProfile");
            var updateAddressDto = await _client.GetFromJsonAsync<UpdateCompanyAddressDto>("CompanyAddresses/UpdateCompanyAddress");
            var updateBankDto = await _client.GetFromJsonAsync<UpdateCompanyBankDto>("CompanyBanks/UpdateCompanyBank");

            return new AdminProfileViewModel
            {
                AdminProfile = profileDto ?? new AdminProfileDto(),
                CompanyAddress = addressDto ?? new CompanyAddressDto(),
                CompanyBank = bankDto ?? new CompanyBankDto(),
                UpdateAdminProfile = updateAdminDto ?? new UpdateAdminProfileDto(),
                UpdateCompanyAddress = updateAddressDto ?? new UpdateCompanyAddressDto(),
                UpdateCompanyBank = updateBankDto ?? new UpdateCompanyBankDto()
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
                return View("Index", await GetAdminProfileViewModel( dto));
            }

            var response = await _client.PutAsJsonAsync("AdminProfiles", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yetkili profili başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Yetkili profili güncellenemedi.";
            ViewBag.ActiveTab = "pills-edit-profile";
            return View("Index", await GetAdminProfileViewModel(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddressProfile(UpdateCompanyAddressDto dto)
        {
            var validator = new UpdateCompanyAddressValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-address";
                return View("Index", await GetAdminProfileViewModel(null, dto));
            }

            var response = await _client.PutAsJsonAsync("CompanyAddresses", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şirket adres bilgisi başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Şirket adresi güncellenemedi.";
            ViewBag.ActiveTab = "pills-edit-address";
            return View("Index", await GetAdminProfileViewModel(null, dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBankProfile(UpdateCompanyBankDto dto)
        {
            var validator = new UpdateCompanyBankValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-address";
                return View("Index", await GetAdminProfileViewModel(updateCompanyBankDto: dto));
            }

            var response = await _client.PutAsJsonAsync("CompanyBanks", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şirket banka bilgisi başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-bank";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Şirket banka bilgisi güncellenemedi.";
            ViewBag.ActiveTab = "pills-edit-bank";
            return View("Index", await GetAdminProfileViewModel(updateCompanyBankDto: dto));
        }
    

    }
}
