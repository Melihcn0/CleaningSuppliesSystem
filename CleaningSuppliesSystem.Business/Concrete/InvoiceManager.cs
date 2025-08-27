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
            var adminUser = await _adminProfileRepository.GetAdminWithCompanyAddressAsync(adminId);
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
                // KDV dahil fiyat
                var unitPriceInclVat = item.Product?.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product?.UnitPrice ?? 0;
                var vatRate = item.Product?.VatRate ?? 0;

                // KDV tutarı
                var vatAmountPerUnit = unitPriceInclVat - (unitPriceInclVat / (1 + vatRate / 100));
                var totalVat = vatAmountPerUnit * item.Quantity;

                // Toplam tutar (KDV dahil)
                var totalInclVat = unitPriceInclVat * item.Quantity;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductName = item.Product?.Name ?? "",
                    Quantity = item.Quantity,
                    Unit = "adet",
                    UnitPrice = unitPriceInclVat,
                    VatRate = vatRate,
                    VatAmount = totalVat,
                    Total = totalInclVat
                });

                totalAmount += totalInclVat;
            }


            invoice.TotalAmount = totalAmount;

            await _invoiceRepository.CreateAsync(invoice);
            return invoice;
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
                                columns.ConstantColumn(30);   // Sıra No
                                columns.RelativeColumn(130);  // Ürün / Hizmet Adı
                                columns.ConstantColumn(30);   // Miktar
                                columns.ConstantColumn(30);   // Birim
                                columns.ConstantColumn(60);   // Birim Fiyat
                                columns.ConstantColumn(30);   // KDV %
                                columns.ConstantColumn(60);   // KDV Tutarı
                                columns.ConstantColumn(60);   // Toplam
                            });

                            var headerStyle = TextStyle.Default.SemiBold().FontSize(8);
                            var cellStyle = TextStyle.Default.FontSize(8);
                            var borderColor = Colors.Black;

                            // Table Header
                            table.Header(header =>
                            {
                                string[] headers = { "S.No", "Ürün", "Miktar", "Birim", "Birim Fiyat", "KDV %", "KDV Tutarı", "Toplam" };
                                foreach (var h in headers)
                                    header.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(h).AlignCenter().Style(headerStyle);
                            });

                            int count = 1;
                            foreach (var item in items)
                            {
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(count.ToString()).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(Safe(item.ProductName)).AlignLeft().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Quantity.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Unit).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.UnitPrice.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text("%" + item.VatRate.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.VatAmount.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Total.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                totalExclVat += item.UnitPrice * item.Quantity - item.VatAmount; // KDV hariç
                                totalVat += item.VatAmount;
                                totalInclVat += item.Total; // KDV dahil
                                count++;
                            }
                        });

                        // === TOTALS SECTION ===
                        col.Item().PaddingTop(10).Column(totalsCol =>
                        {
                            totalsCol.Item().Row(row =>
                            {
                                row.RelativeColumn(2).Text(""); // Boşluk
                                row.RelativeColumn().Column(colInner =>
                                {
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Hariç Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalExclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Genel Toplam
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Dahil Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ürün/Hizmet Toplam Tutarı
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("Ürün Toplam Tutarı:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    colInner.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("ÖDENECEK TUTAR:").FontSize(10).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(10).SemiBold();
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
                                columns.ConstantColumn(30);   // Sıra No
                                columns.RelativeColumn(130);  // Ürün / Hizmet Adı
                                columns.ConstantColumn(30);   // Miktar
                                columns.ConstantColumn(30);   // Birim
                                columns.ConstantColumn(60);   // Birim Fiyat
                                columns.ConstantColumn(30);   // KDV %
                                columns.ConstantColumn(60);   // KDV Tutarı
                                columns.ConstantColumn(60);   // Toplam
                            });

                            var headerStyle = TextStyle.Default.SemiBold().FontSize(8);
                            var cellStyle = TextStyle.Default.FontSize(8);
                            var borderColor = Colors.Black;

                            // Table Header
                            table.Header(header =>
                            {
                                string[] headers = { "S.No", "Ürün", "Miktar", "Birim", "Birim Fiyat", "KDV %", "KDV Tutarı", "Toplam" };
                                foreach (var h in headers)
                                    header.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(h).AlignCenter().Style(headerStyle);
                            });

                            int count = 1;
                            foreach (var item in items)
                            {
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(count.ToString()).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(Safe(item.ProductName)).AlignLeft().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Quantity.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Unit).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.UnitPrice.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text("%" + item.VatRate.ToString("N0")).AlignCenter().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.VatAmount.ToString("N2") + " TL").AlignRight().Style(cellStyle);
                                table.Cell().Border(1).BorderColor(borderColor).Padding(2).Text(item.Total.ToString("N2") + " TL").AlignRight().Style(cellStyle);

                                totalExclVat += item.UnitPrice * item.Quantity - item.VatAmount; // KDV hariç
                                totalVat += item.VatAmount;
                                totalInclVat += item.Total; // KDV dahil
                                count++;
                            }
                        });

                        // === TOTALS SECTION ===
                        col.Item().PaddingTop(10).Column(totalsCol =>
                        {
                            totalsCol.Item().Row(row =>
                            {
                                row.RelativeColumn(2).Text(""); // Boşluk
                                row.RelativeColumn().Column(colInner =>
                                {
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Hariç Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalExclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Genel Toplam
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("K.D.V Dahil Toplam Tutar:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    // Ürün/Hizmet Toplam Tutarı
                                    colInner.Item().PaddingBottom(3).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("Ürün Toplam Tutarı:").FontSize(9).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(9).SemiBold();
                                    });

                                    colInner.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Row(r =>
                                    {
                                        r.RelativeColumn(150).Text("ÖDENECEK TUTAR:").FontSize(10).SemiBold();
                                        r.ConstantColumn(60).AlignRight().Text(totalInclVat.ToString("N2") + " TL").FontSize(10).SemiBold();
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
