using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Invoice
    {
        public int Id { get; set; } // Fatura kimliği
        public DateTime GeneratedAt { get; set; } // Fatura oluşturulma zamanı
        public string FileName { get; set; } // PDF dosya ismi
        public byte[] FileContent { get; set; } // PDF içeriği
        public int OrderId { get; set; } // Hangi siparişin faturası
        public Order Order { get; set; } // Sipariş nesnesi
    }
}
