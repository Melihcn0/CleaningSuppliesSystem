using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal TotalAmount { get; set; }
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
        public string? AddressTitle { get; set; }
        public string? Address { get; set; }
        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
    }
}
