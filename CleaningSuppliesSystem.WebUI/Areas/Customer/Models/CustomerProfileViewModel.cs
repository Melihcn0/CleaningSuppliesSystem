using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using System.Collections.Generic;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Models
{
    public class CustomerProfileViewModel
    {
        // Profil bilgileri
        public CustomerProfileDto CustomerProfile { get; set; }
        public UpdateCustomerProfileDto UpdateCustomerProfile { get; set; }

        // Bireysel adresler
        public List<CustomerIndivivualAddressDto> CustomerIndividualAddresses { get; set; } = new List<CustomerIndivivualAddressDto>();
        public CreateCustomerIndivivualAddressDto CreateIndividualAddress { get; set; }
        public UpdateCustomerIndivivualAddressDto UpdateIndividualAddress { get; set; } = new UpdateCustomerIndivivualAddressDto();

        // Kurumsal adresler
        public List<CustomerCorporateAddressDto> CustomerCorporateAddresses { get; set; } = new List<CustomerCorporateAddressDto>();
        public CreateCustomerCorporateAddressDto CreateCorporateAddress { get; set; }
        public UpdateCustomerCorporateAddressDto UpdateCorporateAddress { get; set; } = new UpdateCustomerCorporateAddressDto();
    }
}
