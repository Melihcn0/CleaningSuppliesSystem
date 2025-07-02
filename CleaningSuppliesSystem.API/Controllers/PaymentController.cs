using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.PaymentDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IGenericService<Payment> _paymentService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _paymentService.TGetListAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _paymentService.TGetByIdAsync(id);
            return Ok(value);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _paymentService.TDeleteAsync(id);
            return Ok("Ödeme Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto createPaymentDto)
        {
            var newValue = _mapper.Map<Payment>(createPaymentDto);
            await _paymentService.TCreateAsync(newValue);
            return Ok("Yeni Ödeme Oluşturuldu");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePaymentDto updatePaymentDto)
        {
            var value = _mapper.Map<Payment>(updatePaymentDto);
            await _paymentService.TUpdateAsync(value);
            return Ok("Ödeme güncellendi");
        }
    }
}
