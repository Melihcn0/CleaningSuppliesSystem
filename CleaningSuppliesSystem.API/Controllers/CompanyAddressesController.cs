using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddresDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Admin")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyAddressesController : ControllerBase
    {
        private readonly ICompanyAddressService _companyAddressService;

        public CompanyAddressesController(ICompanyAddressService companyAddressService)
        {
            _companyAddressService = companyAddressService;
        }

        [HttpGet("Address")]
        public async Task<IActionResult> GetAddress()
        {
            var profile = await _companyAddressService.TGetCompanyAddressAsync();
            if (profile == null)
                return Ok(new CompanyAddressDto());

            return Ok(profile);
        }


        [HttpGet("UpdateCompanyAddress")]
        public async Task<IActionResult> GetUpdateCompanyAddress()
        {
            var updateCompanyAddress = await _companyAddressService.TGetUpdateCompanyAddressAsync();
            if (updateCompanyAddress == null)
                return Ok(new UpdateCompanyAddressDto()); 

            return Ok(updateCompanyAddress);
        }


        // Profil güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyAddressDto dto)
        {
            var (isSuccess, message, updatedId) = await _companyAddressService.TUpdateCompanyAddressAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Şirket Adresi başarıyla güncellendi.", id = updatedId });
        }
    }
}
