using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Order
    {
        public int Id { get; set; } // Sipariş kimliği
        public DateTime CreatedDate { get; set; } // Oluşturulma zamanı
        public string Status { get; set; } // Sipariş durumu ("Pending", "Shipped" vs.)
        public int AppUserId { get; set; } // Siparişi veren kullanıcı
        public AppUser AppUser { get; set; } // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } // Sipariş detay satırları
        public Payment Payment { get; set; } // Ödeme ilişkisi (1:1)
        public Invoice Invoice { get; set; } // Fatura ilişkisi (1:1)
    }
}
