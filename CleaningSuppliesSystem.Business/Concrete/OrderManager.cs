using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Task<List<Order>> TGetOrderItemWithAppUserandOrderItemsandInvoiceAsync()
        {
            return _orderRepository.GetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
        }

        public Task<Order> TGetByIdAsyncWithAppUserandOrderItemsandInvoice(int id)
        {
            return _orderRepository.GetByIdAsyncWithAppUserandOrderItemsandInvoice(id);
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

        public async Task<List<ResultOrderDto>> TGetOrdersWithItemsByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersWithItemsByUserIdAsync(userId);

            return orders.Select(o => new ResultOrderDto
            {
                Id = o.Id,
                AppUserId = o.AppUserId,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                CreatedDate = o.CreatedDate,
                UpdatedDate = o.UpdatedDate,

                Invoice = o.Invoice == null ? null : new InvoiceDto
                {
                    Id = o.Invoice.Id,
                    OrderId = o.Invoice.OrderId,
                    GeneratedAt = o.Invoice.GeneratedAt,
                    TotalAmount = o.Invoice.TotalAmount,
                    InvoiceType = o.Invoice.InvoiceType,

                    FirstName = o.Invoice.FirstName,
                    LastName = o.Invoice.LastName,
                    NationalId = o.Invoice.NationalId,
                    PhoneNumber = o.Invoice.PhoneNumber,
                    Email = o.Invoice.Email,

                    CompanyName = o.Invoice.CompanyName,
                    TaxOffice = o.Invoice.TaxOffice,
                    TaxNumber = o.Invoice.TaxNumber,

                    AddressTitle = o.Invoice.AddressTitle,
                    Address = o.Invoice.Address,
                    CityName = o.Invoice.CityName,
                    DistrictName = o.Invoice.DistrictName
                },

                OrderItems = o.OrderItems?
                    .Select(oi => new ResultOrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,

                        Product = oi.Product == null ? null : new ResultProductDto
                        {
                            Name = oi.Product.Name
                            // Eğer ileride ek alanlar gelecekse buraya ekle
                        }
                    })
                    .ToList()
            }).ToList();
        }

        public async Task<OrderStatusUpdateDto> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(orderId, status);

            if (order == null)
                throw new Exception("Sipariş bulunamadı veya güncellenemedi.");

            if (status.Equals("Hazırlanıyor", StringComparison.OrdinalIgnoreCase) && order.Invoice == null)
            {
                await _ınvoiceService.TCreateInvoiceAsync(orderId);
            }

            return _mapper.Map<OrderStatusUpdateDto>(order);
        }




    }
}
