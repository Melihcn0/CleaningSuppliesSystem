using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.BrandValidatorDto;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly HttpClient _client;

        public BrandController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        private async Task LoadCategoriesDropdownAsync(int? selectedCategoryId = null)
        {
            var categories = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories/active");
            ViewBag.categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }

        public async Task<IActionResult> Index()
        {
            var resultDtos = await _client.GetFromJsonAsync<List<ResultBrandDto>>("Brands/active");

            var vm = new BrandViewModel
            {
                BrandViewList = resultDtos
            };

            return View(vm);
        }

        public async Task<IActionResult> DeletedBrands()
        {
            ViewBag.ShowBackButton = true;
            var resultdeletedDtos = await _client.GetFromJsonAsync<List<ResultBrandDto>>("Brands/deleted");
            var vm = new BrandViewModel
            {
                BrandViewList = resultdeletedDtos
            };

            return View(vm);
        }

        public async Task<IActionResult> CreateBrand()
        {
            await LoadCategoriesDropdownAsync();
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(CreateBrandDto dto)
        {
            var validator = new CreateBrandValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCategoriesDropdownAsync(dto.CategoryId);
                return View(dto);
            }

            var response = await _client.PostAsJsonAsync("Brands", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Marka başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategoriesDropdownAsync(dto.CategoryId);
            TempData["ErrorMessage"] = "Marka oluşturulamadı.";
            return View(dto);
        }

        public async Task<IActionResult> UpdateBrand(int id)
        {
            ViewBag.ShowBackButton = true;

            var brand = await _client.GetFromJsonAsync<UpdateBrandDto>($"brands/{id}");
            if (brand == null)
                return NotFound();

            var categories = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories/active");
            ViewBag.categories = new SelectList(categories, "Id", "Name", brand.CategoryId);

            return View(brand);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBrand(UpdateBrandDto dto)
        {
            var validator = new UpdateBrandValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await LoadCategoriesDropdownAsync(dto.CategoryId);
                return View(dto);
            }

            var response = await _client.PutAsJsonAsync("Brands", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Marka güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesDropdownAsync(dto.CategoryId);
            TempData["ErrorMessage"] = "Marka güncellenemedi.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeletedBrand(int id)
        {
            var response = await _client.PostAsync($"brands/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeletedBrand(int id)
        {
            var response = await _client.PostAsync($"brands/undosoftdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "İlçe Lokasyonu başarıyla geri alındı.";
            else
                TempData["ErrorMessage"] = "Geri alma işlemi başarısız.";

            return RedirectToAction(nameof(DeletedBrands));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteBrand(int id)
        {
            var response = await _client.DeleteAsync($"brands/permanent/{id}");
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
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("brands/DeleteMultiple", ids);

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
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("brands/UndoSoftDeleteMultiple", ids);

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
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("brands/PermanentDeleteMultiple", ids);

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
