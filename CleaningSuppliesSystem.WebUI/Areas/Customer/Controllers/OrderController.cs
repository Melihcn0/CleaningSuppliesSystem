 using CleaningSuppliesSystem.DTO.DTOs.Customer.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

[Authorize(Roles = "Customer")]
[Area("Customer")]
public class OrderController : Controller
{
    private readonly HttpClient _client;
    private readonly PaginationHelper _paginationHelper;

    public OrderController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
    {
        _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        _paginationHelper = paginationHelper;
    }
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var response = await _paginationHelper.GetPagedDataAsync<CustomerResultOrderDto>(
            $"customerOrders?page={page}&pageSize={pageSize}");

        if (response == null || response.Data == null)
        {
            TempData["ErrorMessage"] = "Siparişleriniz yüklenirken bir sorun oluştu. Lütfen tekrar deneyin.";
            return View(new PagedResponse<CustomerResultOrderDto>
            {
                Data = new List<CustomerResultOrderDto>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            });
        }

        if (response.Data.Any())
        {
            ViewBag.AdminIban = response.Data.First().AdminBank?.IBAN ?? "-";
            ViewBag.AdminNameSurname = response.Data.First().AdminBank?.AccountHolder ?? "-";
        }

        return View(response);
    }
    public async Task<IActionResult> OrderDetails(int id)
    {
        var response = await _client.GetAsync($"customerOrders/{id}");

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage;
            if (response.StatusCode == HttpStatusCode.Forbidden)
                errorMessage = "Bu siparişi görüntüleme yetkiniz yok.";
            else if (response.StatusCode == HttpStatusCode.NotFound)
                errorMessage = "Aradığınız sipariş bulunamadı.";
            else
                errorMessage = "Sipariş detayları yüklenirken bir sorun oluştu.";

            return PartialView("_ErrorPartial", errorMessage);
        }

        var order = await response.Content.ReadFromJsonAsync<CustomerResultOrderDto>();

        if (order == null)
        {
            return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
        }

        ViewData["ShowPendingPaymentMessage"] = order.Status == "Onaylandı";

        // 🔑 Admin bilgilerini ViewBag’e set et
        ViewBag.AdminIban = order.AdminBank?.IBAN ?? "-";
        ViewBag.AdminNameSurname = order.AdminBank?.AccountHolder ?? "-";

        return PartialView("_OrderDetailPartial", order);
    }
    public async Task<IActionResult> ReadOnlyDetails(int id)
    {
        var response = await _client.GetAsync($"customerOrders/customerResult/{id}");

        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<CustomerResultOrderDto>();
            if (order == null)
            {
                // Veri gelmediyse, hata partial view'ını döndür.
                return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
            }
            // Başarılı durumda, sipariş verisiyle birlikte partial view'ı döndür.
            return PartialView("_ReadOnlyOrderDetailPartial", order);
        }
        else
        {
            string errorMessage;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                errorMessage = "Bu siparişi görüntüleme yetkiniz yok.";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                errorMessage = "Aradığınız sipariş bulunamadı.";
            }
            else
            {
                errorMessage = "Sipariş detayları yüklenirken bir sorun oluştu.";
            }
            // Hata durumunda da bir partial view döndür.
            return PartialView("_ReadOnlyOrderDetailPartial", errorMessage);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrement(int id, int orderId)
    {
        try
        {
            var response = await _client.PostAsync($"orderitems/decrement/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Miktar azaltılamadı." });
            }

            return Json(new { success = true, message = "Miktar başarıyla azaltıldı." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Increment(int id, int orderId)
    {
        try
        {
            var response = await _client.PostAsync($"orderitems/increment/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Miktar arttırılamadı." });
            }

            return Json(new { success = true, message = "Miktar başarıyla arttırıldı." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            var response = await _client.DeleteAsync($"orderitems/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Ürün silinemedi." });
            }

            return Json(new { success = true, message = "Ürün başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await _client.PostAsync($"customerOrders/cancelOrder/{id}", null);

        if (response.StatusCode == HttpStatusCode.Forbidden)
            return BadRequest("Bu siparişi iptal etme yetkiniz yok.");

        if (!response.IsSuccessStatusCode)
            return BadRequest("Sipariş iptal edilemedi.");

        return Ok();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> AddToCart(AddToOrderDto dto)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["ErrorMessage"] = "Sipariş vermek için giriş yapmalısınız.";
            return RedirectToAction("Index", "Product", new { area = "" });
        }

        if (!User.IsInRole("Customer"))
        {
            TempData["ErrorMessage"] = "Bu işlem sadece müşteri hesabı ile yapılabilir.";
            return RedirectToAction("SignIn", "Login", new { area = "" });
        }

        var response = await _client.PostAsJsonAsync("customerOrders/add-to-order", dto);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Ürün sepete eklendi.";
        else
        {
            var errorJson = await response.Content.ReadAsStringAsync();
            string message = "Ürün sepete eklenemedi.";

            try
            {
                using var doc = JsonDocument.Parse(errorJson);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    message = msgProp.GetString() ?? message;
            }
            catch { }

            TempData["ValidateProfileErrorMessage"] = message;
        }

        return RedirectToAction("Index", "Product", new { area = "" });
    }


    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        var pdfResponse = await _client.GetAsync($"invoices/byorderCustomer/{orderId}");
        if (!pdfResponse.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
        var contentDisposition = pdfResponse.Content.Headers.ContentDisposition;
        string fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName;
        fileName = fileName?.Trim('"');
        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        var response = await _client.GetAsync($"customerOrders/{id}");
        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<CustomerResultOrderDto>();
            if (order == null)
            {
                return View(nameof(Index));
            }
            return View(nameof(Index), order);
        }
        else
        {
            return View(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus([FromBody] OrderStatusUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { message = "Geçersiz veri", errors });
        }

        var response = await _client.PostAsJsonAsync("customerOrders/UpdateStatus", dto);

        if (!response.IsSuccessStatusCode)
            return BadRequest("Sipariş durumu güncellenemedi.");

        return Ok();
    }



}
