using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoryController(CleaningSuppliesSystemContext _context, ICategoryService _categoryService, IMapper _mapper) : Controller
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
            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            return RedirectToAction(nameof(DeletedCategory));
        }
        public IActionResult CreateCategory()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }
        public async Task<IActionResult> DeletedCategory()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories");
            var categories = values.Where(x => x.IsDeleted == true).ToList();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletedCategory(int id)
        {
            var (isSuccess, message) = await _categoryService.TSoftDeleteCategoryAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UndoDeletedCategory(int id)
        {
            var (isSuccess, message) = await _categoryService.TUndoSoftDeleteCategoryAsync(id);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(DeletedCategory));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(DeletedCategory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var validator = new Business.Validators.Validators.CreateCategoryValidator();
            var result = await validator.ValidateAsync(createCategoryDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(createCategoryDto);
            }
            var (isSuccess, message) = await _categoryService.TCreateCategoryAsync(createCategoryDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(createCategoryDto);
            }

            TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var validator = new Business.Validators.Validators.UpdateCategoryValidator();
            var result = await validator.ValidateAsync(updateCategoryDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(updateCategoryDto);
            }
            var (isSuccess, message) = await _categoryService.TUpdateCategoryAsync(updateCategoryDto);
            if (!isSuccess)
            {
                ModelState.AddModelError("", message);
                return View(updateCategoryDto);
            }

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var value = await _client.GetFromJsonAsync<UpdateCategoryDto>($"categories/{id}");
            ViewBag.ShowBackButton = true;
            return View(value);
        }

    }
}
