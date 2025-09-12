using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICompanyBankService
    {
        Task<CompanyBankDto> TGetCompanyBankAsync();
        Task<UpdateCompanyBankDto> TGetUpdateCompanyBankAsync();
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCompanyBankAsync(UpdateCompanyBankDto updateCompanyBankDto);
        Task<CompanyBankDto?> TGetFirstCompanyBankAsync();
    }
}
