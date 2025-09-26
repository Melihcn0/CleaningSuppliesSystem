using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Stock")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignStock([FromBody] CreateStockOperationDto dto)
        {
            var (isSuccess, message) = await _stockService.TAssignStockAsync(dto);
            return isSuccess ? Ok(message) : BadRequest(message);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveStocks(int page = 1, int pageSize = 10)
        {
            var stocks = await _stockService.TGetActiveProductsAsync();
            var totalCount = stocks.Count;

            var pagedData = stocks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultStockOperationDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("active-all")]
        public async Task<IActionResult> GetActiveStock()
        {
            var stocks = await _stockService.TGetActiveProductsAsync();
            return Ok(stocks);
        }

        [HttpPost("quickAssign")]
        public async Task<IActionResult> QuickStockOperation([FromBody] QuickStockOperationDto dto)
        {
            var (isSuccess, message) = await _stockService.TQuickStockOperationAsync(dto);
            return isSuccess ? Ok(message) : BadRequest(message);
        }

    }
}
