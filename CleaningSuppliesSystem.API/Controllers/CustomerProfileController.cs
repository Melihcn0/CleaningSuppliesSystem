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
    public class CustomerProfileController(ICustomerProfileService _customerProfileService) : ControllerBase
    {
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _customerProfileService.TGetProfileAsync();
            if (profile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(profile);
        }

        [HttpGet("UpdateCustomerProfile")]
        public async Task<IActionResult> GetUpdateCustomerProfile()
        {
            var updateCustomerProfile = await _customerProfileService.TGetUpdateCustomerProfileAsync();
            if (updateCustomerProfile == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(updateCustomerProfile);
        }

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
