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
        public string ImageUrl { get; set; } // Ürün Fotoğrafı
        public string Name { get; set; } // Ürün adı
        public decimal UnitPrice { get; set; } // Satış fiyatı
        public decimal DiscountRate { get; set; }  // indirim
        public DateTime CreatedAt { get; set; } // tarih
        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; } // Bağlı olduğu kategori kimliği
        public Category Category { get; set; } // Navigation property
        public ICollection<StockEntry> StockEntries { get; set; } // Ürünün stok girişleri
    }
}
