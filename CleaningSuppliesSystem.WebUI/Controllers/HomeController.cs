using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using CleaningSuppliesSystem.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        public async Task<IActionResult> Index()
        {
            ResultPromoAlertDto promoAlert = null;

            var response = await _client.GetAsync("promoAlerts/first");
            if (response.IsSuccessStatusCode)
            {
                promoAlert = await response.Content.ReadFromJsonAsync<ResultPromoAlertDto>();
            }

            ViewBag.PromoAlert = promoAlert;

            return View();
        }


    }
}
