using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CustomerProfileValidatorDto;
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

        public async Task<IActionResult> Index()
        {
            var model = await GetCustomerProfileViewModel();
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString(); // sadece burada oku
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
                ViewBag.ActiveTab = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Müşteri profili güncellenemedi.";
            ViewBag.ActiveTab = "pills-edit-profile";
            var errorModel = await GetCustomerProfileViewModel(updateProfileDto: dto);
            return View("Index", errorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomerAddress(CreateCustomerAddressDto newAddress)
        {
            var validator = new CreateCustomerAddressValidator();
            var validationResult = await validator.ValidateAsync(newAddress);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-add-address";
                ViewBag.ErrorMessage = "Adres eklenemedi.";
                var model = await GetCustomerProfileViewModel(createAddressDto: newAddress);
                return View("Index", model);
            }

            var response = await _client.PostAsJsonAsync("CustomerAddress", newAddress);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Adres oluşturulamadı.");
                ViewBag.ActiveTab = "pills-add-address";
                var model = await GetCustomerProfileViewModel(createAddressDto: newAddress);
                return View("Index", model);
            }

            // Başarılıysa "Adreslerim" tab'ını aktif yap
            ModelState.Clear();
            ViewBag.ActiveTab = "pills-list-address";
            ViewBag.SuccessMessage = "Adres başarıyla eklendi.";
            var updatedModel = await GetCustomerProfileViewModel();
            return View("Index", updatedModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(UpdateCustomerAddressDto updateAddress)
        {
            var validator = new UpdateCustomerAddressValidator();
            var validationResult = await validator.ValidateAsync(updateAddress);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                TempData["ErrorMessage"] = "Adres güncellenemedi.";
                ViewBag.ActiveTab = "pills-list-address";
                var model = await GetCustomerProfileViewModel(updateAddressDto: updateAddress);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("CustomerAddress", updateAddress);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Adres başarıyla güncellendi.";
                ViewBag.ActiveTab = "pills-list-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres güncellenemedi.";
            ViewBag.ActiveTab = "pills-list-address";
            var errorModel = await GetCustomerProfileViewModel(updateAddressDto: updateAddress);
            return View("Index", errorModel);
        }

        private async Task<CustomerProfileViewModel> GetCustomerProfileViewModel(
            UpdateCustomerProfileDto? updateProfileDto = null,
            CreateCustomerAddressDto? createAddressDto = null,
            UpdateCustomerAddressDto? updateAddressDto = null)
        {
            var profileDto = await _client.GetFromJsonAsync<CustomerProfileDto>("CustomerProfile/Profile");
            if (profileDto == null) return null;

            var updateDto = updateProfileDto ?? await _client.GetFromJsonAsync<UpdateCustomerProfileDto>("CustomerProfile/UpdateCustomerProfile");
            var addresses = await _client.GetFromJsonAsync<List<CustomerAddressDto>>($"CustomerAddress/all/{profileDto.Id}");

            return new CustomerProfileViewModel
            {
                CustomerProfile = profileDto,
                UpdateCustomerProfile = updateDto,
                CustomerAddresses = addresses,
                CreateCustomerProfile = createAddressDto ?? new CreateCustomerAddressDto(),
                UpdateAddress = updateAddressDto ?? new UpdateCustomerAddressDto()
            };
        }
    }
}