using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class OrderManager : GenericManager<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInvoiceService _ınvoiceService;
        private readonly IMapper _mapper;

        public OrderManager(IOrderRepository orderRepository, IInvoiceService ınvoiceService, IMapper mapper) : base(orderRepository)
        {
            _orderRepository = orderRepository;
            _ınvoiceService = ınvoiceService;
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

        public Task TAddToPendingOrderAsync(int userId, int productId, int quantity)
        {
            return _orderRepository.AddToPendingOrderAsync(userId, productId, quantity);
        }
        public async Task<List<ResultOrderDto>> TGetOrdersWithItemsAsync()
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();
            return _mapper.Map<List<ResultOrderDto>>(orders);
        }
        public async Task<List<ResultOrderDto>> TGetOrdersByUserIdWithDetailsAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdWithDetailsAsync(userId);

            return orders.Select(o => new ResultOrderDto
            {
                Id = o.Id,
                AppUserId = o.AppUserId,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                CreatedDate = o.CreatedDate,
                UpdatedDate = o.UpdatedDate,

                Invoice = o.Invoice != null ? new InvoiceDto
                {
                    Id = o.Invoice.Id,
                    OrderId = o.Invoice.OrderId,
                    GeneratedAt = o.Invoice.GeneratedAt,
                    TotalAmount = o.Invoice.TotalAmount,
                    InvoiceType = o.Invoice.InvoiceType,

                    // Customer alanları
                    CustomerFirstName = o.Invoice.CustomerFirstName,
                    CustomerLastName = o.Invoice.CustomerLastName,
                    CustomerNationalId = o.Invoice.CustomerNationalId,
                    CustomerPhoneNumber = o.Invoice.CustomerPhoneNumber,

                    CustomerCompanyName = o.Invoice.CustomerCompanyName,
                    CustomerTaxOffice = o.Invoice.CustomerTaxOffice,
                    CustomerTaxNumber = o.Invoice.CustomerTaxNumber,

                    CustomerAddressTitle = o.Invoice.CustomerAddressTitle,
                    CustomerAddress = o.Invoice.CustomerAddress,
                    CustomerCityName = o.Invoice.CustomerCityName,
                    CustomerDistrictName = o.Invoice.CustomerDistrictName,

                    // Admin snapshot
                    AdminFirstName = o.Invoice.AdminFirstName,
                    AdminLastName = o.Invoice.AdminLastName,
                    AdminPhoneNumber = o.Invoice.AdminPhoneNumber,

                    // Şirket snapshot
                    InvoiceCompanyName = o.Invoice.InvoiceCompanyName,
                    InvoiceCompanyTaxOffice = o.Invoice.InvoiceCompanyTaxOffice,
                    InvoiceCompanyTaxNumber = o.Invoice.InvoiceCompanyTaxNumber,
                    InvoiceCompanyAddress = o.Invoice.InvoiceCompanyAddress,
                    InvoiceCompanyCityName = o.Invoice.InvoiceCompanyCityName,
                    InvoiceCompanyDistrictName = o.Invoice.InvoiceCompanyDistrictName,

                    // Fatura satırları
                    InvoiceItems = o.Invoice.InvoiceItems?.Select(ii => new InvoiceItemDto
                    {
                        Id = ii.Id,
                        InvoiceId = ii.InvoiceId,
                        ProductName = ii.ProductName,
                        Quantity = ii.Quantity,
                        Unit = ii.Unit,
                        UnitPrice = ii.UnitPrice,
                        VatRate = ii.VatRate,
                        VatAmount = ii.VatAmount,
                        Total = ii.Total
                    }).ToList() ?? new List<InvoiceItemDto>()

                } : null

            }).ToList();
        }


        public async Task<OrderStatusUpdateDto> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(orderId, status);

            if (order == null)
                throw new Exception("Sipariş bulunamadı veya güncellenemedi.");

            if (status.Equals("Hazırlanıyor", StringComparison.OrdinalIgnoreCase) && order.Invoice == null)
            {
                await _ınvoiceService.TCreateAdminInvoiceAsync(orderId);
            }

            return _mapper.Map<OrderStatusUpdateDto>(order);
        }




    }
}
