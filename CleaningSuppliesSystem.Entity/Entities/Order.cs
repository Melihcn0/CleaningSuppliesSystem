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
        public string OrderNumber { get; set; } // Sipariş No
        public DateTime CreatedDate { get; set; } // Oluşturulma zamanı
        public DateTime? UpdatedDate { get; set; } // Güncelleme zamanı
        public string Status { get; set; } // Sipariş durumu
        public DateTime? ApprovedDate { get; set; }       // Onaylandı
        public DateTime? PreparingDate { get; set; }       // Hazırlanıyor
        public DateTime? ShippedDate { get; set; }         // Kargoya Verildi
        public DateTime? DeliveredDate { get; set; }       // Teslim Edildi
        public DateTime? CanceledDate { get; set; }        // İptal Edildi
        public int AppUserId { get; set; } // Siparişi veren kullanıcı
        public AppUser AppUser { get; set; } // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } // Sipariş detay satırları
        public Invoice Invoice { get; set; } // Fatura ilişkisi (1:1)
    }
}
