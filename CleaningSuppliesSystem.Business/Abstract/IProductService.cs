using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
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
        Task<List<Product>> TGetProductsWithCategoriesAsync();
        Task<Product> TGetByIdAsyncWithCategory(int id);
        Task<(bool IsSuccess, List<string> Errors)> TApplyDiscountAsync(UpdateProductDto dto);
        Task<UpdateProductDto> TGetProductForDiscountAsync(int id);
        Task<(bool IsSuccess, string Message)> TCreateProductAsync(CreateProductDto createProductDto);
        Task<(bool IsSuccess, string Message)> TUpdateProductAsync(UpdateProductDto updateProductDto);
        Task<(bool IsSuccess, string Message)> TSoftDeleteProductAsync(int id);
        Task<(bool IsSuccess, string Message)> TUndoSoftDeleteProductAsync(int id);

    }
}
