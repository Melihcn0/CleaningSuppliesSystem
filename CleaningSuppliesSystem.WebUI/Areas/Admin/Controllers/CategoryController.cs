using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.CategoryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]/{id?}")]
    public class CategoryController(CleaningSuppliesSystemContext _context) : Controller
    {
        private readonly HttpClient _client = HttpClientInstance.CreateClient();
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            var categories = values.Where(x => x.IsDeleted == false).ToList();
            return View(categories);
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _client.DeleteAsync($"categories/{id}");
            return RedirectToAction(nameof(DeletedCategory));
        }
        public IActionResult CreateCategory()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletedCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletedCategory()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            var categories = values.Where(x => x.IsDeleted == true).ToList();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedCategory(int id)
        {
            ViewBag.ShowBackButton = true;
            var category = await _context.Categories.FindAsync(id);
            category.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeletedCategory));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var validator = new Validators.Validators.CreateCategoryValidator();
            var result = await validator.ValidateAsync(createCategoryDto);
            if(!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(createCategoryDto);
            }
            await _client.PostAsJsonAsync("categories", createCategoryDto);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateCategoryDto>($"categories/{id}");
            ViewBag.ShowBackButton = true;
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var validator = new Validators.Validators.UpdateCategoryValidator();
            var result = await validator.ValidateAsync(updateCategoryDto);
            var category = await _client.GetFromJsonAsync<ResultCategoryDto>($"categories/{updateCategoryDto.Id}");
            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                updateCategoryDto.Name = category.Name;
                return View(updateCategoryDto);
            }

            await _client.PutAsJsonAsync("categories", updateCategoryDto);

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

    }
}
