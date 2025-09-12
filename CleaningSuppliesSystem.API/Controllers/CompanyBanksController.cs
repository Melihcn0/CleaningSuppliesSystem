using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Admin")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyBanksController(ICompanyBankService _companyBankService) : ControllerBase
    {
        [HttpGet("Bank")]
        public async Task<IActionResult> GetAddress()
        {
            var address = await _companyBankService.TGetCompanyBankAsync();
            if (address == null) return Ok(new CompanyBankDto());

            return Ok(address);
        }

        [HttpGet("UpdateCompanyBank")]
        public async Task<IActionResult> GetUpdateCompanyAddress()
        {
            var updateCompanyAddress = await _companyBankService.TGetUpdateCompanyBankAsync();
            if (updateCompanyAddress == null) return Ok(new UpdateCompanyBankDto());

            return Ok(updateCompanyAddress);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyBankDto dto)
        {
            var (isSuccess, message, updatedId) = await _companyBankService.TUpdateCompanyBankAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Şirket banka bilgisi başarıyla güncellendi.", id = updatedId });
        }
    }
}