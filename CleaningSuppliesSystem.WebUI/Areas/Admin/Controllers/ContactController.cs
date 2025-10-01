using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System;
using System.Net.Http;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public ContactController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            ViewData["ActiveTab"] = "Active";
            var response = await _client.GetFromJsonAsync<ApiResponseModel>(
                $"Contacts/active?page={page}&pageSize={pageSize}");

            ViewBag.ActiveCount = response.ActiveCount;
            ViewBag.DeletedCount = response.DeletedCount;

            return View(response.PagedContacts);
        }
        public async Task<IActionResult> DeletedContact(int page = 1, int pageSize = 10)
        {
            ViewData["ActiveTab"] = "Deleted";
            var response = await _client.GetFromJsonAsync<ApiResponseModel>(
                $"Contacts/deleted?page={page}&pageSize={pageSize}");

            ViewBag.ActiveCount = response.ActiveCount;
            ViewBag.DeletedCount = response.DeletedCount;

            return View(response.PagedContacts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç mesaj seçilmedi.");
            var response = await _client.PostAsJsonAsync("Contacts/DeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç mesaj seçilmedi.");
            var response = await _client.PostAsJsonAsync("Contacts/UndoSoftDeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç mesaj seçilmedi.");
            var response = await _client.PostAsJsonAsync("Contacts/PermanentDeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }
    }
}
