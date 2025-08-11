using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteCategoryAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteCategoryAsync(int id);
        Task<(bool isSuccess, string message)> TPermanentDeleteCategoryAsync(int id);
        Task<(bool IsSuccess, string Message)> TSetCategoryDisplayOnHomeAsync(int categoryId, bool isShown);
        Task<List<ResultCategoryDto>> TGetActiveCategoriesAsync();
        Task<List<ResultCategoryDto>> TGetDeletedCategoriesAsync();
        Task<List<ResultCategoryDto>> TGetActiveBySubCategoryIdAsync(int subCategoryId);
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeCategoryAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeCategoryAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TPermanentDeleteRangeCategoryAsync(List<int> ids);
        Task<List<Category>> TGetByIdsAsync(List<int> ids);


    }
}
