using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
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
    public interface ISubCategoryService : IGenericService<SubCategory>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateSubCategoryAsync(CreateSubCategoryDto dto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateSubCategoryAsync(UpdateSubCategoryDto dto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteSubCategoryAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteSubCategoryAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteSubCategoryAsync(int id);
        Task<List<ResultSubCategoryDto>> TGetActiveByTopCategoryIdAsync(int topCategoryId);
        Task<List<ResultSubCategoryDto>> TGetActiveSubCategoriesAsync();
        Task<List<ResultSubCategoryDto>> TGetDeletedSubCategoriesAsync();
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeSubCategoryAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeSubCategoryAsync(List<int> ids);
        Task TPermanentDeleteRangeSubCategoryAsync(List<int> ids);
    }
}
