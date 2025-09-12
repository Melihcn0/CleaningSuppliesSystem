using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos
{
    public class UpdateCompanyAddressDto
    {
        public int Id { get; set; }                  // Kullanıcı ID
        public string CompanyName { get; set; }      // Şirket Adı
        public string TaxOffice { get; set; }        // Vergi Dairesi
        public string TaxNumber { get; set; }        // Vergi Numarası
        public string Address { get; set; }          // Adres Detayı
        public string CityName { get; set; }         // Şehir
        public string DistrictName { get; set; }     // İlçe
    }
}
