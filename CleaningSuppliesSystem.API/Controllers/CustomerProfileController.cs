using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _customerProfileService.TGetProfileAsync();
            if (profile == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerProfileDto updateCustomerProfileDto)
        {
            var (isSuccess, message, updatedId) = await _customerProfileService.TUpdateCustomerProfileAsync(updateCustomerProfileDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Müşteri profili başarıyla güncellendi.", id = updatedId });
        }

    }
}
