using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Models
{
    public class CustomerAddressViewModel
    {
        public List<CustomerAddressDto> CustomerAddresses { get; set; } = new List<CustomerAddressDto>();
        public CustomerAddressDto NewAddress { get; set; } = new CustomerAddressDto();
        public CustomerAddressDto UpdateAddress { get; set; } = new CustomerAddressDto();
    }
}
