using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Customer.CustomerIndivualAddressDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CustomerProfileValidatorDto;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Areas.Customer.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class CustomerProfileController : Controller
    {
        private readonly HttpClient _client;

        public CustomerProfileController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IActionResult> Index(string activeTab = "pills-blank")
        {
            var model = await GetCustomerProfileViewModel();
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? activeTab;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomerProfile(UpdateCustomerProfileDto dto)
        {
            var validator = new UpdateCustomerProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-profile";
                var model = await GetCustomerProfileViewModel(updateProfileDto: dto);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("CustomerProfile", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri profili başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Müşteri profili güncellenemedi.";
            TempData["ActiveTab"] = "pills-edit-profile";
            var errorModel = await GetCustomerProfileViewModel(updateProfileDto: dto);
            return View("Index", errorModel);
        }

        [HttpGet]
        public IActionResult CreateAddressPartial()
        {
            var model = new CreateCustomerIndivivualAddressDto();
            return View("_CreateAddressPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomerAddress(CreateCustomerIndivivualAddressDto newAddress)
        {
            var validator = new CreateIndivivualAddressValidator();
            var validationResult = await validator.ValidateAsync(newAddress);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-add-address";
                var model = await GetCustomerProfileViewModel(createAddressDto: newAddress);
                return View("Index", model);
            }

            var response = await _client.PostAsJsonAsync("CustomerAddress", newAddress);
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var messageObj = System.Text.Json.JsonDocument.Parse(content);
                var message = messageObj.RootElement.GetProperty("message").GetString();
                var isSuccess = messageObj.RootElement.GetProperty("isSuccess").GetBoolean();

                if (isSuccess)
                    TempData["SuccessMessage"] = message;
                else
                    TempData["ErrorMessage"] = message;
            }
            catch
            {
                TempData["ErrorMessage"] = content;
            }

            TempData["ActiveTab"] = "pills-list-address";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(UpdateCustomerIndivivualAddressDto updateAddress)
        {
            var validator = new UpdateIndivivualAddressValidator();
            var validationResult = await validator.ValidateAsync(updateAddress);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                TempData["ErrorMessage"] = "Adres güncellenemedi.";
                TempData["ActiveTab"] = "pills-list-address";
                var model = await GetCustomerProfileViewModel(updateAddressDto: updateAddress);
                return View("Index", model);
            }


            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Adres başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-list-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres güncellenemedi.";
            TempData["ActiveTab"] = "pills-list-address";
            var errorModel = await GetCustomerProfileViewModel(updateAddressDto: updateAddress);
            return View("Index", errorModel);
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int addressId, bool newStatus)
        {
            var response = await _client.PostAsync($"customerAddress/togglestatus?addressId={addressId}&newStatus={newStatus}", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Adres durumu güncellenirken bir hata oluştu: {error}";
            }

            TempData["ActiveTab"] = "pills-list-address";
            return RedirectToAction(nameof(Index));
        }

        private async Task<CustomerProfileViewModel> GetCustomerProfileViewModel(
            UpdateCustomerProfileDto? updateProfileDto = null,
            CreateCustomerIndivivualAddressDto? createAddressDto = null,
            UpdateCustomerIndivivualAddressDto? updateAddressDto = null)
        {
            var profileDto = await _client.GetFromJsonAsync<CustomerProfileDto>("CustomerProfile/Profile");
            if (profileDto == null) return null;

            var updateDto = updateProfileDto ?? await _client.GetFromJsonAsync<UpdateCustomerProfileDto>("CustomerProfile/UpdateCustomerProfile");
            var indivivualAddresses = await _client.GetFromJsonAsync<List<CustomerIndivivualAddressDto>>($"CustomerAddress/all-indivivual/{profileDto.Id}");
            var corporateAddresses = await _client.GetFromJsonAsync<List<CustomerCorporateAddressDto>>($"CustomerAddress/all-corporate/{profileDto.Id}");

            return new CustomerProfileViewModel
            {
                CustomerProfile = profileDto,
                UpdateCustomerProfile = updateDto,
                CustomerAddresses = addresses,
                CreateCustomerAddress = createAddressDto ?? new CreateCustomerIndivivualAddressDto(),
                UpdateAddress = updateAddressDto ?? new UpdateCustomerIndivivualAddressDto()
            };
        }
    }
}
