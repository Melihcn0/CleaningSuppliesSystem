using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Roles")]
    [Authorize(Roles = "Admin,Developer")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleAssignController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public RoleAssignController(
            IUserService userService,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IHttpContextAccessor contextAccessor,
            IMapper mapper)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        [HttpGet("users-including-developers")]
        [Authorize(Roles = "Admin, Developer")]
        public async Task<IActionResult> GetAllUsersIncludingDevelopers(int page = 1, int pageSize = 10)
        {
            var users = await _userService.TGetAllUsersWithRolesAsync();
            var filtered = users.Where(u => !u.Role.Contains("Developer")).ToList();

            var mapped = _mapper.Map<List<UserListDto>>(filtered);

            var response = new PagedResponse<UserListDto>
            {
                Data = mapped.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = mapped.Count,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(mapped.Count / (double)pageSize)
            };

            return Ok(response);
        }
        [HttpGet("users-excluding-developers")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> GetAllUsersExcludingDevelopers(int page = 1, int pageSize = 10)
        {
            var users = await _userService.TGetAllUsersWithRolesAsync();
            var mapped = _mapper.Map<List<UserListDto>>(users);

            var totalCount = mapped.Count;

            var pagedData = mapped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<UserListDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserForRoleAssign(int id)
        {
            var assignRoleList = await _userService.TGetUserRolesForAssignAsync(id);
            if (assignRoleList == null)
                return NotFound();

            return Ok(assignRoleList);
        }

        [HttpPost("assignrole")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> AssignRole([FromBody] UserAssignRoleDto dto)
        {
            foreach (var role in dto.Roles)
            {
                if (role.RoleExist)
                {
                    var result = await _userService.TAssignRoleToUserAsync(dto.UserId, role.RoleName);
                    if (!result)
                        return Content("Kullanıcıya rol atama işlemi başarısız.", "text/plain");
                }
            }

            return Content("Kullanıcıya başarıyla rol atandı.", "text/plain");
        }

        [HttpPost("togglestatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int userId, [FromQuery] bool newStatus)
        {
            var result = await _userService.TToggleUserStatusAsync(userId, newStatus);
            if (!result)
                return BadRequest("Kullanıcı rol durumu güncellenemedi.");

            return Ok("Kullanıcı rol durumu güncellendi.");
        }
    }
}
