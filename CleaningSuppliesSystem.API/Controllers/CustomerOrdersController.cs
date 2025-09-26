using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Customer.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Order")]
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrdersController(IOrderService _orderService, IProductService _productService, IOrderItemService _orderItemService, IInvoiceService _invoiceService, ICompanyBankService _companyBankService, IHttpContextAccessor _httpContextAccessor, IMapper _mapper) : ControllerBase
    {
        private async Task<(bool CanOrder, string? Message)> ValidateCustomerProfileAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _orderService.TValidateCustomerProfileAsync(userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders(int page = 1, int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int appUserId = int.Parse(userIdClaim.Value);

            var orders = await _orderService.TGetOrdersByUserIdWithDetailsAsync(appUserId);
            var adminBank = await _companyBankService.TGetFirstCompanyBankAsync();

            var orderDtos = _mapper.Map<List<CustomerResultOrderDto>>(orders, opts =>
            {
                opts.Items["AdminBank"] = adminBank;
            });

            foreach (var order in orderDtos)
            {
                if (order.OrderItems == null)
                    order.OrderItems = new List<ResultOrderItemDto>();
            }

            var totalCount = orderDtos.Count;

            var pagedData = orderDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<CustomerResultOrderDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("customerResult/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderService.TGetOrderByIdWithDetailsAsync(id);
            if (value == null)
                return NotFound("Sipariş bulunamadı.");

            var adminBank = await _companyBankService.TGetFirstCompanyBankAsync();

            var result = _mapper.Map<CustomerResultOrderDto>(value, opts =>
            {
                opts.Items["AdminBank"] = adminBank;
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.TGetOrderByIdWithDetailsAsync(id);
            if (order == null)
                return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || order.AppUserId != int.Parse(userIdClaim.Value))
                return Forbid();

            var adminBank = await _companyBankService.TGetFirstCompanyBankAsync();
            var result = _mapper.Map<CustomerResultOrderDto>(order, opts =>
            {
                opts.Items["AdminBank"] = adminBank;
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var (canOrder, message) = await ValidateCustomerProfileAsync();
            if (!canOrder)
                return BadRequest(new { message });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            createOrderDto.AppUserId = userId;

            var newOrder = _mapper.Map<Order>(createOrderDto);
            await _orderService.TCreateAsync(newOrder);

            var invoice = await _invoiceService.TCreateAdminInvoiceAsync(newOrder.Id);

            return CreatedAtAction(nameof(GetOrderById),
                                   new { id = newOrder.Id },
                                   new { Order = newOrder, Invoice = invoice });
        }

        [HttpPost("add-to-order")]
        public async Task<IActionResult> AddToPendingOrder(AddToOrderDto dto)
        {
            var (canOrder, message) = await ValidateCustomerProfileAsync();
            if (!canOrder)
            return BadRequest(new { message });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await _productService.TGetByIdAsync(dto.ProductId);

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            await _orderService.TAddToPendingOrderAsync(userId, dto.ProductId, dto.Quantity, product.UnitPrice, product.DiscountRate);

            return Ok(new { message = "Ürün sepete eklendi." });
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

                var order = await _orderService.TGetByIdAsync(dto.Id);
                if (order == null)
                    return NotFound("Sipariş bulunamadı.");

                if (dto.Status == "İptal Edildi" && order.Status == "Onaylandı")
                {
                    return BadRequest("Onaylanmış siparişler iptal edilemez.");
                }

                var result = await _orderService.TUpdateStatusAsync(dto.Id, dto.Status);
                if (result == null)
                    return NotFound("Sipariş bulunamadı.");

                return Ok(result);

        }


    }
}
