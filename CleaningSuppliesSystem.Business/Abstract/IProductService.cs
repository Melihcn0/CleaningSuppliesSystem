using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IProductService : IGenericService<Product>
    {
        Task<(bool IsSuccess, List<string> Errors)> TApplyDiscountAsync(UpdateDiscountDto dto);
        Task<(bool IsSuccess, string Message,int CreatedId)> TCreateProductAsync(CreateProductDto createProductDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateProductAsync(UpdateProductDto updateProductDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteProductAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteProductAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteProductAsync(int id);
        Task<(bool IsSuccess, string Message)> TSetProductDisplayOnHomeAsync(int productId, bool isShown);
        Task<List<ResultProductDto>> TGetActiveProductsAsync();
        Task<List<ResultProductDto>> TGetActiveByBrandIdAsync(int brandId);
        Task<List<ResultProductDto>> TGetDeletedProductsAsync();
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeProductAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeProductAsync(List<int> ids);
        Task TPermanentDeleteRangeProductAsync(List<int> ids);
        Task TDecreaseStockAsync(IEnumerable<OrderItem> orderItems);


    }
}
