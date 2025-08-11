using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;

public class PdfManager : IPdfService
{
    public byte[] CreateInvoicePdf(Order order)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Üst Bilgi
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Invoice #{order.Id}").FontSize(20).Bold();
                        col.Item().Text($"Date Issued: {order.CreatedDate:dd/MM/yyyy}");
                        col.Item().Text($"Due Date: {order.CreatedDate.AddDays(7):dd/MM/yyyy}");
                    });

                    row.ConstantItem(100).Image("wwwroot/images/logo.png"); // Logo yolu
                });

                // İçerik
                page.Content().Column(column =>
                {
                    // Müşteri ve Sipariş Bilgileri
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Issued For:").Bold();
                            col.Item().Text($"{order.AppUser.FirstName} {order.AppUser.LastName}");
                            col.Item().Text("Adres bilgisi yok");
                            col.Item().Text(order.AppUser.PhoneNumber ?? "");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Order Info:").Bold();
                            col.Item().Text($"Order ID: #{order.Id}");
                            col.Item().Text($"Shipment ID: #{order.Id + 1000}");
                        });
                    });

                    column.Item().PaddingVertical(10);

                    // Ürün Tablosu
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(25); // SL.
                            columns.RelativeColumn();   // Item
                            columns.ConstantColumn(35); // Qty
                            columns.ConstantColumn(35); // Unit
                            columns.ConstantColumn(50); // Unit Price
                            columns.ConstantColumn(50); // Price
                        });

                        // Tablo başlık
                        table.Header(header =>
                        {
                            header.Cell().Text("SL.").Bold();
                            header.Cell().Text("Items").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Unit").Bold();
                            header.Cell().Text("Unit Price").Bold();
                            header.Cell().AlignRight().Text("Price").Bold();
                        });

                        // Tablo içerik
                        int sl = 1;
                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().Text(sl++.ToString());
                            table.Cell().Text(item.Product.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text("PC");
                            table.Cell().Text($"${item.UnitPrice:F2}");
                            table.Cell().AlignRight().Text($"${(item.UnitPrice * item.Quantity):F2}");
                        }
                    });

                    // Alt Bilgi: Toplamlar
                    decimal subtotal = order.OrderItems.Sum(x => x.UnitPrice * x.Quantity);
                    decimal discount = 0;
                    decimal tax = 0;
                    decimal total = subtotal - discount + tax;

                    column.Item().AlignRight().Column(totals =>
                    {
                        totals.Item().Text($"Subtotal: ${subtotal:F2}").Bold();
                        totals.Item().Text($"Discount: ${discount:F2}").Bold();
                        totals.Item().Text($"Tax: ${tax:F2}").Bold();
                        totals.Item().Text($"Total: ${total:F2}").FontSize(13).Bold().FontColor(Colors.Blue.Medium);
                    });

                    column.Item().PaddingTop(20).AlignCenter().Text("Thank you for your purchase!").Italic();

                    // İmza alanları
                    column.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("_________________________\nSignature of Customer").FontSize(10);
                        row.RelativeItem().AlignRight().Text("_________________________\nSignature of Authorized").FontSize(10);
                    });
                });

                // Alt Bilgi
                page.Footer().AlignCenter().Text("Cleaning Supplies System © 2025").FontSize(9);
            });
        }).GeneratePdf();

        return pdfBytes;
    }
}
