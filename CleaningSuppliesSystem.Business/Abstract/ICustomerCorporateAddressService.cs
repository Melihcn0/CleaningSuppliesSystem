using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICustomerCorporateAddressService
    {
        Task<List<CustomerCorporateAddressDto>> TGetAllAddressesAsync(int userId);
        Task<CustomerCorporateAddressDto> TGetAddressByIdAsync(int id);
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerCorporateAddressAsync(CreateCustomerCorporateAddressDto createCustomerCorporateAddressDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerCorporateAddressAsync(UpdateCustomerCorporateAddressDto updateCustomerCorporateAddressDto);
        Task<bool> TToggleCustomerCorporateAddressStatusAsync(int addressId, bool newStatus);
    }
}
