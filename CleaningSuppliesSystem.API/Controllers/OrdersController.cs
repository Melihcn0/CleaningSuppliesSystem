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
            var values = await _orderService.TGetActiveOrdersWithDetailsAsync();
            var orders = _mapper.Map<List<AdminResultOrderDto>>(values);
            return Ok(orders);
        }
        [HttpGet("completed")]
        public async Task<ActionResult<List<AdminResultOrderDto>>> GetCompletedOrders()
        {
            var values = await _orderService.TGetCompletedOrdersAsync();
            var orders = _mapper.Map<List<AdminResultOrderDto>>(values);
            return Ok(orders);
        }

        [HttpGet("cancelled")]
        public async Task<ActionResult<List<AdminResultOrderDto>>> GetCancelledOrders()
        {
            var values = await _orderService.TGetCancelledOrdersAsync();
            var orders = _mapper.Map<List<AdminResultOrderDto>>(values);
            return Ok(orders);
        }

        [HttpGet("AdminResult/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderService.TGetOrderByIdWithDetailsAsync(id);
            if (value == null)
                return NotFound();

            var result = _mapper.Map<AdminResultOrderDto>(value);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.TDeleteAsync(id);
            return NoContent();
        }

        [HttpPost("UpdateStatus")]
        public async Task<ActionResult<OrderStatusUpdateDto>> UpdateStatus([FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Geçersiz veri.");

            var result = await _orderService.TUpdateStatusAsync(dto.Id, dto.Status);

            if (result == null)
                return NotFound("Sipariş bulunamadı.");

            return Ok(result);
        }



    }
}
