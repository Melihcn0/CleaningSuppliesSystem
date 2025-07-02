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
    public class InvoicesController(IGenericService<Invoice> _ınvoiceService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _ınvoiceService.TGetListAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _ınvoiceService.TGetByIdAsync(id);
            return Ok(value);
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
