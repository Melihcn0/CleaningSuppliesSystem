using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Payment
    {
        public int Id { get; set; } // Ödeme kimliği
        public int OrderId { get; set; } // Hangi sipariş için
        public Order Order { get; set; }
        public decimal Amount { get; set; } // Toplam ödenen tutar
        public DateTime PaymentDate { get; set; } // Ödeme zamanı
    }
}
