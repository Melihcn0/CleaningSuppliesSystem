using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService _productService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _productService.TGetProductsWithCategoriesAsync();
            var products = _mapper.Map<List<ResultProductDto>>(values);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _productService.TGetByIdAsyncWithCategory(id);
            var result = _mapper.Map<ResultProductDto>(value);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.TDeleteAsync(id);
            return Ok("Ürün Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
            var newValue = _mapper.Map<Product>(createProductDto);
            await _productService.TCreateAsync(newValue);
            return Ok("Yeni Ürün Oluşturuldu");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto updateProductDto)
        {
            var value = _mapper.Map<Product>(updateProductDto);
            await _productService.TUpdateAsync(value);
            return Ok("Ürün güncellendi");
        }
    }
}
