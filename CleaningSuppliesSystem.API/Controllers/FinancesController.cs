using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancesController(IGenericService<Finance> _financeController, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _financeController.TGetListAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _financeController.TGetByIdAsync(id);
            return Ok(value);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _financeController.TDeleteAsync(id);
            return Ok("Finans Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinanceDto createFinanceDto)
        {
            var newValue = _mapper.Map<Finance>(createFinanceDto);
            await _financeController.TCreateAsync(newValue);
            return Ok($"Yeni Finans Oluşturuldu Finans ID={newValue.Id}");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFinanceDto updateFinanceDto)
        {
            var value = _mapper.Map<Finance>(updateFinanceDto);
            await _financeController.TUpdateAsync(value);
            return Ok("Finans güncellendi");
        }
    }
}
