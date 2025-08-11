using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
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
    public interface IBrandService : IGenericService<Brand>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateBrandAsync(CreateBrandDto createBrandDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateBrandAsync(UpdateBrandDto updateBrandDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteBrandAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteBrandAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteBrandAsync(int id);
        Task<List<ResultBrandDto>> TGetActiveBrandsAsync();
        Task<List<ResultBrandDto>> TGetDeletedBrandsAsync();
        Task<List<ResultBrandDto>> TGetActiveByCategoryIdAsync(int categoryId);
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeBrandAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeBrandAsync(List<int> ids);
        Task TPermanentDeleteRangeBrandAsync(List<int> ids);
    }
}
