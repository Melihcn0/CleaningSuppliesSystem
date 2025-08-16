using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Customer")]
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerProfileController : ControllerBase
    {
        private readonly ICustomerProfileService _customerProfileService;

        public CustomerProfileController(ICustomerProfileService customerProfileService)
        {
            _customerProfileService = customerProfileService;
        }

        // Kullanıcının profilini getir
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _customerProfileService.TGetProfileAsync();
            if (profile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(profile);
        }

        // Güncelleme için DTO'yu getir
        [HttpGet("UpdateCustomerProfile")]
        public async Task<IActionResult> GetUpdateCustomerProfile()
        {
            var updateCustomerProfile = await _customerProfileService.TGetUpdateCustomerProfileAsync();
            if (updateCustomerProfile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(updateCustomerProfile);
        }

        // Profil güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerProfileDto dto)
        {
            // AppUserId zaten DTO'da var, servis kendi HttpContext’inden user'ı buluyor
            var (isSuccess, message, updatedId) = await _customerProfileService.TUpdateCustomerProfileAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Müşteri profili başarıyla güncellendi.", id = updatedId });
        }
    }
}
