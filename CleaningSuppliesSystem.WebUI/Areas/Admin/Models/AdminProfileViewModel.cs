using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class AdminProfileViewModel
    {
        public AdminProfileDto AdminProfile { get; set; }
        public UpdateAdminProfileDto UpdateAdminProfile { get; set; }
        public CompanyAddressDto CompanyAddress { get; set; }
        public UpdateCompanyAddressDto UpdateCompanyAddress { get; set; }
        public CompanyBankDto CompanyBank { get; set; }
        public UpdateCompanyBankDto UpdateCompanyBank { get; set; }
    }
}
