using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - Secondary Banner")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SecondaryBannersController : ControllerBase
    {
        private readonly ISecondaryBannerService _secondaryBannerService;
        private readonly IMapper _mapper;

        public SecondaryBannersController(ISecondaryBannerService secondaryBannerService, IMapper mapper)
        {
            _secondaryBannerService = secondaryBannerService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var secondaryBanners = await _secondaryBannerService.TGetListAsync();
            var result = _mapper.Map<List<ResultSecondaryBannerDto>>(secondaryBanners);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var secondaryBanner = await _secondaryBannerService.TGetByIdAsync(id);
            if (secondaryBanner == null)
                return NotFound("İkincil banner bulunamadı.");

            var result = _mapper.Map<ResultSecondaryBannerDto>(secondaryBanner);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateSecondaryBannerDto dto)
        {
            var result = await _secondaryBannerService.TCreateSecondaryBannerAsync(dto);
            return result.IsSuccess
                ? Ok(new { result.Message, result.CreatedId })
                : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateSecondaryBannerDto dto)
        {
            var result = await _secondaryBannerService.TUpdateSecondaryBannerAsync(dto);
            return result.IsSuccess
                ? Ok(new { result.Message, result.UpdatedId })
                : BadRequest(result.Message);
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _secondaryBannerService.TSoftDeleteSecondaryBannerAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _secondaryBannerService.TUndoSoftDeleteSecondaryBannerAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var category = await _secondaryBannerService.TGetByIdAsync(id);
            if (category == null)
                return NotFound("İkincil banner alanı bulunamadı.");

            if (!category.IsDeleted)
                return BadRequest("İkincil banner alanı silinmiş değil. Önce silmeniz gerekir.");

            await _secondaryBannerService.TDeleteAsync(id);
            return Ok("İkincil banner alanı çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int secondaryBannerId, [FromQuery] bool newStatus)
        {
            var result = await _secondaryBannerService.ToggleSecondaryBannerStatusAsync(secondaryBannerId, newStatus);
            if (!result)
                return BadRequest("İkincil banner gösterim durumu güncellenemedi.");

            return Ok("İkincil banner gösterim durumu güncellendi.");
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSecondaryBanner()
        {
            var result = await _secondaryBannerService.TGetActiveSecondaryBannersAsync();
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedSecondaryBanner()
        {
            var result = await _secondaryBannerService.TGetDeletedSecondaryBannersAsync();
            return Ok(result);
        }

    }
}
