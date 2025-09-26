using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Authorize(Roles = "Admin")]
    [Tags("Home - PromoAlert")]
    [Route("api/[controller]")]
    [ApiController]
    public class PromoAlertsController(IPromoAlertService _promoAlertService, IMapper _mapper) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            var values = await _promoAlertService.TGetAllPromoAlertAsync();

            var totalCount = values.Count;

            var pagedData = values
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultPromoAlertDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("first")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromoAlert()
        {
            var result = await _promoAlertService.TGetFirstPromoAlertAsync();
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promoAlert = await _promoAlertService.TGetByIdAsync(id);
            if (promoAlert == null)
                return NotFound("Ön gösterim bulunamadı.");

            var result = _mapper.Map<ResultPromoAlertDto>(promoAlert);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromoAlertDto createDto)
        {
            var (isSuccess, message, createdId) = await _promoAlertService.TCreatePromoAlertAsync(createDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Ön gösterim başarıyla eklendi.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePromoAlertDto updateDto)
        {
            var (isSuccess, message, updatedId) = await _promoAlertService.TUpdatePromoAlertAsync(updateDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Ön gösterim başarıyla güncellendi.", id = updatedId });
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var result = await _promoAlertService.TPermanentDeletePromoAlertAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int promoAlertId, [FromQuery] bool newStatus)
        {
            var result = await _promoAlertService.TTogglePromoAlertStatusAsync(promoAlertId, newStatus);
            if (!result)
                return BadRequest("Ön gösterim durumu güncellenemedi.");

            return Ok("Ön gösterim durumu güncellendi.");
        }
    }
}
