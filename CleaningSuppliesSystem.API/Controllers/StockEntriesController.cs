using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockEntriesController(IStockEntryService _stockEntryService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _stockEntryService.TGetStockEntryWithProductsandCategoriesAsync();
            var stockEntry = _mapper.Map<List<ResultStockEntryDto>>(values);
            return Ok(stockEntry);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _stockEntryService.TGetByIdAsyncWithProductsandCategories(id);
            var result = _mapper.Map<ResultStockEntryDto>(value);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _stockEntryService.TDeleteAsync(id);
            return Ok("Stok Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockEntryDto createStockEntryDto)
        {
            var newValue = _mapper.Map<StockEntry>(createStockEntryDto);
            await _stockEntryService.TCreateAsync(newValue);
            return Ok("Yeni Stok Oluşturuldu");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateStockEntryDto updateStockEntryDto)
        {
            var value = _mapper.Map<StockEntry>(updateStockEntryDto);
            await _stockEntryService.TUpdateAsync(value);
            return Ok("Stok güncellendi");
        }
    }
}
