using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICustomerIndividualAddressService
    {
        Task<List<CustomerIndividualAddressDto>> TGetAllAddressesAsync(int userId);
        Task<CustomerIndividualAddressDto> TGetAddressByIdAsync(int id);
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerIndividualAddressAsync(CreateCustomerIndividualAddressDto createCustomerIndividualAddressDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerIndividualAddressAsync(UpdateCustomerIndividualAddressDto updateCustomerIndividualAddressDto);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteCustomerIndividualAsync(int id);
        Task<bool> TToggleCustomerAddressStatusAsync(int addressId, bool newStatus);
        Task<List<ResultLocationDistrictDto>> TGetActiveByCityIdAsync(int cityId);
    }
}
