using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RoleValidatorDto;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers
{
    [Authorize(Roles = "Developer")]
    [Area("Developer")]
    public class RoleController : Controller
    {
        private readonly HttpClient _client;

        public RoleController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _client.GetFromJsonAsync<List<ResultRoleDto>>("roles");
            var showButton = await _client.GetFromJsonAsync<bool>("roles/show-create-button");

            ViewBag.ShowCreateRoleButton = showButton;
            return View(roles);
        }
        public async Task<IActionResult> CreateRole()
        {
            var existingRoles = await _client.GetFromJsonAsync<List<ResultRoleDto>>("roles");
            var allRoles = new List<string> { "Admin", "Customer", "Developer" };
            var existingRoleNames = existingRoles.Select(r => r.Name).ToList();

            var missingRoles = allRoles.Except(existingRoleNames).ToList();

            ViewBag.SelectableRoles = missingRoles;
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
        {
            var validator = new CreateRoleValidator();
            var validationResult = await validator.ValidateAsync(createRoleDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.SelectableRoles = new List<string>();
                ViewBag.ShowBackButton = true;

                return View(createRoleDto);
            }

            var response = await _client.PostAsJsonAsync("roles", createRoleDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "API Hatası: " + errorMsg);

                ViewBag.SelectableRoles = new List<string>();
                ViewBag.ShowBackButton = true;

                return View(createRoleDto);
            }

            TempData["SuccessMessage"] = "Rol başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
