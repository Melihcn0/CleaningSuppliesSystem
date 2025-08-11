using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - Banner")]
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;

        public BannersController(IBannerService bannerService, IMapper mapper)
        {
            _bannerService = bannerService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var banners = await _bannerService.TGetListAsync();
            var result = _mapper.Map<List<ResultBannerDto>>(banners);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _bannerService.TGetByIdAsync(id);
            if (banner == null)
                return NotFound("Banner bulunamadı.");

            var result = _mapper.Map<ResultBannerDto>(banner);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBannerDto dto)
        {
            var result = await _bannerService.TCreateBannerAsync(dto);
            return result.IsSuccess
                ? Ok(new { result.Message, result.CreatedId })
                : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateBannerDto dto)
        {
            var result = await _bannerService.TUpdateBannerAsync(dto);
            return result.IsSuccess
                ? Ok(new { result.Message, result.UpdatedId })
                : BadRequest(result.Message);
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _bannerService.TSoftDeleteBannerAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _bannerService.TUndoSoftDeleteBannerAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var category = await _bannerService.TGetByIdAsync(id);
            if (category == null)
                return NotFound("Banner alanı bulunamadı.");

            if (!category.IsDeleted)
                return BadRequest("Banner soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _bannerService.TDeleteAsync(id);
            return Ok("Banner kalıcı olarak silindi.");
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int bannerId, [FromQuery] bool newStatus)
        {
            var result = await _bannerService.ToggleBannerStatusAsync(bannerId, newStatus);
            if (!result)
                return BadRequest("Banner durumu güncellenemedi.");

            return Ok("Banner durumu güncellendi.");
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBanner()
        {
            var result = await _bannerService.TGetActiveBannersAsync();
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedBanner()
        {
            var result = await _bannerService.TGetDeletedBannersAsync();
            return Ok(result);
        }

    }
}
