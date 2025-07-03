using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController(IInvoiceService _ınvoiceService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _ınvoiceService.TGetInvoiceWithOrderAsync();
            var ınvoices = _mapper.Map<List<ResultInvoiceDto>>(values);
            return Ok(ınvoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _ınvoiceService.TGetByIdAsyncWithOrder(id);
            var result = _mapper.Map<ResultInvoiceDto>(value);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto createInvoiceDto)
        {
            var newValue = _mapper.Map<Invoice>(createInvoiceDto);
            await _ınvoiceService.TCreateAsync(newValue);
            return Ok($"Yeni Fatura Oluşturuldu Fatura ID={newValue.Id}");
        }
    }
}
