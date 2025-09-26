using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Home")]
    [Tags("Home - ServiceIcon")]
    [Authorize(Roles = "Developer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperServiceIconsController : ControllerBase
    {
        private readonly IServiceIconService _serviceIconService;
        private readonly IMapper _mapper;

        public DeveloperServiceIconsController(IServiceIconService serviceIconService, IMapper mapper)
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
        public async Task<IActionResult> GetActiveServiceIcon(int page = 1, int pageSize = 10)
        {
            var serviceIcons = await _serviceIconService.TGetActiveServiceIconsAsync();
            var totalCount = serviceIcons.Count;

            var pagedData = serviceIcons
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultServiceIconDto>
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
        public async Task<IActionResult> GetDeletedServiceIcons(int page = 1, int pageSize = 10)
        {
            var serviceIcon = await _serviceIconService.TGetDeletedServiceIconsAsync();
            var totalCount = serviceIcon.Count;

            var pagedData = serviceIcon
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultServiceIconDto>
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
            var value = await _serviceIconService.TGetByIdAsync(id);
            if (value == null)
                return NotFound("Hizmet ikonu bulunamadı.");

            var result = _mapper.Map<ResultServiceIconDto>(value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceIconDto createDto)
        {
            var (isSuccess, message, createdId) = await _serviceIconService.TCreateServiceIconAsync(createDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Hizmet ikonu başarıyla eklendi.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceIconDto updateDto)
        {
            var (isSuccess, message, updatedId) = await _serviceIconService.TUpdateServiceIconAsync(updateDto);
            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Hizmet ikonu başarıyla güncellendi.", id = updatedId });
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
                return NotFound("Hizmet ikonu bulunamadı.");

            if (!entity.IsDeleted)
                return BadRequest("Hizmet ikonu silinmiş değil. Önce silmeniz gerekir.");

            await _serviceIconService.TPermanentDeleteServiceIconAsync(id);
            return Ok("Hizmet ikonu kalıcı olarak silindi.");
        }

    }
}
