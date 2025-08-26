using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICustomerProfileService
    {
        Task<CustomerProfileDto> TGetProfileAsync();
        Task<UpdateCustomerProfileDto> TGetUpdateCustomerProfileAsync();
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerProfileAsync(UpdateCustomerProfileDto updateCustomerProfileDto);

    }
}
