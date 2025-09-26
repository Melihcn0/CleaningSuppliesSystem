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

        [HttpPost("decrement/{id}")]
        public async Task<IActionResult> Decrement(int id)
        {
            try
            {
                await _orderItemService.TDecrementQuantityAsync(id);
                var item = await _orderItemService.TGetByIdAsync(id);
                return Ok(new { message = "Başarılı", newQuantity = item?.Quantity ?? 0 });
            }
            catch
            {
                return NotFound(new { message = "Ürün bulunamadı" });
            }
        }

        [HttpPost("increment/{id}")]
        public async Task<IActionResult> Increment(int id)
        {
            try
            {
                await _orderItemService.TIncrementQuantityAsync(id);
                var item = await _orderItemService.TGetByIdAsync(id);
                return Ok(new { message = "Başarılı", newQuantity = item?.Quantity ?? 0 });
            }
            catch
            {
                return NotFound(new { message = "Ürün bulunamadı" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderItemService.TDeleteAsync(id);
            return Ok(new { message = "Siparişteki ürün silindi" });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderItemDto createOrderItemDto)
        {
            var newValue = _mapper.Map<OrderItem>(createOrderItemDto);
            await _orderItemService.TCreateAsync(newValue);
            return Ok($"Siparişe yeni ürün eklendi.");
        }
        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] UpdateOrderItemDto updateOrderItemDto)
        //{
        //    var value = _mapper.Map<OrderItem>(updateOrderItemDto);
        //    await _orderItemService.TUpdateAsync(value);
        //    return Ok("Sipariş içeriği Güncellendi");
        //}

    }
}
