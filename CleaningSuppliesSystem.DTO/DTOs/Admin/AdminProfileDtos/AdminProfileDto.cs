using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;

namespace CleaningSuppliesSystem.DTO.DTOs.Admin.AdminProfileDtos
{
    public class AdminProfileDto
    {
        public int Id { get; set; }                     // Profil Id
        public string? NationalId { get; set; }         // Kimlik Numarası
        public string FirstName { get; set; }           // Ad Soyad
        public string LastName { get; set; }            // Ad Soyad
        public string UserName { get; set; }            // Ad Soyad
        public string Email { get; set; }               // E-posta
        public string? PhoneNumber { get; set; }        // Telefon
        public DateTime LastLogoutAt { get; set; }      // Son çıkış zamanı
        public CompanyAddressDto? CompanyAddress { get; set; }
        public CompanyBankDto? CompanyBank { get; set; }
    }
}
