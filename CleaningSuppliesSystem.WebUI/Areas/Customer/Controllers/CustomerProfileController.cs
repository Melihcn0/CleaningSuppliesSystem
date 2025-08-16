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

        // Profil ve adresleri görüntüle
        public async Task<IActionResult> Index()
        {
            var model = await GetCustomerProfileViewModel();
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

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

                var model = await GetCustomerProfileViewModel(dto: dto);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("CustomerProfile", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri profili başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Müşteri profili güncellenemedi.";
            var errorModel = await GetCustomerProfileViewModel(dto: dto);
            return View("Index", errorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(UpdateCustomerAddressDto newAddress)
        {
            var validator = new UpdateCustomerAddressValidator();
            var validationResult = await validator.ValidateAsync(newAddress);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var model = await GetCustomerProfileViewModel(newAddress: newAddress);
                return View("Index", model);
            }

            var response = await _client.PostAsJsonAsync("CustomerAddress", newAddress);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Adres başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres eklenemedi.";
            var errorModel = await GetCustomerProfileViewModel(newAddress: newAddress);
            return View("Index", errorModel);
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

                var model = await GetCustomerProfileViewModel(updateAddress: updateAddress);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("CustomerAddress", updateAddress);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Adres başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres güncellenemedi.";
            var errorModel = await GetCustomerProfileViewModel(updateAddress: updateAddress);
            return View("Index", errorModel);
        }

        // Profil ve adresleri yükleyen yardımcı metod
        private async Task<CustomerProfileViewModel> GetCustomerProfileViewModel(
        UpdateCustomerProfileDto? dto = null,
        UpdateCustomerAddressDto? newAddress = null,
        UpdateCustomerAddressDto? updateAddress = null)
        {
            var profileDto = await _client.GetFromJsonAsync<CustomerProfileDto>("CustomerProfile/Profile");
            if (profileDto == null) return null;

            var updateDto = dto ?? await _client.GetFromJsonAsync<UpdateCustomerProfileDto>("CustomerProfile/UpdateCustomerProfile");
            var addresses = await _client.GetFromJsonAsync<List<CustomerAddressDto>>($"CustomerAddress/all/{profileDto.Id}");

            return new CustomerProfileViewModel
            {
                CustomerProfile = profileDto,
                UpdateCustomerProfile = updateDto,
                CustomerAddresses = addresses,
                NewAddress = newAddress ?? new UpdateCustomerAddressDto(),
                UpdateAddress = updateAddress ?? new UpdateCustomerAddressDto()
            };
        }

    }
}
