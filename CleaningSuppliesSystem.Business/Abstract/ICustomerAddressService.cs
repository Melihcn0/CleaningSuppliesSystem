using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICustomerAddressService
    {
        Task<List<CustomerAddressDto?>> TGetAllAddressesAsync(int userId);
        Task<CustomerAddressDto> TGetAddressByIdAsync(int id);
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerAddressAsync(CreateCustomerAddressDto createCustomerAddressDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerAddressAsync(UpdateCustomerAddressDto updateCustomerAddressDto);
    }
}
