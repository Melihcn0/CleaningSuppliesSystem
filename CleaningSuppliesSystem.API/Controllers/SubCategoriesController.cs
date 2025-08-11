using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "SubCategories")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSubCategoryDto> _createValidator;
        private readonly IValidator<UpdateSubCategoryDto> _updateValidator;

        public SubCategoriesController(ISubCategoryService service, IMapper mapper,
            IValidator<CreateSubCategoryDto> createValidator,
            IValidator<UpdateSubCategoryDto> updateValidator)
        {
            _subCategoryService = service;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var subCategories = await _subCategoryService.TGetListAsync();
            var result = _mapper.Map<List<ResultSubCategoryDto>>(subCategories);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSubCategories()
        {
            var result = await _subCategoryService.TGetActiveSubCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedCategories()
        {
            var result = await _subCategoryService.TGetDeletedSubCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subCategory = await _subCategoryService.TGetByIdAsync(id);
            if (subCategory == null)
                return NotFound("Alt kategori bulunamadı.");

            return Ok(_mapper.Map<ResultSubCategoryDto>(subCategory));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubCategoryDto createSubCategoryDto)
        {
            var (isSuccess, message, createdId) = await _subCategoryService.TCreateSubCategoryAsync(createSubCategoryDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Alt kategori başarıyla oluşturuldu.", id = createdId });

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSubCategoryDto updateSubCategoryDto)
        {
            var (isSuccess, message, updatedId) = await _subCategoryService.TUpdateSubCategoryAsync(updateSubCategoryDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Alt kategori başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _subCategoryService.TSoftDeleteSubCategoryAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _subCategoryService.TUndoSoftDeleteSubCategoryAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var subCategory = await _subCategoryService.TGetByIdAsync(id);
            var (IsSuccess, Message) = await _subCategoryService.TPermanentDeleteSubCategoryAsync(id);
            if (!IsSuccess)
                return BadRequest(Message);
            return Ok(Message);
        }

        [HttpGet("ByTopCategory/{topCategoryId}")]
        public async Task<IActionResult> GetByTopCategory(int topCategoryId)
        {
            var subCategories = await _subCategoryService.TGetActiveByTopCategoryIdAsync(topCategoryId);
            return Ok(subCategories);
        }

        [HttpGet("GetSubCategories/{id}")]
        public async Task<IActionResult> GetByTopCategories(int id)
        {
            var subCategories = await _subCategoryService.TGetActiveByTopCategoryIdAsync(id);
            return Ok(subCategories);
        }


        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek kategori bulunamadı.");

            var results = await _subCategoryService.TSoftDeleteRangeSubCategoryAsync(ids);

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

            var results = await _subCategoryService.TUndoSoftDeleteRangeSubCategoryAsync(ids);

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

            await _subCategoryService.TPermanentDeleteRangeSubCategoryAsync(ids);
            return Ok("Kategoriler başarıyla silindi.");
        }



    }
}
