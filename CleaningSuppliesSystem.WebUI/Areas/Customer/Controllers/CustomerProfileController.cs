using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CustomerProfileValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Areas.Customer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var resultDto = await _client.GetFromJsonAsync<CustomerProfileDto>("customerProfile");

            var vm = new CustomerProfileViewModel
            {
                CustomerProfile = resultDto
            };

            return View(vm);
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
                return View(dto);
            }

            var response = await _client.PutAsJsonAsync("customerProfile", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri profili başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Müşteri profili güncellenemedi.";
            return View(dto);
        }

    }
}
