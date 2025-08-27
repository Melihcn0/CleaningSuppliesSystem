using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
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
    public class CustomerOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerOrdersController(IOrderService orderService, IOrderItemService orderItemService, IInvoiceService invoiceService , IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _invoiceService = invoiceService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // Müşteri kendi siparişlerini listeleyecek
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            // JWT'den kullanıcı ID'si alınır
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int appUserId = int.Parse(userIdClaim.Value);

            // Sadece bu kullanıcıya ait siparişleri çekiyoruz
            var orders = await _orderService.TGetFilteredListAsync(o => o.AppUserId == appUserId);

            var result = _mapper.Map<List<ResultOrderDto>>(orders);
            return Ok(result);
        }

        // Müşteri kendi sipariş detayını görür
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.TGetOrderByIdWithDetailsAsync(id);

            if (order == null)
                return NotFound();

            // Sipariş, istek yapan kullanıcıya ait değilse erişim engellenir
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || order.AppUserId != int.Parse(userIdClaim.Value))
                return Forbid();

            var result = _mapper.Map<ResultOrderDto>(order);
            return Ok(result);
        }

        // Yeni sipariş oluşturabilir (örneğin sepetteki ürünleri order olarak kaydetmek için)
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            createOrderDto.AppUserId = userId;

            var newOrder = _mapper.Map<Order>(createOrderDto);

            await _orderService.TCreateAsync(newOrder);
            // Fatura oluştur
            var invoice = await _invoiceService.TCreateAdminInvoiceAsync(newOrder.Id);

            return CreatedAtAction(nameof(GetOrderById),
                                   new { id = newOrder.Id },
                                   new { Order = newOrder, Invoice = invoice });
        }


        // Müşteri sipariş iptal etmek isterse (duruma göre)
        [HttpPost("cancelOrder/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderService.TGetByIdAsync(id);

            if (order == null)
                return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || order.AppUserId != int.Parse(userIdClaim.Value))
                return Forbid();

            if (order.Status == "Onaylandı")
            {
                return BadRequest("Onaylanmış siparişler iptal edilemez.");
            }

            order.Status = "İptal Edildi";
            await _orderService.TUpdateAsync(order);

            return NoContent();
        }

        [HttpPost("add-to-order")]
        public async Task<IActionResult> AddToPendingOrder(AddToOrderDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _orderService.TAddToPendingOrderAsync(userId, dto.ProductId, dto.Quantity);
            return Ok();
        }

    }
}
