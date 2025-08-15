using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Invoice
    {
        public int Id { get; set; }                        // PK
        public int OrderId { get; set; }                   // Sipariş ilişkisi
        public Order Order { get; set; }                   // Navigation property
        public DateTime GeneratedAt { get; set; } = DateTime.Now; // Fatura tarihi
        public decimal TotalAmount { get; set; }           // Fatura toplam tutarı

        // --- Address Snapshot ---
        public string City { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string PostalCode { get; set; }
        public string AddressTitle { get; set; }
        public string RecipientName { get; set; }
    }

}
