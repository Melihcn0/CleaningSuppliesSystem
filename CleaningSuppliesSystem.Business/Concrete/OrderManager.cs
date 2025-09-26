using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class OrderManager : GenericManager<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;
        private readonly ICompanyBankService _companyBankService;
        private readonly IMapper _mapper;

        public OrderManager(
            IOrderRepository orderRepository,
            IInvoiceService invoiceService,
            IProductService productService,
            ICompanyBankService companyBankService,
            IMapper mapper
        ) : base(orderRepository) 
        {
            _orderRepository = orderRepository;
            _invoiceService = invoiceService;
            _productService = productService;
            _companyBankService = companyBankService;
            _mapper = mapper;
        }
        public Task<List<Order>> TGetActiveOrdersWithDetailsAsync()
        {
            return _orderRepository.GetActiveOrdersWithDetailsAsync();
        }

        public Task<Order> TGetOrderByIdWithDetailsAsync(int id)
        {
            return _orderRepository.GetOrderByIdWithDetailsAsync(id);
        }
        public Task<List<Order>> TGetCompletedOrdersAsync()
        {
            return _orderRepository.GetCompletedOrdersAsync();
        }
        public Task<List<Order>> TGetCancelledOrdersAsync()
        {
            return _orderRepository.GetCancelledOrdersAsync();
        }

        public async Task<Order?> TGetPendingOrderByUserIdAsync(int userId)
        {
            return await _orderRepository.GetPendingOrderByUserIdAsync(userId);
        }

        public Task TAddToPendingOrderAsync(int userId, int productId, int quantity, decimal unitPrice, decimal? discountRate)
        {
            // Repository metodu da bunu desteklemeli
            return _orderRepository.AddToPendingOrderAsync(userId, productId, quantity, unitPrice, discountRate);
        }

        public async Task<List<Order>> TGetOrdersWithItemsAsync()
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();
            return _mapper.Map<List<Order>>(orders);
        }
        // OrderManager
        public async Task<List<Order>> TGetOrdersByUserIdWithDetailsAsync(int userId)
        {
            return await _orderRepository.GetOrdersByUserIdWithDetailsAsync(userId);
        }

        public async Task<OrderStatusUpdateDto> TUpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(orderId, status);

            if (order == null)
                throw new Exception("Sipariş bulunamadı veya güncellenemedi.");

            if (status.Equals("Onaylandı", StringComparison.OrdinalIgnoreCase))
            {
                await _productService.TDecreaseStockAsync(order.OrderItems);
            }

            if (status.Equals("Hazırlanıyor", StringComparison.OrdinalIgnoreCase) && order.Invoice == null)
            {
                await _invoiceService.TCreateAdminInvoiceAsync(orderId);
            }

            return _mapper.Map<OrderStatusUpdateDto>(order);
        }

        public async Task<(bool CanOrder, string? Message)> TValidateCustomerProfileAsync(int userId)
        {
            return await _orderRepository.ValidateCustomerProfileAsync(userId);
        }

        public async Task<List<ExpiredOrderDto>> TGetExpiredOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();

            var ongoingStatuses = new[] { "Onay Bekleniyor", "Onaylandı", "Hazırlanıyor", "Kargoya Verildi" };

            var ongoingOrders = orders
                .Where(o => ongoingStatuses.Contains(o.Status)) // sadece devam eden siparişler
                .Select(o => _mapper.Map<ExpiredOrderDto>(o))
                .ToList();

            return ongoingOrders;
        }



    }
}
