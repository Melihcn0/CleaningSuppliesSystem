using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.AdminProfileDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Admin")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProfilesController : ControllerBase
    {
        private readonly IAdminProfileService _adminProfileService;

        public AdminProfilesController(IAdminProfileService adminProfileService)
        {
            _adminProfileService = adminProfileService;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _adminProfileService.TGetAdminProfileAsync();
            if (profile == null) return Ok(new AdminProfileDto());

            return Ok(profile);
        }

        [HttpGet("UpdateAdminProfile")]
        public async Task<IActionResult> GetUpdateAdminProfile()
        {
            var updateAdminProfile = await _adminProfileService.TGetUpdateAdminProfileAsync();
            if (updateAdminProfile == null) return Ok(new UpdateAdminProfileDto());

            return Ok(updateAdminProfile);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAdminProfileDto dto)
        {
            var (isSuccess, message, updatedId) = await _adminProfileService.TUpdateAdminProfileAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Yetkili profili başarıyla güncellendi.", id = updatedId });
        }
    }
}
