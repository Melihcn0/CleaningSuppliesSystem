using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class StockEntryController(CleaningSuppliesSystemContext _context, IStockEntryService _stockEntryService, IMapper _mapper) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task LoadSelectableProducts(int? includedProductId = null)
        {
            var productList = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");

            var usedProductIds = await _context.StockEntries
                                               .Select(s => s.ProductId)
                                               .Distinct()
                                               .ToListAsync();

            var filteredProducts = productList
                .Where(p => !usedProductIds.Contains(p.Id) || (includedProductId.HasValue && p.Id == includedProductId.Value))
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                })
                .ToList();

            ViewBag.products = filteredProducts;
        }
        public async Task ProductDropdown()
        {
            var productList = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            List<SelectListItem> products = (from x in productList
                                             select new SelectListItem
                                               {
                                                   Text = x.Name,
                                                   Value = x.Id.ToString()
                                               }).ToList();
            ViewBag.categories = products;
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultStockEntryDto>>("stockEntries");
            var stockEntries = values.Where(x => x.IsDeleted == false).ToList();
            return View(stockEntries);
        }
        [HttpPost]
        public async Task<IActionResult> SoftDeletedStockEntry(int id)
        {
            var (isSuccess, message) = await _stockEntryService.TSoftDeleteStockEntryAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedStockEntry(int id)
        {
            var (isSuccess, message) = await _stockEntryService.TUndoSoftDeleteStockEntryAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(DeletedStockEntry));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(DeletedStockEntry));
        }

        public async Task<IActionResult> DeletedStockEntry()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultStockEntryDto>>("stockEntries");
            var stockEntries = values.Where(x => x.IsDeleted == true).ToList();
            return View(stockEntries);
        }
        public async Task<IActionResult> DeleteStockEntry(int id)
        {
            await _client.DeleteAsync($"stockEntries/{id}");
            return RedirectToAction(nameof(DeletedStockEntry));
        }
        public async Task<IActionResult> CreateStockEntry()
        {
            await ProductDropdown();
            ViewBag.ShowBackButton = true;
            return View();
        }
        public async Task<IActionResult> UpdateStockEntry(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateStockEntryDto>($"stockEntries/{id}");
            await LoadSelectableProducts(value.ProductId);
            ViewBag.ShowBackButton = true;
            Console.WriteLine("Current Product ID: " + value.ProductId);
            return View(value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStockEntry(CreateStockEntryDto createStockEntryDto)
        {
            var validator = new Business.Validators.Validators.CreateStockEntryValidator();
            var result = await validator.ValidateAsync(createStockEntryDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(createStockEntryDto);
            }
            var (isSuccess, message) = await _stockEntryService.TCreateStockEntryAsync(createStockEntryDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(createStockEntryDto);
            }

            TempData["SuccessMessage"] = "Stok kaydı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStockEntry(UpdateStockEntryDto updateStockEntryDto)
        {
            var validator = new Business.Validators.Validators.UpdateStockEntryValidator();
            var result = await validator.ValidateAsync(updateStockEntryDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(updateStockEntryDto);
            }
            var (isSuccess, message) = await _stockEntryService.TUpdateStockEntryAsync(updateStockEntryDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(updateStockEntryDto);
            }

            TempData["SuccessMessage"] = "Stok kaydı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

    }
}
