using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Order")]
    [Authorize(Roles = "Admin")]
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
        // Teslim Edilen ve İptal Edilen siparişleri getir
        [HttpGet("completed-cancelled")]
        public async Task<ActionResult<List<ResultOrderDto>>> GetCompletedAndCancelledOrders()
        {
            var values = await _orderService.TGetCompletedAndCancelledOrdersAsync();
            var orders = _mapper.Map<List<ResultOrderDto>>(values);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderService.TGetByIdAsyncWithAppUserandOrderItemsandInvoice(id);
            if (value == null)
                return NotFound();

            var result = _mapper.Map<ResultOrderDto>(value);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.TDeleteAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto createOrderDto)
        {
            var newValue = _mapper.Map<Order>(createOrderDto);
            await _orderService.TCreateAsync(newValue);
            return CreatedAtAction(nameof(GetById), new { id = newValue.Id }, newValue);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOrderDto updateOrderDto)
        {
            var value = _mapper.Map<Order>(updateOrderDto);
            await _orderService.TUpdateAsync(value);
            return NoContent();
        }

        [HttpPost("UpdateStatus")]
        public async Task<ActionResult<OrderStatusUpdateDto>> UpdateStatus([FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Geçersiz veri.");

            var result = await _orderService.UpdateStatusAsync(dto.Id, dto.Status);

            if (result == null)
                return NotFound("Sipariş bulunamadı.");

            return Ok(result);
        }





    }
}
