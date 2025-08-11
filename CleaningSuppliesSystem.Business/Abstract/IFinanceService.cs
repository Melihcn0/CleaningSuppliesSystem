using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
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
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateFinanceAsync(CreateFinanceDto createFinanceDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateFinanceAsync(UpdateFinanceDto updateFinanceDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteFinanceAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteFinanceAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteFinanceAsync(int id);
        Task<List<ResultFinanceDto>> TGetActiveFinancesAsync();
        Task<List<ResultFinanceDto>> TGetDeletedFinancesAsync();
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeFinanceAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeFinanceAsync(List<int> ids);
        Task TPermanentDeleteRangeFinanceAsync(List<int> ids);
    }
}
