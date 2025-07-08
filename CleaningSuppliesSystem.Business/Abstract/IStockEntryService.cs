using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IStockEntryService : IGenericService<StockEntry>
    {
        Task<List<StockEntry>> TGetStockEntryWithProductsandCategoriesAsync();
        Task<StockEntry> TGetByIdAsyncWithProductsandCategories(int id);
        Task<(bool IsSuccess, string Message)> TCreateStockEntryAsync(CreateStockEntryDto createStockEntryDto);
        Task<(bool IsSuccess, string Message)> TUpdateStockEntryAsync(UpdateStockEntryDto updateStockEntryDto);
        Task<(bool IsSuccess, string Message)> TSoftDeleteStockEntryAsync(int id);
        Task<(bool IsSuccess, string Message)> TUndoSoftDeleteStockEntryAsync(int id);
    }
}
