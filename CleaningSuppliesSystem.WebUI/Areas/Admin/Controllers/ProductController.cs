using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
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
    public class ProductController(CleaningSuppliesSystemContext context, IProductService _productService) : Controller
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
            (bool isSuccess, List<string> errors) = await _productService.TApplyDiscountAsync(updateProductDto);
            if (!isSuccess)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(updateProductDto);
            }

            TempData["SuccessMessage"] = $"Üründe %{updateProductDto.DiscountRate} indirim yapıldı.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ApplyDiscount(int id)
        {
            var productDto = await _productService.TGetProductForDiscountAsync(id);
            if (productDto == null)
                return NotFound();

            ViewBag.ShowBackButton = true;
            return View(productDto);
        }



        public async Task<IActionResult> DeletedProduct()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultProductDto>>("products");
            var products = values.Where(x => x.IsDeleted == true).ToList();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletedProduct(int id)
        {
            var (isSuccess, message) = await _productService.TSoftDeleteProductAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedProduct(int id)
        {
            var (isSuccess, message) = await _productService.TUndoSoftDeleteProductAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(DeletedProduct));
            }

            TempData["SuccessMessage"] = message;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var validator = new Business.Validators.Validators.CreateProductValidator();
            var result = await validator.ValidateAsync(createProductDto);


            await CategoryDropdown();

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(createProductDto);
            }

            var (isSuccess, message) = await _productService.TCreateProductAsync(createProductDto);

            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(createProductDto);
            }

            TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var validator = new Business.Validators.Validators.UpdateProductValidator();
            var result = await validator.ValidateAsync(updateProductDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await CategoryDropdown();
                return View(updateProductDto);
            }

            var (isSuccess, message) = await _productService.TUpdateProductAsync(updateProductDto);

            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                await CategoryDropdown();
                return View(updateProductDto);
            }

            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            await CategoryDropdown();
            var value = await _client.GetFromJsonAsync<UpdateProductDto>($"products/{id}");
            ViewBag.ShowBackButton = true;
            return View(value);
        }

    }
}
