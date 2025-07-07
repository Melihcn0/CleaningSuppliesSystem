using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.CategoryDtos;
using CleaningSuppliesSystem.WebUI.DTOs.FinanceDtos;
using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static CleaningSuppliesSystem.WebUI.Validators.Validators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]/{id?}")]
    public class ProductController(CleaningSuppliesSystemContext _context) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();

        [HttpGet]
        public async Task CategoryDropdown()
        {
            var categoryList = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            List<SelectListItem> categories = (from x in categoryList
                                               select new SelectListItem
                                               {
                                                   Text = x.Name,
                                                   Value = x.Id.ToString()
                                               }).ToList();
            ViewBag.categories = categories;
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var products = values.Where(x => x.IsDeleted == false).ToList();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyDiscount(UpdateProductDto updateProductDto)
        {
            var validator = new DiscountValidator();
            var result = await validator.ValidateAsync(updateProductDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(updateProductDto);
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == updateProductDto.Id);
            product.DiscountRate = updateProductDto.DiscountRate;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Üründe %{updateProductDto.DiscountRate} indirim yapıldı.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ApplyDiscount(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id)
                .Select(p => new UpdateProductDto
                {
                    Id = p.Id,
                    UnitPrice = p.UnitPrice,
                    DiscountRate = p.DiscountRate
                })
                .FirstOrDefaultAsync();
            ViewBag.ShowBackButton = true;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletedProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletedProduct()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var products = values.Where(x => x.IsDeleted == true).ToList();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedProduct(int id)
        {
            ViewBag.ShowBackButton = true;
            var products = await _context.Products.FindAsync(id);
            products.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeletedProduct));
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _client.DeleteAsync($"products/{id}");
            return RedirectToAction(nameof(DeletedProduct));
        }
        public async Task<IActionResult> CreateProduct()
        {
            await CategoryDropdown();
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var validator = new Validators.Validators.CreateProductValidator();
            var result = await validator.ValidateAsync(createProductDto);
            await CategoryDropdown();
            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(createProductDto);
            }

            await _client.PostAsJsonAsync("products", createProductDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            await CategoryDropdown();
            var value = await _client.GetFromJsonAsync<UpdateProductDto>($"products/{id}");
            ViewBag.ShowBackButton = true;
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var validator = new Validators.Validators.UpdateProductValidator();
            var result = await validator.ValidateAsync(updateProductDto);

            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                await CategoryDropdown();
                return View(updateProductDto);
            }
            await CategoryDropdown();
            await _client.PutAsJsonAsync("products", updateProductDto);

            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
