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

    // Müşterinin tüm siparişleri gösterir.
    public async Task<IActionResult> Index()
    {
        // API'den sipariş listesini çekme
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

        // Diğer başarısız durumlar için genel bir hata mesajı gösterir
        TempData["ErrorMessage"] = "Siparişleriniz yüklenirken bir sorun oluştu. Lütfen tekrar deneyin.";
        return View(new List<ResultOrderDto>());
    }

    // Belirli bir siparişin detaylarını gösterir.

    public async Task<IActionResult> Details(int id)
    {
        var response = await _client.GetAsync($"customerOrders/{id}");

        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<ResultOrderDto>();
            if (order == null)
            {
                // Veri gelmediyse, hata partial view'ını döndür.
                return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
            }
            // Başarılı durumda, sipariş verisiyle birlikte partial view'ı döndür.
            return PartialView("_OrderDetailPartial", order);
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
            return PartialView("_ErrorPartial", errorMessage);
        }
    }


    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await _client.DeleteAsync($"customerOrders/{id}");

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            TempData["ErrorMessage"] = "Bu siparişi iptal etme yetkiniz yok.";
        }
        else if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Sipariş iptal edilemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Sipariş başarıyla iptal edildi.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AllowAnonymous] // Giriş yapmamış da olsa kontrol içeride yapılır
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
        var pdfResponse = await _client.GetAsync($"invoices/byorder/{orderId}");
        if (!pdfResponse.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
        return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
    }



}
