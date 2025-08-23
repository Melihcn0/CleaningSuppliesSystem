using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.UserProfileDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CustomerProfileViewModel
{
    public CustomerProfileDto CustomerProfile { get; set; }
    public UpdateCustomerProfileDto UpdateCustomerProfile { get; set; }

    public List<CustomerIndividualAddressDto> CustomerIndividualAddresses { get; set; } = new();
    public List<CustomerCorporateAddressDto> CustomerCorporateAddresses { get; set; } = new();

    public CreateCustomerIndividualAddressDto CreateIndividualAddress { get; set; }
    public UpdateCustomerIndividualAddressDto UpdateIndividualAddress { get; set; }
    public CreateCustomerCorporateAddressDto CreateCorporateAddress { get; set; }
    public UpdateCustomerCorporateAddressDto UpdateCorporateAddress { get; set; }

    public CustomerIndividualAddressDto DefaultIndividualAddress =>
        CustomerIndividualAddresses.FirstOrDefault(a => a.IsDefault);

    public CustomerCorporateAddressDto DefaultCorporateAddress =>
        CustomerCorporateAddresses.FirstOrDefault(a => a.IsDefault);

    public object? DefaultAddress
    {
        get
        {
            // Eğer kurumsal adres varsa onu öncelikli kullan
            if (DefaultCorporateAddress != null) return DefaultCorporateAddress;
            if (DefaultIndividualAddress != null) return DefaultIndividualAddress;
            return null;
        }
    }

    public List<SelectListItem> Cities { get; set; } = new();



}
