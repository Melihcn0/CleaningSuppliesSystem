using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Products")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductDto> _createProductValidator;
        private readonly IValidator<UpdateProductDto> _updateProductValidator;
        private readonly IValidator<UpdateDiscountDto> _updateDiscountValidator;
        private readonly IWebHostEnvironment _env;

        public ProductsController(IProductService productService, IMapper mapper, 
            IValidator<CreateProductDto> createProductValidator, 
            IValidator<UpdateProductDto> updateProductValidator, 
            IValidator<UpdateDiscountDto> updateDiscountValidator, IWebHostEnvironment env)
        {
            _productService = productService;
            _mapper = mapper;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _updateDiscountValidator = updateDiscountValidator;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.TGetListAsync();
            var result = _mapper.Map<List<ResultProductDto>>(products);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts(int page = 1, int pageSize = 10)
        {
            var products = await _productService.TGetActiveProductsAsync();
            var totalCount = products.Count;

            var pagedData = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultProductDto>
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
        public async Task<IActionResult> GetActiveAllProducts()
        {
            var products = await _productService.TGetActiveProductsAsync();
            var result = _mapper.Map<List<ResultProductDto>>(products);
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedProducts(int page = 1, int pageSize = 10)
        {
            var products = await _productService.TGetDeletedProductsAsync();
            var totalCount = products.Count;

            var pagedData = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultProductDto>
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
            var product = await _productService.TGetByIdAsync(id);
            if (product == null)
                return NotFound();

            var dto = _mapper.Map<ResultProductDto>(product);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto createProductDto)
        {
            var (isSuccess, message, createdId) = await _productService.TCreateProductAsync(createProductDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Ürün başarıyla eklendi.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateProductDto updateProductDto)
        {
            var (isSuccess, message, updatedId) = await _productService.TUpdateProductAsync(updateProductDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Ürün başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _productService.TSoftDeleteProductAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _productService.TUndoSoftDeleteProductAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var product = await _productService.TGetByIdAsync(id);
            if (product == null)
                return NotFound("Ürün bulunamadı.");

            if (!product.IsDeleted)
                return BadRequest("Ürün silinmiş değil. Önce silmeniz gerekir.");

            // Kalıcı silme işlemini yap
            var (isSuccess, message) = await _productService.TPermanentDeleteProductAsync(id);

            if (!isSuccess)
            {
                return BadRequest(message);
            }

            // Görsel varsa sil (yol formatını düzeltiyoruz)
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                // URL'den ~, / gibi karakterleri temizle, işletim sistemine uygun yol oluştur
                var relativePath = product.ImageUrl.TrimStart('~').TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var imagePath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            return Ok(message);
        }

        [HttpGet("discountproduct/{id}")]
        public async Task<IActionResult> GetDiscountProduct(int id)
        {
            var product = await _productService.TGetByIdAsync(id);
            if (product == null)
                return NotFound();

            var dto = _mapper.Map<UpdateDiscountDto>(product);

            return Ok(dto);
        }

        [HttpPost("applydiscount")]
        public async Task<IActionResult> ApplyDiscount([FromBody] UpdateDiscountDto dto)
        {
            var validationResult = await _updateDiscountValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var (isSuccess, errors) = await _productService.TApplyDiscountAsync(dto);
            if (!isSuccess)
            {
                return BadRequest(errors);
            }

            return Ok(new { message = "Ürüne indirim başarıyla uygulandı." });
        }


        [HttpPut("set-home-display/{id}")]
        public async Task<IActionResult> SetProductDisplayOnHome(int id, [FromBody] bool isShown)
        {
            var result = await _productService.TSetProductDisplayOnHomeAsync(id, isShown);
            if (!result.IsSuccess)
                return NotFound(new { Message = result.Message });

            return Ok(new { Message = result.Message });
        }

        [HttpGet("GetProducts/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var brands = await _productService.TGetActiveByBrandIdAsync(brandId);
            return Ok(brands);
        }
        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek ürünler bulunamadı.");

            var results = await _productService.TSoftDeleteRangeProductAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili ürünler başarıyla çöp kutusuna taşındı.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak ürün bulunamadı.");

            var results = await _productService.TUndoSoftDeleteRangeProductAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili ürünler başarıyla geri alındı.");
        }

        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek ürünler bulunamadı.");

            await _productService.TPermanentDeleteRangeProductAsync(ids);
            return Ok("Seçili ürünler çöp kutusundan kalıcı olarak silindi.");
        }
    }
}
