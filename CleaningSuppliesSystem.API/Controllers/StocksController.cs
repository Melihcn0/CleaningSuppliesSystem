using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
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
            var (isSuccess, message) = await _stockService.AssignStockAsync(dto);
            return isSuccess ? Ok(message) : BadRequest(message);
        }



        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var result = await _stockService.TGetActiveProductsAsync();
            return Ok(result);
        }
    }
}
