using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeProductController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public HomeProductController(ICategoryService categoryService, IProductService productService, IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
        }

        // Kategorileri anonim erişime açabiliriz
        [HttpGet("home-categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.TGetActiveCategoriesAsync();
            var result = _mapper.Map<List<ResultCategoryDto>>(categories);
            return Ok(result);
        }

        [HttpGet("home-products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] int? categoryId)
        {
            var products = await _productService.TGetActiveProductsAsync();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();

            var result = _mapper.Map<List<ResultProductDto>>(products);
            return Ok(result);
        }

    }
}
