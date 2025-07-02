using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Product
    {
        public int Id { get; set; } // Ürün kimliği
        public string Name { get; set; } // Ürün adı
        public decimal UnitPrice { get; set; } // Satış fiyatı
        public int CategoryId { get; set; } // Bağlı olduğu kategori kimliği
        public Category Category { get; set; } // Navigation property
        public ICollection<StockEntry> StockEntries { get; set; } // Ürünün stok girişleri
        public ICollection<OrderItem> OrderItems { get; set; } // Siparişlerde ürün satırları
    }
}
