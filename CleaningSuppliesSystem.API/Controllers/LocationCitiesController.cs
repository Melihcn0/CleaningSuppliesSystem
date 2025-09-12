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
    public class LocationCitiesController : ControllerBase
    {
        private readonly ILocationCityService _locationCityService;
        private readonly IMapper _mapper;

        public LocationCitiesController(ILocationCityService locationCityService, IMapper mapper)
        {
            _locationCityService = locationCityService;
            _mapper = mapper;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var city = await _locationCityService.TGetListAsync();
            var result = _mapper.Map<List<ResultLocationCityDto>>(city);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCity()
        {
            var result = await _locationCityService.TGetActiveLocationCitysAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedCity()
        {
            var result = await _locationCityService.TGetDeletedLocationCitysAsync();
            return Ok(result);
        }
        [HttpGet("all-cityWithdistrict")]
        public async Task<IActionResult> GetActiveCityWithDistrict()
        {
            var result = await _locationCityService.TGetLocationCityWithLocationDistrictAsync();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationCityDto createLocationCityDto)
        {
            var (isSuccess, message, createdId) = await _locationCityService.TCreateLocationCityAsync(createLocationCityDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Şehir lokasyonu başarıyla eklendi.", id = createdId });
        }
        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _locationCityService.TSoftDeleteLocationCityAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _locationCityService.TUndoSoftDeleteLocationCityAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var brand = await _locationCityService.TGetByIdAsync(id);
            if (brand == null)
                return NotFound("Şehir lokasyonu bulunamadı.");

            if (!brand.IsDeleted)
                return BadRequest("Şehir lokasyonu soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _locationCityService.TDeleteAsync(id);
            return Ok("Şehir lokasyonu çöp kutusundan kalıcı olarak silindi.");
        }
    }
}
