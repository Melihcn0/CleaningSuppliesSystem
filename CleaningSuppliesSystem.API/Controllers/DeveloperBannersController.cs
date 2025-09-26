using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Authorize(Roles = "Developer")]
    [Tags("Home - Banner")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperBannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;

        public DeveloperBannersController(IBannerService bannerService, IMapper mapper)
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
                return BadRequest("Banner silinmiş değil. Önce silmeniz gerekir.");

            await _bannerService.TDeleteAsync(id);
            return Ok("Banner çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int bannerId, [FromQuery] bool newStatus)
        {
            var result = await _bannerService.ToggleBannerStatusAsync(bannerId, newStatus);
            if (!result)
                return BadRequest("Banner gösterim durumu güncellenemedi.");

            return Ok("Banner gösterim durumu güncellendi.");
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBanner(int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.TGetActiveBannersAsync();
            var totalCount = banners.Count;

            var pagedData = banners
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultBannerDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedBanner(int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.TGetDeletedBannersAsync();
            var totalCount = banners.Count;

            var pagedData = banners
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultBannerDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

    }
}
