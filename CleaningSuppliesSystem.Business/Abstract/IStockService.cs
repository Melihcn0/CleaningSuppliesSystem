using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IStockService
    {
        Task<(bool IsSuccess, string Message)> AssignStockAsync(CreateStockOperationDto dto);
        Task<List<ResultStockOperationDto>> TGetActiveProductsAsync();
    }
}
