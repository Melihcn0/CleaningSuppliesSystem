using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - Service")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public ServicesController(
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
            var values = await _serviceService.TGetListAsync();
            var result = _mapper.Map<List<ResultServiceDto>>(values);
            return Ok(result);
        }
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveServices()
        {
            var result = await _serviceService.TGetActiveServicesAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedServices()
        {
            var result = await _serviceService.TGetDeletedServicesAsync();
            return Ok(result);
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
            return Ok(new { message = "Hizmet başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceDto updateServiceDto)
        {
            var (isSuccess, message, updatedId) = await _serviceService.TUpdateServiceAsync(updateServiceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Hizmet başarıyla oluşturuldu.", id = updatedId });
        }

        // Soft Delete
        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var (IsSuccess, Message, _) = await _serviceService.TSoftDeleteServiceAsync(id);
            if (!IsSuccess)
                return BadRequest(Message);
            return Ok(Message);
        }

        // Undo Soft Delete
        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var product = await _serviceService.TGetByIdAsync(id);
            var (IsSuccess, Message, UndoSoftDeleteId) = await _serviceService.TUndoSoftDeleteServiceAsync(id);
            if (!IsSuccess)
                return BadRequest(Message);
            return Ok(Message);
        }
        // Permanent Delete
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var entity = await _serviceService.TGetByIdAsync(id);
            if (entity == null)
                return NotFound("Hizmet bulunamadı");

            if (!entity.IsDeleted)
                return BadRequest("Hizmet soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _serviceService.TDeleteAsync(id);

            return Ok("Hizmet kalıcı olarak silindi");
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
