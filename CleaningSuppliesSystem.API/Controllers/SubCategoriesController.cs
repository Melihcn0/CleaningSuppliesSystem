using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
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
        public async Task<IActionResult> GetActiveSubCategories(int page = 1, int pageSize = 10)
        {
            var subCategories = await _subCategoryService.TGetActiveSubCategoriesAsync();
            var totalCount = subCategories.Count;

            var pagedData = subCategories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultSubCategoryDto>
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
        public async Task<IActionResult> GetActiveAllSubCategories()
        {
            var subCategories = await _subCategoryService.TGetActiveSubCategoriesAsync();
            var result = _mapper.Map<List<ResultSubCategoryDto>>(subCategories);
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedSubCategories(int page = 1, int pageSize = 10)
        {
            var subCategories = await _subCategoryService.TGetDeletedSubCategoriesAsync();
            var totalCount = subCategories.Count;

            var pagedData = subCategories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultSubCategoryDto>
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
            return Ok(new { message = "Alt kategori başarıyla eklendi.", id = createdId });

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
            if (subCategory == null)
                return NotFound("Alt kategori bulunamadı.");

            if (!subCategory.IsDeleted)
                return BadRequest("Alt kategori silinmiş değil. Önce silmeniz gerekir.");

            await _subCategoryService.TDeleteAsync(id);
            return Ok("Alt kategori çöp kutusundan kalıcı olarak silindi.");
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
                return BadRequest("Silinecek alt kategori bulunamadı.");

            var results = await _subCategoryService.TSoftDeleteRangeSubCategoryAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili kategoriler başarıyla çöp kutusuna taşındı.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak alt kategori bulunamadı.");

            var results = await _subCategoryService.TUndoSoftDeleteRangeSubCategoryAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili alt kategoriler başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek alt kategoriler bulunamadı.");

            await _subCategoryService.TPermanentDeleteRangeSubCategoryAsync(ids);
            return Ok("Seçili alt kategoriler çöp kutusundan kalıcı olarak silindi.");
        }



    }
}
