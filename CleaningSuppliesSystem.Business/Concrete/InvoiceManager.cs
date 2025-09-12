using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class InvoiceManager(IInvoiceRepository _invoiceRepository, IAdminProfileRepository _adminProfileRepository, IOrderRepository _orderRepository, IWebHostEnvironment _env, IHttpContextAccessor _httpContextAccessor) : GenericManager<Invoice>(_invoiceRepository), IInvoiceService
    {
        public async Task<List<Invoice>> TGetInvoiceWithOrderAsync()
        {
            return await _invoiceRepository.GetInvoiceWithOrderAsync();
        }
        public async Task<Invoice> TGetByIdAsyncWithOrder(int orderId)
        {
            return await _invoiceRepository.GetByIdAsyncWithOrder(orderId);
        }
        public async Task<Invoice> TGetInvoiceByOrderIdAsync(int orderId)
        {
            return await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
        }
        public async Task<List<Invoice>> TGetInvoicesByUserIdAsync(int userId)
        {
            return await _invoiceRepository.GetInvoicesByUserIdAsync(userId);
        }

        public async Task<Invoice> TCreateAdminInvoiceAsync(int orderId)
        {
            // Siparişi getir
            var order = await _orderRepository.GetOrderByIdWithDetailsAsync(orderId);
            if (order == null) throw new Exception("Sipariş bulunamadı.");

            // Fatura zaten varsa dön
            var existingInvoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
            if (existingInvoice != null) return existingInvoice;

            // Login olan admini al
            var adminId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var adminUser = await _adminProfileRepository.GetUserWithCompanyBankAndAddressAsync(adminId);
            if (adminUser == null) throw new Exception("Admin bilgisi bulunamadı.");

            // Invoice nesnesini oluştur
            Invoice invoice = new Invoice
            {
                OrderId = order.Id,
                GeneratedAt = DateTime.Now,
                TotalAmount = 0,

                // Admin bilgileri
                AdminId = adminUser.Id,
                AdminFirstName = adminUser.FirstName,
                AdminLastName = adminUser.LastName,
                AdminPhoneNumber = adminUser.PhoneNumber,

                // Şirket snapshot
                InvoiceCompanyName = adminUser.CompanyAddress?.CompanyName,
                InvoiceCompanyTaxOffice = adminUser.CompanyAddress?.TaxOffice,
                InvoiceCompanyTaxNumber = adminUser.CompanyAddress?.TaxNumber,
                InvoiceCompanyAddress = adminUser.CompanyAddress?.Address,
                InvoiceCompanyCityName = adminUser.CompanyAddress?.CityName,
                InvoiceCompanyDistrictName = adminUser.CompanyAddress?.DistrictName
            };

            // Kullanıcının varsayılan adresini çek
            var individualAddress = order.AppUser.CustomerIndividualAddresses.FirstOrDefault(a => a.IsDefault);
            var corporateAddress = order.AppUser.CustomerCorporateAddresses.FirstOrDefault(a => a.IsDefault);

            if (individualAddress != null)
            {
                invoice.InvoiceType = "Individual";
                invoice.CustomerFirstName = order.AppUser.FirstName;
                invoice.CustomerLastName = order.AppUser.LastName;
                invoice.CustomerNationalId = order.AppUser.NationalId;
                invoice.CustomerPhoneNumber = order.AppUser.PhoneNumber;
                invoice.CustomerAddressTitle = individualAddress.AddressTitle;
                invoice.CustomerAddress = individualAddress.Address;
                invoice.CustomerCityName = individualAddress.CityName;
                invoice.CustomerDistrictName = individualAddress.DistrictName;
            }
            else if (corporateAddress != null)
            {
                invoice.InvoiceType = "Corporate";
                invoice.CustomerFirstName = order.AppUser.FirstName;
                invoice.CustomerLastName = order.AppUser.LastName;
                invoice.CustomerNationalId = order.AppUser.NationalId;
                invoice.CustomerPhoneNumber = order.AppUser.PhoneNumber;
                invoice.CustomerCompanyName = corporateAddress.CompanyName;
                invoice.CustomerTaxOffice = corporateAddress.TaxOffice;
                invoice.CustomerTaxNumber = corporateAddress.TaxNumber;
                invoice.CustomerAddressTitle = corporateAddress.AddressTitle;
                invoice.CustomerAddress = corporateAddress.Address;
                invoice.CustomerCityName = corporateAddress.CityName;
                invoice.CustomerDistrictName = corporateAddress.DistrictName;
            }
            else
            {
                throw new Exception("Kullanıcının varsayılan bir adresi bulunamadı.");
            }

            // InvoiceItem’ları ekle
            decimal totalAmount = 0;
            invoice.InvoiceItems = new List<InvoiceItem>();
            foreach (var item in order.OrderItems)
            {
                var product = item.Product;

                // KDV dahil liste fiyatı
                var priceWithVat = product.PriceWithVat;

                // KDV dahil indirimli fiyat
                var discountedPriceWithVat = product.DiscountedPriceWithVat;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    Unit = "adet",
                    UnitPrice = product.UnitPrice,                   // KDV hariç liste fiyatı
                    DiscountRate = product.DiscountRate,            // % (DB’de varsa)
                    DiscountedUnitPrice = product.DiscountRate > 0
                                          ? product.DiscountedPriceWithVat
                                          : product.PriceWithVat, // KDV dahil indirimli fiyat
                    DiscountAmount = product.DiscountRate > 0
                                     ? (priceWithVat - discountedPriceWithVat) * item.Quantity
                                     : 0,                        // KDV dahil indirim tutarı
                    VatRate = product.VatRate,
                    VatAmount = discountedPriceWithVat - (product.UnitPrice * item.Quantity), // KDV tutarı
                    TotalPrice = discountedPriceWithVat * item.Quantity                  // KDV dahil toplam
                });
            }


            invoice.TotalAmount = totalAmount;

            await _invoiceRepository.CreateAsync(invoice);
            return invoice;
        }


        private static string ConvertAmountToText(decimal amount)
        {
            int lira = (int)Math.Floor(amount);
            int kurus = (int)Math.Round((amount - lira) * 100);

            string liraText = UppercaseFirst(NumberToTurkishText(lira));
            string kurusText = UppercaseFirst(NumberToTurkishText(kurus));

            if (kurus > 0)
                return $"Yalnız: #{liraText} Türk Lirası {kurusText} Kuruş#";
            else
                return $"Yalnız: #{liraText} Türk Lirası#";
        }

        // Basit sayıdan metne dönüşüm (1-9999 arası)
        private static string NumberToTurkishText(int number)
        {
            if (number == 0) return "sıfır";

            string[] birler = { "", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz" };
            string[] onlar = { "", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
            string text = "";

            if (number >= 1000)
            {
                int binler = number / 1000;
                text += (binler > 1 ? birler[binler] : "") + "bin ";
                number %= 1000;
            }

            if (number >= 100)
            {
                int yuzler = number / 100;
                text += (yuzler > 1 ? birler[yuzler] : "") + "yüz ";
                number %= 100;
            }

            if (number >= 10)
            {
                int on = number / 10;
                text += onlar[on] + " ";
                number %= 10;
            }

            if (number > 0)
                text += birler[number] + " ";

            return text.Trim();
        }

        // İlk harfi büyük yap
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }


        public async Task<byte[]> TGenerateAdminDeliveryNoteInvoicePdfAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdWithDetailsAsync(orderId);
            if (order == null) throw new Exception("Sipariş bulunamadı.");

            var invoice = order.Invoice;
            if (invoice == null) throw new Exception("Fatura bulunamadı.");

            var logoPath = Path.Combine(_env.WebRootPath, "images", "ess_star_logo_dark.png");
            byte[] logoData = File.Exists(logoPath) ? await File.ReadAllBytesAsync(logoPath) : Array.Empty<byte>();

            // Null güvenli helper
            string Safe(string? value) => string.IsNullOrEmpty(value) ? "-" : value;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));

                    page.Content().Column(col =>
                    {
                        // HEADER
                        col.Item().Row(row =>
                        {
                            // Şirket Bilgileri
                            row.RelativeColumn(2).Column(c =>
                            {
                                c.Item().Text("Satıcı (Şube):").FontSize(8).SemiBold();
                                c.Item().Text(Safe(invoice.InvoiceCompanyName)).FontSize(10).SemiBold();
                                c.Item().Text("Vergi Dairesi: " + Safe(invoice.InvoiceCompanyTaxOffice)).FontSize(8);
                                c.Item().Text("Vergi No: " + Safe(invoice.InvoiceCompanyTaxNumber)).FontSize(8);
                                c.Item().Text("Adres: " + Safe(invoice.InvoiceCompanyAddress)).FontSize(8);
                                c.Item().Text(Safe(invoice.InvoiceCompanyDistrictName) + " / " + Safe(invoice.InvoiceCompanyCityName)).FontSize(8);
                                c.Item().PaddingTop(2).Text("Yetkili: " + Safe(invoice.AdminFirstName) + " " + Safe(invoice.AdminLastName)).FontSize(8);

                                c.Item().Text("Telefon: " + Safe(invoice.AdminPhoneNumber)).FontSize(8);
                            });

                            // Logo
                            row.ConstantColumn(130).Column(c =>
                            {
                                if (logoData.Length > 0)
                                {
                                    c.Item()
                                     .PaddingRight(20) // Sağdan boşluk bırakır
                                     .Height(110)
                                     .AlignCenter()
                                     .Image(logoData, ImageScaling.FitHeight);
                                }
                                else
                                {
                                    c.Item()
                                     .PaddingRight(20) // Sağdan boşluk bırakır
                                     .Height(110)
                                     .AlignCenter()
                                     .Background(Colors.Grey.Lighten3)
                                     .Border(1)
                                     .BorderColor(Colors.Grey.Lighten1)
                                     .Text("LOGO")
                                     .FontSize(12);
                                }
                            });

                            // Fatura Bilgileri
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignRight().Text("İRSALİYELİ FATURA").FontSize(12).SemiBold();
                                c.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(inner =>
                                {
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura No:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"#{Safe(order.OrderNumber)}").FontSize(8);
                                    });
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura Tarihi:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"{invoice.GeneratedAt:dd/MM/yyyy}").FontSize(8);
                                    });
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura Saati:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"{invoice.GeneratedAt:HH:mm:ss}").FontSize(8);
                                    });
                                });
                            });
                        });

                        // Alıcı Bilgileri
                        col.Item().Column(c =>
                        {
                            c.Item().Text("Alıcı:").FontSize(8).SemiBold();
                            if (string.IsNullOrEmpty(invoice.CustomerCompanyName))
                            {
                                c.Item().Text($"{Safe(invoice.CustomerFirstName)} {Safe(invoice.CustomerLastName)}").FontSize(9).SemiBold();
                                c.Item().Text("T.C. Kimlik No: " + Safe(invoice.CustomerNationalId)).FontSize(8);
                            }
                            else
                            {
                                c.Item().Text(Safe(invoice.CustomerCompanyName)).FontSize(10).SemiBold();
                                c.Item().Text("Vergi Dairesi: " + Safe(invoice.CustomerTaxOffice)).FontSize(8);
                                c.Item().Text("Vergi No: " + Safe(invoice.CustomerTaxNumber)).FontSize(8);
                                c.Item().Text("Yetkili: " + Safe(invoice.CustomerFirstName) + " " + Safe(invoice.CustomerLastName)).FontSize(8);
                            }
                            c.Item().Text("Adres: " + Safe(invoice.CustomerAddress)).FontSize(8);
                            c.Item().Text(Safe(invoice.CustomerDistrictName) + " / " + Safe(invoice.CustomerCityName)).FontSize(8);
                            c.Item().Text("Telefon: " + Safe(invoice.CustomerPhoneNumber)).FontSize(8);
                        });

                        // Ürünler ve toplamlar
                        var items = invoice.InvoiceItems ?? new List<InvoiceItem>();
                        decimal totalExclVat = 0, totalVat = 0, totalInclVat = 0;

                        col.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);   // Sıra No
                                columns.RelativeColumn(130);  // Malzeme
                                columns.ConstantColumn(45);   // Miktar
                                columns.ConstantColumn(55);   // Birim fiyat
                                columns.ConstantColumn(45);   // İskonto oranı
                                columns.ConstantColumn(55);   // İskonto tutarı
                                columns.ConstantColumn(35);   // KDV oranı
                                columns.ConstantColumn(55);   // KDV tutarı
                                columns.ConstantColumn(55);   // Diğer vergiler
                                columns.ConstantColumn(60);   // Tutar (KDV hariç)
                            });

                            var headerStyle = TextStyle.Default.SemiBold().FontSize(9);
                            var cellStyle = TextStyle.Default.FontSize(8);
                            var borderColor = Colors.Black;

                            table.Header(header =>
                            {
                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("SIRA").AlignCenter().Style(headerStyle);
                                    col.Item().Text("NO").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("MAL/HİZMET").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("MİKTAR").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("BİRİM").AlignCenter().Style(headerStyle);
                                    col.Item().Text("FİYAT").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("İSKONTO").AlignCenter().Style(headerStyle);
                                    col.Item().Text("ORANI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("İSKONTO").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTARI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("KDV").AlignCenter().Style(headerStyle);
                                    col.Item().Text("ORANI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("KDV").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTARI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("DİĞER").AlignCenter().Style(headerStyle);
                                    col.Item().Text("VERGİLER").AlignCenter().Style(headerStyle);
                                });
                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTAR").AlignCenter().Style(headerStyle);
                                });

                                // Alt çizgi
                                header.Cell().ColumnSpan(10).PaddingTop(2).BorderBottom(1).BorderColor(Colors.Black);
                            });

                            int count = 1;
                            var separatorColor = Colors.Grey.Lighten2; // Grimsi ton

                            foreach (var item in items)
                            {
                                decimal unitPrice = item.UnitPrice;
                                decimal discountRate = item.DiscountRate ?? 0;
                                bool hasDiscount = discountRate > 0;
                                decimal quantity = item.Quantity;

                                decimal discountedUnitPrice = unitPrice * (1 - discountRate / 100);
                                decimal discountAmount = hasDiscount ? (unitPrice - discountedUnitPrice) * quantity : 0;

                                decimal lineTotalExclVat = discountedUnitPrice * quantity;
                                decimal vatAmount = lineTotalExclVat * (item.VatRate / 100);
                                decimal total = lineTotalExclVat + vatAmount;

                                // Ürün satırı
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(count.ToString()).AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(Safe(item.ProductName)).AlignLeft().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text($"{quantity:N0} adet").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(unitPrice.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(hasDiscount ? "%" + discountRate.ToString("N0") : "-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(hasDiscount ? discountAmount.ToString("N2") + " TL" : "-").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(item.VatRate > 0 ? "%" + item.VatRate.ToString("N0") : "-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(vatAmount.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text("-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(lineTotalExclVat.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                totalExclVat += lineTotalExclVat;
                                totalVat += vatAmount;
                                totalInclVat += total;
                                count++;
                            }



                        });

                        // === TOTALS SECTION ===
                        col.Item().PaddingTop(10).Column(totalsCol =>
                        {
                            totalsCol.Item().Row(row =>
                            {
                                row.RelativeColumn(2).Text("");
                                row.RelativeColumn().Column(colInner =>
                                {
                                    // Toplam Tutar (KDV Hariç)
                                    decimal totalExclVat = items.Sum(i =>
                                    {
                                        decimal discountRate = i.DiscountRate ?? 0;
                                        decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                        return discountedUnitPrice * i.Quantity;
                                    });

                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Toplam Tutar").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalExclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Toplam İskonto
                                    decimal totalDiscount = items.Sum(i =>
                                    {
                                        decimal discountRate = i.DiscountRate ?? 0;
                                        decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                        return (i.UnitPrice - discountedUnitPrice) * i.Quantity;
                                    });

                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Toplam İskonto").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalDiscount.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // KDV grupları
                                    var vatGroups = items.GroupBy(i => i.VatRate).OrderBy(g => g.Key);
                                    decimal totalInclVat = 0;

                                    foreach (var group in vatGroups)
                                    {
                                        decimal groupMatrah = group.Sum(i =>
                                        {
                                            decimal discountRate = i.DiscountRate ?? 0;
                                            decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                            return discountedUnitPrice * i.Quantity;
                                        });

                                        decimal groupVat = groupMatrah * (group.Key / 100);
                                        totalInclVat += groupMatrah + groupVat;

                                        colInner.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).AlignRight().Text($"KDV Matrahı (%{group.Key:N0},00)").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(groupMatrah.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });

                                        colInner.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).AlignRight().Text($"Hesaplanan KDV (%{group.Key:N0},00)").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(groupVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });
                                    }

                                    // Vergiler Dahil Toplam Tutar
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Vergiler Dahil Toplam Tutar").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ödenecek Tutar
                                    colInner.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("ÖDENECEK TUTAR").FontSize(10).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(10).SemiBold();
                                    });

                                    // Yazıyla tutar
                                    colInner.Item().PaddingTop(2).Row(r =>
                                    {
                                        r.RelativeColumn().AlignCenter().Text(ConvertAmountToText(totalInclVat)).FontSize(9).Italic();
                                    });

                                    colInner.Item().PaddingTop(1).Row(r =>
                                    {
                                        r.RelativeColumn().AlignCenter().Text("İşbu sevk irsaliyesi yerine geçer").FontSize(9).Italic();
                                    });
                                });
                            });
                        });

                        // Footer
                        page.Footer().Column(c =>
                        {
                            c.Item().AlignCenter().Text($"Bu Fatura {invoice.InvoiceCompanyName} tarafından hazırlanmıştır.").FontSize(8).FontColor(Colors.Grey.Darken1);
                            c.Item().PaddingTop(5).AlignRight().Text(x =>
                            {
                                x.Span("Sayfa ").FontSize(8);
                                x.CurrentPageNumber().FontSize(8);
                                x.Span(" / ").FontSize(8);
                                x.TotalPages().FontSize(8);
                            });
                        });
                    });
                });
            });

            // HATA AYIKLAMA
            //using var ms = new MemoryStream();
            //try
            //{
            //    document.GeneratePdf(ms);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("PDF oluşturulamadı: " + ex.Message);
            //}

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        public async Task<byte[]> TGenerateCustomerInvoicePdfAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdWithDetailsAsync(orderId);
            if (order == null) throw new Exception("Sipariş bulunamadı.");

            var invoice = order.Invoice;
            if (invoice == null) throw new Exception("Fatura bulunamadı.");

            var logoPath = Path.Combine(_env.WebRootPath, "images", "ess_star_logo_dark.png");
            byte[] logoData = File.Exists(logoPath) ? await File.ReadAllBytesAsync(logoPath) : Array.Empty<byte>();

            // Null güvenli helper
            string Safe(string? value) => string.IsNullOrEmpty(value) ? "-" : value;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));

                    page.Content().Column(col =>
                    {
                        // HEADER
                        col.Item().Row(row =>
                        {
                            // Şirket Bilgileri
                            row.RelativeColumn(2).Column(c =>
                            {
                                c.Item().Text("Satıcı (Şube):").FontSize(8).SemiBold();
                                c.Item().Text(Safe(invoice.InvoiceCompanyName)).FontSize(10).SemiBold();
                                c.Item().Text("Vergi Dairesi: " + Safe(invoice.InvoiceCompanyTaxOffice)).FontSize(8);
                                c.Item().Text("Vergi No: " + Safe(invoice.InvoiceCompanyTaxNumber)).FontSize(8);
                                c.Item().Text("Adres: " + Safe(invoice.InvoiceCompanyAddress)).FontSize(8);
                                c.Item().Text(Safe(invoice.InvoiceCompanyDistrictName) + " / " + Safe(invoice.InvoiceCompanyCityName)).FontSize(8);
                                c.Item().PaddingTop(2).Text("Yetkili: " + Safe(invoice.AdminFirstName) + " " + Safe(invoice.AdminLastName)).FontSize(8);

                                c.Item().Text("Telefon: " + Safe(invoice.AdminPhoneNumber)).FontSize(8);
                            });

                            // Logo
                            row.ConstantColumn(130).Column(c =>
                            {
                                if (logoData.Length > 0)
                                {
                                    c.Item()
                                     .PaddingRight(20) // Sağdan boşluk bırakır
                                     .Height(110)
                                     .AlignCenter()
                                     .Image(logoData, ImageScaling.FitHeight);
                                }
                                else
                                {
                                    c.Item()
                                     .PaddingRight(20) // Sağdan boşluk bırakır
                                     .Height(110)
                                     .AlignCenter()
                                     .Background(Colors.Grey.Lighten3)
                                     .Border(1)
                                     .BorderColor(Colors.Grey.Lighten1)
                                     .Text("LOGO")
                                     .FontSize(12);
                                }
                            });

                            // Fatura Bilgileri
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignRight().Text("FATURA").FontSize(12).SemiBold();
                                c.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(inner =>
                                {
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura No:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"#{Safe(order.OrderNumber)}").FontSize(8);
                                    });
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura Tarihi:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"{invoice.GeneratedAt:dd/MM/yyyy}").FontSize(8);
                                    });
                                    inner.Item().Row(r =>
                                    {
                                        r.RelativeColumn().Text("Fatura Saati:").FontSize(8).SemiBold();
                                        r.RelativeColumn().AlignRight().Text($"{invoice.GeneratedAt:HH:mm:ss}").FontSize(8);
                                    });
                                });
                            });
                        });

                        // Alıcı Bilgileri
                        col.Item().Column(c =>
                        {
                            c.Item().Text("Alıcı:").FontSize(8).SemiBold();
                            if (string.IsNullOrEmpty(invoice.CustomerCompanyName))
                            {
                                c.Item().Text($"{Safe(invoice.CustomerFirstName)} {Safe(invoice.CustomerLastName)}").FontSize(9).SemiBold();
                                c.Item().Text("T.C. Kimlik No: " + Safe(invoice.CustomerNationalId)).FontSize(8);
                            }
                            else
                            {
                                c.Item().Text(Safe(invoice.CustomerCompanyName)).FontSize(10).SemiBold();
                                c.Item().Text("Vergi Dairesi: " + Safe(invoice.CustomerTaxOffice)).FontSize(8);
                                c.Item().Text("Vergi No: " + Safe(invoice.CustomerTaxNumber)).FontSize(8);
                                c.Item().Text("Yetkili: " + Safe(invoice.CustomerFirstName) + " " + Safe(invoice.CustomerLastName)).FontSize(8);
                            }
                            c.Item().Text("Adres: " + Safe(invoice.CustomerAddress)).FontSize(8);
                            c.Item().Text(Safe(invoice.CustomerDistrictName) + " / " + Safe(invoice.CustomerCityName)).FontSize(8);
                            c.Item().Text("Telefon: " + Safe(invoice.CustomerPhoneNumber)).FontSize(8);
                        });

                        // Ürünler ve toplamlar
                        var items = invoice.InvoiceItems ?? new List<InvoiceItem>();
                        decimal totalExclVat = 0, totalVat = 0, totalInclVat = 0;

                        col.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);   // Sıra No
                                columns.RelativeColumn(130);  // Malzeme
                                columns.ConstantColumn(45);   // Miktar
                                columns.ConstantColumn(55);   // Birim fiyat
                                columns.ConstantColumn(45);   // İskonto oranı
                                columns.ConstantColumn(55);   // İskonto tutarı
                                columns.ConstantColumn(35);   // KDV oranı
                                columns.ConstantColumn(55);   // KDV tutarı
                                columns.ConstantColumn(55);   // Diğer vergiler
                                columns.ConstantColumn(60);   // Tutar (KDV hariç)
                            });

                            var headerStyle = TextStyle.Default.SemiBold().FontSize(9);
                            var cellStyle = TextStyle.Default.FontSize(8);
                            var borderColor = Colors.Black;

                            table.Header(header =>
                            {
                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("SIRA").AlignCenter().Style(headerStyle);
                                    col.Item().Text("NO").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("MAL/HİZMET").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("MİKTAR").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("BİRİM").AlignCenter().Style(headerStyle);
                                    col.Item().Text("FİYAT").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("İSKONTO").AlignCenter().Style(headerStyle);
                                    col.Item().Text("ORANI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("İSKONTO").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTARI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("KDV").AlignCenter().Style(headerStyle);
                                    col.Item().Text("ORANI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("KDV").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTARI").AlignCenter().Style(headerStyle);
                                });

                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("DİĞER").AlignCenter().Style(headerStyle);
                                    col.Item().Text("VERGİLER").AlignCenter().Style(headerStyle);
                                });
                                header.Cell().Column(col =>
                                {
                                    col.Item().Text("").AlignCenter().Style(headerStyle);
                                    col.Item().Text("TUTAR").AlignCenter().Style(headerStyle);
                                });

                                // Alt çizgi
                                header.Cell().ColumnSpan(10).PaddingTop(2).BorderBottom(1).BorderColor(Colors.Black);
                            });

                            int count = 1;
                            var separatorColor = Colors.Grey.Lighten2; // Grimsi ton

                            foreach (var item in items)
                            {
                                decimal unitPrice = item.UnitPrice;
                                decimal discountRate = item.DiscountRate ?? 0;
                                bool hasDiscount = discountRate > 0;
                                decimal quantity = item.Quantity;

                                decimal discountedUnitPrice = unitPrice * (1 - discountRate / 100);
                                decimal discountAmount = hasDiscount ? (unitPrice - discountedUnitPrice) * quantity : 0;

                                decimal lineTotalExclVat = discountedUnitPrice * quantity;
                                decimal vatAmount = lineTotalExclVat * (item.VatRate / 100);
                                decimal total = lineTotalExclVat + vatAmount;

                                // Ürün satırı
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(count.ToString()).AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(Safe(item.ProductName)).AlignLeft().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text($"{quantity:N0} adet").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(unitPrice.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(hasDiscount ? "%" + discountRate.ToString("N0") : "-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(hasDiscount ? discountAmount.ToString("N2") + " TL" : "-").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(item.VatRate > 0 ? "%" + item.VatRate.ToString("N0") : "-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(vatAmount.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text("-").AlignCenter().Style(cellStyle);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .Text(lineTotalExclVat.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                totalExclVat += lineTotalExclVat;
                                totalVat += vatAmount;
                                totalInclVat += total;
                                count++;
                            }



                        });

                        // === TOTALS SECTION ===
                        col.Item().PaddingTop(10).Column(totalsCol =>
                        {
                            totalsCol.Item().Row(row =>
                            {
                                row.RelativeColumn(2).Text("");
                                row.RelativeColumn().Column(colInner =>
                                {
                                    // Toplam Tutar (KDV Hariç)
                                    decimal totalExclVat = items.Sum(i =>
                                    {
                                        decimal discountRate = i.DiscountRate ?? 0;
                                        decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                        return discountedUnitPrice * i.Quantity;
                                    });

                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Toplam Tutar").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalExclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Toplam İskonto
                                    decimal totalDiscount = items.Sum(i =>
                                    {
                                        decimal discountRate = i.DiscountRate ?? 0;
                                        decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                        return (i.UnitPrice - discountedUnitPrice) * i.Quantity;
                                    });

                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Toplam İskonto").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalDiscount.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // KDV grupları
                                    var vatGroups = items.GroupBy(i => i.VatRate).OrderBy(g => g.Key);
                                    decimal totalInclVat = 0;

                                    foreach (var group in vatGroups)
                                    {
                                        decimal groupMatrah = group.Sum(i =>
                                        {
                                            decimal discountRate = i.DiscountRate ?? 0;
                                            decimal discountedUnitPrice = i.UnitPrice * (1 - discountRate / 100);
                                            return discountedUnitPrice * i.Quantity;
                                        });

                                        decimal groupVat = groupMatrah * (group.Key / 100);
                                        totalInclVat += groupMatrah + groupVat;

                                        colInner.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).AlignRight().Text($"KDV Matrahı (%{group.Key:N0},00)").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(groupMatrah.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });

                                        colInner.Item().PaddingBottom(3).Row(r =>
                                        {
                                            r.RelativeColumn(150).AlignRight().Text($"Hesaplanan KDV (%{group.Key:N0},00)").FontSize(9).SemiBold();
                                            r.ConstantColumn(60).AlignRight().Text(groupVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                        });
                                    }

                                    // Vergiler Dahil Toplam Tutar
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("Vergiler Dahil Toplam Tutar").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ödenecek Tutar
                                    colInner.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Row(r =>
                                    {
                                        r.RelativeColumn(150).AlignRight().Text("ÖDENECEK TUTAR").FontSize(10).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(10).SemiBold();
                                    });

                                    // Yazıyla tutar
                                    colInner.Item().PaddingTop(2).Row(r =>
                                    {
                                        r.RelativeColumn().AlignCenter().Text(ConvertAmountToText(totalInclVat)).FontSize(9).Italic();
                                    });
                                });
                            });
                        });

                        // Footer
                        page.Footer().Column(c =>
                        {
                            c.Item().AlignCenter().Text($"Bu Fatura {invoice.InvoiceCompanyName} tarafından hazırlanmıştır.").FontSize(8).FontColor(Colors.Grey.Darken1);
                            c.Item().PaddingTop(5).AlignRight().Text(x =>
                            {
                                x.Span("Sayfa ").FontSize(8);
                                x.CurrentPageNumber().FontSize(8);
                                x.Span(" / ").FontSize(8);
                                x.TotalPages().FontSize(8);
                            });
                        });
                    });
                });
            });

            using var ms = new MemoryStream();
            try
            {
                document.GeneratePdf(ms);
            }
            catch (Exception ex)
            {
                throw new Exception("pdf oluşturulamadı: " + ex.Message);
            }

            //using var ms = new MemoryStream();
            //document.GeneratePdf(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        private static string NumberToWords(decimal number)
        {
            var intPart = (int)Math.Floor(number);
            var decPart = (int)Math.Round((number - intPart) * 100);

            return $"{intPart:N0}"; // Basitleştirilmiş
        }

    }
}
