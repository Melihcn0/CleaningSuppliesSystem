using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;
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

        // Kullanıcının profilini getir
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _adminProfileService.TGetAdminProfileAsync();
            if (profile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(profile);
        }

        // Güncelleme için DTO'yu getir
        [HttpGet("UpdateAdminProfile")]
        public async Task<IActionResult> GetUpdateAdminProfile()
        {
            var updateAdminProfile = await _adminProfileService.TGetUpdateAdminProfileAsync();
            if (updateAdminProfile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(updateAdminProfile);
        }

        // Profil güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAdminProfileDto dto)
        {
            // DTO'dan AppUser entity'ye güncelleme yapacak servis metodu
            var (isSuccess, message, updatedId) = await _adminProfileService.TUpdateAdminProfileAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Yetkili profili başarıyla güncellendi.", id = updatedId });
        }
    }
}
