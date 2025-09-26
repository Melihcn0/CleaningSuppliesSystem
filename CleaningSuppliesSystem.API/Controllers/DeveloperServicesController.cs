using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - Service")]
    [Authorize(Roles = "Developer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public DeveloperServicesController(
            IServiceService serviceService,
            IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var values = await _serviceService.TGetActiveServicesAsync();
            var result = _mapper.Map<List<ResultServiceDto>>(values);
            return Ok(result);
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveService(int page = 1, int pageSize = 10)
        {
            var services = await _serviceService.TGetActiveServicesAsync();
            var totalCount = services.Count;

            var pagedData = services
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultServiceDto>
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
        public async Task<IActionResult> GetDeletedService(int page = 1, int pageSize = 10)
        {
            var services = await _serviceService.TGetDeletedServicesAsync();
            var totalCount = services.Count;

            var pagedData = services
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultServiceDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _serviceService.TGetByIdAsync(id);
            if (value == null)
                return NotFound("Hizmet bulunamadı");

            var result = _mapper.Map<ResultServiceDto>(value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceDto createServiceDto)
        {
            var (isSuccess, message, createdId) = await _serviceService.TCreateServiceAsync(createServiceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Hizmet başarıyla eklendi..", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceDto updateServiceDto)
        {
            var (isSuccess, message, updatedId) = await _serviceService.TUpdateServiceAsync(updateServiceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Hizmet başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _serviceService.TSoftDeleteServiceAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _serviceService.TUndoSoftDeleteServiceAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
        // Permanent Delete
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var entity = await _serviceService.TGetByIdAsync(id);
            if (entity == null)
                return NotFound("Hizmet bulunamadı");

            if (!entity.IsDeleted)
                return BadRequest("Hizmet silinmiş değil. Önce silmeniz gerekir.");

            await _serviceService.TDeleteAsync(id);

            return Ok("Hizmet çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int serviceId, [FromQuery] bool newStatus)
        {
            var (isSuccess, message) = await _serviceService.ToggleServiceStatusAsync(serviceId, newStatus);

            if (!isSuccess)
                return BadRequest(message);

            return Ok(message);
        }


    }
}
