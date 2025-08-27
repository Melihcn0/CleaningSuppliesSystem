using System;
using System.Collections.Generic;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string InvoiceType { get; set; }

        // Müşteri bilgileri
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerNationalId { get; set; }
        public string? CustomerPhoneNumber { get; set; }

        public string? CustomerCompanyName { get; set; }
        public string? CustomerTaxOffice { get; set; }
        public string? CustomerTaxNumber { get; set; }

        public string CustomerAddressTitle { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCityName { get; set; }
        public string CustomerDistrictName { get; set; }

        // Admin bilgileri
        public int AdminId { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminLastName { get; set; }
        public string AdminPhoneNumber { get; set; }

        // Şirket bilgileri
        public string InvoiceCompanyName { get; set; }
        public string InvoiceCompanyTaxOffice { get; set; }
        public string InvoiceCompanyTaxNumber { get; set; }
        public string InvoiceCompanyAddress { get; set; }
        public string InvoiceCompanyCityName { get; set; }
        public string InvoiceCompanyDistrictName { get; set; }

        // Fatura satırları
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
