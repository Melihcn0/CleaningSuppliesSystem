using AutoMapper;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Services.RoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController(IRoleService _roleService) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task<IActionResult> Index()
        {
            var values = await _roleService.GetAllRolesAsync();
            var showButton = await _roleService.ShouldShowCreateRoleButtonAsync();
            ViewBag.ShowCreateRoleButton = showButton;
            return View(values);
        }
        public async Task<IActionResult> CreateRole()
        {
            var rolesToShow = await _roleService.GetMissingRolesAsync();
            ViewBag.SelectableRoles = rolesToShow;
            ViewBag.ShowBackButton = true;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
        {
            await _roleService.CreateRoleAsync(createRoleDto);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            TempData["SuccessMessage"] = "Rol başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
