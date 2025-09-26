using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Developer")]
    [Authorize(Roles = "Developer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperProfilesController : ControllerBase
    {
        private readonly IDeveloperProfileService _developerProfileService;

        public DeveloperProfilesController(IDeveloperProfileService developerProfileService)
        {
            _developerProfileService = developerProfileService;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _developerProfileService.TGetDeveloperProfileAsync();
            if (profile == null) return Ok(new DeveloperProfileDto());

            return Ok(profile);
        }

        [HttpGet("UpdateDeveloperProfile")]
        public async Task<IActionResult> GetUpdateDeveloperProfile()
        {
            var updateAdminProfile = await _developerProfileService.TGetUpdateDeveloperProfileAsync();
            if (updateAdminProfile == null) return Ok(new UpdateDeveloperProfileDto());

            return Ok(updateAdminProfile);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDeveloperProfileDto dto)
        {
            var (isSuccess, message, updatedId) = await _developerProfileService.TUpdateDeveloperProfileAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Geliştirici profili başarıyla güncellendi.", id = updatedId });
        }
    }
}
