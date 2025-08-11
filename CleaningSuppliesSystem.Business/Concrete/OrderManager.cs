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
        public Task<List<Order>> TGetCompletedAndCancelledOrdersAsync()
        {
            return _orderRepository.GetCompletedAndCancelledOrdersAsync();
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
            // Asenkron metodu await ile çağırarak sonucun gelmesini bekliyoruz.
            var orders = await _orderRepository.GetOrdersWithItemsAsync();

            // Gelen List<Order> nesnesini haritalıyoruz.
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
                    // Dilersen burada mapping detaylarını ekle
                },
                OrderItems = o.OrderItems.Select(oi => new ResultOrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Product = oi.Product == null ? null : new ResultProductDto
                    {
                        Name = oi.Product.Name,
                        // Ek alanlar varsa buraya ekle
                    }
                }).ToList()
            }).ToList();
        }

        public async Task<OrderStatusUpdateDto> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(orderId, status);

            if (order == null)
                throw new Exception("Sipariş bulunamadı veya güncellenemedi.");

            // Durum 'Hazırlanıyor' ise fatura oluştur
            if (status == "Hazırlanıyor")
            {
                await _ınvoiceService.TCreateInvoiceAsync(orderId);
            }

            return _mapper.Map<OrderStatusUpdateDto>(order);
        }




    }
}
