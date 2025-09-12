using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - ServiceIcon")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceIconsController : ControllerBase
    {
        private readonly IServiceIconService _serviceIconService;
        private readonly IMapper _mapper;

        public ServiceIconsController(IServiceIconService serviceIconService, IMapper mapper)
        {
            _serviceIconService = serviceIconService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _serviceIconService.TGetListAsync();
            var result = _mapper.Map<List<ResultServiceIconDto>>(values);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveServiceIcons()
        {
            var result = await _serviceIconService.TGetActiveServiceIconsAsync();
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedServiceIcons()
        {
            var result = await _serviceIconService.TGetDeletedServiceIconsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _serviceIconService.TGetByIdAsync(id);
            if (value == null)
                return NotFound("Service icon bulunamadı.");

            var result = _mapper.Map<ResultServiceIconDto>(value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceIconDto createDto)
        {
            var (isSuccess, message, createdId) = await _serviceIconService.TCreateServiceIconAsync(createDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Icon başarıyla eklendi.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceIconDto updateDto)
        {
            var (isSuccess, message, updatedId) = await _serviceIconService.TUpdateServiceIconAsync(updateDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Icon başarıyla eklendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _serviceIconService.TSoftDeleteServiceIconAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _serviceIconService.TUndoSoftDeleteServiceIconAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var entity = await _serviceIconService.TGetByIdAsync(id);
            if (entity == null)
                return NotFound("Service ikonu bulunamadı.");

            if (!entity.IsDeleted)
                return BadRequest("Servis ikonu silinmiş değil. Önce silmeniz gerekir.");

            await _serviceIconService.TPermanentDeleteServiceIconAsync(id);
            return Ok("Servis ikonu kalıcı olarak silindi.");
        }

        //[HttpGet("unused-active")]
        //public async Task<IActionResult> GetUnusedActiveIcons()
        //{
        //    var icons = await _serviceIconService.TGetUnusedActiveServiceIconsAsync();
        //    return Ok(icons);
        //}


    }
}
