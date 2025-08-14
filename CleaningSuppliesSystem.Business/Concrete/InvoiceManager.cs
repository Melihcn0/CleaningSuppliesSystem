using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class InvoiceManager : GenericManager<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _ınvoiceRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _env;
        public InvoiceManager(IInvoiceRepository ınvoiceRepository, IOrderRepository orderRepository, IWebHostEnvironment env) : base(ınvoiceRepository)
        {
            _ınvoiceRepository = ınvoiceRepository;
            _orderRepository = orderRepository;
            _env = env;
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
            var existingInvoice = await _ınvoiceRepository.GetInvoiceByOrderIdAsync(orderId);

            if (existingInvoice != null)
            {
                return;
            }

            var invoice = new Invoice
            {
                OrderId = orderId,
                GeneratedAt = DateTime.Now
            };

            await _ınvoiceRepository.CreateAsync(invoice);
        }



        public async Task<byte[]> TGenerateInvoicePdfAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsyncWithAppUserandOrderItemsandInvoice(orderId);
            if (order == null)
                throw new Exception("Sipariş bulunamadı.");

            decimal subtotal = 0;
            decimal totalVat = 0;
            decimal discountTotal = 0;

            var logoPath = Path.Combine(_env.WebRootPath, "images", "ess_star_logo_dark.png");
            byte[] logoData = File.Exists(logoPath)
                ? await File.ReadAllBytesAsync(logoPath)
                : Array.Empty<byte>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50); // margin biraz azaltıldı ki logo tam üste yaklaşsın
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                    page.Content()
                .Column(column =>
                {
                    // --- HEADER: fatura ve logo aynı hizada, dikey ortalı ---
                    column.Item().Row(row =>
                    {
                        // Soldaki sütun (fatura bilgileri) -> dikey ortalanmış
                        row.RelativeColumn()
                            .AlignMiddle() // önemli: burada AlignMiddle çağrısı RelativeColumn() üzerinde olmalı
                            .Column(col =>
                            {
                                col.Item().Text($"Fatura #{order.OrderNumber ?? string.Empty}")
                                    .FontSize(14)
                                    .SemiBold()
                                    .FontColor(Colors.Black);

                                col.Item().Text($"Düzenlenme Tarihi: {order.Invoice?.GeneratedAt:dd.MM.yyyy}")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                        // Sağdaki sütun (logo) -> dikey ortalanmış, sağa yaslı
                        if (logoData.Length > 0)
                        {
                            row.ConstantColumn(140)
                                .Height(80)
                                .AlignRight()
                                .AlignMiddle() // AlignMiddle burada ConstantColumn() üzerinde
                                .Image(logoData, ImageScaling.FitHeight);
                        }
                        else
                        {
                            // Logo yoksa placeholder kutu
                            row.ConstantColumn(140)
                                .Height(80)
                                .AlignRight()
                                .AlignMiddle()
                                .Background(Colors.Grey.Lighten3)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten1)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text("LOGO")
                                .FontSize(12)
                                .FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // --- FROM & FOR SECTION ---
                    column.Item().PaddingTop(10).Row(row =>
                    {
                        // From (Gönderen)
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Şirket Bilgileri (Gönderen)")
                                .FontSize(11)
                                .SemiBold()
                                .FontColor(Colors.Black);

                            col.Item().PaddingTop(4).BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(4);

                            col.Item().PaddingTop(4).Text("Şirket Adı: Ess Star Tedarik").FontSize(10).SemiBold();
                            col.Item().Text("Yetkili: Zülküf Aktaş").FontSize(10).SemiBold();
                            col.Item().Text("Adres: Mustafapaşa, 709 sok no 25/b, 41400 Gebze/Kocaeli, Türkiye").FontSize(9);
                            col.Item().Text("E-posta: zulkufa71@gmail.com").FontSize(9);
                            col.Item().Text("Telefon: +90 533 407 31 97").FontSize(9);
                        });

                        // For (Alıcı)
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Müşteri Bilgileri (Alıcı)")
                                .FontSize(11)
                                .SemiBold()
                                .FontColor(Colors.Black);

                            col.Item().PaddingTop(4).BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(4);

                            col.Item().PaddingTop(4).Text($"Müşteri Adı Soyadı: {order.AppUser?.FirstName} {order.AppUser?.LastName}").FontSize(10).SemiBold();
                            col.Item().Text($"E-posta: {order.AppUser?.Email}").FontSize(9);
                            col.Item().Text($"Kullanıcı Adı: {order.AppUser?.UserName}").FontSize(9);
                            col.Item().Text($"Sipariş Numarası: #{order.OrderNumber}").FontSize(9);
                            col.Item().Text($"Sipariş Tarihi: {order.CreatedDate:dd.MM.yyyy}").FontSize(9);
                        });
                    });


                    // --- PRODUCTS TABLE ---
                    column.Item().PaddingTop(40).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);   // #
                                    columns.RelativeColumn(5);    // Product
                                    columns.ConstantColumn(80);   // Unit Price (excl VAT)
                                    columns.ConstantColumn(50);   // VAT Rate
                                    columns.ConstantColumn(60);   // VAT Amount
                                    columns.ConstantColumn(60);   // Quantity
                                    columns.ConstantColumn(80);   // Total (incl VAT)
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Text("#").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().Text("Ürün").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().AlignRight().Text("Birim Fiyat (KDV'siz)").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().AlignRight().Text("KDV %").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().AlignRight().Text("KDV Tutarı").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().AlignRight().Text("Miktar").FontSize(11).SemiBold().FontColor(Colors.Black);
                                    header.Cell().AlignRight().Text("Toplam (KDV Dahil)").FontSize(11).SemiBold().FontColor(Colors.Black);
                                });

                                // Products
                                int count = 1;
                                foreach (var item in order.OrderItems)
                                {
                                    var originalPrice = item.Product?.UnitPrice ?? 0;
                                    var discountedPrice = item.Product?.DiscountedPrice ?? 0;
                                    var unitPriceExclVat = discountedPrice > 0 ? discountedPrice : originalPrice; // KDV'siz fiyat

                                    var vatRate = item.Product?.VatRate ?? 0;
                                    var vatAmountPerUnit = unitPriceExclVat * vatRate / 100;
                                    var unitPriceInclVat = unitPriceExclVat + vatAmountPerUnit;

                                    var quantity = item.Quantity;

                                    decimal totalExclVat = unitPriceExclVat * quantity;
                                    decimal vatAmountTotal = vatAmountPerUnit * quantity;  // Değişken ismi değişti çakışma önlendi
                                    decimal totalInclVat = unitPriceInclVat * quantity;

                                    subtotal += totalExclVat;
                                    totalVat += vatAmountTotal;
                                    discountTotal += (originalPrice - unitPriceExclVat) * quantity;

                                    if (count == 1)
                                    {
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).Text(count.ToString()).FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).Text(item.Product?.Name ?? "").FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).AlignRight().Text(unitPriceExclVat.ToString("C")).FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).AlignRight().Text(vatRate.ToString("N2") + "%").FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).AlignRight().Text(vatAmountTotal.ToString("C")).FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).AlignRight().Text(quantity.ToString()).FontSize(10);
                                        table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8).AlignRight().Text(totalInclVat.ToString("C")).FontSize(10).SemiBold();
                                    }
                                    else
                                    {
                                        table.Cell().PaddingVertical(4).Text(count.ToString()).FontSize(10);
                                        table.Cell().PaddingVertical(4).Text(item.Product?.Name ?? "").FontSize(10);
                                        table.Cell().PaddingVertical(4).AlignRight().Text(unitPriceExclVat.ToString("C")).FontSize(10);
                                        table.Cell().PaddingVertical(4).AlignRight().Text(vatRate.ToString("N2") + "%").FontSize(10);
                                        table.Cell().PaddingVertical(4).AlignRight().Text(vatAmountTotal.ToString("C")).FontSize(10);
                                        table.Cell().PaddingVertical(4).AlignRight().Text(quantity.ToString()).FontSize(10);
                                        table.Cell().PaddingVertical(4).AlignRight().Text(totalInclVat.ToString("C")).FontSize(10).SemiBold();
                                    }
                                    count++;
                                }

                                // Total row
                                decimal finalTotal = subtotal + totalVat;

                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8);
                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8);
                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8);
                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8);
                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8).AlignRight().Text("Genel Toplam:").FontSize(11).SemiBold();
                                table.Cell().BorderTop(1).BorderColor(Colors.Black).PaddingTop(8).AlignRight().Text(finalTotal.ToString("C")).FontSize(11).SemiBold();
                            });

                            // --- COMMENTS SECTION ---
                            column.Item().PaddingTop(40)
                                .Background(Colors.Grey.Lighten4)
                                .Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("Yorumlar")
                                        .FontSize(12)
                                        .SemiBold()
                                        .FontColor(Colors.Black);

                                    col.Item().PaddingTop(5).Text("Bizi tercih ettiğiniz için teşekkür ederiz. Ürünlerimizin kalitesi ve hizmet anlayışımızla sizleri memnun etmeye devam ediyoruz. Herhangi bir sorunuz veya talebiniz için bizimle iletişime geçebilirsiniz.")
                                        .FontSize(10)
                                        .FontColor(Colors.Black)
                                        .LineHeight(1.4f);
                                });
                        });

                    // --- FOOTER ---
                    page.Footer()
                        .AlignCenter()
                        .Text("Sayfa 1")
                        .FontSize(10)
                        .FontColor(Colors.Black);
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            ms.Position = 0;
            return ms.ToArray();
        }







    }
}
