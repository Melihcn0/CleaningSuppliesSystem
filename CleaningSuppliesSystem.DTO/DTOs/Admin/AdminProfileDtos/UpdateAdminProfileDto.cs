using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos
{
    public class UpdateAdminProfileDto
    {
        public int Id { get; set; }                  // Kullanıcı ID
        public string? NationalId { get; set; }           // Kimlik Numarası
        public string FirstName { get; set; }              // Ad Soyad
        public string LastName { get; set; }              // Ad Soyad
        public string UserName { get; set; }              // Kullanıcı Adı
        public string Email { get; set; }                 // E-posta
        public string? PhoneNumber { get; set; }           // Telefon
    }
}
