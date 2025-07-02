using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Category
    {
        public int Id { get; set; } // Kategori kimliği
        public string Name { get; set; } // Kategori adı (ör: "Temizlik")
        public ICollection<Product> Products { get; set; } // Bu kategoriye bağlı ürünler
    }
}
