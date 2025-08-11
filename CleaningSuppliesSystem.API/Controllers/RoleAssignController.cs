using CleaningSuppliesSystem.Business.Abstract;
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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleAssignController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public RoleAssignController(
            IUserService userService,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IHttpContextAccessor contextAccessor)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
        }

        // GET api/roleassign
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _userService.GetAllUsersWithRolesAsync();
            return Ok(userList);
        }

        // GET api/roleassign/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserForRoleAssign(int id)
        {
            var assignRoleList = await _userService.GetUserRolesForAssignAsync(id);
            if (assignRoleList == null)
                return NotFound();

            return Ok(assignRoleList);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] UserAssignRoleDto dto)
        {
            foreach (var role in dto.Roles)
            {
                if (role.RoleExist)
                {
                    var result = await _userService.AssignRoleToUserAsync(dto.UserId, role.RoleName);
                    if (!result)
                        return BadRequest("Rol atama işlemi başarısız.");
                }
            }

            return Ok("Rol atama başarılı.");
        }


        // POST api/roleassign/togglestatus
        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int userId, [FromQuery] bool newStatus)
        {
            var result = await _userService.ToggleUserStatusAsync(userId, newStatus);
            if (!result)
                return BadRequest("Kullanıcı durumu güncellenemedi.");

            return Ok("Kullanıcı durumu güncellendi.");
        }

        // GET api/roleassign/rolesindex
        [HttpGet("rolesindex")]
        public async Task<IActionResult> RolesIndex()
        {
            var userList = await _userService.GetAllUsersWithRolesAsync();
            return Ok(userList);
        }
    }
}
