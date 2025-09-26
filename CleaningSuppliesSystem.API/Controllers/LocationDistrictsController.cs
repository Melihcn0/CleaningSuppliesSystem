using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Location")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationDistrictsController : ControllerBase
    {
        private readonly ILocationDistrictService _locationDistrictService;
        private readonly IMapper _mapper;

        public LocationDistrictsController(ILocationDistrictService locationDistrictService, IMapper mapper)
        {
            _locationDistrictService = locationDistrictService;
            _mapper = mapper;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var districts = await _locationDistrictService.TGetListAsync();
            var result = _mapper.Map<List<ResultLocationDistrictDto>>(districts);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTopCategories(int page = 1, int pageSize = 10)
        {
            var districts = await _locationDistrictService.TGetActiveLocationDistrictsAsync();
            var totalCount = districts.Count;

            var pagedData = districts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultLocationDistrictDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("active-all")]
        public async Task<IActionResult> GetActiveAllTopCategories()
        {
            var districts = await _locationDistrictService.TGetActiveLocationDistrictsAsync();
            var result = _mapper.Map<List<ResultLocationDistrictDto>>(districts);
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedTopCategories(int page = 1, int pageSize = 10)
        {
            var districts = await _locationDistrictService.TGetDeletedLocationDistrictsAsync();
            var totalCount = districts.Count;

            var pagedData = districts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultLocationDistrictDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDistrictDto createLocationDistrictDto)
        {
            var (isSuccess, message, createdId) = await _locationDistrictService.TCreateLocationDistrictAsync(createLocationDistrictDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "İlçe lokasyonu başarıyla eklendi.", id = createdId });
        }
        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _locationDistrictService.TSoftDeleteLocationDistrictAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _locationDistrictService.TUndoSoftDeleteLocationDistrictAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var brand = await _locationDistrictService.TGetByIdAsync(id);
            if (brand == null)
                return NotFound("İlçe lokasyonu bulunamadı.");

            if (!brand.IsDeleted)
                return BadRequest("İlçe lokasyonu silinmiş değil. Önce silmeniz gerekir.");

            await _locationDistrictService.TDeleteAsync(id);
            return Ok("İlçe lokasyonu çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpGet("GetCities/{cityId}")]
        public async Task<IActionResult> GetByCity(int cityId)
        {
            var categories = await _locationDistrictService.TGetActiveByCityIdAsync(cityId);
            return Ok(categories);
        }
    }
}
