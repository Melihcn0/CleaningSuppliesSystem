using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<(bool IsSuccess, string Message)> TCreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<(bool IsSuccess, string Message)> TUpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task<(bool IsSuccess, string Message)> TSoftDeleteCategoryAsync(int id);
        Task<(bool IsSuccess, string Message)> TUndoSoftDeleteCategoryAsync(int id);
    }
}
