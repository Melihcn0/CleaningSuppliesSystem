using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos
{
    public class CustomerProfileDto
    {
        public int Id { get; set; }                     // Profil Id
        public string? NationalId { get; set; }         // Kimlik Numarası
        public string FirstName { get; set; }           // Ad Soyad
        public string LastName { get; set; }            // Ad Soyad
        public string UserName { get; set; }            // Ad Soyad
        public string Email { get; set; }               // E-posta
        public string? PhoneNumber { get; set; }        // Telefon
        public DateTime LastLogoutAt { get; set; }      // Son çıkış zamanı
        public string? Address { get; set; }            // Sokak
        public string? AddressTitle { get; set; }       // Adres başlığı

    }
}
