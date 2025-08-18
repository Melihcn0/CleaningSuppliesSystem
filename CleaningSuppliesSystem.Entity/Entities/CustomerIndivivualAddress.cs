using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class CustomerIndivivualAddress
    {
        public int Id { get; set; }                   // PK
        public int AppUserId { get; set; }            // AppUser ilişkisi
        public AppUser AppUser { get; set; }         // Navigation property
        public string? Address { get; set; }        // Adres
        public string? AddressTitle { get; set; }   // Adres başlığı
        public string? City { get; set; }             // Şehir
        public string? District { get; set; }         // İlçe
        public bool IsDefault { get; set; } = false;  // Varsayılan adres mi
    }
}
