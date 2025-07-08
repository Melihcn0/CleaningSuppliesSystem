using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IFinanceService : IGenericService<Finance>
    {
        Task<(bool IsSuccess, string Message)> TCreateFinanceAsync(CreateFinanceDto createFinanceDto);
        Task<(bool IsSuccess, string Message)> TUpdateFinanceAsync(UpdateFinanceDto updateFinanceDto);
        Task<(bool IsSuccess, string Message)> TSoftDeleteFinanceAsync(int id);
        Task<(bool IsSuccess, string Message)> TUndoSoftDeleteFinanceAsync(int id);
    }
}
