using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos
{
    public class CreateCustomerCorporateAddressDto
    {
        public int AppUserId { get; set; }            // AppUser ilişkisi
        public string CompanyName { get; set; }      // Şirket Adı
        public string TaxOffice { get; set; }        // Vergi Dairesi
        public string TaxNumber { get; set; }        // Vergi Numarası (KDV / KVKK için)  
        public string AddressTitle { get; set; }     // Adres Başlığı
        public string Address { get; set; }          // Adres Detayı
        public string City { get; set; }             // Şehir
        public string District { get; set; }         // İlçe
    }
}
