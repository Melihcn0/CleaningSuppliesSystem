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

            var logoPath = Path.Combine(_env.WebRootPath, "images", "ess_star_logo_dark.png");
            byte[] logoData = File.Exists(logoPath)
                ? await File.ReadAllBytesAsync(logoPath)
                : Array.Empty<byte>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));

                    page.Content().Column(column =>
                    {
                        // Calculate totals first
                        decimal subtotal = 0;
                        decimal totalVat = 0;

                        foreach (var item in order.OrderItems)
                        {
                            var quantity = item.Quantity;
                            var unitPriceInclVat = item.Product?.UnitPrice ?? 0;
                            var vatRate = item.Product?.VatRate ?? 0;

                            var unitPriceExclVat = unitPriceInclVat / (1 + vatRate / 100);
                            var vatAmountPerUnit = unitPriceInclVat - unitPriceExclVat;
                            decimal totalExclVat = unitPriceExclVat * quantity;
                            decimal vatAmountTotal = vatAmountPerUnit * quantity;

                            subtotal += totalExclVat;
                            totalVat += vatAmountTotal;
                        }

                        // === HEADER SECTION ===
                        column.Item().Row(row =>
                        {
                            // Sol taraf - Şirket Bilgileri
                            row.RelativeColumn(2).Column(col =>
                            {
                                col.Item().Text("Satıcı (Şube):").FontSize(8).SemiBold();
                                col.Item().Text("ESS STAR TEDARİK A.Ş.").FontSize(10).SemiBold();

                                col.Item().PaddingTop(2).Text("Adres:").FontSize(8).SemiBold();
                                col.Item().Text("MUSTAFAPAŞA MAHALLESİ, 41400 ")
                                    .FontSize(8);
                                col.Item().Text("709 SOK NO 25/B").FontSize(8);
                                col.Item().Text("41400 KOCAELİ, TÜRKİYE").FontSize(8);

                                col.Item().PaddingTop(2).Row(r =>
                                {
                                    r.RelativeColumn().Column(c =>
                                    {
                                        c.Item().Text("Telefon / Faks:").FontSize(8).SemiBold();
                                        c.Item().Text("+90 533 407 31 97").FontSize(8);

                                        c.Item().PaddingTop(1).Text("E-Posta / Web:").FontSize(8).SemiBold();
                                        c.Item().Text("zulkufa71@gmail.com").FontSize(8);

                                        c.Item().PaddingTop(1).Text("Vergi No / Dairesi:").FontSize(8).SemiBold();
                                        c.Item().Text("4470211661 / KADIKÖY").FontSize(8);

                                        c.Item().PaddingTop(1).Text("Ticaret Sicil No:").FontSize(8).SemiBold();
                                        c.Item().Text("123456").FontSize(8);

                                        c.Item().PaddingTop(1).Text("Mersis No:").FontSize(8).SemiBold();
                                        c.Item().Text("12-87-54-24-41-58-74").FontSize(8);
                                    });  
                                });
                            });

                            // Orta - Logo
                            row.ConstantColumn(120).AlignCenter().Column(col =>
                            {
                                if (logoData.Length > 0)
                                {
                                    col.Item().Height(80).AlignCenter()
                                        .Image(logoData, ImageScaling.FitHeight);
                                }
                                else
                                {
                                    col.Item().Height(80).AlignCenter()
                                        .Background(Colors.Grey.Lighten3)
                                        .Border(1).BorderColor(Colors.Grey.Lighten1)
                                        .AlignCenter().Text("LOGO").FontSize(12).FontColor(Colors.Grey.Darken1);
                                }
                            });

                            // Yeni hali:
                            // Sağ taraf - Fatura Bilgileri
                            row.RelativeColumn().Column(col =>
                            {
                                col.Item().AlignRight().Text("FATURA").FontSize(12).SemiBold(); // Başlık değişti

                                col.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Darken1)
                                    .Padding(8).Column(innerCol =>
                                    {
                                        innerCol.Item().Row(r =>
                                        {
                                            r.RelativeColumn().Text("Fatura No:").FontSize(8).SemiBold();
                                            r.RelativeColumn().AlignRight().Text($"#{order.OrderNumber}").FontSize(8);
                                        });

                                        innerCol.Item().Row(r =>
                                        {
                                            r.RelativeColumn().Text("Fatura Tarihi:").FontSize(8).SemiBold();
                                            r.RelativeColumn().AlignRight().Text($"{order.Invoice?.GeneratedAt:dd/MM/yyyy}").FontSize(8);
                                        });

                                        innerCol.Item().Row(r =>
                                        {
                                            r.RelativeColumn().Text("Fatura Saati:").FontSize(8).SemiBold();
                                            r.RelativeColumn().AlignRight().Text($"{order.Invoice?.GeneratedAt:HH:mm:ss}").FontSize(8);
                                        });
                                    });
                            });
                        });

                        // === CUSTOMER INFO ===
                        column.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Alıcı (Şube):").FontSize(8).SemiBold();
                            col.Item().Text($"{order.AppUser?.FirstName} {order.AppUser?.LastName}").FontSize(10).SemiBold();

                            col.Item().PaddingTop(2).Text("Adres:").FontSize(8).SemiBold();
                            col.Item().Text("Aydenevler Mh. Samatya Cd. Mendirlek Sk. No: 3 Orka Plaza")
                                .FontSize(8);
                            col.Item().Text("Maltepe, İstanbul, TÜRKİYE").FontSize(8);

                            col.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeColumn().Column(c =>
                                {
                                    c.Item().Text("Telefon / Faks:").FontSize(8).SemiBold();
                                    c.Item().Text("2162130061").FontSize(8);

                                    c.Item().PaddingTop(1).Text("Vergi No / Dairesi:").FontSize(8).SemiBold();
                                    c.Item().Text($"{order.AppUser?.UserName ?? "11111111111"}").FontSize(8);
                                });
                            });
                        });
                       
                        foreach (var item in order.OrderItems)
                        {
                            var quantity = item.Quantity;
                            var unitPriceInclVat = item.Product?.UnitPrice ?? 0;
                            var vatRate = item.Product?.VatRate ?? 0;

                            var unitPriceExclVat = unitPriceInclVat / (1 + vatRate / 100);
                            var vatAmountPerUnit = unitPriceInclVat - unitPriceExclVat;
                            decimal totalExclVat = unitPriceExclVat * quantity;
                            decimal vatAmountTotal = vatAmountPerUnit * quantity;

                            // Tanımlanmış değişkenlere değer ekleyin
                            subtotal += totalExclVat;
                            totalVat += vatAmountTotal;
                        }

                        // === PRODUCTS TABLE ===
                        column.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);   // Sıra No
                                columns.RelativeColumn(4);    // Ürün Adı
                                columns.ConstantColumn(50);   // Miktar
                                columns.ConstantColumn(60);   // Birim
                                columns.ConstantColumn(70);   // Birim Fiyat
                                columns.ConstantColumn(50);   // Brüt Fiyat
                                columns.ConstantColumn(45);   // KDV Oranı
                                columns.ConstantColumn(70);   // KDV Tutarı
                                columns.ConstantColumn(70);   // Diğer Vergiler
                                columns.ConstantColumn(70);   // Ürün Tutarı
                            });

                            var headerStyle = TextStyle.Default.SemiBold().FontSize(8);
                            var borderColor = Colors.Black;
                            var cellStyle = TextStyle.Default.FontSize(8);

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Sıra No").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Ürün / Hizmet Adı").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Miktar").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Birim").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Birim Fiyat").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("K.D.V. Oranı").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("K.D.V. Tutarı").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Diğer Vergiler").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Ürün / Hizmet Tutarı").AlignCenter().Style(headerStyle);
                            });

                            // Table Rows
                            int count = 1;
                            foreach (var item in order.OrderItems)
                            {
                                var quantity = item.Quantity;
                                var unitPriceInclVat = item.Product?.UnitPrice ?? 0;
                                var vatRate = item.Product?.VatRate ?? 0;
                                var productName = item.Product?.Name ?? "";

                                var unitPriceExclVat = unitPriceInclVat / (1 + vatRate / 100);
                                var vatAmountPerUnit = unitPriceInclVat - unitPriceExclVat;
                                decimal totalExclVat = unitPriceExclVat * quantity;
                                decimal vatAmountTotal = vatAmountPerUnit * quantity;
                                decimal totalInclVat = unitPriceInclVat * quantity;

                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(count.ToString()).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(productName).AlignLeft().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text($"{quantity:N0}").AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("adet").AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(unitPriceExclVat.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("%" + vatRate.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(vatAmountTotal.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("-").AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(totalInclVat.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                count++;
                            }
                        });

                        // === TOTALS SECTION ===
                        column.Item().PaddingTop(10).Column(col =>
                        {
                            // KDV oranlarına göre gruplandırılmış toplamları hesaplayın
                            var vatRateGroups = order.OrderItems
                                .GroupBy(item => item.Product?.VatRate ?? 0)
                                .Select(group => new
                                {
                                    VatRate = group.Key,
                                    Subtotal = group.Sum(item => (item.Product?.UnitPrice ?? 0) * item.Quantity / (1 + (item.Product?.VatRate ?? 0) / 100)),
                                    TotalVat = group.Sum(item => ((item.Product?.UnitPrice ?? 0) - (item.Product?.UnitPrice ?? 0) / (1 + (item.Product?.VatRate ?? 0) / 100)) * item.Quantity)
                                })
                                .ToList();

                            decimal finalTotal = subtotal + totalVat;

                            col.Item().Row(row =>
                            {
                                row.RelativeColumn(2).Text("");
                                row.RelativeColumn().Column(totalsCol =>
                                {
                                    // Ürün/Hizmet Toplam Tutarı
                                    totalsCol.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Ürün / Hizmet Toplam Tutarı:").FontSize(9).SemiBold();
                                        r.ConstantColumn(80).AlignRight().Text(subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // KDV Oranlarına Göre Matrah ve KDV Tutarları
                                    foreach (var group in vatRateGroups)
                                    {
                                        totalsCol.Item().Row(r =>
                                        {
                                            r.RelativeColumn().Text($"K.D.V. Matrahı (%{group.VatRate}):").FontSize(9).SemiBold();
                                            r.ConstantColumn(80).AlignRight().Text(group.Subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });

                                        totalsCol.Item().Row(r =>
                                        {
                                            r.RelativeColumn().Text($"Hesaplanan K.D.V. (%{group.VatRate}):").FontSize(9).SemiBold();
                                            r.ConstantColumn(80).AlignRight().Text(group.TotalVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });
                                    }

                                    // Vergiler Dahil Toplam Tutar
                                    totalsCol.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Vergiler Dahil Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(80).AlignRight().Text(finalTotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Vergiler Hariç Toplam Tutar
                                    totalsCol.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Vergiler Hariç Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(80).AlignRight().Text(subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ödenecek Tutar
                                    totalsCol.Item().Border(1).BorderColor(Colors.Black).Padding(3).Row(r =>
                                    {
                                        r.RelativeColumn().Text("ÖDENECEK TUTAR:").FontSize(10).SemiBold();
                                        r.ConstantColumn(80).AlignRight().Text(finalTotal.ToString("N2") + " TL").FontSize(10).SemiBold();
                                    });
                                });
                            });
                        });

                        // === NOTLAR BÖLÜMÜ ===
                        column.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Notlar:").FontSize(9).SemiBold();
                            // Bu ibare, belgenin sevk irsaliyesi yerine geçtiğini belirtir.
                            col.Item().Text("İŞBU BELGE SEVK İRSALİYESİ YERİNE GEÇER.").FontSize(8).SemiBold();
                            col.Item().Text("Yalnız: " + NumberToWords(subtotal + totalVat) + " TL").FontSize(8);
                            col.Item().PaddingTop(5).Text("Sipariş Notu: " +
                                (!string.IsNullOrEmpty(order.OrderNote) ? order.OrderNote : "Sipariş notu bulunmamaktadır."))
                                .FontSize(8);
                        });
                    });

                    // === FOOTER ===
                    page.Footer().Column(col =>
                    {
                        col.Item().AlignCenter().Text("ESS Star Tedarik tarafından hazırlanmıştır - www.essstartedarik.com")
                            .FontSize(8).FontColor(Colors.Grey.Darken1);

                        col.Item().PaddingTop(5).AlignRight().Text(x =>
                        {
                            x.Span("Sayfa ").FontSize(8);
                            x.CurrentPageNumber().FontSize(8);
                            x.Span(" / ").FontSize(8);
                            x.TotalPages().FontSize(8);
                        });
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        // Helper method to convert number to words (simplified version)
        private static string NumberToWords(decimal number)
        {
            // Bu metodu tam implementasyon için genişletebilirsiniz
            // Şimdilik basit bir yaklaşım
            var intPart = (int)Math.Floor(number);
            var decPart = (int)Math.Round((number - intPart) * 100);

            return $"{intPart:N0}"; // Basitleştirilmiş
        }




    }
}
