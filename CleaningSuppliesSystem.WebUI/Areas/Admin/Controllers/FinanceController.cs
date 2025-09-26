using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.FinanceValidatorDto;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class FinanceController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public FinanceController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultFinanceDto>(
                $"Finances/active?page={page}&pageSize={pageSize}");

            return View(response);
        }

        public async Task<IActionResult> DeletedFinances(int page = 1, int pageSize = 10)
        {
            ViewBag.ShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultFinanceDto>(
                $"Finances/deleted?page={page}&pageSize={pageSize}");

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeletedFinance(int id)
        {
            var response = await _client.PostAsync($"finances/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeletedFinance(int id)
        {
            var response = await _client.PostAsync($"finances/undosoftdelete/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Finans kaydı çöp kutusundan başarıyla geri alındı.";
            else
                TempData["ErrorMessage"] = "Finans kaydının çöp kutusundan geri alma işlemi başarısız.";

            return RedirectToAction(nameof(DeletedFinances));
        }

        public IActionResult CreateFinance()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        public async Task<IActionResult> UpdateFinance(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateFinanceDto>($"finances/{id}");
            ViewBag.ShowBackButton = true;
            if (value == null) return NotFound();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFinance(CreateFinanceDto createFinanceDto)
        {
            var validator = new CreateFinanceValidator();
            var result = await validator.ValidateAsync(createFinanceDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(createFinanceDto);
            }

            var response = await _client.PostAsJsonAsync("finances", createFinanceDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Finans oluşturma işlemi başarısız.");
                return View(createFinanceDto);
            }

            TempData["SuccessMessage"] = "Finans kaydı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFinance(UpdateFinanceDto updateFinanceDto)
        {
            var validator = new UpdateFinanceValidator();
            var result = await validator.ValidateAsync(updateFinanceDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(updateFinanceDto);
            }

            var response = await _client.PutAsJsonAsync("finances", updateFinanceDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Finans güncelleme işlemi başarısız.");
                return View(updateFinanceDto);
            }

            TempData["SuccessMessage"] = "Finans kaydı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteFinance(int id)
        {
            var response = await _client.DeleteAsync($"finances/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);
            else
                return BadRequest(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç finans kaydı seçilmedi.");
            var response = await _client.PostAsJsonAsync("finances/DeleteMultiple", ids);

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
                return BadRequest("Hiç finans kaydı seçilmedi.");
            var response = await _client.PostAsJsonAsync("finances/UndoSoftDeleteMultiple", ids);

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
                return BadRequest("Hiç finans kaydı seçilmedi.");
            var response = await _client.PostAsJsonAsync("finances/PermanentDeleteMultiple", ids);

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
