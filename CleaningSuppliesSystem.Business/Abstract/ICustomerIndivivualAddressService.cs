using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICustomerIndivivualAddressService
    {
        Task<List<CustomerIndivivualAddressDto>> TGetAllAddressesAsync(int userId);
        Task<CustomerIndivivualAddressDto> TGetAddressByIdAsync(int id);
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerIndivivualAddressAsync(CreateCustomerIndivivualAddressDto createCustomerIndivivualAddressDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerIndivivualAddressAsync(UpdateCustomerIndivivualAddressDto updateCustomerIndividualAddressDto);
        Task<bool> TToggleCustomerIndivivualAddressStatusAsync(int addressId, bool newStatus);
    }
}
