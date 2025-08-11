using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiExplorerSettings(GroupName = "Invoices")]
[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IMapper _mapper;

    public InvoicesController(IInvoiceService invoiceService, IMapper mapper)
    {
        _invoiceService = invoiceService;
        _mapper = mapper;
    }

    // Admin: Tüm faturaları görür, Customer: sadece kendi faturalarını görür
    [HttpGet]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> Get()
    {
        if (User.IsInRole("Admin"))
        {
            var invoices = await _invoiceService.TGetInvoiceWithOrderAsync();
            var invoicesDto = _mapper.Map<List<InvoiceDto>>(invoices);
            return Ok(invoicesDto);
        }
        else
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var invoices = await _invoiceService.TGetInvoicesByUserIdAsync(userId); // Bu metodu servise eklemelisin
            var invoicesDto = _mapper.Map<List<InvoiceDto>>(invoices);
            return Ok(invoicesDto);
        }
    }

    // ID'ye göre fatura detayını getirir, müşteri sadece kendi faturasını görebilir
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _invoiceService.TGetByIdAsyncWithOrder(id);
        if (invoice == null)
            return NotFound($"Fatura id={id} bulunamadı.");

        if (User.IsInRole("Customer"))
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            if (invoice.Order == null || invoice.Order.AppUserId != userId)
                return Forbid("Bu faturayı görmeye yetkiniz yok.");
        }

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return Ok(invoiceDto);
    }

    // Sadece Admin fatura oluşturabilir
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto createInvoiceDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _invoiceService.TCreateInvoiceAsync(createInvoiceDto.OrderId);
        return Ok($"Yeni Fatura Oluşturuldu. Sipariş ID={createInvoiceDto.OrderId}");
    }

    [HttpGet("{id}/pdf")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var invoice = await _invoiceService.TGetByIdAsyncWithOrder(id);

        if (User.IsInRole("Customer"))
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            if (invoice.Order == null || invoice.Order.AppUserId != userId)
                return Forbid("Bu faturayı indirme yetkiniz yok.");
        }

        var pdfBytes = await _invoiceService.TGenerateInvoicePdfAsync(invoice.OrderId);
        return File(pdfBytes, "application/pdf", $"Invoice_{invoice.Id}.pdf");
    }

    [HttpGet("byorder/{orderId}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetPdfByOrderId(int orderId)
    {
        var invoice = await _invoiceService.TGetInvoiceByOrderIdAsync(orderId);

        if (invoice == null)
            return NotFound();

        if (User.IsInRole("Customer"))
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            if (invoice.Order == null || invoice.Order.AppUserId.ToString() != userIdString)
                return Forbid("Bu faturayı indirme yetkiniz yok.");
        }

        var pdfBytes = await _invoiceService.TGenerateInvoicePdfAsync(invoice.OrderId);
        return File(pdfBytes, "application/pdf", $"Invoice_{invoice.Id}.pdf");
    }




}
