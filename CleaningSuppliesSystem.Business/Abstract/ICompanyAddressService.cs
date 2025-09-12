using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICompanyAddressService
    {
        Task<CompanyAddressDto> TGetCompanyAddressAsync();
        Task<UpdateCompanyAddressDto> TGetUpdateCompanyAddressAsync();
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCompanyAddressAsync(UpdateCompanyAddressDto updateCompanyAddressDto);
        
    }
}
