using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Brands")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;

        public BrandsController(
            IBrandService brandService,
            IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        [HttpGet("bycategory/{categoryId}")]
        public async Task<IActionResult> GetBrandsByCategory(int categoryId)
        {
            var brands = await _brandService.TGetActiveByCategoryIdAsync(categoryId);
            var result = brands.Select(b => new { b.Id, b.Name }).ToList();
            return Ok(result);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var brands = await _brandService.TGetListAsync();
            var result = _mapper.Map<List<ResultBrandDto>>(brands);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBrands()
        {
            var result = await _brandService.TGetActiveBrandsAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedCategories()
        {
            var result = await _brandService.TGetDeletedBrandsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.TGetByIdAsync(id);
            if (brand == null)
                return NotFound("Marka bulunamadı.");

            var result = _mapper.Map<ResultBrandDto>(brand);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto createBrandDto)
        {
            var (isSuccess, message, createdId) = await _brandService.TCreateBrandAsync(createBrandDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Marka başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateBrandDto updateBrandDto)
        {
            var (isSuccess, message, updatedId) = await _brandService.TUpdateBrandAsync(updateBrandDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Marka başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _brandService.TSoftDeleteBrandAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _brandService.TUndoSoftDeleteBrandAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var brand = await _brandService.TGetByIdAsync(id);
            if (brand == null)
                return NotFound("Marka bulunamadı.");

            if (!brand.IsDeleted)
                return BadRequest("Marka silinmiş değil. Önce silmeniz gerekir.");

            await _brandService.TDeleteAsync(id);
            return Ok("Marka çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpGet("GetBrands/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var categories = await _brandService.TGetActiveByCategoryIdAsync(categoryId);
            return Ok(categories);
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek marka bulunamadı.");

            var results = await _brandService.TSoftDeleteRangeBrandAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili marka başarıyla çöp kutusuna taşındı.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak marka bulunamadı.");

            var results = await _brandService.TUndoSoftDeleteRangeBrandAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili marka başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek marka bulunamadı.");

            await _brandService.TPermanentDeleteRangeBrandAsync(ids);
            return Ok("Seçili marka çöp kutusundan kalıcı olarak silindi.");
        }

    }
}