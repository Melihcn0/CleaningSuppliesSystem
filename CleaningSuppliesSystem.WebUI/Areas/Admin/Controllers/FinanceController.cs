using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.WebUI.DTOs.FinanceDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]/{id?}")]
    public class FinanceController(CleaningSuppliesSystemContext _context) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultFinanceDto>>("finances");
            var finances = values.Where(x => x.IsDeleted == false).ToList();
            return View(finances);
        }
        [HttpPost]
        public async Task<IActionResult> SoftDeletedFinance(int id)
        {
            var finance = await _context.Finances.FirstOrDefaultAsync(x => x.Id == id);
            finance.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletedFinance()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultFinanceDto>>("finances");
            var finances = values.Where(x => x.IsDeleted == true).ToList();
            return View(finances);
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedFinance(int id)
        {
            ViewBag.ShowBackButton = true;
            var finances = await _context.Finances.FindAsync(id);
            finances.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeletedFinance));
        }
        public async Task<IActionResult> DeleteFinance(int id)
        {
            await _client.DeleteAsync($"finances/{id}");
            return RedirectToAction(nameof(DeletedFinance));
        }
        public IActionResult CreateFinance()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFinance(CreateFinanceDto createFinanceDto)
        {
            var validator = new Validators.Validators.CreateFinanceValidator();
            var result = await validator.ValidateAsync(createFinanceDto);
            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(createFinanceDto);
            }
            await _client.PostAsJsonAsync("finances", createFinanceDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateFinance(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateFinanceDto>($"finances/{id}");
            ViewBag.ShowBackButton = true;
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFinance(UpdateFinanceDto updateFinanceDto)
        {
            var validator = new Validators.Validators.UpdateFinanceValidator();
            var result = await validator.ValidateAsync(updateFinanceDto);
            var category = await _client.GetFromJsonAsync<ResultFinanceDto>($"finances/{updateFinanceDto.Id}");
            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(updateFinanceDto);
            }

            await _client.PutAsJsonAsync("finances", updateFinanceDto);

            TempData["SuccessMessage"] = "Finans başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
