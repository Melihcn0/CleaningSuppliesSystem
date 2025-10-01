using Microsoft.AspNetCore.Mvc;
using CleaningSuppliesSystem.WebUI.Models;
using System.Net.Http.Json;
using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _client;

        public ContactController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(CreateContactDto createContactDto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");

            var response = await _client.PostAsJsonAsync("Contacts", createContactDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi!";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["ErrorMessage"] = "Mesaj gönderilemedi. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");

            }
        }
    }
}
