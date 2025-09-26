using CleaningSuppliesSystem.DTO.DTOs.ToggleDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RoleValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ToggleStatusValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Models;
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
        private readonly PaginationHelper _paginationHelper;

        public RoleAssignController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<UserListDto>(
                $"RoleAssign/users-including-developers?page={page}&pageSize={pageSize}");

            if (response == null)
                response = new PagedResponse<UserListDto>
                {
                    Data = new List<UserListDto>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0,
                    TotalPages = 0
                };

            ViewBag.AllRoles = new List<string> { "Admin", "Customer", "Developer" };

            return View(response);
        }

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

            var response = await _client.PostAsync($"roleassign/togglestatus?userId={userId}&newStatus={newStatus}", null);

            return RedirectToAction("Index");
        }


    }
}
