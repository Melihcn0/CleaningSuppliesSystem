using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddresDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class AdminProfileViewModel
    {
        public AdminProfileDto AdminProfile { get; set; }
        public UpdateAdminProfileDto UpdateAdminProfile { get; set; }
        public CompanyAddressDto CompanyAddress { get; set; }
        public UpdateCompanyAddressDto UpdateCompanyAddress { get; set; }
    }
}
