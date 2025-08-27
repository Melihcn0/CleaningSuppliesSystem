 using CleaningSuppliesSystem.DTO.DTOs.Customer.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;

[Authorize(Roles = "Customer")]
[Area("Customer")]
public class OrderController : Controller
{
    private readonly HttpClient _client;

    public OrderController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _client.GetAsync("customerOrders");

        if (response.IsSuccessStatusCode)
        {
            var orders = await response.Content.ReadFromJsonAsync<List<ResultOrderDto>>();
            return View(orders ?? new List<ResultOrderDto>());
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            TempData["ErrorMessage"] = "Siparişlerinizi görüntülemek için lütfen giriş yapın.";
            return RedirectToAction("SignIn", "Login");
        }

        TempData["ErrorMessage"] = "Siparişleriniz yüklenirken bir sorun oluştu. Lütfen tekrar deneyin.";
        return View(new List<ResultOrderDto>());
    }


    public async Task<IActionResult> Details(int id)
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

        var order = await response.Content.ReadFromJsonAsync<ResultOrderDto>();

        if (order == null)
        {
            return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
        }

        ViewData["ShowPendingPaymentMessage"] = order.Status == "Onay Bekleniyor";

        return PartialView("_OrderDetailPartial", order);
    }

    // WebUI Katmanı
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        // API'nin URL formatına uygun şekilde id'yi URL'ye ekle
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
        // Kullanıcı giriş yapmamışsa
        if (!User.Identity.IsAuthenticated)
        {
            TempData["ErrorMessage"] = "Sipariş vermek için giriş yapmalısınız.";
            return RedirectToAction("Index", "Product", new { area = "" });
        }

        // Kullanıcı Customer rolüne sahip değilse
        if (!User.IsInRole("Customer"))
        {
            TempData["ErrorMessage"] = "Bu işlem sadece müşteri hesabı ile yapılabilir.";
            return RedirectToAction("Index", "Product", new { area = "" });
        }

        // Customer rolündeki kullanıcı için işlem yapılır
        var response = await _client.PostAsJsonAsync("customerOrders/add-to-order", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Ürün sepete eklendi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Ürün sepete eklenemedi.";
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




}
