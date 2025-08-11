using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class InvoiceManager : GenericManager<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _ınvoiceRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPdfService _pdfService;

        public InvoiceManager(IInvoiceRepository ınvoiceRepository, IOrderRepository orderRepository, IPdfService pdfService) : base(ınvoiceRepository)
        {
            _ınvoiceRepository = ınvoiceRepository;
            _orderRepository = orderRepository;
            _pdfService = pdfService;
        }
        public async Task<List<Invoice>> TGetInvoiceWithOrderAsync()
        {
            return await _ınvoiceRepository.GetInvoiceWithOrderAsync();
        }
        public async Task<Invoice> TGetByIdAsyncWithOrder(int orderId)
        {
            return await _ınvoiceRepository.GetByIdAsyncWithOrder(orderId);
        }
        public async Task<Invoice> TGetInvoiceByOrderIdAsync(int orderId)
        {
            return await _ınvoiceRepository.GetInvoiceByOrderIdAsync(orderId);
        }
        public async Task<List<Invoice>> TGetInvoicesByUserIdAsync(int userId)
        {
            return await _ınvoiceRepository.GetInvoicesByUserIdAsync(userId);
        }

        public async Task TCreateInvoiceAsync(int orderId)
        {
            try
            {
                var existingInvoice = await _ınvoiceRepository.GetInvoiceByOrderIdAsync(orderId);
                if (existingInvoice != null) return;

                var invoice = new Invoice
                {
                    OrderId = orderId,
                    GeneratedAt = DateTime.UtcNow
                };

                await _ınvoiceRepository.CreateAsync(invoice);
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                throw new Exception($"Fatura oluşturma sırasında hata: {ex.Message}", ex);
            }
        }


        public async Task<byte[]> TGenerateInvoicePdfAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsyncWithAppUserandOrderItemsandInvoice(orderId);
            if (order == null)
                throw new Exception("Sipariş bulunamadı");

            return _pdfService.CreateInvoicePdf(order);
        }

    }
}