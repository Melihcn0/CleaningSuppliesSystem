using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]/{id?}")]
    public class FinanceController(CleaningSuppliesSystemContext _context, IFinanceService _financeService, IMapper _mapper) : Controller
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
            var (isSuccess, message) = await _financeService.TSoftDeleteFinanceAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedFinance(int id)
        {
            var (isSuccess, message) = await _financeService.TUndoSoftDeleteFinanceAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(DeletedFinance));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(DeletedFinance));
        }

        public async Task<IActionResult> DeletedFinance()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultFinanceDto>>("finances");
            var finances = values.Where(x => x.IsDeleted == true).ToList();
            return View(finances);
        }

        public async Task<IActionResult> DeleteFinance(int id)
        { 
            await _client.DeleteAsync($"finances/{id}");
            TempData["SuccessMessage"] = "Finans kaydı başarıyla silindi.";
            return RedirectToAction(nameof(DeletedFinance));
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
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFinance(CreateFinanceDto createFinanceDto)
        {
            var validator = new Business.Validators.Validators.CreateFinanceValidator();
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
            var (isSuccess, message) = await _financeService.TCreateFinanceAsync(createFinanceDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(createFinanceDto);
            }

            TempData["SuccessMessage"] = "Finans kaydı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFinance(UpdateFinanceDto updateFinanceDto)
        {
            var validator = new Business.Validators.Validators.UpdateFinanceValidator();
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
            var (isSuccess, message) = await _financeService.TUpdateFinanceAsync(updateFinanceDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(updateFinanceDto);
            }

            TempData["SuccessMessage"] = "Finans Kaydı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
