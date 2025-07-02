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
    public class CategoriesController(IGenericService<Category> _categoryService , IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _categoryService.TGetListAsync();
            var result = _mapper.Map<List<ResultCategoryDto>>(values);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _categoryService.TGetByIdAsync(id);
            var result = _mapper.Map<ResultCategoryDto>(value);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.TDeleteAsync(id);
            return Ok("Kategori Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            var newValue = _mapper.Map<Category>(createCategoryDto);
            await _categoryService.TCreateAsync(newValue);
            return Ok($"Yeni Kategori Oluşturuldu Kategori ID={newValue.Id}");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var value = _mapper.Map<Category>(updateCategoryDto);
            await _categoryService.TUpdateAsync(value);
            return Ok("Kategori güncellendi");
        }
    }
}
