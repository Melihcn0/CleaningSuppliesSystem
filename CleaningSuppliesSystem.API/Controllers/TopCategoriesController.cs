using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "TopCategories")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TopCategoriesController : ControllerBase
    {
        private readonly ITopCategoryService _topCategoryService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTopCategoryDto> _createValidator;
        private readonly IValidator<UpdateTopCategoryDto> _updateValidator;

        public TopCategoriesController(ITopCategoryService service, IMapper mapper,
            IValidator<CreateTopCategoryDto> createValidator,
            IValidator<UpdateTopCategoryDto> updateValidator)
        {
            _topCategoryService = service;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var topCategories = await _topCategoryService.TGetListAsync();
            var result = _mapper.Map<List<ResultTopCategoryDto>>(topCategories);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTopCategories()
        {
            var result = await _topCategoryService.TGetActiveTopCategoriesAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedTopCategories()
        {
            var result = await _topCategoryService.TGetDeletedTopCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var topCategory = await _topCategoryService.TGetByIdAsync(id);
            if (topCategory == null)
                return NotFound("Üst kategori bulunamadı.");

            return Ok(_mapper.Map<ResultTopCategoryDto>(topCategory));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTopCategoryDto createTopCategoryDto)
        {
            var (isSuccess, message, createdId) = await _topCategoryService.TCreateTopCategoryAsync(createTopCategoryDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Üst kategori başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTopCategoryDto updateTopCategoryDto)
        {
            var (isSuccess, message, updatedId) = await _topCategoryService.TUpdateTopCategoryAsync(updateTopCategoryDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Üst kategori başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _topCategoryService.TSoftDeleteTopCategoryAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _topCategoryService.TUndoSoftDeleteTopCategoryAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var topCategory = await _topCategoryService.TGetByIdAsync(id);
            if (topCategory == null)
                return NotFound("Üst kategori bulunamadı.");

            if (!topCategory.IsDeleted)
                return BadRequest("Üst kategori soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _topCategoryService.TDeleteAsync(id);
            return Ok("Üst kategori kalıcı olarak silindi.");
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek kategori bulunamadı.");

            var results = await _topCategoryService.TSoftDeleteRangeTopCategoryAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Tüm kategoriler başarıyla silindi.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak kategori bulunamadı.");

            var results = await _topCategoryService.TUndoSoftDeleteRangeTopCategoryAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Tüm kategoriler başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek kategoriler bulunamadı.");

            await _topCategoryService.TPermanentDeleteRangeTopCategoryAsync(ids);
            return Ok("Kategoriler başarıyla silindi.");
        }
    }
}
