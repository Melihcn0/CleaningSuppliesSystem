using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService _orderService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _orderService.TGetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
            var orders = _mapper.Map<List<ResultOrderDto>>(values);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderService.TGetByIdAsyncWithAppUserandOrderItemsandInvoice(id);
            var result = _mapper.Map<ResultOrderDto>(value);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.TDeleteAsync(id);
            return Ok("Sipariş Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto createOrderDto)
        {
            var newValue = _mapper.Map<Order>(createOrderDto);
            await _orderService.TCreateAsync(newValue);
            return Ok($"Yeni Sipariş Oluşturuldu Sipariş ID={newValue.Id}");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOrderDto updateOrderDto)
        {
            var value = _mapper.Map<Order>(updateOrderDto);
            await _orderService.TUpdateAsync(value);
            return Ok("Sipariş Durumu Güncellendi");
        }
    }
}
