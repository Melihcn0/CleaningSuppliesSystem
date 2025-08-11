using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ITopCategoryService : IGenericService<TopCategory>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateTopCategoryAsync(CreateTopCategoryDto dto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateTopCategoryAsync(UpdateTopCategoryDto dto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteTopCategoryAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteTopCategoryAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteTopCategoryAsync(int id);
        Task<List<ResultTopCategoryDto>> TGetActiveTopCategoriesAsync();
        Task<List<ResultTopCategoryDto>> TGetDeletedTopCategoriesAsync();
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeTopCategoryAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeTopCategoryAsync(List<int> ids);
        Task TPermanentDeleteRangeTopCategoryAsync(List<int> ids);
    }
}
