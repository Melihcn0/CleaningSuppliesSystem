using CleaningSuppliesSystem.DTO.DTOs.InvoiceItemDtos;

namespace CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceType { get; set; }

        // 📌 Bireysel müşteri bilgileri
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerNationalId { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerEmail { get; set; }

        // 📌 Kurumsal müşteri bilgileri
        public string? CustomerCompanyName { get; set; }
        public string? CustomerTaxOffice { get; set; }
        public string? CustomerTaxNumber { get; set; }

        // 📌 Ortak adres bilgileri
        public string? CustomerAddressTitle { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerCityName { get; set; }
        public string? CustomerDistrictName { get; set; }

        // 📌 Yetkili (Admin) snapshot bilgileri
        public string? AdminFirstName { get; set; }
        public string? AdminLastName { get; set; }
        public string? AdminPhoneNumber { get; set; }

        // 📌 Şirket snapshot bilgileri
        public string? InvoiceCompanyName { get; set; }
        public string? InvoiceCompanyTaxOffice { get; set; }
        public string? InvoiceCompanyTaxNumber { get; set; }
        public string? InvoiceCompanyAddress { get; set; }
        public string? InvoiceCompanyCityName { get; set; }
        public string? InvoiceCompanyDistrictName { get; set; }

        // 📌 Fatura satırları
        public ICollection<InvoiceItemDto>? InvoiceItems { get; set; }
    }
}
