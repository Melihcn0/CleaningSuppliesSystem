using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.StockValidatorDto;
using Microsoft.AspNetCore.Mvc.Rendering;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class StockController : Controller
    {
        private readonly HttpClient _client;
        private readonly PaginationHelper _paginationHelper;

        public StockController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _paginationHelper = paginationHelper;
        }

        private async Task LoadTopCategoriesDropdownAsync(int? selectedTopCategoryId = null)
        {
            var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("topcategories/active-all");
            ViewBag.topCategories = new SelectList(topCategories, "Id", "Name", selectedTopCategoryId);
        }
        private async Task LoadAllDropdownsAsync(CreateStockOperationDto dto)
        {
            var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("topCategories/active-all");
            ViewBag.topCategories = topCategories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == dto.TopCategoryId
            }).ToList();

            if (dto.TopCategoryId > 0)
            {
                var subCategories = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>($"subCategories/GetSubCategories/{dto.TopCategoryId}");
                ViewBag.subCategories = subCategories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.SubCategoryId
                }).ToList();
            }
            else
            {
                ViewBag.subCategories = new List<SelectListItem>();
            }

            if (dto.SubCategoryId > 0)
            {
                var categories = await _client.GetFromJsonAsync<List<ResultCategoryDto>>($"categories/GetCategories/{dto.SubCategoryId}");
                ViewBag.categories = categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.CategoryId
                }).ToList();
            }
            else
            {
                ViewBag.categories = new List<SelectListItem>();
            }

            if (dto.CategoryId > 0)
            {
                var brands = await _client.GetFromJsonAsync<List<ResultBrandDto>>($"brands/GetBrands/{dto.CategoryId}");
                ViewBag.brands = brands.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.BrandId
                }).ToList();
            }
            else
            {
                ViewBag.brands = new List<SelectListItem>();
            }

            if (dto.BrandId > 0)
            {
                var products = await _client.GetFromJsonAsync<List<ResultProductDto>>($"products/GetProducts/{dto.BrandId}");
                ViewBag.products = products.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.ProductId
                }).ToList();
            }
            else
            {
                ViewBag.products = new List<SelectListItem>();
            }
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultStockOperationDto>(
                $"Stocks/active?page={page}&pageSize={pageSize}");

            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStockOperation()
        {
            var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("topCategories/active-all");
            ViewBag.ShowBackButton = true;
            ViewBag.topCategories = topCategories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStockOperation(CreateStockOperationDto dto)
        {
            var validator = new CreateStockOperationValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadAllDropdownsAsync(dto);

                return View(dto);
            }

            var response = await _client.PostAsJsonAsync("stocks/assign", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Stok işlemi başarıyla yapıldı.";
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();

            await LoadAllDropdownsAsync(dto);

            TempData["ErrorMessage"] = string.IsNullOrEmpty(errorMsg) ? "Stok işlemi oluşturulamadı." : errorMsg;

            return View(dto);
        }

        [HttpGet("/Stock/GetSubCategories/{id}")]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            var subCategories = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>($"subCategories/GetSubCategories/{id}");
            return Json(subCategories);
        }
        [HttpGet("/Stock/GetCategories/{id}")]
        public async Task<IActionResult> GetCategories(int id)
        {
            var categories = await _client.GetFromJsonAsync<List<ResultCategoryDto>>($"categories/GetCategories/{id}");
            return Json(categories);
        }
        [HttpGet("/Stock/GetBrands/{id}")]
        public async Task<IActionResult> GetBrands(int id)
        {
            var brands = await _client.GetFromJsonAsync<List<ResultBrandDto>>($"brands/GetBrands/{id}");
            return Json(brands);
        }
        [HttpGet("/Stock/GetProducts/{id}")]
        public async Task<IActionResult> GetProducts(int id)
        {
            var products = await _client.GetFromJsonAsync<List<ResultProductDto>>($"products/GetProducts/{id}");
            return Json(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickStockOperation([FromForm] QuickStockOperationDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest("Geçersiz miktar.");

            var response = await _client.PostAsJsonAsync("stocks/quickAssign", dto);

            var apiMessage = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(string.IsNullOrEmpty(apiMessage) ? "İşlem başarılı." : apiMessage);

            return BadRequest(string.IsNullOrEmpty(apiMessage) ? "Stok işlemi gerçekleştirilemedi." : apiMessage);
        }




    }
}
