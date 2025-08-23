using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // Fatura tipi (Individual / Corporate)
        public string InvoiceType { get; set; }

        // Bireysel için alanlar
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        // Kurumsal için alanlar
        public string? CompanyName { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }

        // Ortak adres bilgileri
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
    }


}
