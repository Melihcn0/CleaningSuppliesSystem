using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Models
{
    public class CreateCustomerAddressViewModel
    {
        public CreateCustomerIndividualAddressDto IndividualAddress { get; set; }
        public CreateCustomerCorporateAddressDto CorporateAddress { get; set; }
    }

}
