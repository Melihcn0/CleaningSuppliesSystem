using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
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
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            var values = await _orderService.TGetActiveOrdersWithDetailsAsync();
            var totalCount = values.Count;

            var pagedData = values
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedData = _mapper.Map<List<AdminResultOrderDto>>(pagedData);

            var response = new PagedResponse<AdminResultOrderDto>
            {
                Data = mappedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }


        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedOrders(int page = 1, int pageSize = 10)
        {
            var values = await _orderService.TGetCompletedOrdersAsync();
            var totalCount = values.Count;

            var pagedData = values
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedData = _mapper.Map<List<AdminResultOrderDto>>(pagedData);

            var response = new PagedResponse<AdminResultOrderDto>
            {
                Data = mappedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }
        [HttpGet("cancelled")]
        public async Task<IActionResult> GetCancelledOrders(int page = 1, int pageSize = 10)
        {
            var values = await _orderService.TGetCancelledOrdersAsync();
            var totalCount = values.Count;

            var pagedData = values
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedData = _mapper.Map<List<AdminResultOrderDto>>(pagedData);

            var response = new PagedResponse<AdminResultOrderDto>
            {
                Data = mappedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
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

        [HttpGet("expired")]
        public async Task<IActionResult> GetExpiredOrders()
        {
            var result = await _orderService.TGetExpiredOrdersAsync();
            return Ok(result);
        }

    }
}
