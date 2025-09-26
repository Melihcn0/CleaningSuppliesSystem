using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RoleValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Developer.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers
{
    [Authorize(Roles = "Developer")]
    [Area("Developer")]
    public class RoleAssignController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public RoleAssignController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }

        public async Task<IActionResult> RolesIndex(int page = 1, int pageSize = 10)
        {
            // API'den paged veri al
            var response = await _paginationHelper.GetPagedDataAsync<UserListDto>(
                $"roleassign/users-excluding-developers?page={page}&pageSize={pageSize}");

            // DTO'dan ViewModel'e maple
            var viewModelList = response.Data.Select(u => new UserViewModel
            {
                Id = u.Id,
                NameSurname = u.NameSurname,
                UserName = u.UserName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive,
                Role = u.Role ?? new List<string>(), // Rol listesi boş ise hata vermemesi için
                AllRoles = new List<string> { "Admin", "Customer", "Developer" } // Opsiyonel
            }).ToList();

            // PagedResponse<UserViewModel> oluştur
            var pagedViewModel = new PagedResponse<UserViewModel>
            {
                Data = viewModelList,
                TotalCount = response.TotalCount,
                Page = response.Page,
                PageSize = response.PageSize,
                TotalPages = response.TotalPages
            };

            return View(pagedViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(List<AssignRoleDto> assignRoleList, int userId)
        {
            var dto = new UserAssignRoleDto
            {
                UserId = userId,
                Roles = assignRoleList.Where(r => r.RoleExist).ToList()
            };

            var validator = new UserAssignRoleValidator();
            var result = await validator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => e.ErrorMessage));
                return RedirectToAction("RolesIndex");
            }

            var response = await _client.PostAsJsonAsync("roleassign/assignrole", dto);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = msg;
            else
                TempData["ErrorMessage"] = msg;

            return RedirectToAction("RolesIndex");
        }

    }
}
