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


        public async Task<Invoice> TCreateInvoiceAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsyncWithAppUserandOrderItemsandInvoice(orderId);
            if (order == null)
                throw new Exception("Sipariş bulunamadı.");

            // Eğer fatura zaten varsa dön
            var existingInvoice = await _ınvoiceRepository.GetInvoiceByOrderIdAsync(orderId);
            if (existingInvoice != null)
                return existingInvoice;

            // Toplam tutar, indirimli fiyat varsa onu kullan
            decimal totalAmount = order.OrderItems.Sum(x => x.Quantity * ((x.Product?.DiscountedPrice > 0 ? x.Product.DiscountedPrice : x.Product?.UnitPrice) ?? 0));

            Invoice invoice = new Invoice
            {
                OrderId = order.Id,
                GeneratedAt = DateTime.Now,
                TotalAmount = totalAmount
            };

            // Default adresleri kontrol et
            var individualAddress = order.AppUser.CustomerIndividualAddresses
                .FirstOrDefault(a => a.IsDefault);

            var corporateAddress = order.AppUser.CustomerCorporateAddresses
                .FirstOrDefault(a => a.IsDefault);

            if (individualAddress != null)
            {
                invoice.InvoiceType = "Individual";
                invoice.FirstName = order.AppUser.FirstName;
                invoice.LastName = order.AppUser.LastName;
                invoice.NationalId = order.AppUser.NationalId;
                invoice.Email = order.AppUser.Email;
                invoice.PhoneNumber = order.AppUser.PhoneNumber;

                invoice.AddressTitle = individualAddress.AddressTitle;
                invoice.Address = individualAddress.Address;
                invoice.CityName = individualAddress.CityName;
                invoice.DistrictName = individualAddress.DistrictName;
            }
            else if (corporateAddress != null)
            {
                invoice.InvoiceType = "Corporate";
                invoice.FirstName = order.AppUser.FirstName;
                invoice.LastName = order.AppUser.LastName;
                invoice.NationalId = order.AppUser.NationalId;
                invoice.Email = order.AppUser.Email;
                invoice.PhoneNumber = order.AppUser.PhoneNumber;

                invoice.CompanyName = corporateAddress.CompanyName;
                invoice.TaxOffice = corporateAddress.TaxOffice;
                invoice.TaxNumber = corporateAddress.TaxNumber;

                invoice.AddressTitle = corporateAddress.AddressTitle;
                invoice.Address = corporateAddress.Address;
                invoice.CityName = corporateAddress.CityName;
                invoice.DistrictName = corporateAddress.DistrictName;
            }
            else
            {
                // Kullanıcının hiç default adresi yoksa hata fırlat
                throw new Exception("Kullanıcının default bir adresi bulunamadı.");
            }

            await _ınvoiceRepository.CreateAsync(invoice);
            return invoice;
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
                            row.ConstantColumn(130).Column(col =>
                            {
                                if (logoData.Length > 0)
                                {
                                    col.Item().Height(100)
                                        .PaddingRight(30)
                                        .AlignCenter()
                                        .Image(logoData, ImageScaling.FitHeight);
                                }
                                else
                                {
                                    col.Item().Height(100)
                                        .PaddingRight(30)
                                        .AlignCenter()
                                        .Background(Colors.Grey.Lighten3)
                                        .Border(1).BorderColor(Colors.Grey.Lighten1)
                                        .Text("LOGO").FontSize(12).FontColor(Colors.Grey.Darken1);
                                }
                            });

                            // Yeni hali:
                            // Sağ taraf - Fatura Bilgileri
                            row.RelativeColumn().Column(col =>
                            {
                                col.Item().AlignRight().Text("İRSALİYELİ FATURA").FontSize(12).SemiBold();

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
                            var invoice = order.Invoice;
                            if (invoice == null)
                            {
                                col.Item().Text("Alıcı bilgisi bulunamadı.").FontSize(8);
                                return;
                            }

                            col.Item().Text("Alıcı:").FontSize(8).SemiBold();

                            if (string.IsNullOrEmpty(invoice.CompanyName))
                            {
                                // Bireysel kullanıcı
                                col.Item().Text($"{invoice.FirstName} {invoice.LastName}").FontSize(9).SemiBold();
                            }
                            else
                            {
                                // Kurumsal kullanıcı
                                col.Item().Text($"Alıcı Şirket: {invoice.CompanyName}").FontSize(8).SemiBold();
                                col.Item().Text($"Alıcı Ad Soyad: {invoice.FirstName} {invoice.LastName}").FontSize(8);
                            }

                            col.Item().Text("Adres Adı: ").FontSize(8).SemiBold();
                            col.Item().Text($"{invoice.AddressTitle ?? ""}").FontSize(8);

                            col.Item().Text("Açık Adres: ").FontSize(8).SemiBold();
                            col.Item().Text($"{invoice.Address ?? ""}").FontSize(8);

                            col.Item().Text($"{invoice.DistrictName ?? ""}, {invoice.CityName ?? ""}, TÜRKİYE").FontSize(8);

                            col.Item().PaddingTop(2).Row(r =>
                            {
                                r.RelativeColumn().Column(c =>
                                {
                                    c.Item().Text("Telefon:").FontSize(8).SemiBold();
                                    c.Item().Text($"{invoice.PhoneNumber ?? "-"}").FontSize(8);

                                    if (string.IsNullOrEmpty(invoice.CompanyName))
                                    {
                                        // Bireysel kullanıcı → T.C. Kimlik No göster
                                        c.Item().PaddingTop(1).Text("T.C. Kimlik No:").FontSize(8).SemiBold();
                                        c.Item().Text($"{invoice.NationalId ?? "-"}").FontSize(8);
                                    }
                                    else
                                    {
                                        // Kurumsal kullanıcı → Vergi bilgileri göster
                                        c.Item().PaddingTop(1).Text("Vergi Dairesi:").FontSize(8).SemiBold();
                                        c.Item().Text($"{invoice.TaxOffice ?? "-"}").FontSize(8);

                                        c.Item().PaddingTop(1).Text("Vergi No:").FontSize(8).SemiBold();
                                        c.Item().Text($"{invoice.TaxNumber ?? "-"}").FontSize(8);
                                    }
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
                                columns.ConstantColumn(30);   // Sıra No
                                columns.RelativeColumn(100);     // Ürün Adı
                                columns.ConstantColumn(30);   // Miktar
                                columns.ConstantColumn(30);   // Birim
                                columns.ConstantColumn(60);   // Birim Fiyat
                                columns.ConstantColumn(60);   // Brüt Fiyat
                                columns.ConstantColumn(30);   // KDV Oranı
                                columns.ConstantColumn(60);   // KDV Tutarı
                                columns.ConstantColumn(60);   // Ürün Tutarı
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
                                    .Text("Ürün / Hizmet Adı").AlignCenter().Style(headerStyle).WrapAnywhere();
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Miktar").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Birim").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Birim Fiyat").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("Brüt Fiyat").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("K.D.V. Oranı").AlignCenter().Style(headerStyle);
                                header.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("K.D.V. Tutarı").AlignCenter().Style(headerStyle);
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
                                decimal totalInclVat = unitPriceInclVat * quantity;
                                decimal vatAmountTotal = vatAmountPerUnit * quantity;
                                decimal grossPrice = unitPriceInclVat * quantity;

                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(count.ToString()).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(productName).AlignLeft().Style(cellStyle).WrapAnywhere();
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text($"{quantity:N0}").AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("adet").AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(unitPriceExclVat.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(grossPrice.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text("%" + vatRate.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2)
                                    .Text(vatAmountTotal.ToString("N2") + " TL").AlignRight().Style(cellStyle);
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
                                    totalsCol.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("Ürün / Hizmet Toplam Tutarı:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // KDV Oranlarına Göre Matrah ve KDV Tutarları
                                    foreach (var group in vatRateGroups)
                                    {
                                        totalsCol.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).Text($"K.D.V. Matrahı (%{group.VatRate}):").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(group.Subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });

                                        totalsCol.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).Text($"Hesaplanan K.D.V. (%{group.VatRate}):").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(group.TotalVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });
                                    }

                                    // Vergiler Dahil Toplam Tutar
                                    totalsCol.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Dahil Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(finalTotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Vergiler Hariç Toplam Tutar
                                    totalsCol.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Hariç Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(subtotal.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ödenecek Tutar
                                    totalsCol.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("ÖDENECEK TUTAR:").FontSize(10).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(finalTotal.ToString("N2") + " TL").FontSize(10).SemiBold();
                                    });
                                });
                            });
                        });

                        // === NOTLAR BÖLÜMÜ ===
                        column.Item().PaddingTop(20).Column(col =>
                        {
                            col.Item().Text("Notlar:").FontSize(9).SemiBold();
                            col.Item().Text("İŞBU BELGE SEVK İRSALİYESİ YERİNE GEÇER.").FontSize(8).SemiBold();
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
