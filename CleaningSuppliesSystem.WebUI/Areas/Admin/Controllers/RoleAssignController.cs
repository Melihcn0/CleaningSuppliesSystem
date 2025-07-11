using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleAssignController(IUserService _userService, UserManager<AppUser> _userManager, RoleManager<AppRole> _roleManager, IEmailService _emailService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var values = await _userService.GetAllUsersAsync();

            var userList = new List<UserViewModel>();
            foreach (var user in values)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    NameSurname = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Role = roles.ToList()
                });
            }

            return View(userList);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool newStatus)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı.");
                return RedirectToAction("Index");
            }

            user.IsActive = newStatus;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded && newStatus)
            {
                // ✅ Hesap aktif edildiyse bilgilendirme maili gönder
                await _emailService.SendAccountActivationMailAsync(user.UserName, user.Email);
                Console.WriteLine("Aktivasyon maili gönderildi.");
            }

            Console.WriteLine($"Kullanıcının durumu güncellendi: {(newStatus ? "Aktif" : "Pasif")}");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RolesIndex()
        {
            var users = await _userService.GetAllUsersAsync();
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    NameSurname = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Role = roles.ToList(),
                    AllRoles = allRoles
                });
            }

            return View(userList); // View modelin List<UserViewModel> olacak
        }       
        public async Task<IActionResult> RolesIndex2()
        {
            var users = await _userService.GetAllUsersAsync();
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    NameSurname = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Role = roles.ToList(),
                    AllRoles = allRoles
                });
            }

            return View(userList); // View modelin List<UserViewModel> olacak
        }


        public async Task<IActionResult> AssignRole(int id)
        {
            ViewBag.ShowBackButton = true;

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı.");
                return View("Index");
            }

            TempData["userId"] = user.Id;

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var assignRoleList = roles.Select(role => new AssignRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleExist = userRoles.Contains(role.Name)
            }).ToList();

            return View(assignRoleList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string selectedRole, int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("Rol atama işlemi user null.");
                return RedirectToAction("Index");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Önce mevcut tüm rolleri kaldır
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Yeni rolü ata (boş değilse)
            if (!string.IsNullOrEmpty(selectedRole))
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
            }

            Console.WriteLine("Rol atama işlemi başarıyla tamamlandı.");
            return RedirectToAction("RolesIndex", new { id = userId });
        }

    }
}