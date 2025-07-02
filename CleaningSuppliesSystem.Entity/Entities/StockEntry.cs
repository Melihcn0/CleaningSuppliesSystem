using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class StockEntry
    {
        public int Id { get; set; } // Stok kayıt kimliği
        public int ProductId { get; set; } // Hangi ürünün stoğu
        public Product Product { get; set; } // Navigation
        public int Quantity { get; set; } // Girilen miktar (adet)
        public DateTime EntryDate { get; set; } // Giriş tarihi
        public string? Description { get; set; } // Açıklama (opsiyonel)
    }
}
