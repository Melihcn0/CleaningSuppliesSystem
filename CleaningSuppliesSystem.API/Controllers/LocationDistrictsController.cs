using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
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
            var district = await _locationDistrictService.TGetListAsync();
            var result = _mapper.Map<List<ResultLocationDistrictDto>>(district);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDistrict()
        {
            var result = await _locationDistrictService.TGetActiveLocationDistrictsAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedDistrict()
        {
            var result = await _locationDistrictService.TGetDeletedLocationDistrictsAsync();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDistrictDto createLocationDistrictDto)
        {
            var (isSuccess, message, createdId) = await _locationDistrictService.TCreateLocationDistrictAsync(createLocationDistrictDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "İlçe lokasyonu başarıyla oluşturuldu.", id = createdId });
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
                return BadRequest("İlçe lokasyonu soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _locationDistrictService.TDeleteAsync(id);
            return Ok("İlçe lokasyonu kalıcı olarak silindi.");
        }

        [HttpGet("GetCities/{cityId}")]
        public async Task<IActionResult> GetByCity(int cityId)
        {
            var categories = await _locationDistrictService.TGetActiveByCityIdAsync(cityId);
            return Ok(categories);
        }
    }
}
