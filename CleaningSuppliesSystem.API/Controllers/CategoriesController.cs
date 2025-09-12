using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiExplorerSettings(GroupName = "Categories")]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;


    public CategoriesController(
        ICategoryService categoryService,
        IMapper mapper,
        IValidator<CreateCategoryDto> createCategoriesValidator,
        IValidator<UpdateCategoryDto> updateCategoriesValidator,
        IWebHostEnvironment env)
    {
        _categoryService = categoryService;
        _mapper = mapper;
        _env = env;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var categories = await _categoryService.TGetListAsync();
        var result = _mapper.Map<List<ResultCategoryDto>>(categories);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCategories()
    {
        var result = await _categoryService.TGetActiveCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedCategories()
    {
        var result = await _categoryService.TGetDeletedCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.TGetByIdAsync(id);
        if (category == null)
            return NotFound("Kategori bulunamadı.");

        var result = _mapper.Map<ResultCategoryDto>(category);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateCategoryDto createCategoryDto)
    {
        var (isSuccess, message, categoryId) = await _categoryService.TCreateCategoryAsync(createCategoryDto);

        if (!isSuccess)
            return BadRequest(new { message });

        return Ok(new { message = "Ürün grubu / Kategori başarıyla eklendi.", id = categoryId });
    }


    [HttpPut]
    public async Task<IActionResult> Update([FromForm] UpdateCategoryDto updateCategoryDto)
    {
        var (isSuccess, message, createdId) = await _categoryService.TUpdateCategoryAsync(updateCategoryDto);

        if (!isSuccess)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message = "Ürün grubu / Kategori başarıyla güncellendi.", id = createdId });
    }

    [HttpPost("softdelete/{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var result = await _categoryService.TSoftDeleteCategoryAsync(id);
        return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
    }


    [HttpPost("undosoftdelete/{id}")]
    public async Task<IActionResult> UndoSoftDelete(int id)
    {
        var result = await _categoryService.TUndoSoftDeleteCategoryAsync(id);
        return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
    }

    [HttpDelete("permanent/{id}")]
    public async Task<IActionResult> PermanentDelete(int id)
    {
        var category = await _categoryService.TGetByIdAsync(id);
        if (category == null)
            return NotFound("Ürün Grubu / Kategori bulunamadı.");

        if (!category.IsDeleted)
            return BadRequest("Ürün Grubu / Kategori silinmiş değil. Önce silmeniz gerekir.");

        await _categoryService.TDeleteAsync(id);
        return Ok("Ürün Grubu / Kategori çöp kutusundan kalıcı olarak silindi.");
    }

    [HttpPut("set-home-display/{id}")]
    public async Task<IActionResult> SetCategoryDisplayOnHome(int id, [FromBody] bool isShown)
    {
        var result = await _categoryService.TSetCategoryDisplayOnHomeAsync(id, isShown);
        if (!result.IsSuccess)
            return NotFound(new { Message = result.Message });

        return Ok(new { Message = result.Message });
    }

    [HttpGet("GetCategories/{subCategoryId}")]
    public async Task<IActionResult> GetBySubCategory(int subCategoryId)
    {
        var subCategories = await _categoryService.TGetActiveBySubCategoryIdAsync(subCategoryId);
        return Ok(subCategories);
    }

    [HttpPost("DeleteMultiple")]
    public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Silinecek Ürün Grubu / Kategori  bulunamadı.");

        var results = await _categoryService.TSoftDeleteRangeCategoryAsync(ids);

        var failed = results.Where(r => !r.IsSuccess).ToList();
        if (failed.Any())
        {
            var messages = string.Join("\n", failed.Select(f => f.Message));
            return BadRequest(messages);
        }

        return Ok("Seçili Ürün Grubu / Kategori başarıyla çöp kutusuna taşındı.");
    }

    [HttpPost("UndoSoftDeleteMultiple")]
    public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Geri alınacak Ürün Grubu / Kategori bulunamadı.");

        var results = await _categoryService.TUndoSoftDeleteRangeCategoryAsync(ids);

        var failed = results.Where(r => !r.IsSuccess).ToList();
        if (failed.Any())
        {
            var messages = string.Join("\n", failed.Select(f => f.Message));
            return BadRequest(messages);
        }

        return Ok("Seçili Ürün Grubu / Kategori başarıyla geri alındı.");
    }

    [HttpPost("PermanentDeleteMultiple")]
    public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Silinecek ürün Grupları / kategoriler bulunamadı.");

        await _categoryService.TPermanentDeleteRangeCategoryAsync(ids);
        return Ok("Seçili Ürün Grubu / Kategori çöp kutusundan kalıcı olarak silindi.");
    }


}
