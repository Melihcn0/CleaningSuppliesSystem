using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Models
{
    public class CustomerProfileViewModel
    {
        public CustomerProfileDto CustomerProfile { get; set; }
        public UpdateCustomerProfileDto UpdateCustomerProfile { get; set; }
        public List<CustomerAddressDto> CustomerAddresses { get; set; } = new List<CustomerAddressDto>();
        public UpdateCustomerAddressDto NewAddress { get; set; } = new UpdateCustomerAddressDto();
        public UpdateCustomerAddressDto UpdateAddress { get; set; } = new UpdateCustomerAddressDto();
    }
}
