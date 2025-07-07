using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.WebUI.DTOs.FinanceDtos;
using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using CleaningSuppliesSystem.WebUI.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.WebUI.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]/{id?}")]
    public class StockEntryController(CleaningSuppliesSystemContext _context) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();

        [HttpGet]
        public async Task LoadAvailableProducts()
        {
            var productList = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var usedProductIds = await _context.StockEntries.Select(s => s.ProductId).ToListAsync();
            var availableProducts = productList.Where(p => !usedProductIds.Contains(p.Id)).ToList();

            List<SelectListItem> categories = availableProducts.Select(x => new SelectListItem
                                                                      {
                                                                          Text = x.Name,
                                                                          Value = x.Id.ToString()
                                                                      }).ToList();
            ViewBag.categories = categories;
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
            var stockEntry = await _context.StockEntries.FirstOrDefaultAsync(x => x.Id == id);
            stockEntry.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletedStockEntry()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultStockEntryDto>>("stockEntries");
            var stockEntries = values.Where(x => x.IsDeleted == true).ToList();
            return View(stockEntries);
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedStockEntry(int id)
        {
            ViewBag.ShowBackButton = true;
            var stockEntries = await _context.StockEntries.FindAsync(id);
            stockEntries.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeletedStockEntry));
        }
        public async Task<IActionResult> DeleteStockEntry(int id)
        {
            await _client.DeleteAsync($"stockEntries/{id}");
            return RedirectToAction(nameof(DeletedStockEntry));
        }
        public async Task<IActionResult> CreateStockEntry()
        {
            await LoadAvailableProducts();
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStockEntry(CreateStockEntryDto createStockEntryDto)
        {
            var validator = new Validators.Validators.CreateStockEntryValidator();
            var result = await validator.ValidateAsync(createStockEntryDto);

            if (!result.IsValid)
            {
                await LoadAvailableProducts();
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(createStockEntryDto);
            }
            await _client.PostAsJsonAsync("stockEntries", createStockEntryDto);
            TempData["Success"] = "Stok başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateStockEntry(int id)
        {
            await ProductDropdown();
            var value = await _client.GetFromJsonAsync<UpdateStockEntryDto>($"stockEntries/{id}");            
            ViewBag.ShowBackButton = true;
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStockEntry(UpdateStockEntryDto updateStockEntryDto)
        {
            var validator = new Validators.Validators.UpdateStockEntryValidator();
            var result = await validator.ValidateAsync(updateStockEntryDto);
            var category = await _client.GetFromJsonAsync<ResultStockEntryDto>($"stockEntries/{updateStockEntryDto.Id}");
            if (!result.IsValid)
            {
                await ProductDropdown();
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(updateStockEntryDto);
            }
            await _client.PutAsJsonAsync("stockEntries", updateStockEntryDto);
            TempData["SuccessMessage"] = "Stok başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
