using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } // Satır kimliği
        public int OrderId { get; set; } // Hangi siparişe ait
        public Order Order { get; set; }
        public int ProductId { get; set; } // Hangi ürün
        public Product Product { get; set; }
        public int Quantity { get; set; } // Alınan adet
        public decimal UnitPrice { get; set; } // Sipariş esnasındaki fiyat
    }
}
