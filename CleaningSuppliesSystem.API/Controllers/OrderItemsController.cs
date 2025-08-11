using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Order")]
    [Authorize(Roles = "Admin,Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController(IOrderItemService _orderItemService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _orderItemService.TGetOrderItemWithProductandCategoriesAsync();
            var orderItems = _mapper.Map<List<ResultOrderItemDto>>(values);
            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderItemService.TGetByIdAsyncWithProductandCategories(id);
            var result = _mapper.Map<ResultOrderItemDto>(value);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderItemService.TDeleteAsync(id);
            return Ok("Siparişteki Ürün Silindi");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderItemDto createOrderItemDto)
        {
            var newValue = _mapper.Map<OrderItem>(createOrderItemDto);
            await _orderItemService.TCreateAsync(newValue);
            return Ok($"Siparişe Yeni Ürün Oluşturuldu Sipariş ID={newValue.Id}");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOrderItemDto updateOrderItemDto)
        {
            var value = _mapper.Map<OrderItem>(updateOrderItemDto);
            await _orderItemService.TUpdateAsync(value);
            return Ok("Sipariş İçeriği Güncellendi");
        }

    }
}
