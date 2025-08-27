using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.InvoiceItemDtos
{
    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }           // Invoice ile ilişki

        public string ProductName { get; set; }      // O anki ürün adı
        public decimal Quantity { get; set; }        // Miktar
        public string Unit { get; set; }             // Birim (adet, kg, vs.)

        public decimal UnitPrice { get; set; }       // KDV hariç
        public decimal VatRate { get; set; }         // KDV %
        public decimal VatAmount { get; set; }       // KDV tutarı
        public decimal Total { get; set; }           // KDV dahil toplam
    }
}
