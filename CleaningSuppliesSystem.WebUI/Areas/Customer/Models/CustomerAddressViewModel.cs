using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Models
{
    public class CustomerAddressViewModel
    {
        public List<CustomerAddressDto> CustomerAddresses { get; set; } = new List<CustomerAddressDto>();
        public CreateCustomerAddressDto NewAddress { get; set; } = new CreateCustomerAddressDto();
        public CustomerAddressDto UpdateAddress { get; set; } = new CustomerAddressDto();
    }
}
