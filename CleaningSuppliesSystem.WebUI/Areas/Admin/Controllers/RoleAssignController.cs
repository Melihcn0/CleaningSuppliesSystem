using CleaningSuppliesSystem.DTO.DTOs.ToggleDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RoleValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ToggleStatusValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleAssignController : Controller
    {
        private readonly HttpClient _client;

        public RoleAssignController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("roleassign");
            if (!response.IsSuccessStatusCode)
                return View(new List<UserListDto>());

            var users = await response.Content.ReadFromJsonAsync<List<UserListDto>>();
            ViewBag.AllRoles = new List<string> { "Admin", "Customer" };

            return View(users);
        }


        // GET: /Admin/RoleAssign/RolesIndex
        public async Task<IActionResult> RolesIndex()
        {
            var response = await _client.GetAsync("roleassign/rolesindex");
            if (!response.IsSuccessStatusCode)
                return View(new List<UserViewModel>());

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }


        [HttpPost]
        public async Task<IActionResult> AssignRole(List<AssignRoleDto> assignRoleList, int userId)
        {
            var dto = new UserAssignRoleDto
            {
                UserId = userId,
                Roles = assignRoleList.Where(r => r.RoleExist).ToList()  // Sadece seçili rolleri al
            };

            var validator = new UserAssignRoleValidator();
            var result = await validator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => e.ErrorMessage));
                return RedirectToAction("RolesIndex");
            }

            var response = await _client.PostAsJsonAsync("roleassign/assignrole", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Rol başarıyla atandı.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rol atama sırasında bir hata oluştu.";
            }

            return RedirectToAction("RolesIndex");
        }




        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool newStatus)
        {
            var dto = new ToggleStatusDto
            {
                UserId = userId,
                NewStatus = newStatus
            };

            var validator = new ToggleStatusValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return RedirectToAction("Index");
            }

            var response = await _client.PostAsJsonAsync($"roleassign/togglestatus?userId={userId}&newStatus={newStatus}", new { });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
