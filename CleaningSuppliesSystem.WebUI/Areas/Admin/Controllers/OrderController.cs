using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly HttpClient _client;

        public OrderController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("orders");

            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<AdminResultOrderDto>>() ?? new List<AdminResultOrderDto>();
                return View(orders);
            }

            TempData["ErrorMessage"] = "Siparişler yüklenemedi.";
            return View(new List<AdminResultOrderDto>());
        }
        public async Task<IActionResult> CompletedOrders()
        {
            var response = await _client.GetAsync("orders/completed");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Siparişler yüklenemedi.";
                return View(new List<AdminResultOrderDto>());
            }

            var orders = await response.Content.ReadFromJsonAsync<List<AdminResultOrderDto>>() ?? new List<AdminResultOrderDto>();
            return View(orders);
        }
        public async Task<IActionResult> CancelledOrders()
        {
            var response = await _client.GetAsync("orders/cancelled");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Siparişler yüklenemedi.";
                return View(new List<AdminResultOrderDto>());
            }

            var orders = await response.Content.ReadFromJsonAsync<List<AdminResultOrderDto>>() ?? new List<AdminResultOrderDto>();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var response = await _client.GetAsync($"orders/AdminResult/{id}");

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<AdminResultOrderDto>();
                if (order == null)
                {
                    return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
                }
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
                return PartialView("_ErrorPartial", errorMessage);
            }
        }
        public async Task<IActionResult> ReadOnlyDetails(int id)
        {
            var response = await _client.GetAsync($"orders/AdminResult/{id}");

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<AdminResultOrderDto>();
                if (order == null)
                {
                    return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
                }
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
                return PartialView("_ReadOnlyOrderDetailPartial", errorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var response = await _client.GetAsync($"orders/AdminResult/{id}");

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<AdminResultOrderDto>();
                if (order == null)
                {
                    return PartialView("_ErrorPartial", "Belirtilen sipariş bulunamadı.");
                }
                return PartialView("_UpdateStatusPartial", order);
            }
            else
            {
                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Forbidden => "Bu sipariş durumunu güncelleme yetkiniz yok.",
                    HttpStatusCode.NotFound => "Aradığınız sipariş bulunamadı.",
                    _ => "Sipariş durumu yüklenirken bir sorun oluştu."
                };
                return PartialView("_ErrorPartial", errorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Geçersiz veri.");

            var response = await _client.PostAsJsonAsync("orders/UpdateStatus", dto);
            var updatedOrder = await response.Content.ReadFromJsonAsync<OrderStatusUpdateDto>();


            return Json(updatedOrder);
        }
        public async Task<IActionResult> DownloadDeliveryNoteInvoice(int orderId)
        {
            var pdfResponse = await _client.GetAsync($"invoices/byorderAdmin/{orderId}");
            if (!pdfResponse.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
            var contentDisposition = pdfResponse.Content.Headers.ContentDisposition;
            string fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName;
            fileName = fileName?.Trim('"');
            return File(pdfBytes, "application/pdf", fileName);
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


    }
}
