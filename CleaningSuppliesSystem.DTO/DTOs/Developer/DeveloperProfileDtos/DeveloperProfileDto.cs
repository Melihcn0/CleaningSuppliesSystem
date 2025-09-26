
namespace CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos
{
    public class DeveloperProfileDto
    {
        public int Id { get; set; }                     // Profil Id
        public string? NationalId { get; set; }         // Kimlik Numarası
        public string FirstName { get; set; }           // Ad Soyad
        public string LastName { get; set; }            // Ad Soyad
        public string UserName { get; set; }            // Ad Soyad
        public string Email { get; set; }               // E-posta
        public string? PhoneNumber { get; set; }        // Telefon
        public DateTime LastLogoutAt { get; set; }      // Son çıkış zamanı
    }
}
