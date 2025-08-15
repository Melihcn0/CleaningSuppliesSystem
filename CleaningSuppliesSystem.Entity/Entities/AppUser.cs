using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string? NationalId { get; set; } // Ad
        public string FirstName { get; set; } // Ad
        public string LastName { get; set; } // Soyad
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Kayıt tarihi
        public bool IsActive { get; set; } = false; // Kullanıcı aktif mi
        public bool WantsNewsletter { get; set; } = false; // Bülten aboneliği
        public DateTime? LastLogoutAt { get; set; }
        public ICollection<Order> Orders { get; set; }   // Kullanıcının siparişleri
        public ICollection<CustomerAddress> CustomerAddresses { get; set; } // Teslimat adresleri (birden fazla olabilir)
        public string PreferredTheme { get; set; } = "light"; // "light" veya "dark"
    }
}
